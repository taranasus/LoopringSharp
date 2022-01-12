namespace LoopringSharp
{
    public class ApiAccountResult
    {
        public int accountId { get; set; }
        public string owner { get; set; }
        public bool frozen { get; set; }
        public PublicKey publicKey { get; set; }
        public string tags { get; set; }
        public int nonce { get; set; }
        public string keyNonce { get; set; }
        public string keySeed { get; set; }        
    }
}
