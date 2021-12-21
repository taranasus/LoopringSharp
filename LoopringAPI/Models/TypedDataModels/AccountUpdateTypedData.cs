using System.Collections.Generic;
using System.Numerics;

namespace LoopringAPI
{
    public class AccountUpdateTypedData
    {
        public Domain domain { get; set; }
        public Message message { get; set; }
        public string primaryType { get; set; }
        public Types types { get; set; }

        public class Domain
        {
            public BigInteger chainId { get; set; }
            public string name { get; set; }
            public string verifyingContract { get; set; }
            public string version { get; set; }
        }

        public class Message
        {
            public string owner { get; set; }
            public long accountID { get; set; }
            public int feeTokenID { get; set; }
            public string maxFee { get; set; }
            public string publicKey { get; set; } // this seemed like jut pubkey y so screw it!
            public long validUntil { get; set; }
            public long nonce { get; set; }
        }    

        public class Types
        {
            public List<Type> EIP712Domain { get; set; }
            public List<Type> AccountUpdate { get; set; }
        }
    }
}
