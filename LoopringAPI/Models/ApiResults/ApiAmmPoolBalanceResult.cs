using System;
using System.Collections.Generic;
using System.Text;

namespace LoopringSharp
{
    public class ApiAmmPoolBalanceResult
    {
        public string poolName { get; set; }
        public string poolAddress { get; set; }

        public List<TokenVolume> pooled { get; set; }
        public TokenVolume lp { get; set; }
        public bool risky { get; set; }
    }
}
