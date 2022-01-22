using System;
using System.Collections.Generic;
using System.Text;

namespace LoopringSharp
{
    public class ApiOrderUserRateAmountResult
    {
        public string gasPrice { get; set; }
        public List<TokenAmount> amounts {get;set;}
        public long cacheOverdueAt { get; set; }
    }
}
