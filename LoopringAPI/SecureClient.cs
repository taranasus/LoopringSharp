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
        ExchangeInfo _exchange;
        HttpClient _client;

        public SecureClient(string apiUrl)
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
            {
                return true;
            };
            _client = new HttpClient(httpClientHandler);
            _apiUrl = apiUrl;
            _ = GetTokenId("ETH");
            _exchange = ExchangeInfo().Result;
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
            (string, string)[] parameters = { ("market", string.Join(",", pairs)) };
            (string, string)[] headers = null;

            var apiTickersResult = JsonConvert.DeserializeObject<ApiTickersResult>(
                await Http(Constants.TickerUrl, parameters, headers));

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

        /// <summary>
        /// Returns the relayer's current time in millisecond
        /// </summary>
        /// <returns>Current time in milliseconds</returns>
        /// <exception cref="System.Exception">Gets thrown when there's a problem getting info from the Loopring API endpoint</exception>
        public async Task<long> Timestamp()
        {
            (string, string)[] parameters = null;
            (string, string)[] headers = null;

            var apiresult = JsonConvert.DeserializeObject<ApiTimestampResult>(
                await Http(Constants.TimestampUrl, parameters, headers));
            return apiresult.timestamp;
        }

        /// <summary>
        /// Returns data associated with the user's exchange account.
        /// </summary>
        /// <param name="address">Ethereum / Loopring public address</param>
        /// <returns>A lot of data about the account</returns>
        public async Task<Account> GetAccountInfo(string address)
        {

            (string, string)[] parameters = { ("owner", address) };
            (string, string)[] headers = null;

            var apiresult = JsonConvert.DeserializeObject<ApiAccountResult>(

                await Http(Constants.AccountUrl, parameters, headers));

            return new Account()
            {
                accountId = apiresult.accountId,
                frozen = apiresult.frozen,
                keyNonce = apiresult.keyNonce,
                keySeed = apiresult.keySeed,
                nonce = apiresult.nonce,
                owner = apiresult.owner,
                publicKey = apiresult.publicKey,
                tags = apiresult.tags
            };
        }

        /// <summary>
        /// Return various configurations of Loopring.io
        /// </summary>
        /// <returns>Fees, exchange address, all sort of useful stuff</returns>
        public async Task<ExchangeInfo> ExchangeInfo()
        {
            (string, string)[] parameters = null;
            (string, string)[] headers = null;

            var apiresult = JsonConvert.DeserializeObject<ApiExchangeInfoResult>(

                await Http(Constants.InfoUrl, parameters, headers));

            return new ExchangeInfo()

            {

                ammExitFees = apiresult.ammExitFees,

                chainId = apiresult.chainId,

                depositAddress = apiresult.depositAddress,

                exchangeAddress = apiresult.exchangeAddress,

                fastWithdrawalFees = apiresult.fastWithdrawalFees,

                onchainFees = apiresult.onchainFees,

                openAccountFees = apiresult.openAccountFees,

                transferFees = apiresult.transferFees,

                updateFees = apiresult.updateFees,

                withdrawalFees = apiresult.withdrawalFees

            };
        }

        /// <summary>
        /// Get a list of all the markets available on the exchange
        /// </summary>
        /// <returns>List of all the markets available on the exchange and their configurations</returns>
        public async Task<List<Market>> GetMarkets()
        {

            (string, string)[] parameters = null;
            (string, string)[] headers = null;

            var apiresult = JsonConvert.DeserializeObject<ApiMarketsGetResult>(

                await Http(Constants.MarketsUrl, parameters, headers));

            return apiresult.markets;
        }

        /// <summary>
        /// Returns the configurations of all supported tokens, including Ether.
        /// </summary>
        /// <returns>List of all the supported tokens and their configurations</returns>
        public async Task<List<TokenConfig>> GetTokens()
        {
            (string, string)[] parameters = null;
            (string, string)[] headers = null;

            return JsonConvert.DeserializeObject<List<TokenConfig>>(

                await Http(Constants.TokensUrl, parameters, headers));
        }

        /// <summary>
        /// Returns the order book of a given trading pair.
        /// </summary>
        /// <param name="market">The ID of a trading pair.</param>
        /// <param name="level">Order book aggregation level, larger value means further price aggregation. Default: 2</param>
        /// <param name="limit">Maximum numbers of bids/asks. Default : 50</param>
        /// <returns>Returns the order book of a given trading pair.</returns>
        public async Task<Depth> GetDepth(string market, int level = 2, int limit = 50)
        {
            (string, string)[] parameters = { ("market", market), ("level", level.ToString()), ("limit", limit.ToString()) };
            (string, string)[] headers = null;
            var apiresult = JsonConvert.DeserializeObject<ApiDepthResult>(
                await Http(Constants.DepthUrl, parameters, headers));
            return new Depth()
            {
                asks = apiresult.asks,
                market = apiresult.market,
                bids = apiresult.bids,
                timestamp = apiresult.timestamp,
                version = apiresult.version
            };
        }

        /// <summary>
        /// Return the candlestick data of a given trading pair.
        /// </summary>
        /// <param name="market">Trading pair ID, multi-market is not supported</param>
        /// <param name="intervals">Candlestick interval</param>
        /// <param name="start">(Optional)Start time in milliseconds</param>
        /// <param name="end">(Optional)End time in milliseconds</param>
        /// <param name="limit">(Optional)Number of data points. If more data points are available, the API will only return the first 'limit' data points.</param>
        /// <returns>List of candlesticks... what else?</returns>
        public async Task<List<Candlestick>> GetCandlesticks(string market, Intervals intervals, string start = null, string end = null, int? limit = null)
        {
            (string, string)[] parameters =
            {
                ("market", market),
                ("interval", Utils.IntervalsEnumToString(intervals)),
                ("start", start),
                ("end", end),
                ("limit", limit.ToString())
            };
            (string, string)[] headers = null;
            return JsonConvert.DeserializeObject<ApiCandlestickResult>(
                await Http(Constants.CandlestickUrl, parameters, headers)).candlesticks.Select(s => new Candlestick()
                {
                    startTime = s[0],
                    numberOfTransactions = long.Parse(s[1]),
                    open = decimal.Parse(s[2]),
                    close = decimal.Parse(s[3]),
                    high = decimal.Parse(s[4]),
                    low = decimal.Parse(s[5]),
                    baseTokenVolume = decimal.Parse(s[6]),
                    quoteTokenVolume = decimal.Parse(s[7]),
                }).ToList();

        }

        /// <summary>
        /// Fetches, for all the tokens supported by Loopring, their fiat price.
        /// </summary>
        /// <param name="legal">The fiat currency to uses. Currently the following values are supported: USD,CNY,JPY,EUR,GBP,HKD</param>
        /// <returns>Fiat price of all the tokens in the system</returns>
        public async Task<List<Price>> GetPrice(LegalCurrencies legal)
        {
            (string, string)[] parameters = { ("legal", legal.ToString())};
            (string, string)[] headers = null;
            var apiresult = JsonConvert.DeserializeObject<ApiPriceResult>(
                await Http(Constants.PriceUrl, parameters, headers));
            return apiresult.prices;
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
        public async Task<string> ApiKey(string l2Pk, int accountId)
        {
            var signedMessage = EddsaSignUrl(
                l2Pk,
                HttpMethod.Get,
                new List<(string Key, string Value)>() { ("accountId", accountId.ToString()) },
                null,
                Constants.ApiKeyUrl);

            (string, string)[] parameters = { ("accountId", accountId.ToString()) };
            (string, string)[] headers = { (Constants.HttpHeaderAPISigName, signedMessage) };
            var apiresult = JsonConvert.DeserializeObject<ApiApiKeyResult>(
                await Http(Constants.ApiKeyUrl, parameters, headers));
            return apiresult.apiKey;
        }

        #endregion
        #region apiKeyL2

        /// <summary>
        /// Submit an order to exchange two currencies, but with all the nonsense removed
        /// </summary>
        /// <param name="l2Pk">Loopring Private Key</param>
        /// <param name="apiKey">Current Loopring API Key</param>
        /// <param name="accountId">Wallet Account Id</param>
        /// <param name="orderHash">The hash of the order you wish to nuke.</param>
        /// <param name="clientOrderId">The unique order ID of the client</param>
        /// <returns>Returns OrderResult which basically contains the status of your transaction after the cancel was succesfully requested</returns>
        public async Task<OrderResult> DeleteOrder(string l2Pk, string apiKey, int accountId, string orderHash, string clientOrderId)
        {
            var signedMessage = EddsaSignUrl(
                l2Pk,
                HttpMethod.Delete,
                new List<(string Key, string Value)>() { ("accountId", accountId.ToString()), ("clientOrderId", clientOrderId), ("orderHash", orderHash) },
                null,
                Constants.OrderUrl);

            (string, string)[] parameters = { ("accountId", accountId.ToString()), ("clientOrderId", clientOrderId), ("orderHash", orderHash)  };
            (string, string)[] headers = { (Constants.HttpHeaderAPISigName, signedMessage), (Constants.HttpHeaderAPIKeyName, apiKey) };
            var apiresult = JsonConvert.DeserializeObject<ApiOrderSubmitResult>(
                await Http(Constants.OrderUrl, parameters, headers, "delete"));
            return new OrderResult(apiresult);
        }


        /// <summary>
        /// Submit an order to exchange two currencies, but with all the nonsense removed
        /// </summary>
        /// <param name="l2Pk">Loopring Private Key</param>
        /// <param name="apiKey">Current Loopring API Key</param>
        /// <param name="accountId">Wallet Account Id</param>
        /// <param name="sellCurrency">The name of the token you are selling (ETH, LRC, USDT, etc)</param>
        /// <param name="sellAmmount">How much of that token you are selling</param>
        /// <param name="buyCurrency">The name of the token you are buying (ETH, LRC, USDT, etc)</param>
        /// <param name="buyAmmount">How much of that token you are buying</param>        
        /// <param name="orderType">Order types, can be AMM, LIMIT_ORDER, MAKER_ONLY, TAKER_ONLY</param>
        /// <param name="poolAddress">The AMM pool address if order type is AMM</param>
        /// <returns>Returns OrderResult which basically contains the status of your transaction after it was succesfully requested</returns>
        public Task<OrderResult> SubmitOrder(string l2Pk, string apiKey, int accountId,
            string sellCurrency,
            decimal sellAmmount,
            string buyCurrency,
            decimal buyAmmount,
            OrderType orderType,
            string poolAddress = null)
        {
            var tradeChannel = TradeChannel.MIXED;
            if (orderType == OrderType.MAKER_ONLY)
                tradeChannel = TradeChannel.ORDER_BOOK;

            return SubmitOrder(l2Pk, apiKey, accountId,
                new Token() { tokenId = GetTokenId(sellCurrency), volume = (sellAmmount * 1000000000000000000m).ToString("0") },
                new Token() { tokenId = GetTokenId(buyCurrency), volume = (buyAmmount * 1000000000000000000m).ToString("0") },
                false,
                false,
                GetUnixTimestamp() + (int)TimeSpan.FromDays(365).TotalSeconds, // one year
                63,
                null,
                orderType,
                tradeChannel,
                null,
                poolAddress,
                null);
        }

        /// <summary>
        /// Submit an order to exchange two currencies
        /// </summary>
        /// <param name="l2Pk">Loopring Private Key</param>
        /// <param name="apiKey">Current Loopring API Key</param>
        /// <param name="accountId">Wallet Account Id</param>
        /// <param name="sellToken">The token you are selling</param>
        /// <param name="buyToken">The token you are buying</param>
        /// <param name="allOrNone">Whether the order supports partial fills or not.Currently only supports false as a valid value</param>
        /// <param name="fillAmountBOrS">Fill size by buy token or by sell token</param>
        /// <param name="validUntil">Order expiration time, accuracy is in seconds</param>
        /// <param name="maxFeeBips">Maximum order fee that the user can accept, value range (in ten thousandths) 1 ~ 63</param>
        /// <param name="clientOrderId">An arbitrary, client-set unique order identifier, max length is 120 bytes</param>
        /// <param name="orderType">Order types, can be AMM, LIMIT_ORDER, MAKER_ONLY, TAKER_ONLY</param>
        /// <param name="tradeChannel">	Order channel, can be ORDER_BOOK, AMM_POOL, MIXED</param>
        /// <param name="taker">Used by the P2P order which user specify the taker, so far its 0x0000000000000000000000000000000000000000</param>
        /// <param name="poolAddress">The AMM pool address if order type is AMM</param>
        /// <param name="affiliate">An accountID who will recieve a share of the fee of this order</param>
        /// <returns>Returns OrderResult which basically contains the status of your transaction after it was succesfully requested</returns>
        public async Task<OrderResult> SubmitOrder(string l2Pk, string apiKey, int accountId,
            Token sellToken,
            Token buyToken,
            bool allOrNone,
            bool fillAmountBOrS,
            long validUntil,
            int maxFeeBips = 20,
            string clientOrderId = null,
            OrderType? orderType = null,
            TradeChannel? tradeChannel = null,
            string taker = null,
            string poolAddress = null,
            string affiliate = null)

        {
            var request = new ApiSubmitOrderRequest()
            {
                exchange = _exchange.exchangeAddress,
                accountId = accountId,
                storageId = (await StorageId(apiKey, accountId, sellToken.tokenId)).orderId, // MAYBE? NOT SURE
                sellToken = sellToken,
                buyToken = buyToken,
                allOrNone = allOrNone,
                fillAmountBOrS = fillAmountBOrS,
                validUntil = validUntil,
                maxFeeBips = maxFeeBips,
            };

            if (!string.IsNullOrWhiteSpace(clientOrderId))
                request.clientOrderId = clientOrderId;
            if (orderType.HasValue)
                request.orderType = orderType.Value.ToString();
            if (tradeChannel.HasValue)
                request.tradeChannel = tradeChannel.Value.ToString();
            if (!string.IsNullOrWhiteSpace(taker))
                request.taker = taker;
            if (!string.IsNullOrWhiteSpace(poolAddress))
                request.poolAddress = poolAddress;
            if (!string.IsNullOrWhiteSpace(affiliate))
                request.affiliate = affiliate;

            int MAX_INPUT = 11;
            var poseidonHasher = new Poseidon(MAX_INPUT + 1, 6, 53, "poseidon", 5, _securityTarget: 128);



            BigInteger[] inputs = {
                ParseHexUnsigned(request.exchange),
                request.storageId,
                request.accountId,
                request.sellToken.tokenId,
                request.buyToken.tokenId,
                BigInteger.Parse(request.sellToken.volume),
                BigInteger.Parse(request.buyToken.volume),
                request.validUntil,
                request.maxFeeBips,
                (fillAmountBOrS ? 1 : 0),
                string.IsNullOrWhiteSpace(request.taker) ? 0 : ParseHexUnsigned(request.taker)
            };

            request.eddsaSignature = EDDSAHelper.EDDSASign(inputs, l2Pk);


            (string, string)[] parameters = { };
            (string, string)[] headers = { (Constants.HttpHeaderAPIKeyName, apiKey) };
            var apiresult = JsonConvert.DeserializeObject<ApiOrderSubmitResult>(
                await Http(Constants.OrderUrl, parameters, headers, "post", JsonConvert.SerializeObject(request)));
            return new OrderResult(apiresult);
        }

        /// <summary>
        /// Change the ApiKey associated with the user's account
        /// </summary>
        /// <param name="l2Pk">Loopring Private Key</param>
        /// <param name="apiKey">Current Loopring API Key</param>
        /// <param name="accountId">Wallet Account Id</param>
        /// <returns>The new apiKey as string</returns>
        /// <exception cref="System.Exception">Gets thrown when there's a problem getting info from the Loopring API endpoint</exception>
        public async Task<string> UpdateApiKey(string l2Pk, string apiKey, int accountId)
        {
            string requestBody = "{\"accountId\":" + accountId + "}";
            var signedMessage = EddsaSignUrl(
                l2Pk,
                HttpMethod.Post,
                null,
                requestBody,
                Constants.ApiKeyUrl);

            (string, string)[] parameters = { };
            (string, string)[] headers = { (Constants.HttpHeaderAPIKeyName, apiKey), (Constants.HttpHeaderAPISigName, signedMessage) };
            var apiresult = JsonConvert.DeserializeObject<ApiApiKeyResult>(
                await Http(Constants.ApiKeyUrl, parameters, headers, "post", requestBody));
            return apiresult.apiKey;
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
        public async Task<StorageId> StorageId(string apiKey, int accountId, int sellTokenId, int maxNext = 0)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new System.Exception("StorageId REQUIRES a valid Loopring wallet apiKey");


            (string, string)[] parameters = { ("accountId", accountId.ToString()),("sellTokenId", sellTokenId.ToString()),("maxNext", maxNext.ToString()) };
            (string, string)[] headers = { (Constants.HttpHeaderAPIKeyName, apiKey) };
            var apiresult = JsonConvert.DeserializeObject<ApiStorageIdResult>(
                await Http(Constants.StorageIdUrl, parameters, headers));
            return new StorageId()
            {
                offchainId = apiresult.offchainId,
                orderId = apiresult.orderId
            };           
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
        public async Task<OffchainFee> OffchainFee(string apiKey, int accountId, OffChainRequestType requestType, string tokenSymbol, string amount)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new System.Exception("StorageId REQUIRES a valid Loopring wallet apiKey");

            (string, string)[] parameters = { ("accountId", accountId.ToString()), ("requestType", ((int)requestType).ToString()), ("tokenSymbol", tokenSymbol), ("amount", amount) };
            (string, string)[] headers = { (Constants.HttpHeaderAPIKeyName, apiKey) };
            var apiresult = JsonConvert.DeserializeObject<ApiOffchainFeeResult>(
                await Http(Constants.OffchainFeeUrl, parameters, headers));
            return new OffchainFee()
            {
                fees = apiresult.fees,
                gasPrice = apiresult.gasPrice
            };

        }

        /// <summary>
        /// Get the details of an order based on order hash.
        /// </summary>
        /// <param name="apiKey">Current Loopring API Key</param>
        /// <param name="accountId">Wallet Account Id</param>
        /// <param name="orderHash">The hash of the worder for which you want details</param>
        /// <returns>OrderDetails object filled with awesome order details</returns>
        /// <exception cref="System.Exception">Gets thrown when there's a problem getting info from the Loopring API endpoint</exception>
        public async Task<OrderDetails> OrderDetails(string apiKey, int accountId, string orderHash)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new System.Exception("StorageId REQUIRES a valid Loopring wallet apiKey");
            if (string.IsNullOrWhiteSpace(orderHash))
                throw new System.Exception("StorageId REQUIRES a valid order hash. Use one of the get order methods to get one");

            (string, string)[] parameters = { ("accountId", accountId.ToString()), ("orderHash", orderHash)};
            (string, string)[] headers = { (Constants.HttpHeaderAPIKeyName, apiKey) };
            var apiresult = JsonConvert.DeserializeObject<ApiOrderGetResult>(
                await Http(Constants.OrderUrl, parameters, headers));
            return new OrderDetails(apiresult);
        }

        /// <summary>
        /// Get a list of orders satisfying certain criteria.
        /// </summary>
        /// <param name="apiKey">Your Loopring API Key</param>
        /// <param name="accountId">Loopring account identifier</param>
        /// <param name="market">Trading pair (ex. Trading pair)</param>
        /// <param name="start">Lower bound of order's creation timestamp in millisecond (ex. 1567053142000)</param>
        /// <param name="end">Upper bound of order's creation timestamp in millisecond (ex. 1567053242000)</param>
        /// <param name="side">"BUY" or "SELL"</param>
        /// <param name="statuses">Order statuses to search by</param>
        /// <param name="orderTypes">Order types to search by</param>
        /// <param name="tradeChannels">Trade channels to search by</param>
        /// <param name="limit">How many results per call? Default 50</param>
        /// <param name="offset">How many results to skip? Default 0 </param>
        /// <returns>List of OrderDetails objects containing the searched-for items</returns>
        public async Task<List<OrderDetails>> OrdersDetails(string apiKey,

            int accountId,
            int limit = 50,
            int offset = 0,
            string market = null,

            long start = 0,

            long end = 0,

            Side? side = 0,

            List<OrderStatus> statuses = null,

            List<OrderType> orderTypes = null,

            List<TradeChannel> tradeChannels = null
            )
        {
            List<(string, string)> parameters = new List<(string, string)>();
            parameters.Add(("accountId", accountId.ToString()));
            if (!string.IsNullOrWhiteSpace(market))
                parameters.Add(("market", market));
            if (start != 0)
                parameters.Add(("start", start.ToString()));
            if (end != 0)
                parameters.Add(("end", end.ToString()));
            if (side.HasValue)
                parameters.Add(("side", side.ToString()));
            if (statuses != null)
                parameters.Add(("status", string.Join(",", statuses.Select(s => s.ToString()))));
            if (limit != 50)
                parameters.Add(("limit", limit.ToString()));
            if (offset != 0)
                parameters.Add(("offset", offset.ToString()));
            if (orderTypes != null)
                parameters.Add(("orderTypes", string.Join(",", orderTypes.Select(s => s.ToString()))));
            if (tradeChannels != null)
                parameters.Add(("tradeChannels", string.Join(",", tradeChannels.Select(s => s.ToString()))));
            
            (string, string)[] headers = { (Constants.HttpHeaderAPIKeyName, apiKey) };
            var apiresult = JsonConvert.DeserializeObject<ApiOrdersGetResult>(
                await Http(Constants.OrdersUrl, parameters.ToArray(), headers));
            if (apiresult != null && apiresult.totalNum != 0)
            {
                return apiresult.orders.Select(s => new OrderDetails(s)).ToList();
            }
            return null;            
        }

        #endregion
        #region apiKeyL1L2
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
        public async Task<Transfer> Transfer(string apiKey, string l2Pk, string l1Pk, TransferRequest request, string memo, string clientId, CounterFactualInfo counterFactualInfo)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new System.Exception("Transfer REQUIRES a valid Loopring wallet apiKey");
            if (string.IsNullOrWhiteSpace(l2Pk))
                throw new System.Exception("Transfer REQUIRES a valid Loopring Wallet Layer 2 Private key");
            if (string.IsNullOrWhiteSpace(l1Pk))
                throw new System.Exception("Transfer REQUIRES a valid Eth Wallet Layer 1 Private key");

            var account = await GetAccountInfo(request.payerAddr);



            BigInteger[] inputs = {
                ParseHexUnsigned(request.exchange),
                (BigInteger)request.payerId,
                (BigInteger)request.payeeId,
                (BigInteger)request.token.tokenId,
                BigInteger.Parse(request.token.volume),
                (BigInteger)request.maxFee.tokenId,
                BigInteger.Parse(request.maxFee.volume),
                ParseHexUnsigned(request.payeeAddr),
                0,
                0,
                (BigInteger)request.validUnitl,
                (BigInteger)request.storageId
            };



            var apiRequest = request.GetApiTransferRequest(memo, clientId, counterFactualInfo);
            apiRequest.eddsaSignature = EDDSAHelper.EDDSASign(inputs, l2Pk);

            apiRequest.ecdsaSignature = ECDSAHelper.TransferSign(
                _exchange.chainId,
                apiRequest,
                l1Pk);

            (string, string)[] parameters = { };
            (string, string)[] headers = { (Constants.HttpHeaderAPIKeyName, apiKey), (Constants.HttpHeaderAPISigName, apiRequest.ecdsaSignature) };
            var apiresult = JsonConvert.DeserializeObject<ApiTransferResult>(
                await Http(Constants.TransferUrl, parameters, headers, "post", JsonConvert.SerializeObject(apiRequest)));
            return new Transfer(apiresult);           
        }

        /// <summary>
        /// Send some tokens to anyone else on L2
        /// </summary>
        /// <param name="apiKey">Your Loopring API Key</param>
        /// <param name="l2Pk">Loopring Private Key</param>
        /// <param name="l1Pk">Ethereum Private Key</param>
        /// <param name="accountId">Loopring account identifier</param>
        /// <param name="fromAddress">The loopring address that's doing the sending</param>
        /// <param name="toAddress">The loopring address that's doing the receiving</param>
        /// <param name="token">What token is being sent</param>
        /// <param name="value">And how much of that token are we sending</param>
        /// <param name="feeToken">In what token are we paying the fee</param>
        /// <param name="memo">(Optional)And do you want the transaction to contain a reference. From loopring's perspective, this is just a text field</param>
        /// <returns>An object containing the status of the transfer at the end of the request</returns>
        public async Task<Transfer> Transfer(string apiKey, string l2Pk, string l1Pk, int accountId, string fromAddress,

            string toAddress, string token, decimal value, string feeToken, string memo)
        {
            var amount = (value * 1000000000000000000m).ToString("0");
            var feeamountresult = await OffchainFee(apiKey, accountId, OffChainRequestType.Transfer, feeToken, amount);
            var feeamount = feeamountresult.fees.Where(w => w.token == feeToken).First().fee;



            TransferRequest req = new TransferRequest()
            {
                exchange = (await ExchangeInfo()).exchangeAddress,
                maxFee = new Token()
                {
                    tokenId = GetTokenId(feeToken),
                    volume = feeamount
                },
                token = new Token()
                {
                    tokenId = GetTokenId(token),
                    volume = amount
                },
                payeeAddr = toAddress,
                payerAddr = fromAddress,
                payeeId = 0,
                payerId = accountId,
                storageId = (await StorageId(apiKey, accountId, GetTokenId(token))).offchainId,
                validUnitl = GetUnixTimestamp() + (int)TimeSpan.FromDays(365).TotalSeconds
            };
            return await Transfer(apiKey, l2Pk, l1Pk, req, memo, null, null);
        }

        #endregion

        #region private methods
        private string UrlEncodeUpperCase(string stringToEncode)
        {
            var reg = new Regex(@"%[a-f0-9]{2}");
            stringToEncode = HttpUtility.UrlEncode(stringToEncode);
            return reg.Replace(stringToEncode, m => m.Value.ToUpperInvariant());
        }

        private async Task<bool> ThrowIfHttpFail(HttpResponseMessage httpResult)
        {
            if (httpResult.IsSuccessStatusCode)
                return true;
            if (httpResult.Content != null)
            {
                var exString = "Error from Loopring API: " + httpResult.StatusCode.ToString() + " | " + (await httpResult.Content.ReadAsStringAsync());
                throw new System.Exception(exString);
            }
            throw new System.Exception("Error from Loopring API: " + httpResult.StatusCode.ToString());
        }

        private BigInteger CreateSha256Signature(HttpMethod method, List<(string Key, string Value)> queryParams, string postBody, string apiMethod)
        {
            var signatureBase = "";
            var parameterString = "";
            if (method == HttpMethod.Post)
            {
                signatureBase += "POST&";
                parameterString = postBody;
            }
            else if (method == HttpMethod.Get)
            {
                signatureBase += "GET&";
                if (queryParams != null)
                {
                    int i = 0;
                    foreach (var parameter in queryParams)
                    {
                        parameterString += parameter.Key + "=" + parameter.Value;
                        if (i < queryParams.Count - 1)
                            parameterString += "&";
                        i++;
                    }
                }
            }
            else if (method == HttpMethod.Delete)
            {
                signatureBase += "DELETE&";
                if (queryParams != null)
                {
                    int i = 0;
                    foreach (var parameter in queryParams)
                    {
                        parameterString += parameter.Key + "=" + parameter.Value;
                        if (i < queryParams.Count - 1)
                            parameterString += "&";
                        i++;
                    }
                }
            }
            else
                throw new Exception("Http method type not supported");

            signatureBase += UrlEncodeUpperCase(_apiUrl + apiMethod) + "&";
            signatureBase += UrlEncodeUpperCase(parameterString);

            return SHA256Helper.CalculateSHA256HashNumber(signatureBase);
        }

        private string EddsaSignUrl(string l2Pk, HttpMethod method, List<(string Key, string Value)> queryParams, string postBody, string apiMethod)
        {
            var message = CreateSha256Signature(
                method,
                queryParams,
                postBody,
                apiMethod);

            var signer = new Eddsa(message, l2Pk);
            return signer.Sign();
        }

        private async Task<string> Http(string endpoint, (string, string)[] parameters, (string, string)[] headers, string method = "get", string body = null)
        {
            var url = $"{_apiUrl}{endpoint}";
            if (parameters != null && parameters.Length > 0)
            {
                if (parameters.Any(a => !string.IsNullOrWhiteSpace(a.Item2)))
                {
                    url += "?";
                    foreach (var parameter in parameters)
                    {
                        if (!string.IsNullOrWhiteSpace(parameter.Item2))
                        {
                            url += parameter.Item1 + "=" + parameter.Item2 + "&";
                        }
                    }
                    url = url.TrimEnd('&');
                }
            }
            HttpMethod tmethod = HttpMethod.Get;
            if(method=="delete")
                tmethod = HttpMethod.Delete;
            if (method == "post")
                tmethod = HttpMethod.Post;

            using (var httpRequest = new HttpRequestMessage(tmethod, url))
            {
                if (headers != null && headers.Length > 0)
                {
                    foreach (var header in headers)
                    {
                        httpRequest.Headers.Add(header.Item1, header.Item2);
                    }
                }
                if (!string.IsNullOrWhiteSpace(body))
                {                    
                    httpRequest.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }
                using (var httpResult = await _client.SendAsync(httpRequest).ConfigureAwait(continueOnCapturedContext: false))
                {
                    _ = await ThrowIfHttpFail(httpResult);
                    return await httpResult.Content.ReadAsStringAsync();
                }
            }
        }
        #endregion

        #region public methods

        public int GetTokenId(string token)
        {
            if (Constants.TokenIDMapper.Count == 0 || !Constants.TokenIDMapper.ContainsKey(token))
            {
                var tokens = GetTokens().Result;
                foreach (var rtoken in tokens)
                {
                    if (!Constants.TokenIDMapper.ContainsKey(rtoken.symbol))
                        Constants.TokenIDMapper.Add(rtoken.symbol, rtoken.tokenId);
                    else
                        Constants.TokenIDMapper[rtoken.symbol] = rtoken.tokenId;
                }
            }
            return Constants.TokenIDMapper[token];
        }

        public int GetUnixTimestamp() => (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        public static BigInteger ParseHexUnsigned(string toParse)
        {
            toParse = toParse.Replace("0x", "");
            var parsResult = BigInteger.Parse(toParse, System.Globalization.NumberStyles.HexNumber);
            if (parsResult < 0)
                parsResult = BigInteger.Parse("0" + toParse, System.Globalization.NumberStyles.HexNumber);
            return parsResult;
        }

        #endregion
    }

}
