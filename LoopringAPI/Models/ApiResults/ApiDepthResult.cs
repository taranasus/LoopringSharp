using System.Collections.Generic;

namespace LoopringAPI
{
    public class ApiDepthResult
    {
        public long version { get; set; }
        public long timestamp { get; set; }
        public string market { get; set; }
        public List<List<string>> bids { get; set; }
        public List<List<string>> asks { get; set; }
    }
}
