using System.Collections.Generic;

namespace LoopringSharp
{
    public class ApiTransfersGetResult
    {
        public long totalNum { get; set; }
        public List<ApiTransferData> transactions { get; set; }
    }
}
