using System;
using System.Collections.Generic;
using System.Text;

namespace LoopringSharp
{
    public class AmmPoolInfo
    {
        public string name { get; set; }
        public string market { get; set; }
        public string address { get; set; }
        public string version { get; set; }
        public AmmPoolTokens tokens { get; set; }
        public int feeBips { get; set; }
        public AmmPoolPrecisions precisions { get; set; }
        public string createdAt { get; set; }
        public int status { get; set; }
    }
}
