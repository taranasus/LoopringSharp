using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static LoopringAPI.ApiTransferRequest;

namespace LoopringAPI
{
    public class SecureClient
    {
        string _apiUrl;
        HttpClient _client;
        public SecureClient(bool useTestNet)
        {
            _client = new HttpClient();
            if(useTestNet)
            {
                _apiUrl = "https://uat2.loopring.io/";
            }
            else
            {
                _apiUrl = "https://api3.loopring.io/";
            }
        }

        /// <summary>
        /// Gets the current exchange prices between varius cryptos on the Loopring Protocol
        /// </summary>        
        /// <param name="pairs">The tickers to retreive. (Ex. LRC-USDT, LRC-ETH)</param>
        /// <returns>Returns a list of all the ticker details for your requested tickers</returns>
        public async Task<List<Ticker>> Ticker(params string[] pairs)
        {
            string url = $"{_apiUrl}{Constants.TickerUrl}?market={string.Join(",", pairs)}";
            using (var httpResult = await _client.GetAsync(url))
            {
                if (httpResult.IsSuccessStatusCode)
                {
                    var resultBody = await httpResult.Content.ReadAsStringAsync();
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
                else
                {
                    if (httpResult.Content != null)
                    {
                        throw new System.Exception("Error from Loopring API: " + httpResult.StatusCode.ToString() + " | " + (await httpResult.Content.ReadAsStringAsync()));
                    }
                    throw new System.Exception("Error from Loopring API: " + httpResult.StatusCode.ToString());
                }
            }
        }

        /// <summary>
        /// Fetches the next order id for a given sold token
        /// </summary>
        /// <param name="apiKey">Your Loopring API Key</param>
        /// <param name="accountId">Loopring account identifier</param>
        /// <param name="sellTokenId">The unique identifier of the token which the user wants to sell in the next order.</param>
        /// <param name="maxNext">Return the max of the next available storageId, so any storageId > returned value is avaliable, to help user manage storageId by themselves. for example, if [20, 60, 100] is avaliable, all other ids < 100 is used before, user gets 20 if flag is false (and 60 in next run), but gets 100 if flag is true, so he can use 102, 104 freely</param>
        /// <returns>Returns an object instance of StorageId which contains the next offchainId and orderId</returns>
        public async Task<StorageId> StorageId(string apiKey, string accountId, int sellTokenId, int maxNext = 0)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new System.Exception("StorageId REQUIRES a valid Loopring wallet apiKey");
            var url = $"{_apiUrl}{Constants.StorageIdUrl}?accountId={accountId}&sellTokenId={sellTokenId}&maxNext={maxNext}";
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, url))
            {
                httpRequest.Headers.Add("X-API-KEY", apiKey);
                var httpResult = await _client.SendAsync(httpRequest);
                if (httpResult.IsSuccessStatusCode)
                {
                    var resultBody = await httpResult.Content.ReadAsStringAsync();
                    var apiresult = JsonConvert.DeserializeObject<ApiStorageIdResult>(resultBody);
                    return new StorageId()
                    {
                        offchainId = apiresult.offchainId,
                        orderId = apiresult.orderId
                    };
                }
                else
                {
                    if (httpResult.Content != null)
                    {
                        throw new System.Exception("Error from Loopring API: " + httpResult.StatusCode.ToString() + " | " + (await httpResult.Content.ReadAsStringAsync()));
                    }
                    throw new System.Exception("Error from Loopring API: " + httpResult.StatusCode.ToString());
                }
            }
        }

        /// <summary>
        /// Get how much fee you need to pay right now to carry out a transaction of a specified type
        /// </summary>
        /// <param name="apiKey">Your Loopring API Key</param>
        /// <param name="accountId">Loopring account identifier</param>
        /// <param name="requestType">Off-chain request type</param>
        /// <param name="tokenSymbol">Required only for withdrawls - The token you wish to withdraw</param>
        /// <param name="amount">Required only for withdrawls - how much of that token you wish to withdraw</param>
        /// <returns>Returns the fee amount</returns>
        public async Task<OffchainFee> OffchainFee(string apiKey, string accountId, OffChainRequestType requestType, string tokenSymbol, string amount)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new System.Exception("StorageId REQUIRES a valid Loopring wallet apiKey");
            var url = $"{_apiUrl}{Constants.OffchainFeeUrl}?accountId={accountId}&requestType={(int)requestType}&tokenSymbol={tokenSymbol}&amount={amount}";
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, url))
            {
                httpRequest.Headers.Add("X-API-KEY", apiKey);
                var httpResult = await _client.SendAsync(httpRequest);
                if (httpResult.IsSuccessStatusCode)
                {
                    var resultBody = await httpResult.Content.ReadAsStringAsync();
                    var apiresult = JsonConvert.DeserializeObject<ApiOffchainFeeResult>(resultBody);
                    return new OffchainFee()
                    {
                        fees =apiresult.fees,
                        gasPrice = apiresult.gasPrice
                    };
                }
                else
                {
                    if (httpResult.Content != null)
                    {
                        throw new System.Exception("Error from Loopring API: " + httpResult.StatusCode.ToString() + " | " + (await httpResult.Content.ReadAsStringAsync()));
                    }
                    throw new System.Exception("Error from Loopring API: " + httpResult.StatusCode.ToString());
                }
            }
        }


        public async Task<Transfer> Transfer(string apiKey, string l2Pk, string l1Pk, TransferRequest request, string memo, string clientId, CounterFactualInfo counterFactualInfo)
        {
            throw new NotImplementedException("Still working on it...");

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new System.Exception("Transfer REQUIRES a valid Loopring wallet apiKey");
            if (string.IsNullOrWhiteSpace(l2Pk))
                throw new System.Exception("Transfer REQUIRES a valid Loopring Wallet Layer 2 Private key");
            if (string.IsNullOrWhiteSpace(l1Pk))
                throw new System.Exception("Transfer REQUIRES a valid Eth Wallet Layer 1 Private key");

            string poseidon = ""; //Need to generate
            string ecdsa = ""; //Need to generate
            string eddsa = ""; //Need to generate
            string apiSig = ""; //Need to generate            

            var apiRequest = request.GetApiTransferRequest(poseidon,ecdsa, eddsa, memo, clientId, counterFactualInfo);

            var url = $"{_apiUrl}{Constants.TransferUrl}";
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, url))
            {
                httpRequest.Headers.Add("X-API-KEY", apiKey);
                httpRequest.Headers.Add("X-API-SIG", apiSig);

                using (var stringContent = new StringContent(JsonConvert.SerializeObject(apiRequest), Encoding.UTF8, "application/json"))
                {
                    httpRequest.Content = stringContent;
                    var httpResult = await _client.SendAsync(httpRequest);
                    if (httpResult.IsSuccessStatusCode)
                    {
                        var resultBody = await httpResult.Content.ReadAsStringAsync();
                        var apiresult = JsonConvert.DeserializeObject<ApiTransferResult>(resultBody);
                        return new Transfer(apiresult);
                    }
                    else
                    {
                        if (httpResult.Content != null)
                        {
                            throw new System.Exception("Error from Loopring API: " + httpResult.StatusCode.ToString() + " | " + (await httpResult.Content.ReadAsStringAsync()));
                        }
                        throw new System.Exception("Error from Loopring API: " + httpResult.StatusCode.ToString());
                    }
                }
            }
        }
    }
}
