using System.Numerics;

namespace LoopringSharp
{
    public class ApiWithdrawRequest
    {
        public string exchange { get; set; }
        public int accountId { get; set; }
        public string owner { get; set; }
        public Token token { get; set; }
        public Token maxFee { get; set; }
        public int storageId { get; set; }
        public int validUntil { get; set; }
        public int minGas { get; set; }
        public string to { get; set; }
        public string extraData { get; set; }
        public bool fastWithdrawlMode { get; set; }

        // FOR FUTURE USE PROBABLY
        public CounterFactualInfo counterFactualInfo { get; set; }

        // ESSENTIAL AUTHENTICATION AND VALIDATION INFO
        public string ecdsaSignature { get; set; }
        public string eddsaSignature { get; set; }

        // OPTIONAL TRANSFER INFO
        public string hashApproved { get; set; } // ?????

    }
}
