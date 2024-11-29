using Microsoft.Extensions.Options;
using Moq;
using Nop.Services.Tests;
using NUnit.Framework;
using QB.Shipping.LalaMove;
using QB.Shipping.LalaMove.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Nop.Core.Domain.ShippingShuq;
using YadiYad.Tests;

namespace YadiYad.Pro.Tests.Services.Shipping
{
    [TestFixture]
    public class LalaMoveServiceTests : ServiceTest
    {
        private IHttpClientFactory _clientFactory;
        private LalaMoveService _lalamoveService;
        private ShippingLalamoveSettings _shippingLalamoveSettings;
        public LalaMoveServiceTests()
        {

        }

        [SetUp]
        public void Setup()
        {
            var lalamoveClient = new FakeLalaMoveClient();
            _clientFactory = new FakeHttpClientFactory(lalamoveClient);
            _lalamoveService = new LalaMoveService(_clientFactory, _shippingLalamoveSettings);
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

        [Test]
        public void Check_GetTime_Now()
        {
            var time = _lalamoveService.GetEpochTimeWithLocal();

            Assert.That(time == 1632216240000);
        }

    }

}

