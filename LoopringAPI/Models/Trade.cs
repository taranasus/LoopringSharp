namespace LoopringSharp
{
    public class Trade
    {
        public long TradeTimestamp { get; set; }
        public long RecordId { get; set; }
        public Side Side { get; set; }
        public decimal Volume { get; set; }
        public decimal Price { get; set; }
        public string Market { get; set; }
        public float Fees { get; set; }
    }
}
