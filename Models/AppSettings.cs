using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AndonVoiceBroadcaster.Models
{
    public sealed class AppSettings
    {
        public string MesBaseUrl { get; set; }
        public string LoginEndpoint { get; set; }
        public string TerminalCode { get; set; }
        public long? FactoryId { get; set; }
        public string FactoryName { get; set; }
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
        public string WorkStartTime { get; set; }
        public string WorkEndTime { get; set; }
        public bool StartWithWindows { get; set; }
        public bool StartMinimized { get; set; }
        public int LogRetentionDays { get; set; }

        public AppSettings()
        {
            MesBaseUrl = string.Empty;
            LoginEndpoint = "/loginWithNoCode";
            TerminalCode = Environment.MachineName;
            FactoryName = string.Empty;
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
            WorkStartTime = "08:00";
            WorkEndTime = "17:00";
            StartWithWindows = false;
            StartMinimized = false;
            LogRetentionDays = 30;
        }

        public void Normalize()
        {
            // 清理配置，并将数值限制在程序支持的范围内。
            MesBaseUrl = (MesBaseUrl ?? string.Empty).Trim().TrimEnd('/');
            LoginEndpoint = string.IsNullOrWhiteSpace(LoginEndpoint) ? "/loginWithNoCode" : (LoginEndpoint.Trim().StartsWith("/") ? LoginEndpoint.Trim() : "/" + LoginEndpoint.Trim());
            TerminalCode = string.IsNullOrWhiteSpace(TerminalCode) ? Environment.MachineName : TerminalCode.Trim();
            FactoryName = (FactoryName ?? string.Empty).Trim();
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
            TimeSpan workStart;
            TimeSpan workEnd;
            if (!TryParseWorkTime(WorkStartTime, out workStart)
                || !TryParseWorkTime(WorkEndTime, out workEnd)
                || workEnd <= workStart)
            {
                workStart = TimeSpan.FromHours(8);
                workEnd = TimeSpan.FromHours(17);
            }
            WorkStartTime = workStart.ToString(@"hh\:mm", CultureInfo.InvariantCulture);
            WorkEndTime = workEnd.ToString(@"hh\:mm", CultureInfo.InvariantCulture);
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

        public TimeSpan GetWorkStartTime()
        {
            TimeSpan value;
            return TryParseWorkTime(WorkStartTime, out value) ? value : TimeSpan.FromHours(8);
        }

        public TimeSpan GetWorkEndTime()
        {
            TimeSpan value;
            return TryParseWorkTime(WorkEndTime, out value) ? value : TimeSpan.FromHours(17);
        }

        public bool IsWithinWorkHours(DateTime localTime)
        {
            TimeSpan current = localTime.TimeOfDay;
            return current >= GetWorkStartTime() && current < GetWorkEndTime();
        }

        public TimeSpan GetDelayUntilWorkStarts(DateTime localTime)
        {
            if (IsWithinWorkHours(localTime))
            {
                return TimeSpan.Zero;
            }
            DateTime nextStart = localTime.Date.Add(GetWorkStartTime());
            if (localTime >= nextStart)
            {
                nextStart = nextStart.AddDays(1);
            }
            return nextStart - localTime;
        }

        private static bool TryParseWorkTime(string value, out TimeSpan result)
        {
            return TimeSpan.TryParseExact(
                (value ?? string.Empty).Trim(),
                @"hh\:mm",
                CultureInfo.InvariantCulture,
                out result);
        }
    }
}
