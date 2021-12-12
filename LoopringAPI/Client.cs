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
        private SecureClient _client;

        /// <summary>
        /// The Object you need in order to communicate with the Loopring API. Recommended to use as a signleton.
        /// </summary>
        /// <param name="apiKey">Your wallet API Key, needed for almost all api calls</param>
        /// <param name="loopringPrivateKey">Your Layer 2 Private Key, needed for most api calls</param>
        /// <param name="ethPrivateKey">Your Layer 1, Ethereum Private Key, needed for some very specific API calls</param>
        public Client(string apiKey, string loopringPrivateKey, string ethPrivateKey)
        {
            _apiKey = apiKey;
            _loopringPrivateKey = loopringPrivateKey;
            _ethPrivateKey = ethPrivateKey;
            _client = new SecureClient();
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
    }
}
