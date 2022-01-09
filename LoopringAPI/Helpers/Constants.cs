using System.Collections.Generic;
using System.Security.Cryptography;

namespace LoopringAPI
{
    public static class Constants
    {
        public static Dictionary<string, int> TokenIDMapper = new Dictionary<string, int>();        

        public static string OffchainFeeUrl = "api/v3/user/offchainFee";
        public static string CreateInfoUrl = "api/v3/user/createInfo";
        public static string UpdateInfoUrl = "api/v3/user/updateInfo";
        public static string BalancesUrl = "api/v3/user/balances";
        public static string DepositsUrl = "/api/v3/user/deposits";
        public static string WithdrawlsUrl = "/api/v3/user/withdrawals";
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
        public static string DepthMixUrl = "api/v3/mix/depth";
        public static string CandlestickUrl = "api/v3/candlestick";
        public static string PriceUrl = "api/v3/price";
        public static string TradeUrl = "api/v3/trade";

        public static string L2BlockInfoUrl = "api/v3/block/getBlock";
        public static string PendingRequestsUrl = "api/v3/block/getPendingRequests";

        public static string EIP721DomainName = "Loopring Protocol";
        public static string EIP721DomainVersion = "3.6.0";

        public static string HttpHeaderAPIKeyName = "X-API-KEY";
        public static string HttpHeaderAPISigName = "X-API-SIG";

        public static string MetaMaskWebServerUrl = "http://localhost:42069";      
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
                    else if(!ethereum.isConnected())
                    {
                        document.getElementById('userMesssage').innerHTML = 'Waiting for MetaMask to become availalble';
                        setTimeout(()=>window.location.href = """ + MetaMaskWebServerUrl + @"/l2au.html"", 1000);
                    }
                    else
                    {
                        const accounts = await ethereum.request({ method: 'eth_requestAccounts' });
                        setTimeout(()=> signPackage2(), 100);
                    }
                }
                
                async function signPackage2()
                {
                    const accounts = await ethereum.request({ method: 'eth_requestAccounts' });

                    var from = accounts[0];

                    var accountInfo = await (await fetch(""||--||""+from)).json();

                    var msgParams =  ""Sign this message to access Loopring Exchange: |-|-|-| with key nonce: "" + (accountInfo.nonce |--|--|--|);

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
                            setTimeout(()=>windowClose(),1000);
                        }    
                    );                    
                }

                function windowClose() {
                    window.open('','_parent','');
                    window.close();
                }

                window.onload = setTimeout(()=> signPackage(), 100);
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
                    else if(!ethereum.isConnected())
                    {   
                        document.getElementById('userMesssage').innerHTML = 'Waiting for MetaMask to become availalble';
                        setTimeout(()=>window.location.href = """ + MetaMaskWebServerUrl + @"/sign.html"", 1000);
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
                                setTimeout(()=>windowClose(),1000);
                            }    
                        );
                    }
                }
                function windowClose() {
                    window.open('','_parent','');
                    window.close();
                }

                window.onload = setTimeout(()=> signPackage(), 100);
            </script>";

        public static string WalletConnectHTML = @"<!DOCTYPE html>
            <html>
            <head>
            <title>WalletConnect Details</title>            
            </head>

            <body>   
            <h1>For wallet connect, scan this CR code or paste the bellow code into your wallet</h1>
            <img src=""|----|""/></br>
            <p>|--|--|</p>
            </body>

            </html>";
    }
}
