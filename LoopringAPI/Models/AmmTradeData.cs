using System;
using System.Collections.Generic;
using System.Text;

namespace LoopringSharp
{
    public class AmmTradeData
    {
        public int accountId { get; set; }
        public string orderHash { get; set; }
        public string market { get; set; }
        public string side { get; set; }
        public string size { get; set; }
        public decimal price { get; set; }
        public string feeAmount { get; set; }
        public long createdAt { get; set; }
    }
}
