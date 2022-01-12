namespace LoopringSharp
{
    public class Balance
    {
        public string token { get; set; }
        public decimal total { get; set; }
        public decimal locked { get; set; }
        public BalancePending pending { get; set; }
        public class BalancePending
        {
            public decimal widthdraw { get; set; }
            public decimal deposit { get; set; }
        }
    }

}
