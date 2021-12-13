namespace LoopringAPI
{
    public class ApiTransferRequest
    {
        // ESSENTIAL TRANSFER INFO
        public string exchange { get; set; }
        public int payerId { get; set; }
        public string payerAddr { get; set; }        
        public int payeeId { get; set; } = 0;           // Default of 0 if unknown is fine
        public string payeeAddress { get; set; }
        public Token token { get; set; }
        public Token maxFee { get; set; }
        public int storageId { get; set; }
        public long validUnitl { get; set; }

        // ESSENTIAL AUTHENTICATION AND VALIDATION INFO
        public string hashApproved { get; set; }
        public string ecdsaSignature { get; set; }
        public string eddsaSignature { get; set; }

        // OPTIONAL TRANSFER INFO
        public string memo { get; set; }
        public string clientId { get; set; }

        // FOR FUTURE USE PROBABLY
        public CounterFactualInfo counterFactualInfo { get; set; }        

        public class CounterFactualInfo
        {
            public string walletFactory { get; set; }
            public string walletOwner { get; set; }
            public string walletSalt { get; set; }
        }
    }

}
