# LoopringSharp


## WARNING! The API Is currently under construction and not ready for use

## What is?

A C# NuGet Package wrapper around the Loopring API endpoints to greatly simplify communication. It should theoretically be as simple as importing the package, creating a singleton client, and off to the races. 

## The what api?

The Loopring API https://docs.loopring.io/

## How be do?

Basically it's just

```csharp
LoopringSharp.Client client = new LoopringSharp.Client("<apiKey>","<loopring private key>","<ethereum private key>",<account id>, "<ethreum public address>", "<api url>");
```

More explicitly:

```csharp
LoopringSharp.Client client = new LoopringSharp.Client("pJ3sU5kJ489fLmrewslkreownsbTunMK9fcusikhK6tn5nEDY4vvkWg8PgV2R","0x444444444444444444444444444444444444444444444444444444444444444","0x5555555555555555555555555555555555555555555555555555555555555555", 1, "0x6666666666666666666666666666666666666666", "https://uat2.loopring.io/");
var storageId = await client.StorageId(1);
```

The is also the simplified version:
```csharp
LoopringSharp.Client client = new LoopringSharp.Client("<loopring private key>","<ethereum private key>","<api url>");
```
Which uses API calls within the constructor to gather the missing information. 

The values above are stored in RAM, which means they could be read by specialist software. If you are making a secure app please use the LoopringSharp.SecureClient which doesn't store anything in memory. 

For client applications (like say you want to make a windows charting application or similar), there are now two better alternatives: MetaMask and WalletConnect. You can instruct the API to connect to one of those two services in order to get the information it needs. This is very secure since the private wallet information is not stored in-memory but instead lives within a secure third party service that does all the transfer validation and signing.
```csharp
LoopringAPI.Client client = new LoopringAPI.Client("<api url>");
var storageId = await client.StorageId(1);
```
In the future, I'll look into adding WalletConnect support as well

Here's a list of all the Loopring API Endpoints and which ones have been implemented so far in LoopringSharp

|Implemented?|API Call|Endpoint Path|Description|
|-|-|-|-|
|X|Timestamp|/api/v3/timestamp|Returns the relayer's current time in millisecond|
|X|ApiKey|/api/v3/apiKey|Get the ApiKey associated with the user's account|
|X|UpdateApiKey|/api/v3/apiKey|Request a new ApiKey for this user account|
|X|StorageId|/api/v3/storageId|Fetches the next order id for a given sold token|
|X|OrderDetails|/api/v3/order|Gets the details of one specific order using the hashId|
|X|SubmitOrder|/api/v3/order|Submits a new exchange / swap order|
|X|DeleteOrder|/api/v3/order|Cancels a open exchange / swap order|
|X|OrdersDetails|/api/v3/orders|Gets a detailed list of multiple orders based on the filtering criteria|
|X|GetMarkets|/api/v3/exchange/markets|Get a list of all the markets available on the exchange|
|X|GetTokens|/api/v3/exchange/tokens|Returns the configurations of all supported tokens, including Ether.|
|X|ExchangeInfo|/api/v3/exchange/info|Gets all sorts of properties about the exchange you're contacting|
|X|GetDepth|/api/v3/depth|Returns the order book of a given trading pair.|
|X|Ticker|/api/v3/ticker|Gets the price information for any crypto pair available to trade on Loopring|
|X|GetCandlesticks|/api/v3/candlestick|Return the candlestick data of a given trading pair.|
|X|GetPrice|/api/v3/price|Fetches, for all the tokens supported by Loopring, their fiat price.|
|X|GetTrades|/api/v3/trade|Query latest trades with specified market|
|X|Transfer|/api/v3/transfer|Send some tokens to anyone else on L2|
|X|GetAccountInfo|/api/v3/account|Returns data associated with the user's exchange account|
|X|RequestNewL2PrivateKey|/api/v3/account|Generates a new L2 private key for your account. Does not return they key|
|X|CreateInfo|/api/v3/user/createInfo|Returns a list of Ethereum transactions from users for exchange account registration.|
|X|UpdateInfo|/api/v3/user/updateInfo|Returns a list Ethereum transactions from users for resetting exchange passwords.|
|X|Ballances|/api/v3/user/balances|Returns user's Ether and token balances on exchange.|
|X|GetDeposits|/api/v3/user/deposits|Returns a list of deposit records for the given user.|
|X|GetWithdrawls|/api/v3/user/withdrawals|Get user onchain withdrawal history.|
||Withdraw|/api/v3/user/withdrawals|Submit offchain withdraw request|
|X|GetTransfers|/api/v3/user/transfers|Get user transfer list.|
|X|GetTradeHistory|/api/v3/user/trades|Get user trade history.|
|X|OrderFee|/api/v3/user/orderFee|Returns the fee rate of users placing orders in specific markets|
|X|OrderUserRateAmount|/api/v3/user/orderUserRateAmount|This API returns 2 minimum amounts, one is based on users fee rate, the other is based on the maximum fee bips which is 0.6%. In other words, if user wants to keep fee rate, the minimum order is higher, otherwise he needs to pay more but can place less amount orders.|
|X|OffchainFee|api/v3/user/offchainFee|Returns the fee amount|
|X|GetAmmPools|/api/v3/amm/pools|Get AMM pool configurations|
|X|GetAmmPoolBalance|/api/v3/amm/balance|Get AMM pool balance snapshot|
||JoinAmmPool|/api/v3/amm/join|Join into AMM pool|
||ExitAmmPool|/api/v3/amm/exit|Exit an AMM pool|
|X|AmmTransactions|/api/v3/amm/user/transactions|Return the user's AMM join/exit transactions|
|X|AmmTrades|/api/v3/amm/trades|get AMM pool trade transactions|
|X|GetL2BlockInfo|/api/v3/block/getBlock|Get L2 block info by block id|
|X|GetPendingRequests|/api/v3/block/getPendingRequests|Get pending txs to be packed into next block|

## LoopringAPI.TestConsole

There is a test console in the project that shows off how to use some of the parts within the API. I was also using it for testing. The console is in .net 6 so be prepared for that (VS 2022 and C# 10)

The most important thing about the test console is that you need to run it for the first time and fill in the file as instructed in the error that gets thrown. LoopringSharp needs those Api Keys in order to run.

## LoopringSharp.Layer2TransferExample

This is a windows forms app that basically sends crypto from your wallet to a destination of your choosing. The code is explained in detail in the comments so I encourage you to read through it and understand how it works. Be careful if selecting the real network (not the test net) as you will end up sending your own crypto to someone by mistake (hopefully to me :D)

## Secure client

The regular client has a security flaw in it, in that it stores the apiKey, l2Pk and L1Pk in memory within itself, in order to facilitate easier API calls. This is great for server-side aplications where you have full control of the machine on which the code is running, so there's no risk of someone trying to extract those keys from RAM with a RAM editor.

However, if creating a client application which runs on the users's hardware or on the web, it is recommended to use the secure client

```csharp
LoopringSharp.SecureClient client = new LoopringSharp.SecureClient("https://uat2.loopring.io/");
var storageId = await client.StorageId("<YOUR API KEY HERE>","<YOUR ACCOUNT ID HERE>", 1);
```
The secure client does not accept the apiKey, l2Pk, l1Pk as properties, but rather requires them as parameters on every method that needs them. This means that the keys are not stored in memory within the API class and it is up to the developer to decide how they want to secure this very sensitive infomation.

The regular client also makes use of the SecureClient internally, just that it stores the three keys within itself so they are not provided every single time a method is called in the API, easing development a little.

## Credits to
fudgey.eth - Creator of PoseidonSharp, without which it would be impossible to actually do transfers within this api. https://github.com/fudgebucket27/PoseidonSharp

## Donations
![Donations](https://raw.githubusercontent.com/taranasus/LoopringSharp/main/donations.png 'donations')
