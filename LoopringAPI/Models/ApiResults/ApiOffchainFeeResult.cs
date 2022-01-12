using System.Collections.Generic;

namespace LoopringSharp
{
    public class ApiOffchainFeeResult
    {
        public string gasPrice { get; set; }
        public List<Fee> fees { get; set; }
    }

}
