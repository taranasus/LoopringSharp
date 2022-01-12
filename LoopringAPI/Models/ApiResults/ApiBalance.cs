namespace LoopringSharp
{
    public class ApiBalance
    {
        public int accountId { get; set; }
        public int tokenId { get; set; }
        public string total { get; set; }
        public string locked { get; set; }
        public ApiBalancePending pending { get; set; }
        public class ApiBalancePending
        {
            public string withdraw { get; set; }
            public string deposit { get; set; }
        }
    }

}
