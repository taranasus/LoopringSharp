namespace LoopringAPI
{
    public class TokenConfig
    {
        public string type { get; set; }
        public int tokenId { get; set; }
        public string symbol { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public int decimals { get; set; }
        public int precision { get; set; }
        public int precisionForOrder { get; set; }
        public OrderAmounts orderAmounts { get; set; }
        public OrderAmounts luckyTokenAmounts { get; set; }
        public string fastWithdrawLimit { get; set; }
        public GasAmount gasAmmounts { get; set; }
        public bool enabled { get; set; }
    }
}
