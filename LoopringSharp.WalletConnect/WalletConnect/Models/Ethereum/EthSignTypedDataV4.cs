using Newtonsoft.Json;

namespace WalletConnectSharp.Core.Models.Ethereum
{
    public sealed class EthSignTypedDataV4 : JsonRpcRequest
    {
        [JsonProperty("params")]
        private string[] _parameters;

        public EthSignTypedDataV4(string address, string data)
        {
            this.Method = "eth_signTypedData_v4";
            this._parameters = new string[] { address, data };
        }
    }
}