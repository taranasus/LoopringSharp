using System.Collections.Generic;

namespace LoopringSharp
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
