using System.Collections.Generic;

namespace LoopringSharp
{
    public class ApiDepositsGetResult
    {
        public long totalNum { get; set; }
        public List<ApiDepositTransaction> transactions { get; set; }
    }
}
