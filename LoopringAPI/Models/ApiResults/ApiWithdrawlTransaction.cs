using System.Numerics;

namespace LoopringSharp
{
    public class ApiWithdrawlTransaction
    {
        public long id { get; set; }
        public string txType { get; set; }
        public string hash { get; set; }
        public string symbol { get; set; }
        public BigInteger amount { get; set; }
        public string txHash { get; set; }
        public string feeTokenSymbol { get; set; }
        public BigInteger feeAmount { get; set; }
        public string status { get; set; }
        public string progress { get; set; }
        public long timestamp { get; set; }
        public long blockNum { get; set; }
        public long updatedAt { get; set; }
        public string distributedHash { get;set; }
        public int requestId { get; set; }
        public string fastStatus { get; set; }
        public long blockId { get; set; }
        public long indexInBlock { get; set; }
    }
}
