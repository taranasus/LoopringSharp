using System;
using System.Collections.Generic;
using System.Text;

namespace LoopringSharp
{
    public class ApiAmmJoinExitTransactionsResult
    {
        public int totalNum { get; set; }
        public List<AmmTransactionData> transactions { get; set; }
    }
}
