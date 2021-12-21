namespace LoopringAPI
{
    public class ApiUpdateEDDSARequest
    {
        public string exchange { get; set; }
        public string owner { get; set; } //eth public key
        public long accountId { get; set; }
        public PublicKey publicKey { get; set; }
        public Token maxFee { get; set; }
        public long validUntil { get; set; }
        public long nonce { get; set; }
        public string keySeed { get; set; }
        public CounterFactualInfo counterFactualInfo { get; set; }
        public string eddsaSignature { get; set; }
        public string ecdsaSignature { get; set; }
        public string hashApproved { get; set; }
    }
}
