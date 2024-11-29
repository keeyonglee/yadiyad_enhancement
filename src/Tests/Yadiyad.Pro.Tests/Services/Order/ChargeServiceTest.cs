using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Nop.Data;

using YadiYad.Tests;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Services.Order;
using Moq;
using Nop.Core.Infrastructure;
using Nop.Tests;

namespace YadiYad.Pro.Tests.Services.Order
{
    [TestFixture]
    public class ChargeServiceTest : YadiYadServiceTest
    {
        protected IRepository<Charge> _chargeRepository;
        protected ChargeService _chargeService;

        public ChargeServiceTest()
        {
            #region ChargeS Service
            _chargeRepository = _fakeDataStore.RegRepository<Charge>();
            _chargeService = new ChargeService(_mapper, _dateTimeHelper, _chargeRepository);
            #endregion
        }


        protected static class ChargeMockData
        {
            public readonly static Charge projectChargeTier1 = new Charge
            {
                ProductTypeId = 2,
                SubProductTypeId = 3,
                ValidityDays = 30,
                ValueType = 2,
                Value = 0.05M,
                StartDate = new DateTime(2021, 1, 1),
                EndDate = null,
                IsActive = true,
                Deleted = false,
                CreatedById = 1,
                CreatedOnUTC = new DateTime(2021, 1, 1),
                UpdatedOnUTC = new DateTime(2021, 1, 1),
                MinRange = 0,
                MaxRange = 20000.00M
            };
            public readonly static Charge projectChargeTier2 = new Charge
            {
                ProductTypeId = 2,
                SubProductTypeId = 3,
                ValidityDays = 30,
                ValueType = 2,
                Value = 0.04M,
                StartDate = new DateTime(2021, 1, 1),
                EndDate = null,
                IsActive = true,
                Deleted = false,
                CreatedById = 1,
                CreatedOnUTC = new DateTime(2021, 1, 1),
                UpdatedOnUTC = new DateTime(2021, 1, 1),
                MinRange = 20000,
                MaxRange = 40000.00M
            };
            public readonly static Charge projectChargeTier3 = new Charge()
            {
                ProductTypeId = 2,
                SubProductTypeId = 3,
                ValidityDays = 30,
                ValueType = 2,
                Value = 0.03M,
                StartDate = new DateTime(2021, 1, 1),
                EndDate = null,
                IsActive = true,
                Deleted = false,
                CreatedById = 1,
                CreatedOnUTC = new DateTime(2021, 1, 1),
                UpdatedOnUTC = new DateTime(2021, 1, 1),
                MinRange = 40000,
                MaxRange = 99999999.00M
            };

            public readonly static Charge serviceCharge1 = new Charge()
            {
                ProductTypeId = 1,
                SubProductTypeId = 1,
                ValidityDays = 30,
                ValueType = 2,
                Value = 1,
                StartDate = new DateTime(2021, 1, 1),
                EndDate = null,
                IsActive = true,
                Deleted = false,
                CreatedById = 1,
                CreatedOnUTC = new DateTime(2021, 1, 1),
                UpdatedOnUTC = new DateTime(2021, 1, 1),
                MinRange = null,
                MaxRange = null
            };

            public readonly static Charge consultationMatching = new Charge()
            {
                ProductTypeId = 3,
                SubProductTypeId = 0,
                ValidityDays = 30,
                ValueType = 2,
                Value = 0.5M,
                StartDate = new DateTime(2021, 1, 1),
                EndDate = null,
                IsActive = true,
                Deleted = false,
                CreatedById = 1,
                CreatedOnUTC = new DateTime(2021, 1, 1),
                UpdatedOnUTC = new DateTime(2021, 1, 1),
                MinRange = null,
                MaxRange = null
            };

        }

        [SetUp]
        public void Setup()
        {
            var nopEngine = new Mock<NopEngine>();
            nopEngine.Setup(x => x.ServiceProvider).Returns(new TestServiceProvider());
            EngineContext.Replace(nopEngine.Object);

            _fakeDataStore.ResetStore();

            var chargeRepo = _fakeDataStore.GetRepository<Charge>();
            chargeRepo.Insert(ChargeMockData.projectChargeTier1);
            chargeRepo.Insert(ChargeMockData.projectChargeTier2);
            chargeRepo.Insert(ChargeMockData.projectChargeTier3);
            chargeRepo.Insert(ChargeMockData.serviceCharge1);
            chargeRepo.Insert(ChargeMockData.consultationMatching);
        }

        [TearDown]
        public void TearDown()
        {
            EngineContext.Replace(null);
        }


        [Test]
        public void TestProjectTier()
        {

            var calculatedCharge = _chargeService.GetLatestCharge((int)ProductType.ViewJobCandidateFullProfileSubscription, 3, 5000.90M);

            Assert.AreEqual(0.05M, calculatedCharge.Value);

            calculatedCharge = _chargeService.GetLatestCharge((int)ProductType.ViewJobCandidateFullProfileSubscription, 3, 20000.00M);
            
            Assert.AreEqual(0.04M, calculatedCharge.Value);

            calculatedCharge = _chargeService.GetLatestCharge((int)ProductType.ViewJobCandidateFullProfileSubscription, 3, 40000.01M);

            Assert.AreEqual(0.03M, calculatedCharge.Value);

            calculatedCharge = _chargeService.GetLatestCharge((int)ProductType.ViewJobCandidateFullProfileSubscription, 3, 250000M);

            Assert.AreEqual(0.03M, calculatedCharge.Value);
        }

        [Test]
        public void TestConsultationMatching()
        {
            var calculatedCharge = _chargeService.GetLatestCharge((int)ProductType.ConsultationEngagementMatchingFee, 0, 250000M);

            Assert.AreEqual(0.5M, calculatedCharge.Value);
        }

    }
}
