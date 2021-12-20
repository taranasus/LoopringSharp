# LoopringAPI


## WARNING! The API Is currently under construction and not ready for use

## What is?

A C# NuGet Package wrapper around the Loopring API endpoints to greatly simplify communication. It should theoretically be as simple as importing the package, creating a singleton client, and off to the races. 

## The what api?

The Loopring API https://docs.loopring.io/

## How be do?

Basically it's just

```csharp
LoopringAPI.Client client = new LoopringAPI.Client("<apiKey>","<loopring private key>","<ethereum private key>",<account id>, "<ethreum public address>", "<api url>");
```

More explicitly:

```csharp
LoopringAPI.Client client = new LoopringAPI.Client("pJ3sU5kJ489fLmrewslkreownsbTunMK9fcusikhK6tn5nEDY4vvkWg8PgV2R","0x444444444444444444444444444444444444444444444444444444444444444","0x5555555555555555555555555555555555555555555555555555555555555555", 1, "0x6666666666666666666666666666666666666666", "https://uat2.loopring.io/");
var storageId = await client.StorageId(1);
```

The is also the simplified version:
```csharp
LoopringAPI.Client client = new LoopringAPI.Client("<loopring private key>","<ethereum private key>","<api url>");
```
Which uses API calls within the constructor to gather the missing information. 

The values above are stored in RAM, which means they could be read by specialist software. If you are making a secure app please use the LoopringAPI.SecureClient which doesn't store anything in memory. 

For client applications (like say you want to make a windows charting application or similar), there is a option which allows the client to interact with MetaMask on the user's computer in order to get all the security data. This is very secure since all the critical user information is stored in MetaMask, removing most security concerns from your application.
```csharp
LoopringAPI.Client client = new LoopringAPI.Client("<api url>");
var storageId = await client.StorageId(1);
```
In the future, I'll look into adding WalletConnect support as well

There are three keys that are needed to use the LoopringAPI endpoints. Some endpoints require none of those keys, some of them require all of them, so let me explain what they are.

- apiKey = Your Loopring Wallet API key. You can get this by exporting your loopring wallet info, it's the "ApiKey" field. You can also get it by using the ApiKey() method in the client
- l2Pk = Your Loopring Wallet Private Key. You can get this by exportin your loopring wallet info, it's the "PrivateKey" field. If using MetaMask, this is retreived automatically as needed.
- l1Pk = This is your Etherium Private Key, the one tied to your Loopring wallet. It's not part of the export mentioned above, you'll need to figure out how to get it based on your needs. If using MetaMask, this is retreived automatically as needed.

As for which endpoint requires what, I'll atempt to make a table bellow with all the API calls supported in this package and which keys are needed for which call

|API Call|ApiKey|l1Pk|l2Pk|Description
|-|-|-|-|-|
|Ticker|N|N|N|Gets the price information for any crypto pair available to trade on Loopring|
|Timestamp|N|N|N|Returns the relayer's current time in millisecond
|GetAccountInfo|N|N|N|Returns data associated with the user's exchange account
|ExchangeInfo|N|N|N|Gets all sorts of properties about the exchange you're contacting
|GetMarkets|N|N|N|Get a list of all the markets available on the exchange
|GetTokens|N|N|N|Returns the configurations of all supported tokens, including Ether.
|GetDepth|N|N|N|Returns the order book of a given trading pair.
|GetCandlesticks|N|N|N|Return the candlestick data of a given trading pair.
|GetPrice|N|N|N|Fetches, for all the tokens supported by Loopring, their fiat price.
|ApiKey|N|N|Y|Get the ApiKey associated with the user's account
|DeleteOrder|Y|N|Y|Cancels a open exchange / swap order
|SubmitOrder|Y|N|Y|Submits a new exchange / swap order
|UpdateApiKey|Y|N|Y|Request a new ApiKey for this user account
|StorageId|Y|N|N|Fetches the next order id for a given sold token
|OffchainFee|Y|N|N|Get how much fee you need to pay right now to carry out a transaction of a specified type
|OrderDetails|Y|N|N|Gets the details of one specific order using the hashId
|OrdersDetails|Y|N|N|Gets a detailed list of multiple orders based on the filtering criteria
|Transfer|Y|Y|Y|Send some tokens to anyone else on L2

## LoopringAPI.TestConsole

There is a test console in the project that shows off how to use some of the parts within the API. I was also using it for testing. The console is in .net 6 so be prepared for that (VS 2022 and C# 10)

The most important thing about the test console is that you need to run it for the first time and fill in the file as instructed in the error that gets thrown. LoopringApi needs those Api Keys in order to run.

## LoopringAPI.Layer2TransferExample

This is a windows forms app that basically sends crypto from your wallet to a destination of your choosing. The code is explained in detail in the comments so I encourage you to read through it and understand how it works. Be careful if selecting the real network (not the test net) as you will end up sending your own crypto to someone by mistake (hopefully to me :D)

## Secure client

The regular client has a security flaw in it, in that it stores the apiKey, l2Pk and L1Pk in memory within itself, in order to facilitate easier API calls. This is great for server-side aplications where you have full control of the machine on which the code is running, so there's no risk of someone trying to extract those keys from RAM with a RAM editor.

However, if creating a client application which runs on the users's hardware or on the web, it is recommended to use the secure client

```csharp
LoopringAPI.SecureClient client = new LoopringAPI.SecureClient("https://uat2.loopring.io/");
var storageId = await client.StorageId("<YOUR API KEY HERE>","<YOUR ACCOUNT ID HERE>", 1);
```
The secure client does not accept the apiKey, l2Pk, l1Pk as properties, but rather requires them as parameters on every method that needs them. This means that the keys are not stored in memory within the API class and it is up to the developer to decide how they want to secure this very sensitive infomation.

The regular client also makes use of the SecureClient internally, just that it stores the three keys within itself so they are not provided every single time a method is called in the API, easing development a little.

## Credits to
fudgey.eth - Creator of PoseidonSharp, without which it would be impossible to actually do transfers within this api. https://github.com/fudgebucket27/PoseidonSharp

## Donations
![Donations](donations.png 'donations')