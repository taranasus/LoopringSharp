using System;
using System.Collections.Generic;
using System.Text;

namespace LoopringSharp
{
    public class ApiTradeHistoryResult
    {
        public long totalNum { get; set; }
        public List<List<string>> trades { get; set; }
    }
}
