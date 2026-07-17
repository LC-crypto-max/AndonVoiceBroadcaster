namespace AndonVoiceBroadcaster.Models
{
    public sealed class FactoryOption
    {
        public long? FactoryId { get; set; }
        public string FactoryName { get; set; }

        public override string ToString()
        {
            if (FactoryId.HasValue && !string.IsNullOrWhiteSpace(FactoryName))
            {
                return FactoryId.Value + " - " + FactoryName.Trim();
            }
            if (!string.IsNullOrWhiteSpace(FactoryName))
            {
                return FactoryName.Trim();
            }
            return FactoryId.HasValue ? "工厂 " + FactoryId.Value : "当前登录工厂";
        }
    }
}
