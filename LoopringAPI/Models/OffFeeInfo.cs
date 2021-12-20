namespace LoopringAPI
{
    public class OffFeeInfo
    {
        public string token { get; set; }
        public string fee { get; set; }     
        public decimal normalziedFee
        {
            get
            {
                return decimal.Parse(fee) / 1000000000000000000m;
            }
        }
    }
}
