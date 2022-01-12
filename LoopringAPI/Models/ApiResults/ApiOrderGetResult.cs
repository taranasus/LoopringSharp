namespace LoopringSharp
{
    public class ApiOrderGetResult
    {
        public string hash { get; set; }
        public string clientOrderId { get; set; }
        public string side { get; set; }
        public string market { get; set; }
        public string price { get; set; }
        public Volumes volumes { get; set; }
        public Validity validity { get; set; }
        public string orderType { get; set; }
        public string tradeChannel { get; set; }
        public string status { get; set; }
    }
   
}
