using System.Collections.Generic;

namespace LoopringAPI
{
    public class ApiOrdersGetResult
    {
        public long totalNum { get; set; }
        public List<ApiOrderGetResult> orders { get; set; }
    }
}
