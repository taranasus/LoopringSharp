using System.Collections.Generic;

namespace LoopringSharp
{
    public class ApiInfoGetResult
    {
        public long totalNum { get; set; }
        public List<ApiTransaction> transactions { get; set; }
    }
}
