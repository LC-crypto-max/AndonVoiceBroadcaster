namespace AndonVoiceBroadcaster.Models
{
    public sealed class WorkshopOption
    {
        public long WorkshopId { get; set; }
        public string WorkshopName { get; set; }

        public override string ToString()
        {
            if (WorkshopId >= 0 && !string.IsNullOrWhiteSpace(WorkshopName))
            {
                return WorkshopId + " - " + WorkshopName.Trim();
            }
            if (!string.IsNullOrWhiteSpace(WorkshopName))
            {
                return WorkshopName.Trim();
            }
            return WorkshopId >= 0 ? WorkshopId.ToString() : "未命名车间";
        }
    }
}
