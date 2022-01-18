using System;
using System.Collections.Generic;
using System.Text;

namespace LoopringSharp
{
    public class ApiAmmPoolTradesResult
    {
        public int totalNum { get; set; }
        public List<AmmTradeData> trades { get; set; }
    }
}
