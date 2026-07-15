using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using AndonVoiceBroadcaster.Models;

namespace AndonVoiceBroadcaster.Services
{
    public sealed class MesApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly JavaScriptSerializer _json = new JavaScriptSerializer();
        private readonly string _baseUrl;
        private readonly string _loginEndpoint;
        private string _token;

        public MesApiClient(string baseUrl, string loginEndpoint)
        {
            _baseUrl = (baseUrl ?? string.Empty).TrimEnd('/');
            _loginEndpoint = string.IsNullOrWhiteSpace(loginEndpoint) ? "/loginWithNoCode" : (loginEndpoint.StartsWith("/") ? loginEndpoint : "/" + loginEndpoint);
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        }

        public bool HasToken
        {
            get { return !string.IsNullOrWhiteSpace(_token); }
        }

        public void ClearToken()
        {
            _token = null;
        }

        public async Task LoginAsync(MesCredentials credentials, CancellationToken cancellationToken)
        {
            var request = new Dictionary<string, object>
            {
                { "username", credentials.Username },
                { "password", credentials.Password }
            };
            if (string.Equals(_loginEndpoint, "/login", StringComparison.OrdinalIgnoreCase))
            {
                request["code"] = string.Empty;
                request["uuid"] = string.Empty;
            }
            AjaxResult result = await SendAsync<AjaxResult>(HttpMethod.Post, _loginEndpoint, request, false, cancellationToken);
            if (result.Code != 200 || string.IsNullOrWhiteSpace(result.Token))
            {
                throw new MesApiException(string.IsNullOrWhiteSpace(result.Msg) ? "MES 登录失败。" : result.Msg);
            }
            _token = result.Token;
        }

        public async Task<List<WorkshopOption>> GetWorkshopsAsync(CancellationToken cancellationToken)
        {
            AjaxResultWithData result = await SendAsync<AjaxResultWithData>(
                HttpMethod.Get, "/mes/md/workshop/listAll", null, true, cancellationToken);
            if (result.Code != 200 && result.Code != 0)
            {
                throw new MesApiException(result.Msg ?? "查询车间列表失败");
            }
            if (result.Data == null)
            {
                return new List<WorkshopOption>();
            }
            string raw = _json.Serialize(result.Data);
            return _json.Deserialize<List<WorkshopOption>>(raw) ?? new List<WorkshopOption>();
        }

        public async Task<List<VoiceBroadcastTask>> GetPendingTasksAsync(List<long> workshopIds, CancellationToken cancellationToken)
        {
            var query = new VoiceBroadcastQuery { WorkshopIdList = workshopIds, State = "0" };
            TableDataResponse result = await SendAsync<TableDataResponse>(
                HttpMethod.Post, "/sys/voiceBroadcast/getNotBroadcastRecord", query, true, cancellationToken);
            if (result.Code != 200 && result.Code != 0)
            {
                throw new MesApiException(result.Msg ?? "查询待播报任务失败。");
            }
            return result.Rows ?? new List<VoiceBroadcastTask>();
        }

        public async Task<bool> ConfirmPlayedAsync(long id, int expectedBroadcastedNum, CancellationToken cancellationToken)
        {
            var request = new VoiceBroadcastTask { Id = id, BroadcastedNum = expectedBroadcastedNum };
            AjaxResult result = await SendAsync<AjaxResult>(
                HttpMethod.Post, "/system/voiceBroadcast/confirmPlayed", request, true, cancellationToken);
            if (result.Code != 200)
            {
                throw new MesApiException(result.Msg ?? "确认播报结果失败。");
            }
            return result.Confirmed;
        }

        public async Task<VoiceBroadcastTask> GetTaskAsync(long id, CancellationToken cancellationToken)
        {
            AjaxResultWithData result = await SendAsync<AjaxResultWithData>(
                HttpMethod.Get, "/system/voiceBroadcast/" + id, null, true, cancellationToken);
            if (result.Code != 200)
            {
                throw new MesApiException(result.Msg ?? "查询播报任务详情失败。");
            }
            if (result.Data == null)
            {
                return null;
            }
            string raw = _json.Serialize(result.Data);
            return _json.Deserialize<VoiceBroadcastTask>(raw);
        }

        private async Task<T> SendAsync<T>(HttpMethod method, string relativePath, object body, bool requiresToken, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_baseUrl))
            {
                throw new MesApiException("MES 服务地址未配置。");
            }

            var request = new HttpRequestMessage(method, new Uri(new Uri(_baseUrl + "/"), relativePath.TrimStart('/')));
            if (requiresToken)
            {
                if (string.IsNullOrWhiteSpace(_token))
                {
                    throw new MesUnauthorizedException("MES Token 不存在。");
                }
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            }
            if (body != null)
            {
                request.Content = new StringContent(_json.Serialize(body), Encoding.UTF8, "application/json");
            }

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request, cancellationToken);
            }
            catch (TaskCanceledException exception)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                throw new MesApiException("MES 接口请求超时。", exception);
            }

            string content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new MesUnauthorizedException("MES 登录已失效。");
            }
            if (!response.IsSuccessStatusCode)
            {
                throw new MesApiException("MES 接口返回 " + (int)response.StatusCode + "：" + content);
            }
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new MesApiException("MES 接口未返回有效内容。");
            }

            try
            {
                return _json.Deserialize<T>(content);
            }
            catch (Exception exception)
            {
                throw new MesApiException("MES 接口返回内容无法解析：" + content, exception);
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        private sealed class AjaxResultWithData
        {
            public int Code { get; set; }
            public string Msg { get; set; }
            public object Data { get; set; }
        }
    }

    public class MesApiException : Exception
    {
        public MesApiException(string message) : base(message) { }
        public MesApiException(string message, Exception innerException) : base(message, innerException) { }
    }

    public sealed class MesUnauthorizedException : MesApiException
    {
        public MesUnauthorizedException(string message) : base(message) { }
    }
}
