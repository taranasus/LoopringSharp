using LoopringSharp;
using LoopringSharp.TestConsole;
using Newtonsoft.Json;
using PoseidonSharp;
using System.Net;
using System.Net.Security;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

Console.WriteLine("Hello, Loops!");

ApiKeys apiKeys = ReadConfigFile(false);

//var keys = EDDSAHelper.EDDSASignMetamask("0x2e76ebd1c7c0c8e7c2b875b6d505a260c525d25e", "https://uat2.loopring.io/");
//var keys2 = EDDSAHelper.EDDSASignLocal("0x2e76ebd1c7c0c8e7c2b875b6d505a260c525d25e",2, "0x5ce27884b99146b4d67a3d3c5ea9566401bdc11f1f561b54d62c0e4a516d7aa0", "0x452386e0516cc1600e9f43c719d0c80c6abc51f9", false);

//Environment.Exit(0);

LoopringSharp.Client client = new LoopringSharp.Client(apiKeys.apiUrl, apiKeys.l1Pk);
//LoopringSharp.Client client = new LoopringSharp.Client("https://uat2.loopring.io/", WalletService.WalletConnect);

Console.WriteLine("PRINT BALLANCES!");
var balanceResult = client.Ballances();
Console.WriteLine(JsonConvert.SerializeObject(balanceResult, Formatting.Indented));

Console.WriteLine();
Console.WriteLine();

#region UpdateAccountPrivateKey
Console.WriteLine("Updating your account private key. DO YOU WISH TO CONTINUE? [Y]ONTINUE!!!!!! / [S]kip");
var choice = Console.ReadLine();
if (choice.ToLower().StartsWith("y"))
{
    Console.WriteLine("BEGINNING UPDATE!");
    var transferResult = client.RequestNewL2PrivateKey("ETH");
    Console.WriteLine("Update COMPLETE:");
    Console.WriteLine(JsonConvert.SerializeObject(transferResult, Formatting.Indented));
}
else
{
    Console.WriteLine("Skipping Transfer test as it costs MONEY!");
}

Console.WriteLine();
#endregion

#region TestGetL2BlockInfo
Console.WriteLine("Do you want to see the current L2 Block Info? CONTINUE? [Y]ONTINUE!!!!!! / [S]kip");
choice = Console.ReadLine();
if (choice.ToLower().StartsWith("y"))
{
    Console.WriteLine("GETTING L2 BLOCK INFO!");
    var l2BlockInfoResult = client.Get2BlockInfo(15623);
    Console.WriteLine("L2 BLOCK INFO RETRIEVED!");
    Console.WriteLine(JsonConvert.SerializeObject(l2BlockInfoResult, Formatting.Indented, 
    new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore
    }));
}
else
{
    Console.WriteLine("Skipping retrieving L2 Block Info test!");
}
#endregion

#region TestGetPendingRequests
Console.WriteLine("Do you want to see pending requests for the next block? CONTINUE? [Y]ONTINUE!!!!!! / [S]kip");
choice = Console.ReadLine();
if (choice.ToLower().StartsWith("y"))
{
    Console.WriteLine("GETTING PENDING REQUESTS");
    var pendingRequestsResult = client.GetPendingRequests();
    Console.WriteLine("PENDING REQUESTS RETRIEVED!");
    Console.WriteLine(JsonConvert.SerializeObject(pendingRequestsResult, Formatting.Indented,
    new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore
    }));
}
else
{
    Console.WriteLine("Skipping retrieving pending requests test!");
}
#endregion

#region TestGetAmmPoolTrades
Console.WriteLine("Do you want to see AMM pool trades for 0x194db39e4c99f6c8dd81b4647465f7599f3c215a? CONTINUE? [Y]ONTINUE!!!!!! / [S]kip");
choice = Console.ReadLine();
if (choice.ToLower().StartsWith("y"))
{
    Console.WriteLine("GETTING AMM POOL TRADES");
    var ammPoolTradesResult = client.GetAmmPoolTrades("0x194db39e4c99f6c8dd81b4647465f7599f3c215a", 50, 0);
    Console.WriteLine("AMM POOL TRADES RETRIEVED!");
    Console.WriteLine(JsonConvert.SerializeObject(ammPoolTradesResult, Formatting.Indented));
}
else
{
    Console.WriteLine("Skipping retrieving AMM pool trades!");
}
#endregion

#region TestGetAmmJoinExitTransactions
Console.WriteLine("Do you want to see AMM pool join and exits for account id, 10083(*this only returns data in uat)? CONTINUE? [Y]ONTINUE!!!!!! / [S]kip");
choice = Console.ReadLine();
if (choice.ToLower().StartsWith("y"))
{
    Console.WriteLine("GETTING AMM POOL JOIN AND EXITS");
    var ammJoinExitResult = client.GetAmmJoinExitTransactions(10083);
    Console.WriteLine("AMM POOL JOIN AND EXITS RETRIEVED");
    Console.WriteLine(JsonConvert.SerializeObject(ammJoinExitResult, Formatting.Indented));
}
else
{
    Console.WriteLine("Skipping retrieving AMM pool join and exits!");
}
#endregion


#region TestGetUserTradesHistory
Console.WriteLine("Do you want to see trade history for account id, 12383(*this only returns data in production)? CONTINUE? [Y]ONTINUE!!!!!! / [S]kip");
choice = Console.ReadLine();
if (choice.ToLower().StartsWith("y"))
{
    Console.WriteLine("GETTING TRADE HISTORY");
    var tradeHistory = client.GetTradeHistory(12383);
    Console.WriteLine("TRADE HISTORY RETRIEVED");
    Console.WriteLine(JsonConvert.SerializeObject(tradeHistory, Formatting.Indented));
}
else
{
    Console.WriteLine("Skipping retrieving trade history!");
}
#endregion



#region TestTransfer
Console.WriteLine("Let's start with a TRANSFER TEST of 1 LRC. DO YOU WISH TO CONTINUE? [Y]ONTINUE!!!!!! / [S]kip");
choice = Console.ReadLine();
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
    var transferResult = client.Transfer(transfertoAddress, "LRC", 1m, "LRC", "aaaa");
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
var exchangeInfo = client.ExchangeInfo();
Console.WriteLine(JsonConvert.SerializeObject(exchangeInfo, Formatting.Indented));
Console.WriteLine("");
#endregion

#region Ticker
Console.WriteLine("Testing TICKER: ");
var tickers = client.Ticker("LRC-ETH");
foreach (var ticker in tickers)
{
    Console.WriteLine(ticker.PairId + " - ASK: " + ticker.LowestAskPrice);
}
Console.WriteLine("");
#endregion

#region Timestamp
var timestamp = client.Timestamp();
Console.WriteLine("Testing timestamp: " + timestamp);
Console.WriteLine("");
#endregion

#region StorageId
Console.WriteLine("Testing StorageId");

var storageId = client.StorageId(1);
Console.WriteLine("Normal: " + JsonConvert.SerializeObject(storageId));

storageId = client.StorageId(1, 1);
Console.WriteLine("MaxNext: " + JsonConvert.SerializeObject(storageId));
#endregion

#region GetApiKey
Console.WriteLine("Testing APIKEY GET");

var apikey = client.ApiKey();
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
    apikey = client.UpdateApiKey();
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
var fee = client.OffchainFee(LoopringSharp.OffChainRequestType.Transfer, null, null);
Console.WriteLine("Fee: " + JsonConvert.SerializeObject(fee, Formatting.Indented));
Console.WriteLine("Testing OffChainFee - OffchainWithdrawl");
fee = client.OffchainFee(LoopringSharp.OffChainRequestType.OffchainWithdrawl, "LRC", "10000000000");
Console.WriteLine("Fee: " + JsonConvert.SerializeObject(fee, Formatting.Indented));
Console.WriteLine("");

#endregion

Console.WriteLine("REVIEW RESULTS AND PRESS ENTER TO CONTINUE!");
Console.ReadLine();
Console.Clear();

#region Orders

Console.WriteLine("-------- TESTING ORDERS ---------");
Console.WriteLine("Testing order submit! 0.3 ETH -> 1000 LRC");
var tradeResult = client.SubmitOrder(
        sellToken: new LoopringSharp.Token() { tokenId = 0, /*ETH*/ volume = "30000000000000000" /* 0.03 ETH */  },
        buyToken: new LoopringSharp.Token() { tokenId = 1, /*LRC*/ volume = "1000000000000000000000" /* 1000 LRC */ },
        allOrNone: false,
        fillAmountBOrS: false,
        validUntil: 1700000000, // Will expire eventually...
        maxFeeBips: 63,
        clientOrderId: null,
        orderType: LoopringSharp.OrderType.TAKER_ONLY,
        tradeChannel: LoopringSharp.TradeChannel.MIXED
    );

Console.WriteLine("Testing simple order submit! 0.04 ETH -> 150 LRC");
var simpleTradeResult = client.SubmitOrder(
        sellCurrency: "ETH",
        sellAmmount: 0.04m,
        buyCurrency: "LRC",
        buyAmmount: 150,
        orderType: LoopringSharp.OrderType.MAKER_ONLY
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
var normalTradeDetails = client.OrderDetails(tradeResult.hash);
Console.WriteLine(JsonConvert.SerializeObject(normalTradeDetails, Formatting.Indented));
Console.WriteLine("Simple Order:");
var simpleTradeDetails = client.OrderDetails(simpleTradeResult.hash);
Console.WriteLine(JsonConvert.SerializeObject(simpleTradeDetails, Formatting.Indented));

Console.WriteLine("REVIEW RESULTS AND PRESS ENTER TO CONTINUE!");
Console.ReadLine();
Console.Clear();

Console.WriteLine("Cancel both trades if they are still active: ");
if (normalTradeDetails.status == LoopringSharp.OrderStatus.processing)
{
    var normalDeleteResult = client.CancelOrder(normalTradeDetails.hash, simpleTradeDetails.clientOrderId);
    Console.WriteLine("CANCELED normal trade");
    Console.WriteLine(JsonConvert.SerializeObject(normalDeleteResult, Formatting.Indented));
}
else
    Console.WriteLine("Normal trade no longer active anyway...");

if (simpleTradeDetails.status == LoopringSharp.OrderStatus.processing)
{
    var simpleDeleteResult = client.CancelOrder(simpleTradeDetails.hash, simpleTradeDetails.clientOrderId);
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

    var results = client.OrdersDetails(5);
    Console.WriteLine("You asked for it: ");
    Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
}

#endregion

Console.WriteLine("REVIEW RESULTS AND PRESS ENTER TO CONTINUE!");
Console.ReadLine();
Console.Clear();

#region market

Console.WriteLine("Wanna see the order book for ETH-USDT [Y]es / [S]kip: ");
choice = Console.ReadLine();
if (choice.ToLower().StartsWith("y"))
{
    Console.Clear();

    var results = client.GetDepth("ETH-USDT");
    Console.WriteLine("You asked for it: ");
    Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
}

Console.WriteLine("REVIEW RESULTS AND PRESS ENTER TO CONTINUE!");
Console.ReadLine();
Console.Clear();

Console.WriteLine("Wanna see some candlesticks? Yeah? [Y]es / [S]kip: ");
choice = Console.ReadLine();
if (choice.ToLower().StartsWith("y"))
{
    Console.Clear();

    var results = client.GetCandlesticks("ETH-USDT", Intervals.d1);
    Console.WriteLine("You asked for it: ");
    Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
}

Console.WriteLine("REVIEW RESULTS AND PRESS ENTER TO CONTINUE!");
Console.ReadLine();
Console.Clear();

Console.WriteLine("Wanna see some token prices? Yeah? [Y]es / [S]kip: ");
choice = Console.ReadLine();
if (choice.ToLower().StartsWith("y"))
{
    Console.Clear();

    var results = client.GetPrice(LegalCurrencies.GBP);
    Console.WriteLine("You asked for it: ");
    Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
}

#endregion

Console.ReadLine();

static ApiKeys ReadConfigFile(bool prod)
{
    ApiKeys result;
    string filename = "apiKeys.json";
    if (prod)
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
        Console.WriteLine("WARNING! You need to fill in the details in the appKeys.json file, otherwise this application will not work. FILE IS HERE: " + Directory.GetCurrentDirectory() + "\\" + filename);
        throw new Exception("WARNING! You need to fill in the details in the appKeys.json file, otherwise this application will not work. FILE IS HERE: " + Directory.GetCurrentDirectory() + "\\" + filename);
    }
    return result;
}
