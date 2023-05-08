using System;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Nethereum.JsonRpc.Client;
using Nethereum.Contracts.QueryHandlers.MultiCall;
using System.Diagnostics;

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
#if DEBUG
        public void Run([TimerTrigger("0 0 0 1 1 *", RunOnStartup = true)] MyInfo myTimer)
#else
        public void Run([TimerTrigger("*/15 * * * * *")] MyInfo myTimer) // Every 15 seconds
#endif
        //public void Run([TimerTrigger("0 0 0 1 1 *", RunOnStartup = true)] MyInfo myTimer) // Once on start
        {
            // YOU NEED TO START AZURITE FOR THIS TO WORK ON A MAC
            // OPEN A TERMINAL AND TYPE azurite
            var tokensToInvest = GetTokensOver300USD();
            var walletValue = GetWalletDollarValue();

            _logger.LogInformation($"[DUAL INVESTMENT] [PING] Total: {walletValue.total} ||| Locked: {walletValue.locked}");

#if DEBUG
            //if (Debugger.IsAttached)
            //    tokensToInvest = new List<string>() { "USDT" };
#endif

            if (tokensToInvest == null || tokensToInvest.Count == 0)
            {
                return;
            }


            _logger.LogInformation($"[DUAL INVESTMENT] [FUNDS ACCESSIBLE] Can invest in: {string.Join(" | ", tokensToInvest)}");

            Dictionary<string, List<(string, string, string, string)>> dualInvestmentOptions = new Dictionary<string, List<(string, string, string, string)>>();
            dualInvestmentOptions.Add("ETH", new List<(string, string, string, string)>()
            {
                ("ETH", "USDT", "DUAL_BASE", "USD"),
                ("ETH", "USDC", "DUAL_BASE", "USD")
            });
            dualInvestmentOptions.Add("LRC", new List<(string, string, string, string)>()
            {
                ("LRC", "USDT", "DUAL_BASE", "USDT"),
                ("LRC", "USDC", "DUAL_BASE", "USDT")
            });
            dualInvestmentOptions.Add("WBTC", new List<(string, string, string, string)>()
            {
                ("WBTC", "USDT", "DUAL_BASE", "USD"),
                ("WBTC", "USDC", "DUAL_BASE", "USD")
            });
            dualInvestmentOptions.Add("USDT", new List<(string, string, string, string)>()
            {
                ("ETH", "USDT", "DUAL_CURRENCY", "USD"),
                ("LRC", "USDT", "DUAL_CURRENCY", "USDT"),
                ("WBTC", "USDT", "DUAL_CURRENCY", "USD")
            });
            dualInvestmentOptions.Add("USDC", new List<(string, string, string, string)>()
            {
                ("ETH", "USDC", "DUAL_CURRENCY", "USD"),
                ("LRC", "USDC", "DUAL_CURRENCY", "USDT"),
                ("WBTC", "USDC", "DUAL_CURRENCY", "USD")
            });

            var currentInvestmentOportunities = new List<(string, string, string, string)>();
            foreach (var token in tokensToInvest)
            {
                currentInvestmentOportunities.AddRange(dualInvestmentOptions[token]);
            }
            currentInvestmentOportunities = currentInvestmentOportunities.Distinct(new TupleComparer()).ToList();

            dynamic? maxResult = null;
            long maxTimeHours;
            dynamic maxProfit = -100000;
            (string, string, string, string) selectedOportunity = ("", "", "", "");

            foreach (var investmetnOportunity in currentInvestmentOportunities)
            {
                var currentDateTime = client.Timestamp();
                //Logic required for Quote Symbol
                dynamic results = client.GetDualInvestmetns(investmetnOportunity.Item1, investmetnOportunity.Item2, investmetnOportunity.Item3, 20, investmetnOportunity.Item4);

                foreach (var result in results.infos)
                {
                    long totalTimeHours = ((result.expireTime.ToObject<long>() - currentDateTime) / 1000) / 60 / 60;

                    if (maxProfit < decimal.Parse(result.profit.ToString()) / totalTimeHours
                        )
                    {
                        maxResult = result;
                        maxTimeHours = totalTimeHours;
                        maxProfit = decimal.Parse(result.profit.ToString()) / totalTimeHours;
                        selectedOportunity = investmetnOportunity;
                    }
                }
            }

            if (maxResult != null)
            {
                if (maxResult.dualType.ToString() == "DUAL_CURRENCY")
                {
                    _logger.LogWarning($"[DUAL INVESTMENT] We have USDT/USDC we need to sell and the code is not ready for that!");

                    DualBaseModel dbm = JsonConvert.DeserializeObject<DualBaseModel>(JsonConvert.SerializeObject(maxResult));

                    var moneyWeHave = client.Ballances();
                    var theThing = moneyWeHave.Where(w => w.token == maxResult.currency.ToString()).FirstOrDefault();
                    var sellingAmount = theThing.total - theThing.locked;
                    sellingAmount = TruncateDecimal(sellingAmount, 2);

                    var cm_strikePrice = dbm.Strike;
                    var cm_profit = dbm.Profit;
                    var cm_ratio = dbm.Ratio;
                    var cm_SelectedOportunity = selectedOportunity.Item1;
                    var cm_sellingToken = selectedOportunity.Item2;
                    decimal cryptoBuyingAmmount =
                        (
                            sellingAmount / (decimal.Parse(cm_strikePrice))
                        )
                        *
                        (
                            1
                            +
                            (
                                (decimal)(double.Parse(cm_profit) * cm_ratio)
                            )
                        );

                    if (TruncateDecimal(cryptoBuyingAmmount, 7) < cryptoBuyingAmmount)
                        cryptoBuyingAmmount = TruncateDecimal(cryptoBuyingAmmount, 7) + 0.0000001M;
                    else
                        cryptoBuyingAmmount = TruncateDecimal(cryptoBuyingAmmount, 7);

                    string cryptoBuyingAmmountToken = cm_SelectedOportunity;
                    string sellingToken = cm_sellingToken;

                    var dInvestementResult = client.StartDualInvestment(dbm, sellingAmount, sellingToken, cryptoBuyingAmmount, cryptoBuyingAmmountToken);

                    _logger.LogInformation($"[DUAL INVESTMENT] INVESTING STABLECOIN: {dInvestementResult}");
                    _logger.LogInformation($"[DUAL INVESTMENT] [CYCLE] Total Funds: {walletValue}");

                }
                else
                {
                    DualBaseModel dbm = JsonConvert.DeserializeObject<DualBaseModel>(JsonConvert.SerializeObject(maxResult));

                    var moneyWeHave = client.Ballances();
                    var theThing = moneyWeHave.Where(w => w.token == selectedOportunity.Item1).FirstOrDefault();

                    var cm_cryptoAvailableInWallet = theThing.total - theThing.locked;
                    var cm_strikePrice = dbm.Strike;
                    var cm_profit = dbm.Profit;
                    var cm_ratio = dbm.Ratio;
                    var cm_SelectedOportunity = selectedOportunity.Item1;
                    var cm_sellingToken = selectedOportunity.Item2;

                    decimal cryptoBuyingAmmount = TruncateDecimal(cm_cryptoAvailableInWallet, 7);
                    string cryptoBuyingAmmountToken = cm_SelectedOportunity;
                    string sellingToken = cm_sellingToken;
                    decimal sellingAmount = TruncateDecimal((decimal.Parse(cm_strikePrice) * cryptoBuyingAmmount) * (1.0M + (decimal)(double.Parse(cm_profit) * cm_ratio)), 2);
                    if (sellingAmount < (decimal.Parse(cm_strikePrice) * cryptoBuyingAmmount) * (1.0M + (decimal)(double.Parse(cm_profit) * cm_ratio)))
                        sellingAmount = sellingAmount + 0.01M;

                    var dInvestementResult = client.StartDualInvestment(dbm, sellingAmount, sellingToken, cryptoBuyingAmmount, cryptoBuyingAmmountToken);

                    _logger.LogInformation($"[DUAL INVESTMENT] INVESTING CRYPTO: {dInvestementResult}");
                    _logger.LogInformation($"[DUAL INVESTMENT] [CYCLE] Total Funds: {walletValue}");
                }
            }
            else
            {
                _logger.LogInformation($"[DUAL INVESTMENT] [FUNDS ACCESSIBLE] No oportunity under 3 hours yet");
            }

            //maxResult is the DI we need to subscribe to... so now we just need the endpoint to do it.
        }

        public decimal TruncateDecimal(decimal number, long digits)
        {
            decimal stepper = (decimal)(Math.Pow(10.0, (double)digits));
            long temp = (long)(stepper * number);
            return (decimal)temp / stepper;
        }

        public List<string> GetTokensOver300USD()
        {
            List<string> result = new List<string>();

            var moneyWeHave = client.Ballances();
            var prices = client.GetPrice(LegalCurrencies.USD);

            foreach (var moneys in moneyWeHave)
            {
                //var usdValue = moneys.total * decimal.Parse(prices.Where(w => w.symbol == moneys.token).First().price);
                var usdValue = (moneys.total - moneys.locked) * decimal.Parse(prices.Where(w => w.symbol == moneys.token).First().price);
                if (usdValue >= 300)
                    result.Add(moneys.token);
            }
            return result;
        }

        public (string total, string locked) GetWalletDollarValue()
        {
            List<(string, decimal)> coins = new List<(string, decimal)>();
            List<(string, decimal)> locked = new List<(string, decimal)>();

            var moneyWeHave = client.Ballances();

            foreach (var moneys in moneyWeHave)
            {
                coins.Add((moneys.token, moneys.total));
                locked.Add((moneys.token, moneys.locked));
            }

            return ($"{string.Join(" | ", coins.Select(s => s.Item1 + ": " + s.Item2).ToArray())}", $"{string.Join(" | ", locked.Select(s => s.Item1 + ": " + s.Item2).ToArray())}");
        }
    }

    public class TupleComparer : IEqualityComparer<(string, string, string, string)>
    {
        public bool Equals((string, string, string, string) x, (string, string, string, string) y)
        {
            return x.Item1 == y.Item1 && x.Item2 == y.Item2 && x.Item3 == y.Item3 && x.Item4 == y.Item4;
        }

        public int GetHashCode((string, string, string, string) obj)
        {
            return (obj.Item1, obj.Item2, obj.Item3, obj.Item4).GetHashCode();
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

