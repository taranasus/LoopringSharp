Console.WriteLine("Hello, Loops!");

// If this is confusing, check the read.me file
string apiKey = null;   // Loopring API Key
string l2Pk = null;     // Loopring Private Key (Layer 2 Private Key)
string l1Pk = null;     // Ethereum Private Key (Layer 1 Private Key)

LoopringAPI.Client client = new LoopringAPI.Client(apiKey, l2Pk, l1Pk);

var tickers = await client.Ticker("LRC-USDT", "LRC-ETH");
foreach(var ticker in tickers)
{
    Console.WriteLine(ticker.PairId + " - ASK: " + ticker.LowestAskPrice);
}

Console.ReadLine();