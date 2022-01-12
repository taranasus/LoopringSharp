using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using System.Threading.Tasks;

namespace LoopringSharp.MetaMask
{
    public sealed class MetamaskApiController : WebApiController
    {
        [Route(HttpVerbs.Get, "/sign/{id?}")]
        public async Task<string> GetSign(string id)
            => (await Sign(id).ConfigureAwait(false))
            ?? throw HttpException.NotFound();

        public async Task<string> Sign(string id)
        {
            MetamaskServer.eddsa = id;
            return id;
        }

        [Route(HttpVerbs.Get, "/signatureaddress/{id?}")]
        public async Task<string> GetSignatureAddress(string id)
            => (await SignatureAddress(id).ConfigureAwait(false))
            ?? throw HttpException.NotFound();

        public async Task<string> SignatureAddress(string address)
        {
            MetamaskServer.eddsa = address.Split('|')[0];
            MetamaskServer.ethAddress = address.Split('|')[1];
            return address;
        }
    }
}
