namespace LoopringSharp
{
    public class Candlestick
    {
        public string startTime { get; set; }
        public long numberOfTransactions { get; set; }
        public decimal open { get; set; }
        public decimal close { get; set; }
        public decimal high { get; set; }
        public decimal low { get; set; }
        public decimal baseTokenVolume { get; set; }
        public decimal quoteTokenVolume { get; set; }
    }
}
