using System.Collections.Generic;
using System.Security.Cryptography;

namespace LoopringAPI
{
    public static class Constants
    {
        public static Dictionary<string, int> TokenIDMapper = new Dictionary<string, int>();        

        public static string OffchainFeeUrl = "api/v3/user/offchainFee";
        public static string StorageIdUrl = "api/v3/storageId";
        public static string TickerUrl = "api/v3/ticker";
        public static string TimestampUrl = "api/v3/timestamp";
        public static string TransferUrl = "api/v3/transfer";
        public static string ApiKeyUrl = "api/v3/apiKey";
        public static string ExchangeInfo = "api/v3/exchange/info";
        public static string OrderUrl = "api/v3/order";
        public static string OrdersUrl = "api/v3/orders";
        public static string AccountUrl = "api/v3/account";

        public static string EIP721DomainName = "Loopring Protocol";
        public static string EIP721DomainVersion = "3.6.0";
        public static    int EIP721DomainChainId = 1;

        public static string HttpHeaderAPIKeyName = "X-API-KEY";
        public static string HttpHeaderAPISigName = "X-API-SIG";
    }
}
