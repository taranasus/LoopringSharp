using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoopringAPI
{
    public class Client
    {
        private string _apiKey;
        private string _ethPrivateKey;
        private string _loopringPrivateKey;
        private string _accountId;
        private SecureClient _client;

        /// <summary>
        /// The Object you need in order to communicate with the Loopring API. Recommended to use as a signleton.
        /// </summary>
        /// <param name="apiKey">Your wallet API Key, needed for almost all api calls</param>
        /// <param name="loopringPrivateKey">Your Layer 2 Private Key, needed for most api calls</param>
        /// <param name="ethPrivateKey">Your Layer 1, Ethereum Private Key, needed for some very specific API calls</param>
        /// <param name="accountId">Your Loopring Account ID, used for a surprising amount of calls</param>
        public Client(string apiKey, string loopringPrivateKey, string ethPrivateKey, string accountId, bool useTestNet)
        {
            _apiKey = apiKey;
            _loopringPrivateKey = loopringPrivateKey;
            _ethPrivateKey = ethPrivateKey;
            _client = new SecureClient(useTestNet);
            _accountId = accountId;
        }

        /// <summary>
        /// Returns the relayer's current time in millisecond
        /// </summary>
        /// <returns>Current time in milliseconds</returns>        
        public Task<long> Timestamp()
        {
            return _client.Timestamp();
        }

        /// <summary>
        /// Gets the current exchange prices between varius cryptos on the Loopring Protocol
        /// </summary>        
        /// <param name="pairs">The tickers to retreive. (Ex. LRC-USDT, LRC-ETH)</param>
        /// <returns>Returns a list of all the ticker details for your requested tickers</returns>
        public Task<List<Ticker>> Ticker(params string[] pairs)
        {
            return _client.Ticker(pairs);
        }

        /// <summary>
        /// Get the ApiKey associated with the user's account.
        /// </summary>
        /// <returns>The api key</returns>
        /// <exception cref="System.Exception">Gets thrown when there's a problem getting info from the Loopring API endpoint</exception>
        public Task<string> ApiKey()
        {
            return _client.ApiKey(_loopringPrivateKey, _accountId);
        }

        /// <summary>
        /// Fetches the next order id for a given sold token
        /// </summary>        
        /// <param name="accountId">Loopring account identifier</param>
        /// <param name="sellTokenId">The unique identifier of the token which the user wants to sell in the next order.</param>
        /// <param name="maxNext">Return the max of the next available storageId, so any storageId > returned value is avaliable, to help user manage storageId by themselves. for example, if [20, 60, 100] is avaliable, all other ids < 100 is used before, user gets 20 if flag is false (and 60 in next run), but gets 100 if flag is true, so he can use 102, 104 freely</param>
        /// <returns>Returns an object instance of StorageId which contains the next offchainId and orderId</returns>
        public Task<StorageId> StorageId(int sellTokenId, int maxNext = 0)
        {
            return _client.StorageId(_apiKey, _accountId, sellTokenId, maxNext);
        }

        /// <summary>
        /// Get how much fee you need to pay right now to carry out a transaction of a specified type
        /// </summary>        
        /// <param name="accountId">Loopring account identifier</param>
        /// <param name="requestType">Off-chain request type</param>
        /// <param name="tokenSymbol">Required only for withdrawls - The token you wish to withdraw</param>
        /// <param name="amount">Required only for withdrawls - how much of that token you wish to withdraw</param>
        /// <returns>Returns the fee amount</returns>
        public Task<OffchainFee> OffchainFee(OffChainRequestType requestType, string tokenSymbol, string amount)
        {
            return _client.OffchainFee(_apiKey, _accountId, requestType, tokenSymbol, amount);
        }
    }
}
