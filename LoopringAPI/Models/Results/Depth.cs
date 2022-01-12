using System.Collections.Generic;

namespace LoopringSharp
{
    public class Depth
    {
        public long version { get; set; }
        public long timestamp { get; set; }
        public string market { get; set; }
        public List<Position> bids { get; set; }
        public List<Position> asks { get; set; }

        public class Position
        {
            public float price { get; set; }
            public decimal size { get; set; }
            public decimal volume { get; set; }
            public decimal numberOfOrdersAgregated { get; set; }
        }
    }
}
