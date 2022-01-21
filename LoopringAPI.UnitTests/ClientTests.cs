using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Numerics;

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

        [TestMethod]
        public void TestClassGeneration()
        {
            //arrange
            Client client = new Client(url.TrimEnd('/'), privateKey);
            Assert.IsTrue(client!=null);
        }

        [TestMethod]
        public void TestWithdrawEDDSA()
        {
            string l2Pk = "0x7577a0d0c17628a7b8dbb70f4545af247ef572d39bf04f280996754be33a7d";

            BigInteger[] inputs = {
                Utils.ParseHexUnsigned("0x2e76EBd1c7c0C8e7c2B875b6d505a260C525d25e"),
                (BigInteger)11201,
                (BigInteger)0,
                BigInteger.Parse("1000000000000000"),
                (BigInteger)0,
                BigInteger.Parse("143000000000000"),
                Utils.ParseHexUnsigned("0x0"), // TODO It says onChainDataHash, i haven't the foggiest what the fuck that is                
                (BigInteger)1645384147,
                (BigInteger)7
            };
            
            var result = EDDSAHelper.EDDSASign(inputs, l2Pk);
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