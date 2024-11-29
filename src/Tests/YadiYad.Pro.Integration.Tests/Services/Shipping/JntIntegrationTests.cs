using System;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Nop.Core.Infrastructure;
using Nop.Services.Tests;
using Nop.Services.Tests.FakeServices.Providers;
using Nop.Tests;
using NUnit.Framework;
using NUnit;
using QB.Shipping.JnT;
using QB.Shipping.JnT.Models;
using YadiYad.Tests;
using Nop.Core.Domain.ShippingShuq;

namespace YadiYad.Pro.Integration.Tests.Services.Shipping
{
    public class JntIntegrationTests : ServiceTest
    {
        private JntService _jntService;
        private IHttpClientFactory _httpClientFactory;
        private ShippingJntSettings _settings;

        [SetUp]
        public void Setup()
        {
            var httpClient = new HttpClient();
            _httpClientFactory = new FakeHttpClientFactory(httpClient);
            _settings = new ShippingJntSettings
            {
                TrackingUrl = "https://www.jtexpress.my/tracking/",
                OrderKey = "AKe62df84bJ3d8e4b1hea2R45j11klsb",
                TrackingKey = "ffe62df84bb3d8e4b1eaa2c22775014d",
                BaseUrl = "http://47.57.89.30",
                Username = "TEST",
                ApiKey = "TES123",
                CustomerCode = "ITTEST0001",
                MessageType = "TRACK",
                ECompanyId = "TEST",
                CreateOrder = "/blibli/order/createOrder",
                Tracking = "/common/track/trackings",
                ConsignmentNote = "/jandt_report_web/print/facelistAction!print.action?",
                ServiceType = "6",
                GoodsType = "PARCEL",
                PayType = "PP_PM"
            };
            //_settings = new ShippingJntSettings
            //{
            //    TrackingUrl = "https://www.jtexpress.my/tracking/",
            //    OrderKey = "AKe62df84bJ3d8e4b1hea2R45j11klsb",
            //    TrackingKey = "00bcffb5350b0c4753e1aa6cee1a5043f2be1adcec23c4b84a2709786ad964b3",
            //    BaseUrl = "https://api.jtexpress.my",
            //    Username = "YADIYAD",
            //    ApiKey = "YADIYAD123",
            //    CustomerCode = "600000V127",
            //    MessageType = "TRACK",
            //    ECompanyId = "YADIYAD",
            //    CreateOrder = "/blibli/order/createOrder",
            //    Tracking = "/common/track/trackings",
            //    ConsignmentNote = "/jandt_report_web/print/facelistAction!print.action",
            //    ServiceType = "6",
            //    GoodsType = "PARCEL",
            //    PayType = "PP_PM",
            //    Password = "wqXHmT"
            //};
            _jntService = new JntService(_httpClientFactory, _settings);
        }

        [TearDown]
        public void TearDown()
        {
            var client = _httpClientFactory.CreateClient();
            if (client != null)
            {
                client.Dispose();
            }
            
        }

        [Test]
        public void CreateOrder_ShouldReturn_SuccessResponse()
        {
            var shipment = new OrderRequestDetails
            {
                OrderId = "YD202109151025",
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
                GoodsValue = 400,
                GoodsType = "PARCEL",
                PayType = "PP_PM",
                OfferFeeFlag = 0
            };

            var result = _jntService
                .CreateOrderAsync<ValidResponse<CreateOrderResponse>>(shipment, new CancellationToken())
                .GetAwaiter()
                .GetResult();
            
            Assert.That(result.Details != default);
        }

        [Test]
        public void CreateOrder_ShouldReturn_Awb()
        {
            var shipment = new OrderRequestDetails
            {
                OrderId = "YD202109121356",
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
                Weight = "100",
                ServiceType = "6",
                ItemName = "IPHONE12",
                GoodsDesc = "THIS IS REMARK",
                GoodsValue = 719,
                GoodsType = "PARCEL",
                PayType = "PP_PM",
                OfferFeeFlag = 0,
                CustomerCode = _settings.CustomerCode,
                Username = _settings.Username,
                ApiKey = _settings.ApiKey,
            };

            var result = _jntService
                .CreateOrderAsync<ValidResponse<CreateOrderResponse>>(shipment, new CancellationToken())
                .GetAwaiter()
                .GetResult();

            Assert.That(result.Details[0].AwbNo != "");
        }

        [Test]
        public void CreateOrder_ShouldReturn_InsuranceFee()
        {
            var shipment = new OrderRequestDetails
            {
                OrderId = "YD202110032110",
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
                ItemName = "IPHONE12",
                GoodsDesc = "THIS IS REMARK",
                GoodsValue = 800,
                OfferFeeFlag = 1,
                ServiceType = _settings.ServiceType,
                GoodsType = _settings.GoodsType,
                PayType = _settings.PayType,
                CustomerCode = _settings.CustomerCode,
                Username = _settings.Username,
                ApiKey = _settings.ApiKey,
            };

            var result = _jntService
                .CreateOrderAsync<ValidResponse<CreateOrderResponse>>(shipment, new CancellationToken())
                .GetAwaiter()
                .GetResult();
            if (result.Details != null)
            {
                Assert.That(result.Details[0].Data.InsuranceFee != 0);

            }
            Assert.That(result != default);
        }

        [Test]
        public void GetQuotation()
        {
            var shipment = new OrderRequestDetails
            {
                OrderId = $"YD-{DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss")}",
                SenderName = "Mr. Blue",
                ShipperName = "Mr. Green",
                ShipperAddress = "2, Jalan Kerinchi, Bangsar South",
                ShipperContact = "Company Name",
                ShipperPhone = "60164588281",
                SenderZip = "47400",
                ReceiverName = "Mr White",
                ReceiverAddress = "A-4106, Jalan Kubang Buaya, Kuantan, Johor",
                ReceiverZip = "25250",
                ReceiverPhone = "01111000508",
                Quantity = "1",
                Weight = "15",
                ItemName = "IPHONE12",
                GoodsDesc = "THIS IS REMARK",
                GoodsValue = 800,
                OfferFeeFlag = 1,
                ServiceType = _settings.ServiceType,
                GoodsType = _settings.GoodsType,
                PayType = _settings.PayType,
                CustomerCode = _settings.CustomerCode,
                Username = _settings.Username,
                ApiKey = _settings.ApiKey,
            };

            var result = _jntService
                .CreateOrderAsync<ValidResponse<CreateOrderResponse>>(shipment, new CancellationToken())
                .GetAwaiter()
                .GetResult();

            Assert.That(result.Details[0].AwbNo != "");
        }

        [Test]
        public void Tracking_CheckSuccess()
        {
            var tracking = new Tracking
            {
               Language = "2",
               QueryType = 1,
               QueryNumber = new string[] { "630525666946" }
            };

            var result = _jntService
                .TrackingAsync<TrackingValidResponse<TrackingResponse>>(tracking, new CancellationToken())
                .GetAwaiter()
                .GetResult();

            Assert.That(result != default);
        }


        [Test]
        public void CreateConsignmentNote_Integration()
        {
            var airwaybill = "630525666946";
            var response = _jntService
                .CreateConsignmentNoteAsync<ValidResponse<CreateOrderResponse>>(airwaybill, new CancellationToken())
                .GetAwaiter()
                .GetResult();

            Assert.That(response != default);
        }

        [Test]
        public void Create_Note_Pdf()
        {
            var airwaybill = "630525666946";
            var file = _jntService.CreateConsignmentPdfAsync(airwaybill, new CancellationToken()).Result;
            Assert.That(file != default);
        }
    }
}