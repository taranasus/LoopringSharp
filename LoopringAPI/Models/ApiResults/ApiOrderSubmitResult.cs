namespace LoopringSharp
{
    public class ApiOrderSubmitResult
    { 
        public string hash { get; set; }
        public string clientOrderId { get; set; }
        public string status { get; set; }
        public bool isIdempotent { get; set; }
    }
}
