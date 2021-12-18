
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Labs.EmbedIO.Modules;

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
            string url = "http://localhost:9000/";

            if (server == null)
            {
                server = new WebServer(url);
                server.RegisterModule(new StaticFilesModule(Directory.GetCurrentDirectory()));
                // The static files module will cache small files in ram until it detects they have been modified.
                server.Module<StaticFilesModule>().UseRamCache = true;
                server.RegisterModule(new WebApiModule());

                server.Module<WebApiModule>().RegisterController<PeopleController>();
                server.RunAsync();
            }

            var browser = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo(url+"sign.html") { UseShellExecute = true }
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
    public class PeopleController : WebApiController
    {
        // You need to add a default constructor where the first argument
        // is an IHttpContext
        public PeopleController(IHttpContext context)
            : base(context)
        {
        }

        // You need to include the WebApiHandler attribute to each method
        // where you want to export an endpoint. The method should return
        // bool or Task<bool>.
        [WebApiHandler(HttpVerbs.Get, "/api/people/{id}")]
        public async Task<bool> GetPersonById(string id)
        {
            try
            {
                MetamaskServer.ecdsa = id;
                return await Ok("RECIEVED!");
            }
            catch (Exception ex)
            {
                return await InternalServerError(ex);
            }
        }

        // You can override the default headers and add custom headers to each API Response.
        public override void SetDefaultHeaders() => HttpContext.NoCache();
    }

}
