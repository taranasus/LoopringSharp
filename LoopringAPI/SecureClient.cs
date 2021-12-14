using Newtonsoft.Json;
using PoseidonSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
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
            if (useTestNet)
            {
                _apiUrl = "https://uat2.loopring.io/";
            }
            else
            {
                _apiUrl = "https://api3.loopring.io/";
            }
        }

        #region NoAuthentication
        /// <summary>
        /// Gets the current exchange prices between varius cryptos on the Loopring Protocol
        /// </summary>        
        /// <param name="pairs">The tickers to retreive. (Ex. LRC-USDT, LRC-ETH)</param>
        /// <returns>Returns a list of all the ticker details for your requested tickers</returns>
        /// <exception cref="System.Exception">Gets thrown when there's a problem getting info from the Loopring API endpoint</exception>
        public async Task<List<Ticker>> Ticker(params string[] pairs)
        {
            string url = $"{_apiUrl}{Constants.TickerUrl}?market={string.Join(",", pairs)}";
            using (var httpResult = await _client.GetAsync(url))
            {
                _ = await IsApiSuccess(httpResult);
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
        }

        /// <summary>
        /// Returns the relayer's current time in millisecond
        /// </summary>
        /// <returns>Current time in milliseconds</returns>
        /// <exception cref="System.Exception">Gets thrown when there's a problem getting info from the Loopring API endpoint</exception>
        public async Task<long> Timestamp()
        {
            var url = $"{_apiUrl}{Constants.TimestampUrl}";
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (var httpResult = await _client.SendAsync(httpRequest))
                {
                    _ = await IsApiSuccess(httpResult);
                    var resultBody = await httpResult.Content.ReadAsStringAsync();
                    var apiresult = JsonConvert.DeserializeObject<ApiTimestampResult>(resultBody);
                    return apiresult.timestamp;
                }
            }
        }

        #endregion

        #region L2

        /// <summary>
        /// Get the ApiKey associated with the user's account.
        /// </summary>
        /// <param name="l2Pk">Wallet Layer 2 Private Key</param>
        /// <param name="accountId">The user's account Id</param>
        /// <returns>The api key</returns>
        /// <exception cref="System.Exception">Gets thrown when there's a problem getting info from the Loopring API endpoint</exception>
        public async Task<string> ApiKey(string l2Pk, string accountId)
        {
            var signatureBase = "";
            signatureBase += "GET&" + UrlEncodeUpperCase(_apiUrl + Constants.ApiKeyUrl) + "&";
            var parameterString = "accountId=" + accountId;
            signatureBase += UrlEncodeUpperCase(parameterString);
            var message = SHA256Helper.CalculateSHA256HashNumber(signatureBase);

            var signer = new Eddsa(message, l2Pk);
            var signedMessage = signer.Sign();

            var url = $"{_apiUrl}{Constants.ApiKeyUrl}?accountId={accountId}";
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, url))
            {
                httpRequest.Headers.Add("X-API-SIG", signedMessage);
                using (var httpResult = await _client.SendAsync(httpRequest))
                {
                    _ = await IsApiSuccess(httpResult);
                    var resultBody = await httpResult.Content.ReadAsStringAsync();
                    var apiresult = JsonConvert.DeserializeObject<ApiApiKeyResult>(resultBody);
                    return apiresult.apiKey;
                }
            }
        }

        #endregion
        #region apiKeyL2

        /// <summary>
        /// Change the ApiKey associated with the user's account
        /// </summary>
        /// <param name="l2Pk">Loopring Private Key</param>
        /// <param name="apiKey">Current Loopring API Key</param>
        /// <param name="accountId">Wallet Account Id</param>
        /// <returns>The new apiKey as string</returns>
        /// <exception cref="System.Exception">Gets thrown when there's a problem getting info from the Loopring API endpoint</exception>
        public async Task<string> UpdateApiKey(string l2Pk, string apiKey, string accountId)
        {
            string requestBody = "{\"accountId\":" + accountId + "}";

            var signatureBase = "";
            signatureBase += "POST&" + UrlEncodeUpperCase(_apiUrl + Constants.ApiKeyUrl) + "&";
            var parameterString = requestBody;
            signatureBase += UrlEncodeUpperCase(parameterString);
            var message = SHA256Helper.CalculateSHA256HashNumber(signatureBase);

            var signer = new Eddsa(message, l2Pk);
            var signedMessage = signer.Sign();

            var url = $"{_apiUrl}{Constants.ApiKeyUrl}";
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, url))
            {
                httpRequest.Headers.Add("X-API-SIG", signedMessage);
                httpRequest.Headers.Add("X-API-KEY", apiKey);
                httpRequest.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                using (var httpResult = await _client.SendAsync(httpRequest))
                {
                    _ = IsApiSuccess(httpResult);
                    var resultBody = await httpResult.Content.ReadAsStringAsync();
                    var apiresult = JsonConvert.DeserializeObject<ApiApiKeyResult>(resultBody);
                    return apiresult.apiKey;
                }
            }
        }

        #endregion


        #region apiKey

        /// <summary>
        /// Fetches the next order id for a given sold token
        /// </summary>
        /// <param name="apiKey">Your Loopring API Key</param>
        /// <param name="accountId">Loopring account identifier</param>
        /// <param name="sellTokenId">The unique identifier of the token which the user wants to sell in the next order.</param>
        /// <param name="maxNext">Return the max of the next available storageId, so any storageId > returned value is avaliable, to help user manage storageId by themselves. for example, if [20, 60, 100] is avaliable, all other ids < 100 is used before, user gets 20 if flag is false (and 60 in next run), but gets 100 if flag is true, so he can use 102, 104 freely</param>
        /// <returns>Returns an object instance of StorageId which contains the next offchainId and orderId</returns>
        /// <exception cref="System.Exception">Gets thrown when there's a problem getting info from the Loopring API endpoint</exception>
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
        /// <exception cref="System.Exception">Gets thrown when there's a problem getting info from the Loopring API endpoint</exception>
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
                        fees = apiresult.fees,
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
        #endregion

        #region apiKeyL1L2
        public async Task<Transfer> Transfer(string apiKey, string l2Pk, string l1Pk, TransferRequest request, string memo, string clientId, CounterFactualInfo counterFactualInfo)
        {
            throw new NotImplementedException("Still working on it...");

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new System.Exception("Transfer REQUIRES a valid Loopring wallet apiKey");
            if (string.IsNullOrWhiteSpace(l2Pk))
                throw new System.Exception("Transfer REQUIRES a valid Loopring Wallet Layer 2 Private key");
            if (string.IsNullOrWhiteSpace(l1Pk))
                throw new System.Exception("Transfer REQUIRES a valid Eth Wallet Layer 1 Private key");

            string apiSig = ""; //Need to generate            

            int MAX_INPUT = 13;
            var poseidonHasher = new Poseidon(MAX_INPUT + 1, 6, 53, "poseidon", 5, _securityTarget: 128);
            BigInteger[] inputs = {
                BigInteger.Parse(request.exchange, System.Globalization.NumberStyles.HexNumber),
                (BigInteger)request.payerId,
                (BigInteger)request.payeeId,
                (BigInteger)request.token.tokenId,
                BigInteger.Parse(request.token.volume),
                (BigInteger)request.maxFee.tokenId,
                BigInteger.Parse(request.maxFee.volume),
                BigInteger.Parse(request.payeeAddress, System.Globalization.NumberStyles.HexNumber),
                0,
                0,
                (BigInteger)request.validUnitl,
                (BigInteger)request.storageId
            };

            var apiRequest = request.GetApiTransferRequest(memo, clientId, counterFactualInfo);

            var signer = new Eddsa(poseidonHasher.CalculatePoseidonHash(inputs), l2Pk);
            var signedMessage = signer.Sign(apiRequest);
            apiRequest.eddsaSignature = signedMessage;

            // TODO : Compare Eddsa with python result

            // TODO : Implement ECDSA


            var url = $"{_apiUrl}{Constants.TransferUrl}";
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, url))
            {
                httpRequest.Headers.Add("X-API-KEY", apiKey);
                httpRequest.Headers.Add("X-API-SIG", apiSig);

                using (var stringContent = new StringContent(JsonConvert.SerializeObject(apiRequest), Encoding.UTF8, "application/json"))
                {
                    httpRequest.Content = stringContent;
                    using (var httpResult = await _client.SendAsync(httpRequest))
                    {
                        _ = await IsApiSuccess(httpResult);
                        var resultBody = await httpResult.Content.ReadAsStringAsync();
                        var apiresult = JsonConvert.DeserializeObject<ApiTransferResult>(resultBody);
                        return new Transfer(apiresult);
                    }
                }
            }
        }
        #endregion

        private static string UrlEncodeUpperCase(string stringToEncode)
        {
            var reg = new Regex(@"%[a-f0-9]{2}");
            stringToEncode = HttpUtility.UrlEncode(stringToEncode);
            return reg.Replace(stringToEncode, m => m.Value.ToUpperInvariant());
        }

        private async Task<bool> IsApiSuccess(HttpResponseMessage httpResult)
        {
            if (httpResult.IsSuccessStatusCode)
                return true;
            if (httpResult.Content != null)
                throw new System.Exception("Error from Loopring API: " + httpResult.StatusCode.ToString() + " | " + (await httpResult.Content.ReadAsStringAsync()));
            throw new System.Exception("Error from Loopring API: " + httpResult.StatusCode.ToString());
        }
    }

}
