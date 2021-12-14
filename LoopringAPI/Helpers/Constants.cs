﻿using System.Collections.Generic;

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
    }
}
