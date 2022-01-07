using System;
using System.Collections.Generic;
using System.Text;

namespace LoopringAPI
{
    public class OrderBlock
    {
        public int storageID { get; set; }
        public int accountID { get; set; }
        public string amountS { get; set; }
        public string amountB { get; set; }
        public int tokenS { get; set; }
        public int tokenB { get; set; }
        public int validUntil { get; set; }
        public string taker { get; set; }
        public int feeBips { get; set; }
        public string nftData { get; set; }
        public int fills { get; set; }
    }
}
