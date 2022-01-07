using System.Collections.Generic;

namespace LoopringAPI
{
    public class ApiDepositsGetResult
    {
        public long totalNum { get; set; }
        public List<Transaction> transactions { get; set; }
    }
}
