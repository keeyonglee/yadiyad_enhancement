using Nop.Data;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Services;
using NUnit.Framework;
using Nop.Services.ShippingShuq;
using YadiYad.Tests;
using Nop.Core.Domain.Shipping;
using System.Collections.Generic;
using Moq;
using Nop.Core.Caching;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Domain;
using Nop.Services.Caching;
using System;
using System.Linq;
namespace YadiYad.Pro.Tests.Services.Shipping
{
    public class ShippingChargeCalcualtionTests : YadiYadServiceTest
    {
        private WarehouseService _warehouseService;
        private ICacheKeyService _cacheKeyService;
        private IStaticCacheManager _staticCacheManager;
        private CacheKey _cacheKey;

        public ShippingChargeCalcualtionTests()
        {
            _cacheKey = new CacheKey("string");
            var cacheKeyService = new Mock<ICacheKeyService>();
            cacheKeyService.Setup(s => s.PrepareKeyForShortTermCache(It.IsAny<CacheKey>())).Returns(_cacheKey);
            _cacheKeyService = cacheKeyService.Object;
        }

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _fakeDataStore.RegRepository<Warehouse>();
            _fakeDataStore.RegRepository<ShippingByWeightByTotalRecord>();
        }

        [Test]
        public void Get_Estimated_Shipping()
        {
            var withinEastMalaysia = new Warehouse
            {
                Id = 4,
                Name = "Within.East.Malaysia"
            };
            var sbwtRecord1 = new ShippingByWeightByTotalRecord
            {
                Id = 1,
                WarehouseId = withinEastMalaysia.Id,
                WeightFrom = 0,
                WeightTo = 1000,
                RatePerWeightUnit = 0.0078M,
                ShippingMethodId = 1
            };
            var warehouseRepository = _fakeDataStore.GetRepository<Warehouse>();
            var sbwrtRepository = _fakeDataStore.GetRepository<ShippingByWeightByTotalRecord>();

            var warehouses = new List<Warehouse>();
            warehouses.Add(withinEastMalaysia);
            var swbtRepos = new List<ShippingByWeightByTotalRecord>();
            swbtRepos.Add(sbwtRecord1);

            var staticCacheManager = new Mock<IStaticCacheManager>();
            staticCacheManager.Setup(s => s.Get(_cacheKey, It.IsAny<Func<List<ShippingByWeightByTotalRecord>>>()))
                .Returns(swbtRepos);
            _staticCacheManager = staticCacheManager.Object;
            warehouseRepository.Insert(warehouses);
            _warehouseService = new WarehouseService(warehouseRepository);
            var shippingByWeightByTotalService = new ShippingByWeightByTotalService(_cacheKeyService, sbwrtRepository,
                _staticCacheManager, _warehouseService);

            var receiverStateId = 344;
            var zip = "234234";
            var weight = 699;
            var senderStateId = 343;
            var response = shippingByWeightByTotalService.FindRecords(sbwtRecord1.Id, 1, 1, 131, receiverStateId, zip, weight, 10, senderStateId);
            Assert.That(response != default);
            Assert.That(response.Id == sbwtRecord1.Id);
        }
    }
}