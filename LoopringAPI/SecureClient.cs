using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LoopringAPI
{
    public class SecureClient
    {
        HttpClient _client;
        public SecureClient()
        {
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
