using System.Collections.Generic;

namespace LoopringAPI
{
    public class ApiWithdrawlsGetResult
    {
        public long totalNum { get; set; }
        public List<ApiWithdrawlTransaction> transactions { get; set; }
    }
}
