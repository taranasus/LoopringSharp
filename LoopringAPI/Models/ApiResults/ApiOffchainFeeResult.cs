using System.Collections.Generic;

namespace LoopringAPI
{
    public class Fee
    {
        public string token { get; set; }
        public string fee { get; set; }
        public double discount { get; set; }
    }

    public class ApiOffchainFeeResult
    {
        public string gasPrice { get; set; }
        public List<Fee> fees { get; set; }
    }

}
