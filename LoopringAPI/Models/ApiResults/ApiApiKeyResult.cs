using System.Collections.Generic;

namespace LoopringAPI
{
    public class ApiApiKeyResult
    {
        public string apiKey { get; set; }
    }

    public class ApiPriceResult
    {
        public List<Price> prices { get; set; } 
    }
}
