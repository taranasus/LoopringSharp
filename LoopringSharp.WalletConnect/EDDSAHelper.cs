using System.Threading.Tasks;

namespace LoopringSharp.WalletConnect
{
    public static partial class EDDSAHelper
    {
        public static async Task<(string secretKey, string ethAddress, string publicKeyX, string publicKeyY)> GetL2PKFromWalletConnect(string exchangeAddress, int nonce)
        {
            var sign = await EDDSASignWalletConnect(exchangeAddress, nonce);
            // We're only interested in the secret key for signing packages. Which ironically is the simplest one to get...
            return (sign.secretKey, sign.ethAddress, sign.publicKeyX, sign.publicKeyY);
        }

        public static async Task<(string publicKeyX, string publicKeyY, string secretKey, string ethAddress)> EDDSASignWalletConnect(string exchangeAddress, int nextNonce, bool skipPublicKeyCalculation = false)
        {
            // Requesting metamask to sign our package so we can tare it apart and get our public and secret keys
            var rawKey = await WalletConnectServer.L2Authenticate(exchangeAddress, nextNonce);
            return LoopringSharp.EDDSAHelper.RipKeyAppart(rawKey, skipPublicKeyCalculation);
        }
    }
}
