
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Files;
using EmbedIO.Routing;
using EmbedIO.Security;
using EmbedIO.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace LoopringAPI.Metamask
{
    public static class MetamaskServer
    {
        private static WebServer server;
        public static string ecdsa;
        public static string ECDSASign(string dataGram)
        {
            string htmlPage = Constants.MetaMaskHTMLTemplate.Replace("||||||","\""+dataGram.Replace("\"","\\\"")+"\"");
            File.WriteAllText(Directory.GetCurrentDirectory() + "/sign.html",htmlPage);

            ecdsa = null;
            string url = "http://localhost:9000";

            if (server == null)
            {
                server = new WebServer(o => o
                    .WithUrlPrefix(url)
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
                    .WithController<PeopleController>())
                .WithStaticFolder("/", Directory.GetCurrentDirectory(), true, m => m
                    .WithContentCaching(true)) // Add static files after other modules to avoid conflicts
                .WithModule(new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { Message = "Error" })));

                server.RunAsync();
            }

            var browser = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo(url+"/sign.html") { UseShellExecute = true }
            };
            browser.Start();

            while (ecdsa == null)
            {
                System.Threading.Thread.Sleep(100);
            }

            server.Dispose();
            server = null;

            return ecdsa.Replace("\"","")+"02";
        }
    }
    // A controller is a class where the WebApi module will find available
    // endpoints. The class must extend WebApiController.
    public sealed class PeopleController : WebApiController
    {
        // Gets a single record.
        // This will respond to 
        //     GET http://localhost:9696/api/people/1
        //     GET http://localhost:9696/api/people/{n}
        //
        // If the given ID is not found, this method will return false.
        // By default, WebApiModule will then respond with "404 Not Found".
        //
        // If the given ID cannot be converted to an integer, an exception will be thrown.
        // By default, WebApiModule will then respond with "500 Internal Server Error".
        [Route(HttpVerbs.Get, "/people/{id?}")]
        public async Task<string> GetPeople(string id)
            => (await DoThing(id).ConfigureAwait(false))
            ?? throw HttpException.NotFound();     

        public async Task<string> DoThing(string id)
        {
            MetamaskServer.ecdsa = id;
            return id;
        }
    }
}
