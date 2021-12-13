namespace LoopringAPI
{
    public class ApiTransferResult
    {
        public string hash { get; set; }
        public string status { get; set; }
        public bool isIdempotent { get; set; }
    }
}
