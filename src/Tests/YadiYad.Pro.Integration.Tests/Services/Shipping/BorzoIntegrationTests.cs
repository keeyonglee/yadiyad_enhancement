using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Nop.Core.Domain.ShippingShuq;
using NUnit.Framework;
using QB.Shipping.Borzo;
using QB.Shipping.Borzo.Models;
using YadiYad.Tests;

namespace YadiYad.Pro.Integration.Tests.Services.Shipping
{
    public class BorzoIntegrationTests
    {
        private IHttpClientFactory _httpClientFactory;
        private ShippingBorzoSettings _settings;
        private BorzoService _borzoService;
        
        [SetUp]
        public void Setup()
        {
            var httpClient = new HttpClient();
            _httpClientFactory = new FakeHttpClientFactory(httpClient);
            _settings = new ShippingBorzoSettings
            {
                ApiSecret = "539A5D1AF93F94A45A162DAFDD2B35263E3D3099",
                BaseUrl = "https://robotapitest-my.borzodelivery.com",
                GetQuotation = "/api/business/1.2/calculate-order",
                PlaceOrder = "/api/business/1.2/create-order",
                GetClient = "/api/business/1.2/client",
                GetOrders = "/api/business/1.2/orders"
            };
            _borzoService = new BorzoService(_httpClientFactory, _settings);
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

        // [Test]
        // public void Test_GetClient()
        // {
        //     var result
        //         = _borzoService
        //         .GetAsync<pla<PlaceOrderRequestModel>>( _settings.GetClient, new PlaceOrderRequestModel(), new CancellationToken())
        //         .GetAwaiter()
        //         .GetResult();
        //     
        //     Assert.That(result != null);
        // }
        
        // [Test]
        // public void Test_GetQuotation_Json()
        // {
        //     var model = new PlaceOrderModel();
        //     model.Matter =
        //         "{\"matter\":\"Documents\",\"vehicle_type_id\":\"7\",\"points\":[{\"address\":\"PL-10, Perdana (Tropics) Shopping Centre, Jalan PJU 8/1, Damansara Perdana, 47820 Petaling Jaya, Selangor\"},{\"address\":\"7,7-1 &amp; 7-2, Dinasti Sentral, Jalan Kuchai Maju 18, Off Jalan Kuchai Lama, 58200, Kuala Lumpur, Wilayah Persekutuan\"}]}";
        //     var result
        //         = _borzoService
        //             .GetAsync<ValidResponse<PlaceOrderModel>>( _settings.GetQuotation, model, new CancellationToken())
        //             .GetAwaiter()
        //             .GetResult();
        //     
        //     Assert.That(result != null);
        // }
        
        // [Test]
        // public void Test_PlaceOrder_Json()
        // {
        //     var model = new PlaceOrderModel();
        //     model.Matter =
        //         "{\"matter\":\"Documents\",\"points\":[{\"address\":\"PL-10, Perdana (Tropics) Shopping Centre, Jalan PJU 8/1, Damansara Perdana, 47820 Petaling Jaya, Selangor\",\"contact_person\":{\"phone\":\"60192324875\"}},{\"address\":\"7,7-1 &amp; 7-2, Dinasti Sentral, Jalan Kuchai Maju 18, Off Jalan Kuchai Lama, 58200, Kuala Lumpur, Wilayah Persekutuan\",\"contact_person\":{\"phone\":\"60192324875\"}}]}";
        //     var result
        //         = _borzoService
        //             .GetAsync<ValidResponse<PlaceOrderModel>>( _settings.PlaceOrder, model, new CancellationToken())
        //             .GetAwaiter()
        //             .GetResult();
        //     
        //     Assert.That(result != null);
        // }

        public PlaceOrderRequestModel Create_Dummy_Data()
        {
            var model = new PlaceOrderRequestModel
            {
                Matter = "test start time",
                VehicleTypeId = 7,
                Points = new List<Point>(),
                TotalWeightKg = 2
            };
            var start = new Point
            {
                ContactPerson = new ContactPerson
                {
                    Phone = "0123454567"
                },
                Address = "Seventeen Mall, Jalan 17/38, Seksyen 17, Petaling Jaya, Selangor, Malaysia",
                RequiredStartDatetime = "2023-10-07T07:00:00.0000000Z"
            };
            var end = new Point
            {
                ContactPerson = new ContactPerson
                {
                    Phone = "0123454568"
                },
                Address = "OUG Parklane, Jalan Puchong, Kuala Lumpur, Wilayah Persekutuan Kuala Lumpur, Malaysia"
            };
            model.Points.Add(start);
            model.Points.Add(end);
            return model;
        }

        [Test]
        public void Test_GetQuotation_Model()
        {
            var model = Create_Dummy_Data();
            var result
                = _borzoService
                    .GetAsync<ValidResponse<PlaceOrderResponseModel>>( _settings.GetQuotation, model, new CancellationToken())
                    .GetAwaiter()
                    .GetResult();
            
            Assert.That(result != null);
        }
        
        [Test]
        public void Test_PlaceOrder_Model()
        {
            var model = Create_Dummy_Data();
            var result
                = _borzoService
                    .GetAsync<ValidResponse<PlaceOrderResponseModel>>( _settings.PlaceOrder, model, new CancellationToken())
                    .GetAwaiter()
                    .GetResult();
            
            Assert.That(result != null);
        }
        
        [Test]
        public void Test_GetOrders()
        {
            var orderId = 102637;
            var result
                = _borzoService
                    .GetOrdersAsync<GetOrdersValidResponse<PlaceOrderResponseModel>>( _settings.GetOrders, orderId, new CancellationToken())
                    .GetAwaiter()
                    .GetResult();
            
            Assert.That(result != null);
        }
    }
}