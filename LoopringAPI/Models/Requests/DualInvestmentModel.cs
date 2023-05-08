namespace LoopringSharp
{
    public class DualInvestmentModel
    {
        public int accountId { get; set; }
        public string baseProfit { get; set; }
        public Token buyToken { get; set; }
        public string clientOrderId { get; set; }
        public string eddsaSignature { get; set; }
        public string exchange { get; set; }
        public long expireTime { get; set; }
        public string fee { get; set; }
        public bool fillAmountBOrS { get; set; }
        public int maxFeeBips { get; set; }
        public string productId { get; set; }
        public Token sellToken { get; set; }
        public string settleRatio { get; set; }
        public int storageId { get; set; }
        public long validUntil { get; set; }
    }
}
