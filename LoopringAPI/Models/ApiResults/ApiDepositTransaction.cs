using System.Numerics;

namespace LoopringAPI
{
    public class ApiDepositTransaction
    {
        public long id { get; set; }
        public string hash { get; set; }
        public string symbol { get; set; }
        public BigInteger amount { get; set; }
        public string txHash { get; set; }
        public string status { get; set; }
        public string progress { get; set; }
        public long timestamp { get; set; }
        public long blockNum { get; set; }
        public long updatedAt { get; set; }
        public long blockId { get; set; }
        public long indexInBlock { get; set; }
    }
}
