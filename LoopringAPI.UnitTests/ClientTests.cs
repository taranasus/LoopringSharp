using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LoopringAPI.UnitTests
{
    [TestClass]
    public class ClientTests
    {
        [TestMethod]
        public void TestGetTradeSuccess()
        {
            //arrange
            Client client = new Client("https://uat2.loopring.io/", "0x5ce27884b99146b4d67a3d3c5ea9566401bdc11f1f561b54d62c0e4a516d7aa0");
            
            //act
            var result = client.GetTrades("LRC-ETH",20,new FillTypes[] {FillTypes.dex, FillTypes.amm }).Result;

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual("LRC-ETH", result[0].Market);
        }

        [TestMethod]       
        public void TestGetTradeMarketNotExists()
        {
            //arrange
            Client client = new Client("https://uat2.loopring.io/", "0x5ce27884b99146b4d67a3d3c5ea9566401bdc11f1f561b54d62c0e4a516d7aa0");

            //act
            var result = client.GetTrades("CPP-RCS").Result;

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestCreateInfoSuccess()
        {
            //arrange
            Client client = new Client("https://uat2.loopring.io/", "0x5ce27884b99146b4d67a3d3c5ea9566401bdc11f1f561b54d62c0e4a516d7aa0");

            //act
            var result = client.CreateInfo().Result;

            //assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void TestUpdateInfoSuccess()
        {
            //arrange
            Client client = new Client("https://uat2.loopring.io/", "0x5ce27884b99146b4d67a3d3c5ea9566401bdc11f1f561b54d62c0e4a516d7aa0");

            //act
            var result = client.UpdateInfo().Result;

            //assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
    }
}