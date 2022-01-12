using static LoopringSharp.ApiTransferRequest;

namespace LoopringSharp
{
    public class TransferRequest
    {
        // ESSENTIAL TRANSFER INFO
        public string exchange { get; set; }
        public int payerId { get; set; }
        public string payerAddr { get; set; }
        public int payeeId { get; set; } = 0;           // Default of 0 if unknown is fine
        public string payeeAddr { get; set; }
        public Token token { get; set; }
        public Token maxFee { get; set; }
        public int storageId { get; set; }
        public int validUnitl { get; set; }
        public string tokenName { get; set; }
        public string tokenFeeName { get; set; }


        public ApiTransferRequest GetApiTransferRequest(string memo, string clientId, CounterFactualInfo counterFactualInfo)
        {
            return new ApiTransferRequest()
            {
                exchange = exchange,
                payerId = payerId,
                payerAddr = payerAddr,
                payeeId = payeeId,
                payeeAddr = payeeAddr,
                token = token,
                maxFee = maxFee,
                storageId = storageId,
                validUntil = validUnitl,
                clientId = clientId,
                memo = memo,

                counterFactualInfo = counterFactualInfo
            };
        }
    }


}
