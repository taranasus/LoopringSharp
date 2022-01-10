using System.Collections.Generic;

namespace LoopringSharp
{
    public class ApiWithdrawlsGetResult
    {
        public long totalNum { get; set; }
        public List<ApiWithdrawlTransaction> transactions { get; set; }
    }
}
