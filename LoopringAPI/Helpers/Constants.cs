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

        public static string MetaMaskWebServerUrl = "http://localhost:42069";
        public static string MetaMaskStartTemplate = @"<!DOCTYPE html>
            <html>
            <head>
            <title>Title of the document</title>
            <script src=""https://cdn.jsdelivr.net/npm/web3@latest/dist/web3.min.js""></script>
            </head>

            <body>   
            <h1><span id=""userMesssage"">Waiting for metamask to load and then start authentication process.</span></h1>
            </body>

            </html>
            <script>               
                window.onload = setTimeout(()=> window.location.href = """+MetaMaskWebServerUrl+ @"/l2au.html"" , 5000);
            </script>";
        public static string MetaMaskAuthTemplate = @"<!DOCTYPE html>
            <html>
            <head>
            <title>Title of the document</title>
            <script src=""https://cdn.jsdelivr.net/npm/web3@latest/dist/web3.min.js""></script>
            </head>

            <body>   
            <h1><span id=""userMesssage"">|---------|</span></h1>
            </body>

            </html>
            <script>
                async function signPackage()
                {                    
                    if(typeof ethereum === 'undefined')
                    {
                        document.getElementById('userMesssage').innerHTML = 'Metamask not detected! Please open this link in the browser with metamask installed';     
                    }           
                    else
                    {
                        const accounts = await ethereum.request({ method: 'eth_requestAccounts' });

                        setTimeout(()=> signPackage2(), 1000);
                    }
                }
                
                async function signPackage2()
                {
                    const accounts = await ethereum.request({ method: 'eth_requestAccounts' });

                    var from = accounts[0];

                    var accountInfo = await (await fetch(""||--||""+from)).json();

                    var msgParams =  ""Sign this message to access Loopring Exchange: |-|-|-| with key nonce: "" + (accountInfo.nonce - 1);

                    var params = [from, msgParams];
                    var method = 'personal_sign';

                    ethereum.sendAsync(
                        {
                            method,
                            params,
                            from,
                        },
                        function(err, result) 
                        {
                            if (err) return console.dir(err);
                            if (result.error)
                            {
                                alert(result.error.message);
                            }
                            if (result.error) return console.error('ERROR', result);
                            console.log('TYPED SIGNED:' + JSON.stringify(result.result));
                            fetch(""" + MetaMaskWebServerUrl + @"/api/signatureaddress/""+result.result+""|""+from);
                            document.getElementById('userMesssage').innerHTML = 'Action Completed! You may close this window.';
                        }    
                    );                    
                }

                window.onload = setTimeout(()=> signPackage(), 1000);
            </script>";
        public static string MetaMaskSignatureTemplate = @"<!DOCTYPE html>
            <html>
            <head>
            <title>Title of the document</title>
            <script src=""https://cdn.jsdelivr.net/npm/web3@latest/dist/web3.min.js""></script>
            </head>

            <body>   
            <h1><span id=""userMesssage"">|---------|</span></h1>
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
                        var method = '|-----|';

                        ethereum.sendAsync(
                            {
                                method,
                                params,
                                from,
                            },
                            function(err, result) 
                            {
                                if (err) return console.dir(err);
                                if (result.error)
                                {
                                    alert(result.error.message);
                                }
                                if (result.error) return console.error('ERROR', result);
                                console.log('TYPED SIGNED:' + JSON.stringify(result.result));
                                fetch(""" + MetaMaskWebServerUrl + @"/api/sign/""+result.result);
                                document.getElementById('userMesssage').innerHTML = 'Action Completed! You may close this window.';

                            }    
                        );
                    }
                }
                window.onload = setTimeout(()=> signPackage(), 1000);
            </script>";
    }
}
