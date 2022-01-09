using System.Numerics;

namespace LoopringAPI
{
    public class ApiTransaction
    {
        public long id { get; set; }
        public string hash { get; set; }
        public string owner { get; set; }
        public string txHash { get; set; }
        public string feeTokenSymbol { get; set; }
        public BigInteger feeAmount { get; set; } 
        public string status { get; set; }  
        public string progress { get; set; }
        public long timestamp { get; set; }
        public long blockNum { get; set; }
        public long updatedAt { get; set; }
        public long blockId { get; set; }
        public long indexInBlock { get; set; }
    }
}
