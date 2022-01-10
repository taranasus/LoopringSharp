using System;
using System.Collections.Generic;
using System.Text;
using WalletConnectSharp;
using WalletConnectSharp.Core;
using WalletConnectSharp.Core.Utils;
using WalletConnectSharp.Core.Network;
using WalletConnectSharp.Core.Models;
using System.Threading.Tasks;
using System.Threading;
using Nethereum.Web3;
using WalletConnectSharp.Core.Models.Ethereum;

namespace LoopringSharp.WalletConnect
{
    public static class WalletConnectServer
    {
        private static WalletConnectSession walletConnect;
        private static bool isWaitingForConnection = false;

        public static string Connect()
        {
            var metadata = new ClientMeta()
            {
                Description = "This is a test of the Nethereum.WalletConnect feature",
                Icons = new[] { "https://app.warriders.com/favicon.ico" },
                Name = "WalletConnect Test",
                URL = "https://app.warriders.com"
            };
            TransportFactory.Instance.RegisterDefaultTransport((eventDelegator) => new WebsocketTransport(eventDelegator));
            walletConnect = new WalletConnectSession(metadata);
            Task.Run(async () =>
            {
                isWaitingForConnection = true;
                try
                {
                    await walletConnect.Connect();
                }
                catch (Exception ex)
                {
                    walletConnect = null;
                    throw ex;
                }
                isWaitingForConnection = false;
            });
            return walletConnect.URI;
        }

        public static string GetEthAddress()
        {
            if (walletConnect == null)
            {
                throw new Exception("You must first run the Connect() method and give the resulting stirng to the user so they can allow the connection");
            }

            while (isWaitingForConnection)
            {
                Thread.Sleep(100);
            }

            while (!walletConnect.Connected)
            {
                Thread.Sleep(100);
            }

            return walletConnect.Accounts[0];
        }

        public static async Task<(string eddsa, string ethAddress)> L2Authenticate(string exchangeAddress, int nonce)
        {
            if (walletConnect == null)
            {
                throw new Exception("You must first run the Connect() method and give the resulting stirng to the user so they can allow the connection");
            }

            while (isWaitingForConnection)
            {
                Thread.Sleep(100);
            }

            while (!walletConnect.Connected)
            {
                Thread.Sleep(100);
            }

            string hexData = "Sign this message to access Loopring Exchange: " + exchangeAddress + " with key nonce: " + nonce;
            string address = walletConnect.Accounts[0];

            var result = await walletConnect.EthSign(address, hexData);
            return (result, address);
        }
        public static async Task<string> Sign(string serializedData, string signatureMethod, string ethAddress)
        {
            var result = await walletConnect.EthSignTypedDataV4(ethAddress, serializedData);
            return result;
        }
    }
}
