using System.Collections.Generic;

namespace AndonVoiceBroadcaster.Models
{
    public sealed class VoiceBroadcastTask
    {
        public long Id { get; set; }
        public string ComeType { get; set; }
        public string ComeNo { get; set; }
        public string Content { get; set; }
        public string State { get; set; }
        public int VoiceNum { get; set; }
        public int BroadcastedNum { get; set; }
        public long? LineId { get; set; }
        public string LineName { get; set; }
        public long? WorkstationId { get; set; }
        public string WorkstationName { get; set; }
        // 后端按 yyyy-MM-dd HH:mm:ss 输出，使用字符串避免 .NET Framework JavaScriptSerializer
        // 对非 ISO 日期格式反序列化失败；该格式可直接用于一期 FIFO 字典序排序。
        public string CreateTime { get; set; }

        public bool IsPending
        {
            get { return State == "0" && BroadcastedNum < VoiceNum; }
        }
    }

    public sealed class VoiceBroadcastQuery
    {
        public List<long> WorkshopIdList { get; set; }
        public string State { get; set; }
    }

    public sealed class TableDataResponse
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public long Total { get; set; }
        public List<VoiceBroadcastTask> Rows { get; set; }
    }

    public sealed class AjaxResult
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public string Token { get; set; }
        public bool Confirmed { get; set; }
    }
}
