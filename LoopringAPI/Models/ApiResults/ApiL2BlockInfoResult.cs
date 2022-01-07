using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace LoopringAPI
{
    public class ApiL2BlockInfoResult
    {
        public int blockId { get; set; }
        public int blockSize { get; set; }
        public string exchange { get; set; }
        public string txHash { get; set; }
        public string status { get; set; }
        public BigInteger createdAt { get; set; }
        public List<TransactionBlockInfo> transactions { get; set; }
    }
}
