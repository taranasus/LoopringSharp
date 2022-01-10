using System.Collections.Generic;

namespace LoopringSharp
{
    public class ApiOrdersGetResult
    {
        public long totalNum { get; set; }
        public List<ApiOrderGetResult> orders { get; set; }
    }
}
