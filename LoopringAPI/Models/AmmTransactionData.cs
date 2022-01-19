using System;
using System.Collections.Generic;
using System.Text;

namespace LoopringSharp
{
    public class AmmTransactionData
    {
        public string hash { get; set; }
        public string txType { get; set; }
        public string txStatus { get; set; }
        public string ammPoolAddress { get; set; }
        public string ammLayerType { get; set; }
        public List<AmmTransferData> poolTokens { get; set; }
        public AmmTransferData lpToken { get; set; }
        public long createdAt { get; set; }
        public long updatedAt { get; set; }
        public int blockId { get; set; }
        public int indexInBlock { get; set; }

    }
}
