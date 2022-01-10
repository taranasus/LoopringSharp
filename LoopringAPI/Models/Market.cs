namespace LoopringSharp
{
    public class Market
    {
        public string market { get; set; }
        public int baseTokenId { get; set; }
        public int quoteTokenId { get; set; }
        public int precisionForPrice { get; set; }
        public int orderbookAggLevels { get; set; }
        public bool enabled { get; set; }
    }
}
