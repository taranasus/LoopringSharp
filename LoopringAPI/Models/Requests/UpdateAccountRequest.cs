namespace LoopringSharp
{
    public class UpdateAccountRequest
    {
        public string exchange { get; set; }
        public string owner { get; set; } //eth public key
        public long accountId { get; set; }
        public Token maxFee { get; set; }
        public long validUntil { get; set; }
        public long nonce { get; set; }
        public string PublicKeyX { get; set; }
        public string PublicKeyY { get; set; }

        public ApiUpdateEDDSARequest GetUpdateEDDSARequest(CounterFactualInfo counterFactualInfo)
        {
            return new ApiUpdateEDDSARequest()
            {
                exchange = exchange,
                owner = owner,
                accountId = accountId,
                maxFee = maxFee,
                validUntil = validUntil,
                nonce = nonce,
                counterFactualInfo = counterFactualInfo,
                publicKey = new PublicKey()
                {
                    x = PublicKeyX,
                    y = PublicKeyY,
                }
            };
        }
    }
}
