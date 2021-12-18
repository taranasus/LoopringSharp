using System.Collections.Generic;
using System.Security.Cryptography;

namespace LoopringAPI
{
    public static class Constants
    {
        public static Dictionary<string, int> TokenIDMapper = new Dictionary<string, int>();        

        public static string OffchainFeeUrl = "api/v3/user/offchainFee";
        public static string StorageIdUrl = "api/v3/storageId";
        public static string TickerUrl = "api/v3/ticker";
        public static string TimestampUrl = "api/v3/timestamp";
        public static string TransferUrl = "api/v3/transfer";
        public static string ApiKeyUrl = "api/v3/apiKey";
        
        public static string OrderUrl = "api/v3/order";
        public static string OrdersUrl = "api/v3/orders";
        public static string AccountUrl = "api/v3/account";
        public static string MarketsUrl = "api/v3/exchange/markets";
        public static string TokensUrl = "api/v3/exchange/tokens";
        public static string InfoUrl = "api/v3/exchange/info";
        public static string DepthUrl = "api/v3/depth";
        public static string CandlestickUrl = "api/v3/candlestick";
        public static string PriceUrl = "api/v3/price";

        public static string EIP721DomainName = "Loopring Protocol";
        public static string EIP721DomainVersion = "3.6.0";

        public static string HttpHeaderAPIKeyName = "X-API-KEY";
        public static string HttpHeaderAPISigName = "X-API-SIG";


        public static string MetaMaskHTMLTemplate = @"<!DOCTYPE html>
<html>
<head>
<title>Title of the document</title>
<script src=""https://cdn.jsdelivr.net/npm/web3@latest/dist/web3.min.js""></script>
</head>

<body>   
<h1><span id=""userMesssage"">Signing your request using metamask</span></h1>
</body>

</html>
<script>

async function signPackage()
        {
            const msgParams = ||||||;
            if(typeof ethereum === 'undefined')
{
           document.getElementById('userMesssage').innerHTML = 'Metamask not detected! Please open this link in the browser with metamask installed';     
}           
else
{
            const accounts = await ethereum.request({ method: 'eth_requestAccounts' });

            var from = accounts[0];

            var params = [from, msgParams];
            var method = 'eth_signTypedData_v4';

            ethereum.sendAsync(
    {
                method,
      params,
      from,
    },
    function(err, result) {
                if (err) return console.dir(err);
                if (result.error)
                {
                    alert(result.error.message);
                }
                if (result.error) return console.error('ERROR', result);
                console.log('TYPED SIGNED:' + JSON.stringify(result.result));
                fetch(""http://localhost:9000/api/people/""+JSON.stringify(result.result));
                document.getElementById('userMesssage').innerHTML = 'Signing complete, you may close this window!';

            }

  );
}

        }
        window.onload = signPackage;
</script>";
    }
}
