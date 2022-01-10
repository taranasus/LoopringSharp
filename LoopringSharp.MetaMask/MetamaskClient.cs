using System.Threading.Tasks;

namespace LoopringSharp.MetaMask
{
    public class MetamaskClient : LoopringSharp.Client
    {
        MetamaskSecureClient _metamaskClient;
        public MetamaskClient(string apiUrl) : base(apiUrl, GetEthereumAddress(apiUrl), true)
        {
            _metamaskClient = new MetamaskSecureClient(apiUrl);
        }        

        /// <summary>
        /// Send some tokens to anyone else on L2
        /// </summary>
        /// <param name="request">The basic transaction details needed in order to actually do a transaction</param>
        /// <param name="memo">(Optional)And do you want the transaction to contain a reference. From loopring's perspective, this is just a text field</param>
        /// <param name="clientId">(Optional)A user-defined id. It's similar to the memo field? Again the original documentation is not very clear</param>
        /// <param name="counterFactualInfo">(Optional)Not entirely sure. Official documentation says: field.UpdateAccountRequestV3.counterFactualInfo</param>
        /// <returns>An object containing the status of the transfer at the end of the request</returns>
        /// <exception cref="System.Exception">Gets thrown when there's a problem getting info from the Loopring API endpoint</exception>
        public virtual Task<OperationResult> Transfer(TransferRequest request, string memo, string clientId, CounterFactualInfo counterFactualInfo = null)
        {
            return _metamaskClient.Transfer(_apiKey, _loopringPrivateKey, _ethPrivateKey, request, memo, clientId, counterFactualInfo);
        }

        /// <summary>
        /// Send some tokens to anyone else on L2
        /// </summary>
        /// <param name="toAddress">The loopring address that's doing the receiving</param>
        /// <param name="token">What token is being sent</param>
        /// <param name="value">And how much of that token are we sending</param>
        /// <param name="feeToken">In what token are we paying the fee</param>
        /// <param name="memo">(Optional)And do you want the transaction to contain a reference. From loopring's perspective, this is just a text field</param>
        /// <returns>An object containing the status of the transfer at the end of the request</returns>
        public virtual async Task<OperationResult> Transfer(string toAddress, string token, decimal value, string feeToken, string memo)
        {
            return await _metamaskClient.Transfer(_apiKey, _loopringPrivateKey, _ethPrivateKey, _accountId, _ethAddress, toAddress, token, value, feeToken, memo).ConfigureAwait(false);
        }

        /// <summary>
        /// WARNING!!! This has a fee asociated with it. Make a OffchainFee request of type OffChainRequestType.UpdateAccount to see what the fee is.
        /// Updates the EDDSA key associated with the specified account, making the previous one invalid in the process.
        /// </summary>   
        /// <param name="feeToken">The token in which the fee should be paid for this operation</param>
        /// <returns>Returns the hash and status of your requested operation</returns>
        public virtual Task<OperationResult> RequestNewL2PrivateKey(string feeToken)
        {
            return _metamaskClient.UpdateAccount(_apiKey, _ethPrivateKey, _loopringPrivateKey, _accountId, feeToken, _ethAddress, ExchangeInfo().Result.exchangeAddress);
        }

        private static string GetEthereumAddress(string apiUrl)
        {
            var l2Auth = EDDSAHelper.GetL2PKFromMetaMask(LoopringSharp.SecureClient.ExchangeInfo(apiUrl).Result.exchangeAddress, apiUrl);
            return l2Auth.ethAddress;
        }
    }
}
