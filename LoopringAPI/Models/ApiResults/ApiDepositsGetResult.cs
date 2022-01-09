using System.Collections.Generic;

namespace LoopringAPI
{
    public class ApiDepositsGetResult
    {
        public long totalNum { get; set; }
        public List<ApiDepositTransaction> transactions { get; set; }
    }
}
