using System;
using System.Collections.Generic;
using System.Text;

namespace LoopringSharp
{
    public class TokenAmount
    {
        public string tokenSymbol { get; set; }
        public decimal discount { get; set; }
        public OrderInfo baseOrderInfo { get; set; }
        public OrderInfo userOrderInfo { get; set; }  
        public OrderAmounts marketOrderInfo { get; set; }
    }
}
