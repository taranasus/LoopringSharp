using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;

namespace LoopringSharp.UnitTests
{
    [TestClass]
    public class ClientTests
    {
        public string url;
        public string privateKey;
        public ClientTests()
        {
            ApiKeys apiKeys = ReadConfigFile(false);
            url = apiKeys.apiUrl;
            privateKey = apiKeys.l1Pk;
        }

        [TestMethod]
        public void TestGetTradeSuccess()
        {
            //arrange
            Client client = new Client(url, privateKey);
            
            //act
            var result = client.GetTrades("LRC-ETH",20,new FillTypes[] {FillTypes.dex, FillTypes.amm });

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual("LRC-ETH", result[0].Market);
        }

        [TestMethod]       
        public void TestGetTradeMarketNotExists()
        {
            //arrange
            Client client = new Client(url, privateKey);

            //act
            var result = client.GetTrades("CPP-RCS");

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestCreateInfoSuccess()
        {
            //arrange
            Client client = new Client(url, privateKey);

            //act
            var result = client.CreateInfo();

            //assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void TestUpdateInfoSuccess()
        {
            //arrange
            Client client = new Client(url, privateKey);

            //act
            var result = client.UpdateInfo();

            //assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void GetDepositsSuccess()
        {
            //arrange
            Client client = new Client(url, privateKey);

            //act
            var result = client.GetDeposits();

            //assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void GetWithdrawlsSuccess()
        {
            //arrange
            Client client = new Client(url, privateKey);

            //act
            var result = client.GetWithdrawls();

            //assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void GetTransfersSuccess()
        {
            //arrange
            Client client = new Client(url, privateKey);

            //act
            var result = client.GetTransfers();

            //assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        static ApiKeys ReadConfigFile(bool prod)
        {
            ApiKeys result;
            string filename = "apiKeys.json";
            if (prod)
            {
                filename = "apiKeysProd.json";
            }

            if (!File.Exists(filename))
            {
                result = new ApiKeys()
                {
                    l1Pk = "",
                    l2Pk = "",
                    accountId = "",
                    apiUrl = "",
                    ethAddress = ""
                };
                File.WriteAllText(filename, JsonConvert.SerializeObject(result, Formatting.Indented));
            }
            result = JsonConvert.DeserializeObject<ApiKeys>(File.ReadAllText(filename)) ?? new ApiKeys();

            if (string.IsNullOrWhiteSpace(result.l2Pk))
            {
                Console.WriteLine("WARNING! You need to fill in the details in the appKeys.json file, otherwise this application will not work. FILE IS HERE: " + Directory.GetCurrentDirectory() + "\\" + filename);
                throw new Exception("WARNING! You need to fill in the details in the appKeys.json file, otherwise this application will not work. FILE IS HERE: " + Directory.GetCurrentDirectory() + "\\" + filename);
            }
            return result;
        }
    }
    public class ApiKeys
    {
        public string l1Pk { get; set; }
        public string l2Pk { get; set; }
        public string accountId { get; set; }
        public string ethAddress { get; set; }
        public string apiUrl { get; set; }
    }
}