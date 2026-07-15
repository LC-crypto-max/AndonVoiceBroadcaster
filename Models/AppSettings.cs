using System;
using System.Collections.Generic;
using System.Linq;

namespace AndonVoiceBroadcaster.Models
{
    public sealed class AppSettings
    {
        public string MesBaseUrl { get; set; }
        public string LoginEndpoint { get; set; }
        public string TerminalCode { get; set; }
        public List<long> WorkshopIdList { get; set; }
        public List<string> WorkshopNames { get; set; }
        public int PollIntervalSeconds { get; set; }
        public int NetworkRetrySeconds { get; set; }
        public int MaxRequestRetryCount { get; set; }
        public int TtsRate { get; set; }
        public int TtsVolume { get; set; }
        public string PreferredVoiceName { get; set; }
        public string TestSpeechText { get; set; }
        public int BroadcastIntervalSeconds { get; set; }
        public bool StartWithWindows { get; set; }
        public bool StartMinimized { get; set; }
        public int LogRetentionDays { get; set; }

        public AppSettings()
        {
            MesBaseUrl = string.Empty;
            LoginEndpoint = "/loginWithNoCode";
            TerminalCode = Environment.MachineName;
            WorkshopIdList = new List<long>();
            WorkshopNames = new List<string>();
            PollIntervalSeconds = 2;
            NetworkRetrySeconds = 5;
            MaxRequestRetryCount = 3;
            TtsRate = 1;
            TtsVolume = 100;
            PreferredVoiceName = string.Empty;
            TestSpeechText = "这是一条安灯语音播报测试，语音功能正常。";
            BroadcastIntervalSeconds = 1;
            StartWithWindows = false;
            StartMinimized = false;
            LogRetentionDays = 30;
        }

        public void Normalize()
        {
            MesBaseUrl = (MesBaseUrl ?? string.Empty).Trim().TrimEnd('/');
            LoginEndpoint = string.IsNullOrWhiteSpace(LoginEndpoint) ? "/loginWithNoCode" : (LoginEndpoint.Trim().StartsWith("/") ? LoginEndpoint.Trim() : "/" + LoginEndpoint.Trim());
            TerminalCode = string.IsNullOrWhiteSpace(TerminalCode) ? Environment.MachineName : TerminalCode.Trim();
            WorkshopIdList = (WorkshopIdList ?? new List<long>()).Where(x => x >= 0).Distinct().ToList();
            WorkshopNames = (WorkshopNames ?? new List<string>()).Select(x => (x ?? string.Empty).Trim()).Where(x => x.Length > 0).Distinct().ToList();
            PollIntervalSeconds = Math.Max(1, Math.Min(30, PollIntervalSeconds));
            NetworkRetrySeconds = Math.Max(1, NetworkRetrySeconds);
            MaxRequestRetryCount = Math.Max(1, MaxRequestRetryCount);
            TtsRate = Math.Max(-10, Math.Min(10, TtsRate));
            TtsVolume = Math.Max(0, Math.Min(100, TtsVolume));
            TestSpeechText = string.IsNullOrWhiteSpace(TestSpeechText)
                ? "这是一条安灯语音播报测试，语音功能正常。"
                : TestSpeechText.Trim();
            BroadcastIntervalSeconds = Math.Max(0, BroadcastIntervalSeconds);
            LogRetentionDays = Math.Max(1, LogRetentionDays);
        }

        public bool IsComplete()
        {
            return Uri.IsWellFormedUriString(MesBaseUrl, UriKind.Absolute);
        }

        public bool HasWorkshopSelection
        {
            get { return (WorkshopNames != null && WorkshopNames.Count > 0) || (WorkshopIdList != null && WorkshopIdList.Count > 0); }
        }
    }
}
