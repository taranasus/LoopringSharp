using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace LoopringAPI
{
    public class TransactionBlock
    {
        public string txType { get; set; }
        public int accountId { get; set; }
        public string owner { get; set; }

        public TokenBlockInfo token { get;set;}
        public TokenBlockInfo toToken { get; set; }
        public TokenBlockInfo fee { get; set; }
        public BigInteger validUntil { get; set; }
        public int toAccountId { get; set; }
        public string toAccountAddress { get; set; } 
        public int storageId { get; set; }
        public OrderBlockInfo orderA { get; set; }
        public OrderBlockInfo orderB { get; set; }
        public bool valid { get; set; }
        public int nonce { get; set; }
        public int minterAccountId { get; set; }
        public string minter { get; set; }
        public TokenBlockInfo nftToken { get; set; }
        public string nftType { get; set; }
        public string fromAddress { get; set; }
        public string toAddress { get; set; }
    }
}
