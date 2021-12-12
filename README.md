# LoopringAPI


## WARNING! The API Is currently under construction and not ready for use

## What is?

A C# NuGet Package wrapper around the Loopring API endpoints to greatly simplify communication. It should theoretically be as simple as importing the package, creating a singleton client, and off to the races. 

## The what api?

The Loopring API https://docs.loopring.io/

## How be do?

Basically it's just

```csharp
LoopringAPI.Client client = new LoopringAPI.Client("<YOUR API KEY HERE>", "<YOUR LOOPRING PRIVATE KEY HERE>", "<YOUR ETHEREUM PRIVATE KEY HERE>");
var storageId = await client.StorageId("33794", 1);
```

Nothing too fancy. but let me explain apiKey, l2Pk and l1Pk

There are three things that are needed to use the LoopringAPI endpoints. Some endpoints require none of those keys, some of them require all of them, so let me explain what they are.

- apiKey = Your Loopring Wallet API key. You can get this by exporting your loopring wallet info, it's the "ApiKey" field.
- l2Pk = Your Loopring Wallet Private Key. You can get this by exportin your loopring wallet info, it's the "PrivateKey" field
- l1Pk = This is your Etherium Private Key, the one tied to your Loopring wallet. It's not part of the export mentioned above, you'll need to figure out how to get it based on your needs

As for which endpoint requires what, I'll atempt to make a table bellow with all the API calls supported in this package and which keys are needed for which call

|API Call|ApiKey|l1Pk|l2Pk|Description
|-|-|-|-|-|
|Ticker|N|N|N|Gets the price information for any crypto pair available to trade on Loopring|
|StorageId|Y|N|N|Fetches the next order id for a given sold token
|OffchainFee|Y|N|N|Get how much fee you need to pay right now to carry out a transaction of a specified type

## LoopringAPI.TestConsole

There is a test console in the project that shows off how to use some of the parts within the API. I was also using it for testing. The console is in .net 6 so be prepared for that (VS 2022 and C# 10)

The most important thing about the test console is that you need to run it for the first time and fill in the file as instructed in the error that gets thrown. LoopringApi needs those Api Keys in order to run.


## Secure client

The regular client has a security flaw in it, in that it stores the apiKey, l2Pk and L1Pk in memory within itself, in order to facilitate easier API calls. This is great for server-side aplications where you have full control of the machine the code is running on so there's no risk of someone trying to extrat those keys from the hardware's memory.

However, if creating a client application that will run on users's hardware or on the web, it is recommended to use the secure client

```csharp
LoopringAPI.SecureClient client = new LoopringAPI.SecureClient();
var storageId = await client.StorageId("<YOUR API KEY HERE>","33794", 1);
```
The secure client does not accept the apiKey, l2Pk, l1Pk as properties, but rather requires them as parameters on every method that needs them. This means that the keys are not stored in memory within the API class and it is up to the developer to decide how they want to secure this very sensitive infomation.

The regular client also makes use of the SecureClient internally, just that it stores the three keys within itself so they are not provided every single time a method is called in the API, easing development a little.

## Credits to
fudgey.eth - Creator of PoseidonSharp, without which it would be impossible to actually do transfers within this api. https://github.com/fudgebucket27/PoseidonSharp