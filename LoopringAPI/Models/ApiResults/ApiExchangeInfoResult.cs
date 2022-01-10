using System.Collections.Generic;

namespace LoopringSharp
{
    public class ApiExchangeInfoResult
    {
        public int chainId { get; set; }
        public string exchangeAddress { get; set; }
        public string depositAddress { get; set; }
        public List<FeeInfo> onchainFees { get; set; }
        public List<OffFeeInfo> openAccountFees { get; set; }
        public List<OffFeeInfo> updateFees { get; set; }
        public List<OffFeeInfo> transferFees { get; set; }
        public List<OffFeeInfo> withdrawalFees { get; set; }
        public List<OffFeeInfo> fastWithdrawalFees { get; set; }
        public List<OffFeeInfo> ammExitFees { get; set; }

    }
}
