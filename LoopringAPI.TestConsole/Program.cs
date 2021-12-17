using LoopringAPI;
using LoopringAPI.TestConsole;
using Newtonsoft.Json;
using PoseidonSharp;
using System.Net;
using System.Net.Security;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

Console.WriteLine("Hello, Loops!");

ApiKeys apiKeys = ReadConfigFile(false) ;

LoopringAPI.Client client = new LoopringAPI.Client(apiKeys.l2Pk, apiKeys.l1Pk, apiKeys.apiUrl);

#region TestTransfer
Console.WriteLine("Let's start with a TRANSFER TEST of 1 LRC. DO YOU WISH TO CONTINUE? [Y]ontinue / [S]kip");
var choice = Console.ReadLine();
if (choice.ToLower().StartsWith("y"))
{
    string transfertoAddress = "0x2e76ebd1c7c0c8e7c2b875b6d505a260c525d25e";    
    Console.WriteLine("TYPE RECEPIENT ADDRESS BELLOW:");
    Console.Write("[DEFAULT: " + transfertoAddress + "] ");
    string potentialNewAddress = Console.ReadLine();
    if (potentialNewAddress.StartsWith("0x"))
    {
        transfertoAddress = potentialNewAddress;
        Console.WriteLine("Destination address changed to: " + transfertoAddress);
    }
    Console.WriteLine("BEGINNING TRANSFER!");
    var transferResult = await client.Transfer(transfertoAddress, "LRC", 1m, "LRC", "aaaa");
    Console.WriteLine("TRANSFER COMPLETE:");
    Console.WriteLine(JsonConvert.SerializeObject(transferResult, Formatting.Indented));
}
else
{
    Console.WriteLine("Skipping Transfer test as it costs MONEY!");
}

Console.WriteLine();
#endregion

Console.WriteLine("REVIEW RESULTS AND PRESS ENTER TO CONTINUE!");
Console.ReadLine();
Console.Clear();

//Environment.Exit(0);   

#region Exchange info
Console.WriteLine("Exchange Info: ");
var exchangeInfo = await client.ExchangeInfo();
Console.WriteLine(JsonConvert.SerializeObject(exchangeInfo, Formatting.Indented));
Console.WriteLine("");
#endregion

#region Ticker
Console.WriteLine("Testing TICKER: ");
var tickers = await client.Ticker("LRC-ETH");
foreach (var ticker in tickers)
{
    Console.WriteLine(ticker.PairId + " - ASK: " + ticker.LowestAskPrice);
}
Console.WriteLine("");
#endregion

#region Timestamp
var timestamp = await client.Timestamp();
Console.WriteLine("Testing timestamp: " + timestamp);
Console.WriteLine("");
#endregion

#region StorageId
Console.WriteLine("Testing StorageId");

var storageId = await client.StorageId(1);
Console.WriteLine("Normal: " + JsonConvert.SerializeObject(storageId));

storageId = await client.StorageId(1, 1);
Console.WriteLine("MaxNext: " + JsonConvert.SerializeObject(storageId));
#endregion

#region GetApiKey
Console.WriteLine("Testing APIKEY GET");

var apikey = await client.ApiKey();
Console.WriteLine("Key: " + apikey);

Console.WriteLine();
#endregion

Console.WriteLine("REVIEW RESULTS AND PRESS ENTER TO CONTINUE!");
Console.ReadLine();
Console.Clear();

#region UpdateApiKey
Console.WriteLine("Testing APIKEY UPDATE");
Console.WriteLine("WARNING! WARNING WARNING WARNING! THIS WILL GENERATE A NEW API KEY ON YOUR WALLET! YOU WILL NEED TO USE THAT KEY GOING FORWARD.");
Console.Write("Are you sure you want to continue with this test? [Y]es / [S]kip: ");
choice = Console.ReadLine();
if (choice.ToLower().StartsWith("y"))
{
    apikey = await client.UpdateApiKey();
    Console.WriteLine("New Key: " + apikey);
    Console.WriteLine("Please make a note of the key above before continuing, as you will need it going forward. Press enter to continue...");
    Console.ReadLine();
}
else
{
    Console.WriteLine("Skipping API Key Re-generation");
}

Console.WriteLine();
#endregion

Console.WriteLine("REVIEW RESULTS AND PRESS ENTER TO CONTINUE!");
Console.ReadLine();
Console.Clear();

#region OffChainFee
Console.WriteLine("Testing OffChainFee - Transfer");
var fee = await client.OffchainFee(LoopringAPI.OffChainRequestType.Transfer, null, null);
Console.WriteLine("Fee: " + JsonConvert.SerializeObject(fee, Formatting.Indented));
Console.WriteLine("Testing OffChainFee - OffchainWithdrawl");
fee = await client.OffchainFee(LoopringAPI.OffChainRequestType.OffchainWithdrawl, "LRC", "10000000000");
Console.WriteLine("Fee: " + JsonConvert.SerializeObject(fee, Formatting.Indented));
Console.WriteLine("");

#endregion

Console.WriteLine("REVIEW RESULTS AND PRESS ENTER TO CONTINUE!");
Console.ReadLine();
Console.Clear();

#region Orders

Console.WriteLine("-------- TESTING ORDERS ---------");
Console.WriteLine("Testing order submit! 0.3 ETH -> 1000 LRC");
var tradeResult = await client.SubmitOrder(
        sellToken: new LoopringAPI.Token() { tokenId = 0, /*ETH*/ volume = "30000000000000000" /* 0.03 ETH */  },
        buyToken: new LoopringAPI.Token() { tokenId = 1, /*LRC*/ volume = "1000000000000000000000" /* 1000 LRC */ },
        allOrNone: false,
        fillAmountBOrS: false,
        validUntil: 1700000000, // Will expire eventually...
        maxFeeBips: 63,
        clientOrderId: null,
        orderType: LoopringAPI.OrderType.TAKER_ONLY,
        tradeChannel: LoopringAPI.TradeChannel.MIXED
    );

Console.WriteLine("Testing simple order submit! 0.04 ETH -> 150 LRC");
var simpleTradeResult = await client.SubmitOrder(
        sellCurrency: "ETH",
        sellAmmount: 0.04m,
        buyCurrency: "LRC",
        buyAmmount: 150,
        orderType: LoopringAPI.OrderType.MAKER_ONLY
    );
Console.WriteLine("Trade result:");
Console.WriteLine(JsonConvert.SerializeObject(tradeResult, Formatting.Indented));

Console.WriteLine("Simple Trade Result:");
Console.WriteLine(JsonConvert.SerializeObject(simpleTradeResult, Formatting.Indented));
Console.WriteLine("");

Console.WriteLine("REVIEW RESULTS AND PRESS ENTER TO CONTINUE!");
Console.ReadLine();
Console.Clear();

Console.WriteLine("Gonna take a 1 second pause here...");
System.Threading.Thread.Sleep(1000);
Console.WriteLine("");

Console.WriteLine("Let's get the details around those trades: ");
Console.WriteLine("Normal Order:");
var normalTradeDetails = await client.OrderDetails(tradeResult.hash);
Console.WriteLine(JsonConvert.SerializeObject(normalTradeDetails, Formatting.Indented));
Console.WriteLine("Simple Order:");
var simpleTradeDetails = await client.OrderDetails(simpleTradeResult.hash);
Console.WriteLine(JsonConvert.SerializeObject(simpleTradeDetails, Formatting.Indented));

Console.WriteLine("REVIEW RESULTS AND PRESS ENTER TO CONTINUE!");
Console.ReadLine();
Console.Clear();

Console.WriteLine("Cancel both trades if they are still active: ");
if (normalTradeDetails.status == LoopringAPI.OrderStatus.processing)
{
    var normalDeleteResult = await client.CancelOrder(normalTradeDetails.hash, simpleTradeDetails.clientOrderId);
    Console.WriteLine("CANCELED normal trade");
    Console.WriteLine(JsonConvert.SerializeObject(normalDeleteResult, Formatting.Indented));
}
else
    Console.WriteLine("Normal trade no longer active anyway...");

if (simpleTradeDetails.status == LoopringAPI.OrderStatus.processing)
{
    var simpleDeleteResult = await client.CancelOrder(simpleTradeDetails.hash, simpleTradeDetails.clientOrderId);
    Console.WriteLine("CANCELED simple trade:");
    Console.WriteLine(JsonConvert.SerializeObject(simpleDeleteResult, Formatting.Indented));
}
else
    Console.WriteLine("simple trade no longer active anyway...");

Console.WriteLine("Wana get some previous trades to see if the get trades works? [Y]es / [S]kip: ");
choice = Console.ReadLine();
if (choice.ToLower().StartsWith("y"))
{
    Console.Clear();

    var results = await client.OrdersDetails(5);
    Console.WriteLine("You asked for it: ");
    Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
}

#endregion

Console.ReadLine();

static ApiKeys ReadConfigFile(bool prod)
{
    ApiKeys result;
    string filename = "apiKeys.json";
    if(prod)
    {
        filename = "apiKeysProd.json";
    }

    if (!File.Exists(filename))
    {
        result = new ApiKeys()
        {
            l1Pk = "",
            l2Pk = "",
            accountId = "",
            apiUrl = "",
            ethAddress = ""
        };
        File.WriteAllText(filename, JsonConvert.SerializeObject(result, Formatting.Indented));
    }
    result = JsonConvert.DeserializeObject<ApiKeys>(File.ReadAllText(filename)) ?? new ApiKeys();

    if (string.IsNullOrWhiteSpace(result.l2Pk))
    {
        Console.WriteLine("WARNING! You need to fill in the details in the appKeys.json file, otherwise this application will not work. FILE IS HERE: " + Directory.GetCurrentDirectory() + "\\"+ filename);
        throw new Exception("WARNING! You need to fill in the details in the appKeys.json file, otherwise this application will not work. FILE IS HERE: " + Directory.GetCurrentDirectory() + "\\"+ filename);
    }
    return result;
}
