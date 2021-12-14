using LoopringAPI.TestConsole;
using Newtonsoft.Json;

Console.WriteLine("Hello, Loops!");

ApiKeys apiKeys = ReadConfigFile();

// If this is confusing, check the read.me file
string apiKey = apiKeys.apiKey;         // Loopring API Key
string l2Pk = apiKeys.l2Pk;             // Loopring Private Key (Layer 2 Private Key)
string l1Pk = apiKeys.l1Pk;             // Ethereum Private Key (Layer 1 Private Key)
string accountId = apiKeys.accountId;   // The user's accountId

LoopringAPI.Client client = new LoopringAPI.Client(apiKey, l2Pk, l1Pk, accountId, apiKeys.useTestNet);

#region Ticker
Console.WriteLine("Testing TICKER: ");
var tickers = await client.Ticker("LRC-ETH");
foreach(var ticker in tickers)
{
    Console.WriteLine(ticker.PairId + " - ASK: " + ticker.LowestAskPrice);
}
Console.WriteLine("");
#endregion

#region Timestamp
var timestamp = await client.Timestamp();
Console.WriteLine("Testing timestamp: "+ timestamp);
Console.WriteLine("");
#endregion

#region StorageId
Console.WriteLine("Testing StorageId");

var storageId = await client.StorageId(1);
Console.WriteLine("Normal: "+JsonConvert.SerializeObject(storageId));

storageId = await client.StorageId(1, 1);
Console.WriteLine("MaxNext: " + JsonConvert.SerializeObject(storageId));
#endregion

#region StorageId
Console.WriteLine("Testing APIKEYGET");

var apikey = await client.ApiKey();
Console.WriteLine("Key: " + apikey);

Console.WriteLine();
#endregion

#region OffChainFee
Console.WriteLine("Testing OffChainFee - Transfer");
var fee = await client.OffchainFee(LoopringAPI.OffChainRequestType.Transfer,null,null);
Console.WriteLine("Fee: " + JsonConvert.SerializeObject(fee));
Console.WriteLine("Testing OffChainFee - OffchainWithdrawl");
fee = await client.OffchainFee(LoopringAPI.OffChainRequestType.OffchainWithdrawl, "LRC", "10000000000");
Console.WriteLine("Fee: " + JsonConvert.SerializeObject(fee));

#endregion

Console.ReadLine();

static ApiKeys ReadConfigFile()
{
    ApiKeys result;
    if (!File.Exists("apiKeys.json"))
    {
        result = new ApiKeys()
        {
            apiKey = "",
            l1Pk = "",
            l2Pk = "",
            useTestNet = false,
        };
        File.WriteAllText("apiKeys.json", JsonConvert.SerializeObject(result, Formatting.Indented));
    }
    result = JsonConvert.DeserializeObject<ApiKeys>(File.ReadAllText("apiKeys.json")) ?? new ApiKeys();

    if (string.IsNullOrWhiteSpace(result.apiKey))
    {        
        Console.WriteLine("WARNING! You need to fill in the details in the appKeys.json file, otherwise this application will not work. FILE IS HERE: "+ Directory.GetCurrentDirectory()+"\\apiKeys.json");
        throw new Exception("WARNING! You need to fill in the details in the appKeys.json file, otherwise this application will not work. FILE IS HERE: " + Directory.GetCurrentDirectory() + "\\apiKeys.json");        
    }
    return result;
}

