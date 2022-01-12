namespace LoopringSharp.MetaMask
{
    public static partial class EDDSAHelper
    {
        public static (string publicKeyX, string publicKeyY, string secretKey, string ethAddress) EDDSASignMetamask(string exchangeAddress, string apiUrl, bool skipPublicKeyCalculation = false, bool nextNonce = false)
        {
            // Requesting metamask to sign our package so we can tare it apart and get our public and secret keys
            var rawKey = MetamaskServer.L2Authenticate("We need you to sign this message in Metamask in order to access your Layer 2 wallet", exchangeAddress, apiUrl, nextNonce);
            return LoopringSharp.EDDSAHelper.RipKeyAppart(rawKey, skipPublicKeyCalculation);
        }

        public static (string secretKey, string ethAddress, string publicKeyX, string publicKeyY) GetL2PKFromMetaMask(string exchangeAddress, string apiUrl)
        {
            var sign = EDDSASignMetamask(exchangeAddress, apiUrl, true);
            // We're only interested in the secret key for signing packages. Which ironically is the simplest one to get...
            return (sign.secretKey, sign.ethAddress, sign.publicKeyX, sign.publicKeyY);
        }
    }
}
