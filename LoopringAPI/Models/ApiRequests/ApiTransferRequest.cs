namespace LoopringSharp
{
    public class ApiTransferRequest
    {
        // ESSENTIAL TRANSFER INFO
        public string exchange { get; set; }
        public int payerId { get; set; }
        public string payerAddr { get; set; }        
        public int payeeId { get; set; } = 0;           // Default of 0 if unknown is fine
        public string payeeAddr { get; set; }
        public Token token { get; set; }
        public Token maxFee { get; set; }
        public int storageId { get; set; }
        public int validUntil { get; set; }

        // ESSENTIAL AUTHENTICATION AND VALIDATION INFO
        public string ecdsaSignature { get; set; }
        public string eddsaSignature { get; set; }

        // OPTIONAL TRANSFER INFO
        public string memo { get; set; }
        public string clientId { get; set; }
        public string hashApproved { get; set; } // ?????

        // FOR FUTURE USE PROBABLY
        public CounterFactualInfo counterFactualInfo { get; set; }                
    }
}
