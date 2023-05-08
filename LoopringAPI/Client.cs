using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

namespace LoopringSharp
{
    public class Client
    {
        public string _apiKey;
        public string _ethPrivateKey;
        public string _loopringPrivateKey;
        public string _loopringPublicKeyX;
        public string _loopringPublicKeyY;
        public string _ethAddress;
        public WalletService? _walletType;
        public int _accountId;
        private SecureClient _client;

        /// <summary>
        /// The Object you need in order to communicate with the Loopring API. Recommended to use as a signleton.
        /// </summary>
        /// <param name="apiKey">Your wallet API Key, needed for almost all api calls</param>
        /// <param name="loopringPrivateKey">Your Layer 2 Private Key, needed for most api calls</param>
        /// <param name="ethPrivateKey">Your Layer 1, Ethereum Private Key, needed for some very specific API calls</param>
        /// <param name="accountId">Your Loopring Account ID, used for a surprising amount of calls</param>
        public Client(string apiKey, string loopringPrivateKey, string ethPrivateKey, int accountId, string ethAddress, string apiUrl)
        {
            _client = new SecureClient(apiUrl);
            _apiKey = apiKey;
            _loopringPrivateKey = loopringPrivateKey;
            _ethPrivateKey = ethPrivateKey;
            _accountId = accountId;
            _ethAddress = ethAddress;
        }

        /// <summary>
        /// The Object you need in order to communicate with the Loopring API. Recommended to use as a signleton. Automatically gets the API Key using your Loopring Private Key
        /// </summary>                
        /// <param name="ethPrivateKey">Your Layer 1, Ethereum Private Key, needed for some very specific API calls</param>        
        public Client(string apiUrl, string ethPrivateKey)
        {
            _client = new SecureClient(apiUrl);
            _ethPrivateKey = ethPrivateKey;
            _ethAddress = ECDSAHelper.GetPublicAddress(ethPrivateKey);
            var accountInfo = GetAccountInfo();
            _accountId = accountInfo.accountId;
            var l2Auth = EDDSAHelper.EDDSASignLocal(ExchangeInfo().exchangeAddress, accountInfo.nonce - 1, _ethPrivateKey, _ethAddress);
            _loopringPrivateKey = l2Auth.secretKey;
            _loopringPublicKeyX = l2Auth.publicKeyX;
            _loopringPublicKeyY = l2Auth.publicKeyY;
            _apiKey = ApiKey();
        }

        /// <summary>
        /// Get user onchain withdrawal history.
        /// </summary>
        /// <param name="apiKey">ApiKey</param>
        /// <param name="accountId">Account ID, some hash query APIs doesnt need it if in hash query mode, check require flag of each API to see if its a must.</param>
        /// <param name="limit">Number of records to return</param>
        /// <param name="start">Start time in milliseconds - Default : 0L</param>
        /// <param name="end">End time in milliseconds - Default : 0L</param>
        /// <param name="statuses">Comma separated status values</param>
        /// <param name="tokenSymbol">Token to filter. If you want to return deposit records for all tokens, omit this parameter</param>
        /// <param name="offset">Number of records to skip - Default : 0L</param>
        /// <param name="hashes">The hashes of the transactions, normally its L2 tx hash, except the deposit which uses L1 tx hash.</param>
        /// <param name="withdrawlTypes">The type of withdrawls you want returned</param>        
        /// <returns></returns>
        public List<ApiWithdrawlTransaction> GetWithdrawls(int limit = 50, long start = 0, long end = 0, List<OrderStatus> statuses = null, string tokenSymbol = null, int offset = 0, WithdrawalTypes? withdrawlTypes = null, string[] hashes = null)
        {
            return _client.GetWithdrawls(_apiKey, _accountId, limit, start, end, statuses, tokenSymbol, offset, withdrawlTypes, hashes);
        }

        /// <summary>
        /// Get user transfer list.
        /// </summary>
        /// <param name="limit">Number of records to return</param>
        /// <param name="start">Start time in milliseconds - Default : 0L</param>
        /// <param name="end">End time in milliseconds - Default : 0L</param>
        /// <param name="statuses">Comma separated status values</param>
        /// <param name="tokenSymbol">Token to filter. If you want to return deposit records for all tokens, omit this parameter</param>
        /// <param name="offset">Number of records to skip - Default : 0L</param>
        /// <param name="hashes">The hashes of the transactions, normally its L2 tx hash, except the deposit which uses L1 tx hash.</param>
        /// <param name="transferTypes">The type of withdrawls you want returned</param>        
        /// <returns></returns>
        public List<ApiTransferData> GetTransfers(int limit = 50, long start = 0, long end = 0, List<OrderStatus> statuses = null, string tokenSymbol = null, int offset = 0, TransferTypes? transferTypes = null, string[] hashes = null)
        {
            return _client.GetTransfers(_apiKey, _accountId, limit, start, end, statuses, tokenSymbol, offset, transferTypes, hashes);
        }

        /// <summary>
        /// The Object you need in order to communicate with the Loopring API. Recommended to use as a signleton. Automatically gets the API Key using your Loopring Private Key
        /// </summary>        
        /// <param name="loopringPrivateKey">Your Layer 2 Private Key, needed for most api calls</param>
        /// <param name="ethPrivateKey">Your Layer 1, Ethereum Private Key, needed for some very specific API calls</param>
        /// <param name="accountId">Your Loopring Account ID, used for a surprising amount of calls</param>
        public Client(string apiUrl, (string secretKey, string ethAddress, string publicKeyX, string publicKeyY) l2Auth)
        {
            _ethAddress = l2Auth.ethAddress;
            _client = new SecureClient(apiUrl);
            _loopringPrivateKey = l2Auth.secretKey;
            _loopringPublicKeyX = l2Auth.publicKeyX;
            _loopringPublicKeyY = l2Auth.publicKeyY;
            _accountId = GetAccountInfo().accountId;
            _apiKey = ApiKey();
        }

        /// <summary>
        /// Returns the relayer's current time in millisecond
        /// </summary>
        /// <returns>Current time in milliseconds</returns>        
        public long Timestamp()
        {
            return _client.Timestamp();
        }

        /// <summary>
        /// Returns data associated with the user's exchange account.
        /// </summary>
        /// <param name="address">(optional) Ethereum / Loopring public address. If let null it will get your own account info</param>
        /// <returns>A lot of data about the account</returns>
        public Account GetAccountInfo(string address = null)
        {
            if (address == null)
                return _client.GetAccountInfo(_ethAddress);
            return _client.GetAccountInfo(address);
        }

        /// <summary>
        /// Gets the current exchange prices between varius cryptos on the Loopring Protocol
        /// </summary>        
        /// <param name="pairs">The tickers to retreive. (Ex. LRC-USDT, LRC-ETH)</param>
        /// <returns>Returns a list of all the ticker details for your requested tickers</returns>
        public List<Ticker> Ticker(params string[] pairs)
        {
            return _client.Ticker(pairs);
        }

        /// <summary>
        /// Return various configurations of Loopring.io
        /// </summary>
        /// <returns>Fees, exchange address, all sort of useful stuff</returns>
        public ExchangeInfo ExchangeInfo()
        {
            return _client.ExchangeInfo();
        }

        /// <summary>
        /// Get the ApiKey associated with the user's account.
        /// </summary>
        /// <returns>The api key</returns>
        /// <exception cref="System.Exception">Gets thrown when there's a problem getting info from the Loopring API endpoint</exception>
        public string ApiKey()
        {
            return _client.GetApiKey(_loopringPrivateKey, _accountId);
        }

        /// <summary>
        /// Get a list of all the markets available on the exchange
        /// </summary>
        /// <returns>List of all the markets available on the exchange and their configurations</returns>
        public List<Market> GetMarkets()
        {
            return _client.GetMarkets();

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
        public List<Candlestick> GetCandlesticks(string market, Intervals intervals, string start = null, string end = null, int? limit = null)
        {
            return _client.GetCandlesticks(market, intervals, start, end, limit);
        }
        /// <summary>
        /// Fetches, for all the tokens supported by Loopring, their fiat price.
        /// </summary>
        /// <param name="legal">The fiat currency to uses. Currently the following values are supported: USD,CNY,JPY,EUR,GBP,HKD</param>
        /// <returns>Fiat price of all the tokens in the system</returns>
        public List<Price> GetPrice(LegalCurrencies legal)
        {
            return _client.GetPrice(legal);
        }


        /// <summary>
        /// Returns the configurations of all supported tokens, including Ether.
        /// </summary>
        /// <returns>List of all the supported tokens and their configurations</returns>
        public List<TokenConfig> GetTokens()
        {
            return _client.GetTokens();
        }

        /// <summary>
        /// Returns the order book of a given trading pair.
        /// </summary>
        /// <param name="market">The ID of a trading pair.</param>
        /// <param name="level">Order book aggregation level, larger value means further price aggregation. Default: 2</param>
        /// <param name="limit">Maximum numbers of bids/a sks. Default : 50</param>
        /// <returns>Returns the order book of a given trading pair.</returns>
        public Depth GetDepth(string market, int level = 2, int limit = 50)
        {
            return _client.GetDepth(market, level, limit);
        }

        /// <summary>
        /// EXPERIMENTAL.
        /// </summary>
        /// <param name="market">The ID of a trading pair.</param>
        /// <param name="level">Order book aggregation level, larger value means further price aggregation. Default: 2</param>
        /// <param name="limit">Maximum numbers of bids/asks. Default : 50</param>
        /// <returns>Returns the order book of a given trading pair.</returns>
        public Depth GetMixDepth(string market, int level = 2, int limit = 50)
        {
            return _client.GetMixDepth(market, level, limit);
        }

        /// <summary>
        /// Submit an order to exchange two currencies, but with all the nonsense removed
        /// </summary>
        /// <param name="orderHash">The hash of the order you wish to nuke.</param>
        /// <param name="clientOrderId">The unique order ID of the client</param>
        /// <returns>Returns OrderResult which basically contains the status of your transaction after the cancel was succesfully requested</returns>
        public OrderResult CancelOrder(string orderHash, string clientOrderId)
        {
            return _client.CancelOrder(_loopringPrivateKey, _apiKey, _accountId, orderHash, clientOrderId);
        }

        /// <summary>
        /// Submit an order to exchange two currencies, but with all the nonsense removed
        /// </summary>
        /// <param name="sellCurrency">The name of the token you are selling (ETH, LRC, USDT, etc)</param>
        /// <param name="sellAmmount">How much of that token you are selling</param>
        /// <param name="buyCurrency">The name of the token you are buying (ETH, LRC, USDT, etc)</param>
        /// <param name="buyAmmount">How much of that token you are buying</param>        
        /// <param name="orderType">Order types, can be AMM, LIMIT_ORDER, MAKER_ONLY, TAKER_ONLY</param>
        /// <param name="poolAddress">The AMM pool address if order type is AMM</param>
        /// <returns>Returns OrderResult which basically contains the status of your transaction after it was succesfully requested</returns>
        public OrderResult SubmitOrder(
        string sellCurrency,
        decimal sellAmmount,
        string buyCurrency,
        decimal buyAmmount,
        OrderType orderType,
        string poolAddress = null)
        {
            return _client.SubmitOrder(_loopringPrivateKey, _apiKey, _accountId, sellCurrency, sellAmmount, buyCurrency, buyAmmount, orderType, poolAddress);
        }

        /// <summary>
        /// Submit an order to exchange two currencies
        /// </summary>
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
        public OrderResult SubmitOrder(
        Token sellToken,
        Token buyToken,
        bool allOrNone,
        bool fillAmountBOrS,
        int validUntil,
        int maxFeeBips = 20,
        string clientOrderId = null,
        OrderType? orderType = null,
        TradeChannel? tradeChannel = null,
        string taker = null,
        string poolAddress = null,
        string affiliate = null)

        {
            return _client.SubmitOrder(_loopringPrivateKey, _apiKey, _accountId, sellToken, buyToken, allOrNone, fillAmountBOrS, validUntil, maxFeeBips, clientOrderId, orderType, tradeChannel, taker, poolAddress, affiliate);
        }

        /// <summary>
        /// Requests a new API key from Loopring and starts using it, also returns it for storage.
        /// </summary>
        /// <param name="l2Pk"></param>
        /// <param name="apiKey"></param>
        /// <param name="accountId"></param>
        /// <returns>The new API Key</returns>
        public string UpdateApiKey()
        {
            _apiKey = _client.UpdateApiKey(_loopringPrivateKey, _apiKey, _accountId);
            return _apiKey;
        }

        /// <summary>
        /// Fetches the next order id for a given sold token
        /// </summary>        
        /// <param name="accountId">Loopring account identifier</param>
        /// <param name="sellTokenId">The unique identifier of the token which the user wants to sell in the next order.</param>
        /// <param name="maxNext">Return the max of the next available storageId, so any storageId > returned value is avaliable, to help user manage storageId by themselves. for example, if [20, 60, 100] is avaliable, all other ids < 100 is used before, user gets 20 if flag is false (and 60 in next run), but gets 100 if flag is true, so he can use 102, 104 freely</param>
        /// <returns>Returns an object instance of StorageId which contains the next offchainId and orderId</returns>
        public StorageId StorageId(int sellTokenId, int maxNext = 0)
        {
            return _client.StorageId(_apiKey, _accountId, sellTokenId, maxNext);
        }

        /// <summary>
        /// Get the details of an order based on order hash.
        /// </summary>        
        /// <param name="orderHash">The hash of the worder for which you want details</param>
        /// <returns>OrderDetails object filled with awesome order details</returns>
        /// <exception cref="System.Exception">Gets thrown when there's a problem getting info from the Loopring API endpoint</exception>
        public OrderDetails OrderDetails(string orderHash)
        {
            return _client.OrderDetails(_apiKey, _accountId, orderHash);
        }

        /// <summary>
        /// Get the details of a dualInvestmentMarket
        /// </summary>
        /// <param name="BaseSymbol">The Crypto you want to invest</param>
        /// <param name="Currency">The Crypto you want to parity with</param>
        /// <param name="DualType">DUAL_BASE if crypto, DUAL_CURRENCY if stablecoin</param>
        /// <param name="Limit">How many results per request, default 20</param>
        /// <param name="QuoteSymbol">Fiat currency to quote in</param>
        /// <returns></returns>
        public JObject GetDualInvestmetns(string BaseSymbol = "ETH", string Currency = "USDT", string DualType = "DUAL_BASE", int Limit = 20, string QuoteSymbol = "USD")
        {
            return _client.GetDualInvestmetns(BaseSymbol, Currency, DualType, Limit, QuoteSymbol);
        }

        public string StartDualInvestment(DualBaseModel dualInvestment, decimal stableCoinProfit, string stableCoinTokenName, decimal cryptoPorfit, string CryptoTokenName)
        {
            return _client.StartDualInvestment(_apiKey, _loopringPrivateKey, _ethPrivateKey, _accountId, _ethAddress, dualInvestment, stableCoinProfit, stableCoinTokenName, cryptoPorfit, CryptoTokenName, false);
        }

        /// <summary>
        /// Get the details of an order based on order hash.
        /// </summary>
        /// <param name="apiKey">Current Loopring API Key</param>
        /// <param name="accountId">Wallet Account Id</param>
        /// <param name="tokens">(Optional) list of the tokens which you want returned</param>
        /// <returns>OrderDetails object filled with awesome order details</returns>
        /// <exception cref="System.Exception">Gets thrown when there's a problem getting info from the Loopring API endpoint</exception>
        public List<Balance> Ballances(string tokens = null)
        {
            return _client.Ballances(_apiKey, _accountId, tokens);
        }

        /// <summary>
        /// Get how much fee you need to pay right now to carry out a transaction of a specified type
        /// </summary>        
        /// <param name="requestType">Off-chain request type</param>
        /// <param name="tokenSymbol">Required only for withdrawls - The token you wish to withdraw</param>
        /// <param name="amount">Required only for withdrawls - how much of that token you wish to withdraw</param>
        /// <returns>Returns the fee amount</returns>
        public OffchainFee OffchainFee(OffChainRequestType requestType, string tokenSymbol, string amount)
        {
            return _client.OffchainFee(_apiKey, _accountId, requestType, tokenSymbol, amount);
        }

        /// <summary>
        /// Get how much fee you need to pay right now to carry out a transaction of a specified type
        /// </summary>        
        /// <param name="requestType">Off-chain request type</param>
        /// <param name="tokenSymbol">Required only for withdrawls - The token you wish to withdraw</param>
        /// <param name="amount">Required only for withdrawls - how much of that token you wish to withdraw</param>
        /// <returns>Returns the fee amount</returns>
        public OffchainFee OffchainFee(OffChainRequestType requestType, string tokenSymbol, decimal amount)
        {
            return _client.OffchainFee(_apiKey, _accountId, requestType, tokenSymbol, (amount * 1000000000000000000m).ToString());
        }

        /// <summary>
        /// Get a list of orders satisfying certain criteria.
        /// </summary>
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
        public List<OrderDetails> OrdersDetails(
            int limit = 50,
            int offset = 0,
            string market = null,
            long start = 0,
            long end = 0,
            Side? side = null,
            List<OrderStatus> statuses = null,
            List<OrderType> orderTypes = null,
            List<TradeChannel> tradeChannels = null)
        {
            return _client.Orders(_apiKey, _accountId, limit, offset, market, start, end, side, statuses, orderTypes, tradeChannels);
        }

        /// <summary>
        /// Returns a list of Ethereum transactions from users for exchange account registration.
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="start">Lower bound of order's creation timestamp in millisecond (ex. 1567053142000)</param>
        /// <param name="end">Upper bound of order's creation timestamp in millisecond (ex. 1567053242000)</param>
        /// <param name="limit">How many results per call? Default 50</param>
        /// <param name="offset">How many results to skip? Default 0 </param>
        /// <param name="statuses">Statuses which you would like to filter by</param>
        /// <returns>List of Ethereum transactions from users for exchange account registration.</returns>
        public List<ApiTransaction> CreateInfo(int limit = 50, int offset = 0, long start = 0, long end = 0, List<Status> statuses = null)
        {
            return _client.CreateInfo(_apiKey, _accountId, limit, offset, start, end, statuses);
        }

        /// <summary>
        /// Returns a list Ethereum transactions from users for resetting exchange passwords.
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="start">Lower bound of order's creation timestamp in millisecond (ex. 1567053142000)</param>
        /// <param name="end">Upper bound of order's creation timestamp in millisecond (ex. 1567053242000)</param>
        /// <param name="limit">How many results per call? Default 50</param>
        /// <param name="offset">How many results to skip? Default 0 </param>
        /// <param name="statuses">Statuses which you would like to filter by</param>
        /// <returns>List of Ethereum transactions from users for resetting exchange passwords.</returns>
        public List<ApiTransaction> UpdateInfo(int limit = 50, int offset = 0, long start = 0, long end = 0, List<Status> statuses = null)
        {
            return _client.UpdateInfo(_apiKey, _accountId, limit, offset, start, end, statuses);
        }

        /// <summary>
        /// Returns a list of deposit records for the given user.
        /// </summary>
        /// <param name="limit">Number of records to return</param>
        /// <param name="start">Start time in milliseconds - Default : 0L</param>
        /// <param name="end">End time in milliseconds - Default : 0L</param>
        /// <param name="statuses">Comma separated status values</param>
        /// <param name="tokenSymbol">Token to filter. If you want to return deposit records for all tokens, omit this parameter</param>
        /// <param name="offset">Number of records to skip - Default : 0L</param>
        /// <param name="hashes">The hashes of the transactions, normally its L2 tx hash, except the deposit which uses L1 tx hash.</param>
        /// <returns>A list of deposit transactions. Are you paying attention?</returns>
        public List<ApiDepositTransaction> GetDeposits(int limit = 50, long start = 0, long end = 0, List<OrderStatus> statuses = null, string tokenSymbol = null, int offset = 0, string[] hashes = null)
        {
            return _client.GetDeposits(_apiKey, _accountId, limit, start, end, statuses, tokenSymbol, offset, hashes);
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
            return _client.Transfer(_apiKey, _loopringPrivateKey, _ethPrivateKey, request, memo, clientId, counterFactualInfo);
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
            return _client.Transfer(_apiKey, _loopringPrivateKey, _ethPrivateKey, _accountId, _ethAddress, toAddress, token, value, feeToken, memo);
        }

        /// <summary>
        /// Query trades with specified market
        /// </summary>
        /// <param name="market">Single market to query</param>
        /// <param name="limit">Number of queries</param>
        /// <param name="fillTypes">Filter by types of fills for the trade</param>
        /// <returns>List of Trades</returns>
        public List<Trade> GetTrades(string market, int? limit = null, FillTypes[] fillTypes = null)
        {
            return _client.GetTrades(market, limit, fillTypes);
        }

        /// <summary>
        /// WARNING!!! This has a fee asociated with it. Make a OffchainFee request of type OffChainRequestType.UpdateAccount to see what the fee is.
        /// Updates the EDDSA key associated with the specified account, making the previous one invalid in the process.
        /// </summary>   
        /// <param name="feeToken">The token in which the fee should be paid for this operation</param>
        /// <returns>Returns the hash and status of your requested operation</returns>
        public virtual OperationResult RequestNewL2PrivateKey(string feeToken)
        {
            return _client.UpdateAccount(_apiKey, _ethPrivateKey, _loopringPrivateKey, _accountId, feeToken, _ethAddress, ExchangeInfo().exchangeAddress);
        }

        /// <summary>
        /// WARNING!!! This has a fee asociated with it. Make a OffchainFee request of type OffChainRequestType.UpdateAccount to see what the fee is.
        /// Updates the EDDSA key associated with the specified account, making the previous one invalid in the process.
        /// </summary>   
        /// <param name="req">A UpdateAccountRequest object containing all the needed information for this request</param>
        /// <param name="counterFactualInfo">(Optional)Not entirely sure. Official documentation says: field.UpdateAccountRequestV3.counterFactualInfo</param>
        /// <returns>Returns the hash and status of your requested operation</returns>
        public OperationResult RequestNewL2PrivateKey(UpdateAccountRequest req, CounterFactualInfo counterFactualInfo)
        {
            return _client.UpdateAccount(_loopringPrivateKey, _ethPrivateKey, req, counterFactualInfo);
        }

        /// <summary>
        /// Get's the l2 block info for a requested block id
        /// </summary>   
        /// <param name="id">The l2 block id</param>
        /// <returns>Returns the l2 block info for the requested block id</returns>
        public L2BlockInfo Get2BlockInfo(int id)
        {
            return _client.GetL2BlockInfo(_apiKey, id);
        }

        /// <summary>
        /// Gets pending transactions to be packed into next block
        /// </summary>   
        /// <returns>Returns the pending transactions for next block</returns>
        public List<PendingRequest> GetPendingRequests()
        {
            return _client.GetPendingRequests(_apiKey);
        }

        /// <summary>
        /// Returns amm pool trade transactions
        /// </summary>
        /// <param name="ammPoolAddress">The address of the pool on which the swap was submitted</param>
        /// <param name="limit">How many trades to return per call. Default 50</param>
        /// <param name="offset">How many trades to skip. Default 0</param>
        /// <returns>Returns the AMM pool trade transactions</returns>
        public AmmPoolTrades GetAmmPoolTrades(string ammPoolAddress, int limit = 50, int offset = 0)
        {
            return _client.GetAmmPoolTrades(ammPoolAddress, limit, offset);
        }

        /// <summary>
        /// Returns the users AMM join exit transactions
        /// </summary>
        /// <param name="accountId">Loopring accountId</param>
        /// <param name="start">Date time in milliseconds to start fetching AMM transactions. Default 0</param>
        /// <param name="end">Date time in milliseconds to end fetching AMM transactions. Default 0</param>
        /// <param name="limit">How many transactions to return per call. Default 50</param>
        /// <param name="offset">How many transactions to skip. Default 0</param>
        /// <param name="txTypes">Transaction type to filter on. Default null</param>
        /// <param name="txStatus">Transaction status to filter on. Default null</param>
        /// <param name="ammPoolAddress">The address of the AMM pool. Default null</param>
        /// <returns>Returns the users AMM join exit transactions</returns>
        public AmmJoinExitTransactions GetAmmJoinExitTransactions(int accountId, long start = 0, long end = 0, int limit = 50, int offset = 0, string txTypes = null, string txStatus = null, string ammPoolAddress = null)
        {
            return _client.GetAmmJoinExitTransactions(_apiKey, accountId, start, end, limit, offset, txTypes, txStatus, ammPoolAddress);
        }

        /// <summary>
        /// Returns the users trade history
        /// </summary>
        /// <param name="accountId">Loopring accountId</param>
        /// <param name="market" example="LRC-ETH">Trading pair. Default null</param>
        /// <param name="orderHash">The order Hash. Default nul</param>
        /// <param name="offset">How many transactions to skip. Default 0</param>
        /// <param name="limit">How many transactions to return. Default 50</param>
        /// <param name="fromId">The begin id of query. Default 0</param>
        /// <param name="fillTypes">Fill type. Can be dex or amm. Default null</param>
        /// <returns>Returns the users trade history</returns>
        public TradeHistory GetTradeHistory(int accountId, string market = null, string orderHash = null, int offset = 0, int limit = 50, int fromId = 0, FillTypes[] fillTypes = null)
        {
            return _client.GetTradeHistory(_apiKey, accountId, market, orderHash, offset, limit, fromId, fillTypes);
        }

        /// <summary>
        /// Returns the fee rate of users placing orders in specific markets
        /// </summary>
        /// <param name="accountId">Loopring accountId</param>
        /// <param name="market" example="LRC-ETH">Trading pair</param>
        /// <param name="tokenB">Token Id</param>
        /// <param name="amountB">Amount to buy</param>
        /// <returns>Returns the fee rate of users placing orders in specific markets</returns>
        public OrderFee OrderFee(int accountId, string market, string tokenB, string amountB)
        {
            return _client.OrderFee(_apiKey, accountId, market, tokenB, amountB);
        }

        /// <summary>
        /// Returns 2 minimum amounts, one is based on users fee rate, the other is based on the maximum fee bips which is 0.6%. In other words, if user wants to keep fee rate, the minimum order is higher, otherwise he needs to pay more but can place less amount orders.
        /// </summary>
        /// <param name="accountId">Loopring accountId</param>
        /// <param name="market" example="LRC-ETH">Trading pair</param>
        /// <returns>Returns 2 minimum amounts, one is based on users fee rate, the other is based on the maximum fee bips which is 0.6%. In other words, if user wants to keep fee rate, the minimum order is higher, otherwise he needs to pay more but can place less amount orders.</returns>
        public OrderUserRateAmount OrderUserRateAmount(int accountId, string market)
        {
            return _client.OrderUserRateAmount(_apiKey, accountId, market);
        }

        /// <summary>
        /// Returns the configurations of all supported AMM pools
        /// </summary>
        /// <returns>Returns the configurations of all supported AMM pools</returns>
        public AmmPoolConfiguration GetAmmPools()
        {
            return _client.GetAmmPools(_apiKey);
        }

        /// <summary>
        /// Returns the snapshot of specific AMM pool
        /// </summary>
        /// <param name="poolAddress">The AMM pool address</param>
        /// <returns>Returns the snapshot of specific AMM pool</returns>
        public AmmPoolBalance GetAmmPoolBalance(string poolAddress)
        {
            return _client.GetAmmPoolBalance(_apiKey, poolAddress);
        }
    }
}
