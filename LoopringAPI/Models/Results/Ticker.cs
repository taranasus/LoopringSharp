﻿namespace LoopringSharp
{
    public class Ticker
    {
        public string PairId { get; set; }
        public string TimeStamp { get; set; }
        public string BaseTokenVolume { get; set; }
        public string QuoteTokenVolume { get; set; }
        public string OpenPrice { get; set; }
        public string HeighestPrice { get; set; }
        public string LowestPrice { get; set; }
        public string ClosingPrice { get; set; }
        public string NumberOfTrades { get; set; }
        public float HighestBidPrice { get; set; }
        public float LowestAskPrice { get; set; }
        public string BaseFeeAmmount { get; set; }
        public string QuoteFeeAmount { get; set; }
    }
}
