
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Files;
using EmbedIO.Security;
using EmbedIO.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Text;


namespace LoopringSharp.Metamask
{
    public static class MetamaskServer
    {
        private static WebServer server;
        public static string eddsa;
        public static string ethAddress;        

        public static (string eddsa,string ethAddress) L2Authenticate(string userMessage, string exchangeAddress, string apiUrl, bool nextNonce)
        {
            string nonceModifier = "- 1";
            if (nextNonce)
                nonceModifier = "+ 0";
          
            File.WriteAllText(Directory.GetCurrentDirectory() + "/l2au.html", 
                Constants.MetaMaskAuthTemplate.Replace("||--||", apiUrl + Constants.AccountUrl + "?owner=")
                                              .Replace("|-|-|-|", exchangeAddress)
                                              .Replace("|---------|", userMessage)
                                              .Replace("|--|--|--|", nonceModifier));

            eddsa = null;
            ethAddress = null;

            ServerInitiator("/l2au.html");

            // Wait for authentication to finish
            while (eddsa == null)
            {
                System.Threading.Thread.Sleep(100);
            }

            CloseServer();
            
            File.Delete(Directory.GetCurrentDirectory() + "/l2au.html");

            return (eddsa.Replace("\"", ""),ethAddress);
        }

        public static string Sign(string dataGram, string signatureMethod, string userMessage)
        {
            string htmlPage = Constants.MetaMaskSignatureTemplate.Replace("||||||","\""+dataGram.Replace("\"","\\\"")+"\"");
            htmlPage = htmlPage.Replace("|-----|", signatureMethod);
            htmlPage = htmlPage.Replace("|---------|", userMessage);
            File.WriteAllText(Directory.GetCurrentDirectory() + "/sign.html",htmlPage);

            eddsa = null;

            ServerInitiator("/sign.html");

            // Wait for authentication to finish
            while (eddsa == null)
            {
                System.Threading.Thread.Sleep(100);
            }

            CloseServer();

            File.Delete(Directory.GetCurrentDirectory() + "/sign.html");

            if (signatureMethod == "eth_signTypedData_v4")
                return eddsa.Replace("\"","")+"02";
            else
                return eddsa.Replace("\"", "");
        }

        private static void ServerInitiator(string page)
        {
            if (server == null)
            {
                server = new WebServer(o => o
                    .WithUrlPrefix(Constants.MetaMaskWebServerUrl)
                    .WithMode(HttpListenerMode.EmbedIO))
                .WithIPBanning(o => o
                    .WithMaxRequestsPerSecond()
                    .WithRegexRules("HTTP exception 404"))
                .WithLocalSessionManager()
                .WithCors(
                    // Origins, separated by comma without last slash
                    "http://unosquare.github.io,http://run.plnkr.co",
                    // Allowed headers
                    "content-type, accept",
                    // Allowed methods
                    "post")
                .WithWebApi("/api", m => m
                    .WithController<MetamaskApiController>())
                .WithStaticFolder("/", Directory.GetCurrentDirectory(), true, m => m
                    .WithContentCaching(true)) // Add static files after other modules to avoid conflicts
                .WithModule(new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { Message = "Error" })));

                server.RunAsync();
            }

            var browser = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo(Constants.MetaMaskWebServerUrl + page) { UseShellExecute = true }
            };
            browser.Start();
        }

        private static void CloseServer()
        {
            server.Dispose();
            server = null;
        }
    }
}
