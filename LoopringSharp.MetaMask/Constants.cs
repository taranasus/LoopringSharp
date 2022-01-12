namespace LoopringSharp.MetaMask
{
    public static class Constants
    {
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
    }
}
