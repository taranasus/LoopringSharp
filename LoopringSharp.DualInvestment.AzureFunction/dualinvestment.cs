using System;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Nethereum.JsonRpc.Client;
using Nethereum.Contracts.QueryHandlers.MultiCall;

namespace LoopringSharp.DualInvestment.AzureFunction
{
    public class DualInvestment
    {
        private readonly ILogger _logger;
        private readonly LoopringSharp.Client client;

        public DualInvestment(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DualInvestment>();
            ApiKeys? apiKeys = JsonConvert.DeserializeObject<ApiKeys>(AccountDetails.apiKeysJson);
            client = new LoopringSharp.Client(apiKeys?.apiUrl, apiKeys?.l1Pk);
        }

        [Function("dualinvestment")]
        //public void Run([TimerTrigger("0/5 * * * * *")] MyInfo myTimer) // Every 5 seconds
        public void Run([TimerTrigger("0 0 0 1 1 *", RunOnStartup = true)] MyInfo myTimer) // Once on start
        {
            // YOU NEED TO START AZURITE FOR THIS TO WORK ON A MAC
            // OPEN A TERMINAL AND TYPE azurite

            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            _logger.LogInformation($"Next timer schedule at: {myTimer?.ScheduleStatus?.Next}");

            var tokensToInvest = GetTokensOver300USD();

            Dictionary<string, List<(string, string, string)>> dualInvestmentOptions = new Dictionary<string, List<(string, string, string)>>();
            dualInvestmentOptions.Add("ETH", new List<(string, string, string)>()
            {
                ("ETH", "USDT", "DUAL_BASE"),
                ("ETH", "USDC", "DUAL_BASE")
            });
            dualInvestmentOptions.Add("LRC", new List<(string, string, string)>()
            {
                ("LRC", "USDT", "DUAL_BASE"),
                ("LRC", "USDC", "DUAL_BASE")
            });
            dualInvestmentOptions.Add("WBTC", new List<(string, string, string)>()
            {
                ("WBTC", "USDT", "DUAL_BASE"),
                ("WBTC", "USDC", "DUAL_BASE")
            });
            dualInvestmentOptions.Add("USDT", new List<(string, string, string)>()
            {
                ("ETH", "USDT", "DUAL_CURRENCY"),
                ("LRC", "USDT", "DUAL_CURRENCY"),
                ("WBTC", "USDT", "DUAL_CURRENCY")
            });
            dualInvestmentOptions.Add("USDC", new List<(string, string, string)>()
            {
                ("ETH", "USDC", "DUAL_CURRENCY"),
                ("LRC", "USDC", "DUAL_CURRENCY"),
                ("WBTC", "USDC", "DUAL_CURRENCY")
            });

            var currentInvestmentOportunities = new List<(string, string, string)>();
            foreach (var token in tokensToInvest)
            {
                currentInvestmentOportunities.AddRange(dualInvestmentOptions[token]);
            }
            currentInvestmentOportunities = currentInvestmentOportunities.Distinct(new TupleComparer()).ToList();

            dynamic maxResult;
            long maxTimeHours;
            dynamic maxProfit = -100000;

            foreach (var investmetnOportunity in currentInvestmentOportunities)
            {
                var currentDateTime = client.Timestamp();
                dynamic results = client.GetDualInvestmetns(investmetnOportunity.Item1, investmetnOportunity.Item2, investmetnOportunity.Item3);

                foreach (var result in results.infos)
                {
                    long totalTimeHours = ((result.expireTime.ToObject<long>() - currentDateTime) / 1000) / 60 / 60;

                    if (maxProfit < decimal.Parse(result.profit.ToString()) / totalTimeHours)
                    {
                        maxResult = result;
                        maxTimeHours = totalTimeHours;
                        maxProfit = decimal.Parse(result.profit.ToString()) / totalTimeHours;
                    }
                }
            }

        }

        public List<string> GetTokensOver300USD()
        {
            List<string> result = new List<string>();

            var moneyWeHave = client.Ballances();
            var prices = client.GetPrice(LegalCurrencies.USD);

            foreach (var moneys in moneyWeHave)
            {
                var usdValue = moneys.total * decimal.Parse(prices.Where(w => w.symbol == moneys.token).First().price);
                if (usdValue >= 300)
                    result.Add(moneys.token);
            }
            return result;
        }
    }

    public class TupleComparer : IEqualityComparer<(string, string, string)>
    {
        public bool Equals((string, string, string) x, (string, string, string) y)
        {
            return x.Item1 == y.Item1 && x.Item2 == y.Item2 && x.Item3 == y.Item3;
        }

        public int GetHashCode((string, string, string) obj)
        {
            return (obj.Item1, obj.Item2, obj.Item3).GetHashCode();
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus? ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }

    public class ApiKeys
    {
        public string? l1Pk { get; set; }
        public string? l2Pk { get; set; }
        public string? accountId { get; set; }
        public string? ethAddress { get; set; }
        public string? apiUrl { get; set; }
    }
}

