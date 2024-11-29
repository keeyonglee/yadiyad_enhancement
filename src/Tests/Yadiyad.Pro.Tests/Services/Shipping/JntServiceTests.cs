using System.Net.Http;
using System.Threading;
using NUnit.Framework;
using QB.Shipping.JnT;
using Moq;
using Microsoft.Extensions.Options;
using Nop.Services.Tests;
using QB.Shipping.JnT.Models;
using Microsoft.Extensions.Http;
using YadiYad.Tests;
using Nop.Plugin.Shipping.FixedByWeightByTotal;
using Nop.Services.Shipping;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Services;
using Nop.Services.Caching;
using Nop.Data;
using Nop.Core.Caching;
using Nop.Core.Domain.ShippingShuq;

namespace YadiYad.Pro.Tests.Services.Shipping
{
    [TestFixture]
    public class JntServiceTests : ServiceTest
    {
        private IHttpClientFactory _clientFactory;
        private JntService _jntService;
        private FixedByWeightByTotalComputationMethod _fixedByWeightByTotalComputationMethod;
        private ShippingByWeightByTotalService _shippingByWeightByTotalService;
        private ShippingJntSettings _shippingJntSettings;

        public JntServiceTests()
        {
        }
        
        [SetUp]
        public void Setup()
        {
            var jntClient = new FakeJntClient();
            _clientFactory = new FakeHttpClientFactory(jntClient);
            _jntService = new JntService(_clientFactory, _shippingJntSettings);
        }

        [TearDown]
        public void TearDown()
        {
            var client = _clientFactory.CreateClient();
            client.Dispose();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            var client = _clientFactory.CreateClient();
            client.Dispose();
        }

        [TestCase(arg1: "", arg2: "ODgyMjRkM2E0M2Y5ZjFmNzQwM2MwYzg2YzY4MWEyNzI=", arg3:"AKe62df84bJ3d8e4b1hea2R45j11klsb")]
        [TestCase(arg1: "{\"detail\":[]}", arg2: "OTRmN2E5Zjk0MzhkMTllNDMxMzgwNDlmNTVkZjk2YmM=", arg3:"AKe62df84bJ3d8e4b1hea2R45j11klsb")]
        public void Check_SignatureGeneration_CreateOrder_Matches(string content, string checkSum, string key)
        {
            var signature =  _jntService.GetSignature(content, key);

            Assert.That(signature == checkSum);
        }

        [Test]
        public void CreateOrder_Scenario1()
        {
            var reqObj = new OrderRequestDetails { OrderId = "ABC000390" };
            var response = _jntService
                .CreateOrderAsync<ValidResponse<CreateOrderResponse>>(reqObj, new CancellationToken())
                .GetAwaiter()
                .GetResult();
            
            Assert.That(response != default);
        }

        [Test]
        public void CreateOrder_WithDetails()
        {
            var reqObj = new OrderRequestDetails
            { 
                OrderId = "ABC000390",
                ShipperName = "Mr Blue",
                ShipperAddress = "2, Jalan Kerinchi, Bangsar South",
                ShipperContact = "Mrs Blue",
                ShipperPhone = "60164588281",
                SenderZip = "47400",
                ReceiverName = "Mr White",
                ReceiverAddress = "Setia International Centre, Pantai Baru, Lot 215,Jalan Bangsar,Kampung Haji Abdullah Hukum",
                ReceiverZip = "59200",
                ReceiverPhone = "601645882812",
                Quantity = "1",
                Weight = "2.33",
                ServiceType = "1",
                ItemName = "IPHONE12",
                GoodsDesc = "THIS IS REMARK",
                GoodsType = "PARCEL",
                PayType = "PP_PM",
            };
            var response = _jntService
                .CreateOrderAsync<ValidResponse<CreateOrderResponse>>(reqObj, new CancellationToken())
                .GetAwaiter()
                .GetResult();

            Assert.That(response != default);
        }

        [TestCase(arg1: "{ \"queryType\": 1, \"language\": \"2\", \"queryCodes\":[\"630013726169\"]}", arg2: "YzE4ZTIwMDZiMTcwZDk4N2UxMDUxZDE5NzNiZGVhMjg=", arg3: "ffe62df84bb3d8e4b1eaa2c22775014d")]
        public void Check_SignatureGeneration_Tracking_Matches(string content, string checkSum, string key)
        {
            var signature = _jntService.GetSignature(content, key);

            Assert.That(signature == checkSum);
        }

        [Test]
        public void CreateConsignmentNote()
        {
            var airwaybill = "630019720932";
            var response = _jntService
                .CreateConsignmentNoteAsync<ValidResponse<CreateOrderResponse>>(airwaybill, new CancellationToken())
                .GetAwaiter()
                .GetResult();

            Assert.That(response != default);
        }

        //public void Check_ConsignmentNote_Signature_Matches
        //{

        //}
    }
}