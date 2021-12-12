using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LoopringAPI
{
    public class Client
    {
        private string _apiKey;
        private string _ethPrivateKey;
        private string _loopringPrivateKey;
        private HttpClient _client;

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
            _client = new HttpClient();
        }

        /// <summary>
        /// Gets the current exchange prices between varius cryptos on the Loopring Protocol
        /// </summary>        
        /// <param name="pairs">The tickers to retreive. (Ex. LRC-USDT, LRC-ETH)</param>
        /// <returns>Returns a list of all the ticker details for your requested tickers</returns>
        public async Task<List<Ticker>> Ticker(params string[] pairs)
        {
            string url = Constants.ApiUrl + Constants.TickerUrl + "?market=" + string.Join(",", pairs);
            var apiResult = await _client.GetAsync(url);
            var resultBody = await apiResult.Content.ReadAsStringAsync();
            var apiTickersResult = JsonConvert.DeserializeObject<ApiTickersResult>(resultBody);

            return apiTickersResult.tickers.Select(s => new Ticker()
            {
                PairId = s[0],
                TimeStamp = s[1],
                BaseTokenVolume = s[2],
                QuoteTokenVolume = s[3],
                OpenPrice = s[4],
                HeighestPrice = s[5],
                LowestPrice = s[6],
                ClosingPrice = s[7],
                NumberOfTrades = s[8],
                HighestBidPrice = s[9],
                LowestAskPrice = s[10],
                BaseFeeAmmount = s[11],
                QuoteFeeAmount = s[12]
            }).ToList();                        
        }
    }
}
