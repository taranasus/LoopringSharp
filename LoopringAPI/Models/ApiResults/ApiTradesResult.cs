using System.Collections.Generic;

namespace LoopringAPI
{
    public class ApiTradesResult
    {
        public long totalNum { get; set; }
        public List<List<string>> trades { get; set; }
    }
}
