namespace LoopringSharp
{
    public class DualBaseModel
    {
        public string ProductId { get; set; }
        public string Base { get; set; }
        public string Quote { get; set; }
        public string Currency { get; set; }
        public long CreateTime { get; set; }
        public long ExpireTime { get; set; }
        public string Strike { get; set; }
        public bool Expired { get; set; }
        public string DualType { get; set; }
        public double Ratio { get; set; }
        public string Profit { get; set; }
        public string BaseSize { get; set; }
    }
}
