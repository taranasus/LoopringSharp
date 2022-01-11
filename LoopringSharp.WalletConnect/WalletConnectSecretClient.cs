using Newtonsoft.Json;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace LoopringSharp.WalletConnect
{
    public class WalletConnectSecretClient : LoopringSharp.SecureClient
    {
        string _apiUrl;
        public WalletConnectSecretClient(string apiUrl) : base(apiUrl)
        {

        }

        /// <summary>
        /// Send some tokens to anyone else on L2
        /// </summary>
        /// <param name="apiKey">Your Loopring API Key</param>
        /// <param name="l2Pk">Loopring Private Key</param>
        /// <param name="l1Pk">Ethereum Private Key</param>
        /// <param name="request">The basic transaction details needed in order to actually do a transaction</param>
        /// <param name="memo">(Optional)And do you want the transaction to contain a reference. From loopring's perspective, this is just a text field</param>
        /// <param name="clientId">(Optional)A user-defined id. It's similar to the memo field? Again the original documentation is not very clear</param>
        /// <param name="counterFactualInfo">(Optional)Not entirely sure. Official documentation says: field.UpdateAccountRequestV3.counterFactualInfo</param>
        /// <returns>An object containing the status of the transfer at the end of the request</returns>
        /// <exception cref="System.Exception">Gets thrown when there's a problem getting info from the Loopring API endpoint</exception>
        public override OperationResult Transfer(string apiKey, string l2Pk, string l1Pk, TransferRequest request, string memo, string clientId, CounterFactualInfo counterFactualInfo)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new System.Exception("Transfer REQUIRES a valid Loopring wallet apiKey");
            if (string.IsNullOrWhiteSpace(l2Pk))
                throw new System.Exception("Transfer REQUIRES a valid Loopring Wallet Layer 2 Private key");

            var account = GetAccountInfo(request.payerAddr);

            BigInteger[] inputs = {
                Utils.ParseHexUnsigned(request.exchange),
                (BigInteger)request.payerId,
                (BigInteger)request.payeeId,
                (BigInteger)request.token.tokenId,
                BigInteger.Parse(request.token.volume),
                (BigInteger)request.maxFee.tokenId,
                BigInteger.Parse(request.maxFee.volume),
                Utils.ParseHexUnsigned(request.payeeAddr),
                0,
                0,
                (BigInteger)request.validUnitl,
                (BigInteger)request.storageId
            };
            var apiRequest = request.GetApiTransferRequest(memo, clientId, counterFactualInfo);
            apiRequest.eddsaSignature = LoopringSharp.EDDSAHelper.EDDSASign(inputs, l2Pk);

            var typedData = ECDSAHelper.GenerateTransferTypedData(ExchangeInfo().chainId, apiRequest);
            apiRequest.ecdsaSignature = WalletConnectServer.Sign(JsonConvert.SerializeObject(typedData.Item2), "eth_signTypedData_v4", request.payerAddr) + "02";

            (string, string)[] headers = { (LoopringSharp.Constants.HttpHeaderAPIKeyName, apiKey), (LoopringSharp.Constants.HttpHeaderAPISigName, apiRequest.ecdsaSignature) };
            var apiresult = JsonConvert.DeserializeObject<ApiTransferResult>(
                Utils.Http(_apiUrl + LoopringSharp.Constants.TransferUrl, null, headers, "post", JsonConvert.SerializeObject(apiRequest)));
            return new OperationResult(apiresult);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="l1Pk">User's current eth private key</param>
        /// <param name="l2Pk">User's current loopring private key</param>
        /// <param name="req">A UpdateAccountRequest object containing all the needed information for this request</param>
        /// <param name="counterFactualInfo">(Optional)Not entirely sure. Official documentation says: field.UpdateAccountRequestV3.counterFactualInfo</param>
        /// <returns></returns>
        public virtual async Task<OperationResult> UpdateAccount(string l2Pk, string l1Pk, UpdateAccountRequest req, CounterFactualInfo counterFactualInfo)
        {
            var apiRequest = req.GetUpdateEDDSARequest(counterFactualInfo);

            BigInteger[] inputs = {
                Utils.ParseHexUnsigned(apiRequest.exchange),
                (BigInteger)apiRequest.accountId,
                (BigInteger)apiRequest.maxFee.tokenId ,
                BigInteger.Parse(apiRequest.maxFee.volume),
                Utils.ParseHexUnsigned(apiRequest.publicKey.x),
                Utils.ParseHexUnsigned(apiRequest.publicKey.y),
                (BigInteger)apiRequest.validUntil,
                (BigInteger)apiRequest.nonce
            };

            apiRequest.eddsaSignature = LoopringSharp.EDDSAHelper.EDDSASign(inputs, l2Pk);

            var typedData = ECDSAHelper.GenerateAccountUpdateTypedData(ExchangeInfo().chainId, apiRequest);
            apiRequest.ecdsaSignature = await WalletConnectServer.Sign(JsonConvert.SerializeObject(typedData.Item2), "eth_signTypedData_v4", req.owner) + "02";

            (string, string)[] headers = { (LoopringSharp.Constants.HttpHeaderAPISigName, apiRequest.ecdsaSignature) };
            var apiresult = JsonConvert.DeserializeObject<ApiTransferResult>(
                Utils.Http(_apiUrl + LoopringSharp.Constants.AccountUrl, null, headers, "post", JsonConvert.SerializeObject(apiRequest)));
            return new OperationResult(apiresult);
        }

        /// <summary>
        /// WARNING!!! This has a fee asociated with it. Make a OffchainFee request of type OffChainRequestType.UpdateAccount to see what the fee is.
        /// Updates the EDDSA key associated with the specified account, making the previous one invalid in the process.
        /// </summary>
        /// <param name="apiKey">User's current API Key</param>
        /// <param name="l1Pk">User's current eth private key</param>
        /// <param name="l2Pk">User's current loopring private key</param>
        /// <param name="accountId">User's account id</param>
        /// <param name="feeToken">The token in which the fee should be paid for this operation</param>
        /// <param name="ethPublicAddress">User's public wallet address</param>
        /// <param name="exchangeAddress">Exchange's public address</param>
        /// <returns>Returns the hash and status of your requested operation</returns>
        public virtual OperationResult UpdateAccount(string apiKey, string l1Pk, string l2Pk, int accountId, string feeToken, string ethPublicAddress, string exchangeAddress)
        {
            var newNonce = (GetAccountInfo(ethPublicAddress)).nonce;
            (string publicKeyX, string publicKeyY, string secretKey, string ethAddress) keys;

            keys = EDDSAHelper.EDDSASignWalletConnect(exchangeAddress, newNonce);
            l1Pk = WalletService.WalletConnect.ToString();

            var feeamountresult =  OffchainFee(apiKey, accountId, OffChainRequestType.UpdateAccount, feeToken, "0");
            var feeamount = feeamountresult.fees.Where(w => w.token == feeToken).First().fee;

            UpdateAccountRequest req = new UpdateAccountRequest()
            {
                accountId = accountId,
                exchange = ExchangeInfo().exchangeAddress,
                maxFee = new Token()
                {
                    tokenId = GetTokenId(feeToken),
                    volume = feeamount
                },
                nonce = newNonce,
                owner = ethPublicAddress,
                validUntil = 1700000000,
                PublicKeyX = keys.publicKeyX,
                PublicKeyY = keys.publicKeyY
            };

            return UpdateAccount(l2Pk, l1Pk, req, null);
        }
    }
}
