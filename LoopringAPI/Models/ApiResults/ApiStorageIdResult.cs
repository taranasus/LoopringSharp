namespace LoopringSharp
{
    public class ApiStorageIdResult
    {
        public int orderId { get; set; }
        public int offchainId { get; set; }
        public Side side { get; set; }
        public decimal Volume { get; set; }
        public decimal Price { get; set; }
        public string Market { get; set; }
        public decimal Fee { get; set; }
    }
}
