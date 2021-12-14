namespace LoopringAPI
{
    public class ApiSubmitOrderRequest
    {
        public string exchange { get; set; }
        public int accountId { get; set; }
        public int storageId { get; set; }
        public Token sellToken { get; set; }
        public Token buyToken { get; set; }
        public bool allOrNone { get; set; } = true; // Currently only supports true
        public bool fillAmountBOrS { get; set; } // Wat?
        public long validUntil { get; set; } // It's a timestamp
        public int maxFeeBips { get; set; } = 20; // Maximum order fee that the user can accept, value range (in ten thousandths) 1 ~ 63. WAT???
        public string eddsaSignature { get; set; }

        // And now for the optionals
        public string clientOrderId { get; set; }
        public string orderType { get; set; }    // AMM, LIMIT_ORDER, MAKER_ONLY, TAKER_ONLY
        public string tradeChannel { get; set; } // ORDER_BOOK, AMM_POOL, MIXED
        public string taker { get; set; }        // {Used by the P2P order which user specify the taker, so far its} WAAAAAAAAAAAAAAAT?????
        public string poolAddress { get; set; }  // The AMM pool address if order type is AMM 
        public string affiliate { get; set; }    // This one is very interesting from a profitability standpoint @.@

    }
}
