using LoopringAPI.Metamask;
using LoopringAPI.WalletConnect;
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
        

        public SecureClient(string apiUrl)
        {
            Utils.StartHttpProcessor(200);
            _apiUrl = apiUrl;
            _ = GetTokenId("ETH").Result;            
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

            var apiTickersResult = JsonConvert.DeserializeObject<ApiTickersResult>(
                await Utils.Http(_apiUrl+Constants.TickerUrl, parameters, null).ConfigureAwait(false));

            
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
                HighestBidPrice = float.Parse(s[9]),
                LowestAskPrice = float.Parse(s[10]),
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
            var apiresult = JsonConvert.DeserializeObject<ApiTimestampResult>(
                await Utils.Http(_apiUrl+Constants.TimestampUrl).ConfigureAwait(false));
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
            var apiresult = JsonConvert.DeserializeObject<ApiAccountResult>(
                await Utils.Http(_apiUrl+Constants.AccountUrl, parameters).ConfigureAwait(false));

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


        private DateTime ExchangeInfoShorTermCacheTime;
        private ApiExchangeInfoResult EchangeInfoShorTermCache;
        /// <summary>
        /// Return various configurations of Loopring.io
        /// </summary>
        /// <returns>Fees, exchange address, all sort of useful stuff</returns>
        public async Task<ExchangeInfo> ExchangeInfo()
        {
            if (ExchangeInfoShorTermCacheTime.AddSeconds(2) < DateTime.UtcNow)
            {
                ExchangeInfoShorTermCacheTime = DateTime.UtcNow;
                EchangeInfoShorTermCache = JsonConvert.DeserializeObject<ApiExchangeInfoResult>(
                await Utils.Http(_apiUrl+Constants.InfoUrl).ConfigureAwait(false));
            }

            return new ExchangeInfo()
            {
                ammExitFees = EchangeInfoShorTermCache.ammExitFees,
                chainId = EchangeInfoShorTermCache.chainId,
                depositAddress = EchangeInfoShorTermCache.depositAddress,
                exchangeAddress = EchangeInfoShorTermCache.exchangeAddress,
                fastWithdrawalFees = EchangeInfoShorTermCache.fastWithdrawalFees,
                onchainFees = EchangeInfoShorTermCache.onchainFees,
                openAccountFees = EchangeInfoShorTermCache.openAccountFees,
                transferFees = EchangeInfoShorTermCache.transferFees,
                updateFees = EchangeInfoShorTermCache.updateFees,
                withdrawalFees = EchangeInfoShorTermCache.withdrawalFees
            };
        }

        /// <summary>
        /// Get a list of all the markets available on the exchange
        /// </summary>
        /// <returns>List of all the markets available on the exchange and their configurations</returns>
        public async Task<List<Market>> GetMarkets()
        {
            var apiresult = JsonConvert.DeserializeObject<ApiMarketsGetResult>(
                await Utils.Http(_apiUrl+Constants.MarketsUrl).ConfigureAwait(false));
            return apiresult.markets;
        }

        /// <summary>
        /// Returns the configurations of all supported tokens, including Ether.
        /// </summary>
        /// <returns>List of all the supported tokens and their configurations</returns>
        public async Task<List<TokenConfig>> GetTokens()
        {
            return JsonConvert.DeserializeObject<List<TokenConfig>>(
                await Utils.Http(_apiUrl+Constants.TokensUrl).ConfigureAwait(false));
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
            var apiresult = JsonConvert.DeserializeObject<ApiDepthResult>(
                await Utils.Http(_apiUrl+Constants.DepthUrl, parameters).ConfigureAwait(false));
            return new Depth()
            {
                asks = apiresult.asks.Select(s=>new Depth.Position()
                {
                    price = float.Parse(s[0]),
                    size = decimal.Parse(s[1])/ 1000000000000000000m,
                    volume = decimal.Parse(s[2]),
                    numberOfOrdersAgregated = decimal.Parse((s[3]))
                }).ToList(),
                market = apiresult.market,
                bids =  apiresult.bids.Select(s => new Depth.Position()
                {
                    price = float.Parse(s[0]),
                    size = decimal.Parse(s[1]) / 1000000000000000000m,
                    volume = decimal.Parse(s[2]) / 1000000000000000000m,
                    numberOfOrdersAgregated = decimal.Parse((s[3]))
                }).ToList(),
                timestamp = apiresult.timestamp,
                version = apiresult.version
            };
        }

        /// <summary>
        /// UNDOCUMENTED. Gets the Depth but for some reason does it better. No idea why
        /// </summary>
        /// <param name="market">The ID of a trading pair.</param>
        /// <param name="level">Order book aggregation level, larger value means further price aggregation. Default: 2</param>
        /// <param name="limit">Maximum numbers of bids/asks. Default : 50</param>
        /// <returns>Returns the order book of a given trading pair.</returns>
        public async Task<Depth> GetMixDepth(string market, int level = 2, int limit = 50)
        {
            (string, string)[] parameters = { ("market", market), ("level", level.ToString()), ("limit", limit.ToString()) };
            var apiresult = JsonConvert.DeserializeObject<ApiDepthResult>(
                await Utils.Http(_apiUrl + Constants.DepthMixUrl, parameters).ConfigureAwait(false));
            return new Depth()
            {
                asks = apiresult.asks.Select(s => new Depth.Position()
                {
                    price = float.Parse(s[0]),
                    size = decimal.Parse(s[1]) / 1000000000000000000m,
                    volume = decimal.Parse(s[2]),
                    numberOfOrdersAgregated = decimal.Parse((s[3]))
                }).ToList(),
                market = apiresult.market,
                bids = apiresult.bids.Select(s => new Depth.Position()
                {
                    price = float.Parse(s[0]),
                    size = decimal.Parse(s[1]) / 1000000000000000000m,
                    volume = decimal.Parse(s[2]) / 1000000000000000000m,
                    numberOfOrdersAgregated = decimal.Parse((s[3]))
                }).ToList(),
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
            return JsonConvert.DeserializeObject<ApiCandlestickResult>(
                await Utils.Http(_apiUrl+Constants.CandlestickUrl, parameters).ConfigureAwait(false)).candlesticks.Select(s => new Candlestick()
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
            (string, string)[] parameters = { ("legal", legal.ToString()) };
            var apiresult = JsonConvert.DeserializeObject<ApiPriceResult>(
                await Utils.Http(_apiUrl+Constants.PriceUrl, parameters).ConfigureAwait(false));
            return apiresult.prices;
        }

        /// <summary>
        /// Query trades with specified market
        /// </summary>
        /// <param name="market">Single market to query</param>
        /// <param name="limit">Number of queries</param>
        /// <param name="fillTypes">Filter by types of fills for the trade</param>
        /// <returns>List of Trades</returns>
        public async Task<List<Trade>> GetTrades(string market, int? limit = null, FillTypes[] fillTypes = null)
        {
            List<(string, string)> parameters = new List<(string, string)>(){ ("market", market) };
            if (limit.HasValue)
                parameters.Add(("limit", limit.Value.ToString()));
            if (fillTypes != null && fillTypes.Length > 0)
                parameters.Add(("fillTypes", string.Join(",", fillTypes.Select(s => s.ToString()))));
            var apiresult = JsonConvert.DeserializeObject<ApiTradesResult>(
                await Utils.Http(_apiUrl+Constants.TradeUrl, parameters.ToArray()).ConfigureAwait(false));
            return apiresult.trades.Select(s => new Trade()
            {
                TradeTimestamp = long.Parse(s[0]),
                RecordId = long.Parse(s[1]),
                Side = (Side)Enum.Parse(typeof(Side), s[2], true),
                Volume = decimal.Parse(s[3]),
                Fees = float.Parse(s[4]),
                Market = s[5],
                Price = decimal.Parse(s[6]),
            }).ToList();
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
            var signedMessage = EDDSAHelper.EddsaSignUrl(
                l2Pk,
                HttpMethod.Get,
                new List<(string Key, string Value)>() { ("accountId", accountId.ToString()) },
                null,
                Constants.ApiKeyUrl,
                _apiUrl);

            (string, string)[] parameters = { ("accountId", accountId.ToString()) };
            (string, string)[] headers = { (Constants.HttpHeaderAPISigName, signedMessage) };
            var apiresult = JsonConvert.DeserializeObject<ApiApiKeyResult>(
                await Utils.Http(_apiUrl+Constants.ApiKeyUrl, parameters, headers).ConfigureAwait(false));
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
            var signedMessage = EDDSAHelper.EddsaSignUrl(
                l2Pk,
                HttpMethod.Delete,
                new List<(string Key, string Value)>() { ("accountId", accountId.ToString()), ("clientOrderId", clientOrderId), ("orderHash", orderHash) },
                null,
                Constants.OrderUrl,
                _apiUrl);

            (string, string)[] parameters = { ("accountId", accountId.ToString()), ("clientOrderId", clientOrderId), ("orderHash", orderHash) };
            (string, string)[] headers = { (Constants.HttpHeaderAPISigName, signedMessage), (Constants.HttpHeaderAPIKeyName, apiKey) };
            var apiresult = JsonConvert.DeserializeObject<ApiOrderSubmitResult>(
                await Utils.Http(_apiUrl+Constants.OrderUrl, parameters, headers, "delete").ConfigureAwait(false));
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
        public async Task<OrderResult> SubmitOrder(string l2Pk, string apiKey, int accountId,
            string sellCurrency,
            decimal sellAmmount,
            string buyCurrency,
            decimal buyAmmount,
            OrderType orderType,
            string poolAddress = null,
            string clientId = null)
        {
            var tradeChannel = TradeChannel.MIXED;
            if (orderType == OrderType.MAKER_ONLY)
                tradeChannel = TradeChannel.ORDER_BOOK;

            var tokens = await GetTokens();
            var sellTOkenDecimalCount = tokens.Where(w => w.symbol == sellCurrency).FirstOrDefault().decimals;
            decimal sellTokenMultiplier = 10;
            for (int i = 2; i <= sellTOkenDecimalCount; i++)
                sellTokenMultiplier *= 10;

            var buyTOkenDecimalCount = tokens.Where(w => w.symbol == buyCurrency).FirstOrDefault().decimals;
            decimal buyTokenMultiplier = 10;
            for (int i = 2; i <= buyTOkenDecimalCount; i++)
                buyTokenMultiplier *= 10;

            return await SubmitOrder(l2Pk, apiKey, accountId,
                new Token() { tokenId = await GetTokenId(sellCurrency).ConfigureAwait(false), volume = (sellAmmount * sellTokenMultiplier).ToString("0") },
                new Token() { tokenId = await GetTokenId(buyCurrency).ConfigureAwait(false), volume = (buyAmmount * buyTokenMultiplier).ToString("0") },
                false,
                false,
                Utils.GetUnixTimestamp() + (int)TimeSpan.FromDays(365).TotalSeconds, // one year
                63,
                clientId,
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
                exchange = ExchangeInfo().Result.exchangeAddress,
                accountId = accountId,
                storageId = (await StorageId(apiKey, accountId, sellToken.tokenId).ConfigureAwait(false)).orderId, // MAYBE? NOT SURE
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
                Utils.ParseHexUnsigned(request.exchange),
                request.storageId,
                request.accountId,
                request.sellToken.tokenId,
                request.buyToken.tokenId,
                BigInteger.Parse(request.sellToken.volume),
                BigInteger.Parse(request.buyToken.volume),
                request.validUntil,
                request.maxFeeBips,
                (fillAmountBOrS ? 1 : 0),
                string.IsNullOrWhiteSpace(request.taker) ? 0 : Utils.ParseHexUnsigned(request.taker)
            };

            request.eddsaSignature = EDDSAHelper.EDDSASign(inputs, l2Pk);

            (string, string)[] headers = { (Constants.HttpHeaderAPIKeyName, apiKey) };
            var apiresult = JsonConvert.DeserializeObject<ApiOrderSubmitResult>(
                await Utils.Http(_apiUrl+Constants.OrderUrl, null, headers, "post", JsonConvert.SerializeObject(request)).ConfigureAwait(false));
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
            var signedMessage = EDDSAHelper.EddsaSignUrl(
                l2Pk,
                HttpMethod.Post,
                null,
                requestBody,
                Constants.ApiKeyUrl,
                _apiUrl);
            (string, string)[] headers = { (Constants.HttpHeaderAPIKeyName, apiKey), (Constants.HttpHeaderAPISigName, signedMessage) };
            var apiresult = JsonConvert.DeserializeObject<ApiApiKeyResult>(
                await Utils.Http(_apiUrl+Constants.ApiKeyUrl, null, headers, "post", requestBody).ConfigureAwait(false));
            return apiresult.apiKey;
        }
        #endregion
        #region apiKey
        /// <summary>
        /// Get L2 block info by block id
        /// </summary>
        /// <param name="apiKey">Current Loopring API Key</param>
        /// <param name="id">L2 block id</param>
        /// <returns>The L2 block info as string</returns>
        /// <exception cref="System.Exception">Gets thrown when there's a problem getting info from the Loopring API endpoint</exception>
        public async Task<L2BlockInfo> GetL2BlockInfo(string apiKey, int id)
        {
            (string, string)[] parameters = { ("id", id.ToString()) };
            (string, string)[] headers = { (Constants.HttpHeaderAPIKeyName, apiKey) };

            var apiresult = JsonConvert.DeserializeObject<ApiL2BlockInfoResult>(
                await Utils.Http(_apiUrl + Constants.L2BlockInfoUrl, parameters, headers).ConfigureAwait(false));

            return new L2BlockInfo()
            {
                blockId = apiresult.blockId,
                blockSize = apiresult.blockSize,
                exchange = apiresult.exchange,
                txHash = apiresult.txHash,
                status = apiresult.status,
                createdAt = apiresult.createdAt,
                transactions = apiresult.transactions
            };
        }

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


            (string, string)[] parameters = { ("accountId", accountId.ToString()), ("sellTokenId", sellTokenId.ToString()), ("maxNext", maxNext.ToString()) };
            (string, string)[] headers = { (Constants.HttpHeaderAPIKeyName, apiKey) };
            var apiresult = JsonConvert.DeserializeObject<ApiStorageIdResult>(
                await Utils.Http(_apiUrl+Constants.StorageIdUrl, parameters, headers).ConfigureAwait(false));
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
                await Utils.Http(_apiUrl+Constants.OffchainFeeUrl, parameters, headers).ConfigureAwait(false));
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

            (string, string)[] parameters = { ("accountId", accountId.ToString()), ("orderHash", orderHash) };
            (string, string)[] headers = { (Constants.HttpHeaderAPIKeyName, apiKey) };
            var apiresult = JsonConvert.DeserializeObject<ApiOrderGetResult>(
                await Utils.Http(_apiUrl+Constants.OrderUrl, parameters, headers).ConfigureAwait(false));
            return new OrderDetails(apiresult);
        }      

        /// <summary>
        /// Get the details of an order based on order hash.
        /// </summary>
        /// <param name="apiKey">Current Loopring API Key</param>
        /// <param name="accountId">Wallet Account Id</param>
        /// <param name="tokens">(Optional) list of the tokens which you want returned</param>
        /// <returns>OrderDetails object filled with awesome order details</returns>
        /// <exception cref="System.Exception">Gets thrown when there's a problem getting info from the Loopring API endpoint</exception>
        public async Task<List<Balance>> Ballances(string apiKey, int accountId, string tokens = null)
        {
            string[] tokenSplit = new string[0];
            if (tokens != null)
                tokenSplit = tokens.Split(',');
            int[] tokenIds = new int[0];
            if (tokens!=null && tokens.Length>0)
                tokenIds = tokenSplit.Select(s => GetTokenId(s).Result).ToArray();

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new System.Exception("StorageId REQUIRES a valid Loopring wallet apiKey");

            if (tokenIds.Length == 0)
                tokenIds = null;

            List<(string, string)> parameters = new List<(string, string)>(){ ("accountId", accountId.ToString()) };
            if (tokenIds != null && tokenIds.Length > 0)
                parameters.Add(("tokens", String.Join(",", tokenIds)));
                
            (string, string)[] headers = { (Constants.HttpHeaderAPIKeyName, apiKey) };
            var apiresult = JsonConvert.DeserializeObject<List<ApiBalance>>(
                await Utils.Http(_apiUrl + Constants.BalancesUrl, parameters.ToArray(), headers).ConfigureAwait(false));
            var result = new List<Balance>();

            var tokenConfigs = await GetTokens();

            foreach(var ballance in apiresult)
            {
                var decimals =  tokenConfigs.Where(w => w.tokenId == ballance.tokenId).FirstOrDefault().decimals;
                decimal devideBy = 10;
                for(int i = 2;i<=decimals;i++)
                {
                    devideBy *= 10;
                }
                result.Add(new Balance()
                {
                    token = GetTokenId(ballance.tokenId),
                    locked = decimal.Parse(ballance.locked) / devideBy,
                    pending = new Balance.BalancePending()
                    {                        
                        deposit = decimal.Parse(ballance.pending.deposit)/ devideBy,
                        widthdraw = decimal.Parse(ballance.pending.withdraw) / devideBy,
                    },
                    total = decimal.Parse(ballance.total) / devideBy,
                });;
            }

            return result;
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
        public async Task<List<OrderDetails>> OrdersDetails(string apiKey, int accountId, int limit = 50, int offset = 0, string market = null, long start = 0, long end = 0, Side? side = 0, List<OrderStatus> statuses = null, List<OrderType> orderTypes = null, List<TradeChannel> tradeChannels = null)
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
                await Utils.Http(_apiUrl+Constants.OrdersUrl, parameters.ToArray(), headers).ConfigureAwait(false));
            if (apiresult != null && apiresult.totalNum != 0)
            {
                return apiresult.orders.Select(s => new OrderDetails(s)).ToList();
            }
            return null;
        }

        /// <summary>
        /// Returns a list of Ethereum transactions from users for exchange account registration.
        /// </summary>
        /// <param name="apiKey">Your Loopring API Key</param>
        /// <param name="accountId">Loopring account identifier</param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="start">Lower bound of order's creation timestamp in millisecond (ex. 1567053142000)</param>
        /// <param name="end">Upper bound of order's creation timestamp in millisecond (ex. 1567053242000)</param>
        /// <param name="limit">How many results per call? Default 50</param>
        /// <param name="offset">How many results to skip? Default 0 </param>
        /// <param name="statuses">Statuses which you would like to filter by</param>
        /// <returns>List of Ethereum transactions from users for exchange account registration.</returns>
        public async Task<List<Transaction>> CreateInfo(string apiKey, int accountId, int limit = 50, int offset = 0, long start = 0, long end = 0, List<Status> statuses = null)
        {
            List<(string, string)> parameters = new List<(string, string)>();
            parameters.Add(("accountId", accountId.ToString()));
            if (start != 0)
                parameters.Add(("start", start.ToString()));
            if (end != 0)
                parameters.Add(("end", end.ToString()));            
            if (statuses != null)
                parameters.Add(("status", string.Join(",", statuses.Select(s => s.ToString()))));
            if (limit != 50)
                parameters.Add(("limit", limit.ToString()));
            if (offset != 0)
                parameters.Add(("offset", offset.ToString()));           

            (string, string)[] headers = { (Constants.HttpHeaderAPIKeyName, apiKey) };
            var apiresult = JsonConvert.DeserializeObject<ApiInfoGetResult>(
                await Utils.Http(_apiUrl + Constants.CreateInfoUrl, parameters.ToArray(), headers).ConfigureAwait(false));
            if (apiresult != null && apiresult.totalNum != 0)
            {
                return apiresult.transactions.ToList();
            }
            return null;
        }

        /// <summary>
        /// Returns a list Ethereum transactions from users for resetting exchange passwords.
        /// </summary>
        /// <param name="apiKey">Your Loopring API Key</param>
        /// <param name="accountId">Loopring account identifier</param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="start">Lower bound of order's creation timestamp in millisecond (ex. 1567053142000)</param>
        /// <param name="end">Upper bound of order's creation timestamp in millisecond (ex. 1567053242000)</param>
        /// <param name="limit">How many results per call? Default 50</param>
        /// <param name="offset">How many results to skip? Default 0 </param>
        /// <param name="statuses">Statuses which you would like to filter by</param>
        /// <returns>List of Ethereum transactions from users for resetting exchange passwords.</returns>
        public async Task<List<Transaction>> UpdateInfo(string apiKey, int accountId, int limit = 50, int offset = 0, long start = 0, long end = 0, List<Status> statuses = null)
        {
            List<(string, string)> parameters = new List<(string, string)>();
            parameters.Add(("accountId", accountId.ToString()));
            if (start != 0)
                parameters.Add(("start", start.ToString()));
            if (end != 0)
                parameters.Add(("end", end.ToString()));
            if (statuses != null)
                parameters.Add(("status", string.Join(",", statuses.Select(s => s.ToString()))));
            if (limit != 50)
                parameters.Add(("limit", limit.ToString()));
            if (offset != 0)
                parameters.Add(("offset", offset.ToString()));

            (string, string)[] headers = { (Constants.HttpHeaderAPIKeyName, apiKey) };
            var apiresult = JsonConvert.DeserializeObject<ApiInfoGetResult>(
                await Utils.Http(_apiUrl + Constants.UpdateInfoUrl, parameters.ToArray(), headers).ConfigureAwait(false));
            if (apiresult != null && apiresult.totalNum != 0)
            {
                return apiresult.transactions.ToList();
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
        public async Task<OperationResult> Transfer(string apiKey, string l2Pk, string l1Pk, TransferRequest request, string memo, string clientId, CounterFactualInfo counterFactualInfo, WalletService? walletType)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new System.Exception("Transfer REQUIRES a valid Loopring wallet apiKey");
            if (string.IsNullOrWhiteSpace(l2Pk))
                throw new System.Exception("Transfer REQUIRES a valid Loopring Wallet Layer 2 Private key");

            var account = await GetAccountInfo(request.payerAddr).ConfigureAwait(false);

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
            apiRequest.eddsaSignature = EDDSAHelper.EDDSASign(inputs, l2Pk);

            if (walletType.HasValue && walletType.Value==WalletService.MetaMask)
            {
                var typedData = ECDSAHelper.GenerateTransferTypedData(ExchangeInfo().Result.chainId, apiRequest).Item2;
                apiRequest.ecdsaSignature = MetamaskServer.Sign(JsonConvert.SerializeObject(typedData), "eth_signTypedData_v4", $"Please authorise the sending of {(decimal.Parse(request.token.volume) / 1000000000000000000m)} {request.tokenName} to {request.payeeAddr}. The fee will be {(decimal.Parse(request.maxFee.volume) / 1000000000000000000m)} {request.tokenFeeName}");
            }
            else if (walletType.HasValue && walletType.Value == WalletService.WalletConnect)
            {
                var typedData = ECDSAHelper.GenerateTransferTypedData(ExchangeInfo().Result.chainId, apiRequest);
                apiRequest.ecdsaSignature = await WalletConnectServer.Sign(JsonConvert.SerializeObject(typedData.Item2), "eth_signTypedData_v4", request.payerAddr)+"02";                
            }
            else
            {
                var typedData = ECDSAHelper.GenerateTransferTypedData(ExchangeInfo().Result.chainId, apiRequest).Item1;
                apiRequest.ecdsaSignature = ECDSAHelper.GenerateSignature(typedData, l1Pk);
            }
            (string, string)[] headers = { (Constants.HttpHeaderAPIKeyName, apiKey), (Constants.HttpHeaderAPISigName, apiRequest.ecdsaSignature) };
            var apiresult = JsonConvert.DeserializeObject<ApiTransferResult>(
                await Utils.Http(_apiUrl+Constants.TransferUrl, null, headers, "post", JsonConvert.SerializeObject(apiRequest)).ConfigureAwait(false));
            return new OperationResult(apiresult);
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
        public async Task<OperationResult> Transfer(string apiKey, string l2Pk, string l1Pk, int accountId, string fromAddress,
            string toAddress, string token, decimal value, string feeToken, string memo, WalletService? walletType)
        {
            var amount = (value * 1000000000000000000m).ToString("0");
            var feeamountresult = await OffchainFee(apiKey, accountId, OffChainRequestType.Transfer, feeToken, amount).ConfigureAwait(false);
            var feeamount = feeamountresult.fees.Where(w => w.token == feeToken).First().fee;

            TransferRequest req = new TransferRequest()
            {
                exchange = (await ExchangeInfo().ConfigureAwait(false)).exchangeAddress,
                maxFee = new Token()
                {
                    tokenId = await GetTokenId(feeToken).ConfigureAwait(false),
                    volume = feeamount
                },
                token = new Token()
                {
                    tokenId = await GetTokenId(token).ConfigureAwait(false),
                    volume = amount
                },
                payeeAddr = toAddress,
                payerAddr = fromAddress,
                payeeId = 0,
                payerId = accountId,
                storageId = (await StorageId(apiKey, accountId, await GetTokenId(token).ConfigureAwait(false)).ConfigureAwait(false)).offchainId,
                validUnitl = Utils.GetUnixTimestamp() + (int)TimeSpan.FromDays(365).TotalSeconds,
                tokenName = token,
                tokenFeeName = feeToken
            };
            return await Transfer(apiKey, l2Pk, l1Pk, req, memo, null, null, walletType).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="l1Pk">User's current eth private key</param>
        /// <param name="l2Pk">User's current loopring private key</param>
        /// <param name="req">A UpdateAccountRequest object containing all the needed information for this request</param>
        /// <param name="counterFactualInfo">(Optional)Not entirely sure. Official documentation says: field.UpdateAccountRequestV3.counterFactualInfo</param>
        /// <returns></returns>
        public async Task<OperationResult> UpdateAccount(string l2Pk, string l1Pk, UpdateAccountRequest req, CounterFactualInfo counterFactualInfo)
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

            apiRequest.eddsaSignature = EDDSAHelper.EDDSASign(inputs, l2Pk);

            if (l1Pk == WalletService.MetaMask.ToString())
            {
                var typedData = ECDSAHelper.GenerateAccountUpdateTypedData(ExchangeInfo().Result.chainId, apiRequest).Item2;
                apiRequest.ecdsaSignature = MetamaskServer.Sign(JsonConvert.SerializeObject(typedData), "eth_signTypedData_v4", $"Please authorise the reset of your Loopring private key. The fee will be {(decimal.Parse(apiRequest.maxFee.volume) / 1000000000000000000m)}");
            }
            if (l1Pk == WalletService.WalletConnect.ToString())
            {
                var typedData = ECDSAHelper.GenerateAccountUpdateTypedData(ExchangeInfo().Result.chainId, apiRequest);
                apiRequest.ecdsaSignature = await WalletConnectServer.Sign(JsonConvert.SerializeObject(typedData.Item2), "eth_signTypedData_v4", req.owner) + "02";                
            }
            else
            {
                var typedData = ECDSAHelper.GenerateAccountUpdateTypedData(ExchangeInfo().Result.chainId, apiRequest).Item1;
                apiRequest.ecdsaSignature = ECDSAHelper.GenerateSignature(typedData, l1Pk);
            }

            (string, string)[] headers = { (Constants.HttpHeaderAPISigName, apiRequest.ecdsaSignature) };
            var apiresult = JsonConvert.DeserializeObject<ApiTransferResult>(
                await Utils.Http(_apiUrl+Constants.AccountUrl, null, headers, "post", JsonConvert.SerializeObject(apiRequest)).ConfigureAwait(false));
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
        public async Task<OperationResult> UpdateAccount(string apiKey, string l1Pk,string l2Pk, int accountId, string feeToken, string ethPublicAddress, string exchangeAddress, WalletService? walletType)
        {
            var newNonce = (await GetAccountInfo(ethPublicAddress)).nonce;
            (string publicKeyX, string publicKeyY, string secretKey, string ethAddress) keys;
            if (walletType.HasValue && walletType.Value == WalletService.MetaMask)
            {                
                keys = EDDSAHelper.EDDSASignMetamask(exchangeAddress, _apiUrl, false,true);
                l1Pk = WalletService.MetaMask.ToString();
            }
            if (walletType.HasValue && walletType.Value == WalletService.WalletConnect)
            {
                keys = await EDDSAHelper.EDDSASignWalletConnect(exchangeAddress, newNonce);
                l1Pk = WalletService.WalletConnect.ToString();
            }
            else
            {
                keys = EDDSAHelper.EDDSASignLocal(exchangeAddress, newNonce, l1Pk, ethPublicAddress);
            }

            var feeamountresult = await OffchainFee(apiKey, accountId, OffChainRequestType.UpdateAccount, feeToken, "0").ConfigureAwait(false);
            var feeamount = feeamountresult.fees.Where(w => w.token == feeToken).First().fee;

            UpdateAccountRequest req = new UpdateAccountRequest()
            {
                accountId = accountId,
                exchange = ExchangeInfo().Result.exchangeAddress,
                maxFee = new Token()
                {
                    tokenId = await GetTokenId(feeToken).ConfigureAwait(false),
                    volume = feeamount
                },                
                nonce = newNonce,
                owner = ethPublicAddress,
                validUntil = 1700000000,
                PublicKeyX = keys.publicKeyX,
                PublicKeyY = keys.publicKeyY
            };

            return await UpdateAccount(l2Pk, l1Pk, req, null);
        }

        #endregion

        #region public methods

        public async Task<int> GetTokenId(string token)
        {
            if (Constants.TokenIDMapper.Count == 0 || !Constants.TokenIDMapper.ContainsKey(token))
            {
                var tokens = await GetTokens().ConfigureAwait(false);
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

        public string GetTokenId(int token)
        {
            return Constants.TokenIDMapper.Where(w => w.Value == token).FirstOrDefault().Key;
        }

        #endregion
    }

}
