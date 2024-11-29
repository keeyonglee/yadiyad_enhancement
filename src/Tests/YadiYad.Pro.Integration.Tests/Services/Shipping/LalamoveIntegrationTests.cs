using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using QB.Shipping.LalaMove;
using QB.Shipping.LalaMove.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Nop.Core.Domain.ShippingShuq;
using YadiYad.Tests;

namespace YadiYad.Pro.Integration.Tests.Services.Shipping
{
    public class LalamoveIntegrationTests
    {
        private LalaMoveService _lalaMoveService;
        private IHttpClientFactory _httpClientFactory;
        private ShippingLalamoveSettings _settings;

        [SetUp]
        public void Setup()
        {
            var httpClient = new HttpClient();
            _httpClientFactory = new FakeHttpClientFactory(httpClient);
            _settings = new ShippingLalamoveSettings
            {
                ApiKey = "pk_test_bd6243c6c727a6b6ed6fc55f63d2f884",
                ApiSecret = "sk_test_GPfIJJk4wYTAPoT/N9bGNhv1OqIGhF2fPi+3e+fDgNO454ioMCbdB5/VgY33NzwC",
                BaseUrl = "https://rest.sandbox.lalamove.com",
                TotalFeeCurrency = "MYR",
                GetQuotation = "/v2/quotations",
                PlaceOrder = "/v2/orders",
                CancelOrder = "/v2/orders/{0}/cancel"
            };
            _lalaMoveService = new LalaMoveService(_httpClientFactory, _settings);
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

        public PlaceOrderModel CreateDummyDataOrder(decimal quotedPrice = 0)
        {
            var model = new PlaceOrderModel
            {
                ScheduleAT = "2022-02-15T05:00:00.0000000Z",
                ServiceType = "MOTORCYCLE",
                Market = "MY_KUL"
            };

            var pickUp = new Waypoint();
            var dropOff = new Waypoint();
            var dropOffContact = new DeliveryInfo();

            pickUp.Address.CountryCode.Market = "MY_KUL";
            pickUp.Address.CountryCode.DisplayString = "Bumi Bukit Jalil, No 2-1, Jalan Jalil 1, Lebuhraya Bukit Jalil, Sungai Besi, 57000 Kuala Lumpur, Malaysia";

            dropOff.Address.CountryCode.Market = "MY_KUL";
            dropOff.Address.CountryCode.DisplayString = "64000 Sepang, Selangor, Malaysia";

            model.RequesterContact.Name = "Chen Kel Vin";
            model.RequesterContact.Phone = "0376886555";

            dropOffContact.ToStop = 1;
            dropOffContact.ToContact.Name = "Steven Chen";
            dropOffContact.ToContact.Phone = "0376886556";
            dropOffContact.Remarks = "Remarks for drop-off point (#1).";

            if (quotedPrice != 0)
            {
                model.QuotedTotalFee.Currency = "MYR";
                model.QuotedTotalFee.Amount = quotedPrice;
            }

            model.Stops.Add(pickUp);
            model.Stops.Add(dropOff);
            model.Deliveries.Add(dropOffContact);


            return model;
        }

        [Test]
        public void Test_GetQuotation_With_Json()
        {
            var shipment = new PlaceOrderModel();

            var result = _lalaMoveService
                .GetQuotationAsync<ValidResponse<GetQuotationResponse>>(shipment, new CancellationToken())
                .GetAwaiter()
                .GetResult();

            Assert.That(result.TotalFee != 0);
        }

        [Test]
        public void Test_GetQuotation_With_Model()
        {
            var model = new PlaceOrderModel
            {
                ServiceType = "MOTORCYCLE",
            };

            var pickUp = new Waypoint();
            var dropOff  = new Waypoint();
            var dropOffContact = new DeliveryInfo();

            pickUp.Address.CountryCode.Market = "MY_JHB";
            pickUp.Address.CountryCode.DisplayString = "Paradigm Mall, Lot 3FK-12J, third floor, Jalan Bertingkat Skudai, 81200, Johor";

            dropOff.Address.CountryCode.Market = "MY_JHB";
            dropOff.Address.CountryCode.DisplayString = "Jalan Sri Purnama, Kawasan Perindustrian Seri Pernama, 81100 Johor Bahru, Johor";
            model.Market = "MY_JHB";
            model.RequesterContact.Name = "Chen Kel Vin";
            model.RequesterContact.Phone = "123456789";

            dropOffContact.ToStop = 1;
            dropOffContact.ToContact.Name = "Steven Chen";
            dropOffContact.ToContact.Phone = "123456789";
            dropOffContact.Remarks = "";

            model.Stops.Add(pickUp);
            model.Stops.Add(dropOff);
            model.Deliveries.Add(dropOffContact);

            var result = _lalaMoveService
                .GetQuotationAsync<ValidResponse<PlaceOrderModel>>(model, new CancellationToken())
                .GetAwaiter()
                .GetResult();

            Assert.That(result != null);
        }

        [Test]
        public void Test_PlaceOrder_With_Model()
        {
            var quotationData = CreateDummyDataOrder();

            var quotation = _lalaMoveService
               .GetQuotationAsync<ValidResponse<PlaceOrderModel>>(quotationData, new CancellationToken())
               .GetAwaiter()
               .GetResult();

            TearDown();
            Setup();

            var placeOrderData = CreateDummyDataOrder(quotation.TotalFee);

            var result = _lalaMoveService
                .PlaceOrderAsync<ValidResponse<PlaceOrderModel>>(placeOrderData, new CancellationToken())
                .GetAwaiter()
                .GetResult();

            Assert.That(result != null);
        }

        [Test]
        public void Test_Get_OrderDetails()
        {
            var orderRef = "158450405244";
            var marketCode = "MY_KUL";
            var result = _lalaMoveService
                .GetOrderDetailsAsync<ValidResponse<PlaceOrderModel>>(orderRef, marketCode, new CancellationToken())
                .GetAwaiter()
                .GetResult();

            Assert.That(result != null);
        }

        [Test]
        public void Test_Cancel_Order()
        {
            var orderRef = "150580402110";

            var result = _lalaMoveService
                .GetCancelOrderAsync<ValidResponse<PlaceOrderModel>>(orderRef, new CancellationToken())
                .GetAwaiter()
                .GetResult();

            Assert.That(result != null);
        }
    }
}
