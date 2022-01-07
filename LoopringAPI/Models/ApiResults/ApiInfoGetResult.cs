using System.Collections.Generic;

namespace LoopringAPI
{
    public class ApiInfoGetResult
    {
        public long totalNum { get; set; }
        public List<Transaction> transactions { get; set; }
    }
}
