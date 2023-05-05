using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace LoopringSharp.WalletConnect
{
    public class WalletConnectClient : LoopringSharp.Client
    {
        WalletConnectSecretClient _walletConnectClient;
        public WalletConnectClient(string apiUrl) : base(apiUrl, GetEthereumAddress())
        {
            _walletConnectClient = new WalletConnectSecretClient(apiUrl);
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
        public virtual OperationResult Transfer(TransferRequest request, string memo, string clientId, CounterFactualInfo counterFactualInfo = null)
        {
            return _walletConnectClient.Transfer(_apiKey, _loopringPrivateKey, _ethPrivateKey, request, memo, clientId, counterFactualInfo);
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
        public virtual OperationResult Transfer(string toAddress, string token, decimal value, string feeToken, string memo)
        {
            return _walletConnectClient.Transfer(_apiKey, _loopringPrivateKey, _ethPrivateKey, _accountId, _ethAddress, toAddress, token, value, feeToken, memo);
        }

        /// <summary>
        /// WARNING!!! This has a fee asociated with it. Make a OffchainFee request of type OffChainRequestType.UpdateAccount to see what the fee is.
        /// Updates the EDDSA key associated with the specified account, making the previous one invalid in the process.
        /// </summary>   
        /// <param name="feeToken">The token in which the fee should be paid for this operation</param>
        /// <returns>Returns the hash and status of your requested operation</returns>
        public virtual OperationResult RequestNewL2PrivateKey(string feeToken)
        {
            return _walletConnectClient.UpdateAccount(_apiKey, _ethPrivateKey, _loopringPrivateKey, _accountId, feeToken, _ethAddress, ExchangeInfo().exchangeAddress);
        }

        private static (string secretKey, string ethAddress, string publicKeyX, string publicKeyY) GetEthereumAddress()
        {
            // TODO: THIS NEEDS REFACTORING INTO RETURNING ALL THAT INFO NOT JUST ETH ADDRESS

            string connectURi = WalletConnectServer.Connect();
            Debug.WriteLine("Connection: " + connectURi);
            Console.WriteLine("WalletConnect CODE: " + connectURi);
            File.WriteAllText("walletconnect.html",
                Constants.WalletConnectHTML.Replace("|----|", $"https://api.qrserver.com/v1/create-qr-code/?data={HttpUtility.UrlEncode(connectURi)}!&size=400x400")
                .Replace("|--|--|", connectURi));
            var browser = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo(Directory.GetCurrentDirectory() + "/walletconnect.html") { UseShellExecute = true }
            };
            browser.Start();

            File.Delete("walletconnect.html");
            browser.Kill();

            return (null, null, null, null);
            //return WalletConnectServer.GetEthAddress();            
        }
    }
}
