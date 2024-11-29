using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Moq;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Tests;
using Nop.Tests;
using NUnit.Framework;
using YadiYad.Pro.Core.Domain.Campaign;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Subscription;
using YadiYad.Pro.Services.Services.Campaign;
using YadiYad.Pro.Services.Services.Subscription;
using YadiYad.Tests;

namespace YadiYad.Pro.Tests.Services.Campaign
{
    [TestFixture]
    public class CampaignServiceTest : YadiYadServiceTest
    {
        protected IRepository<CampaignSubscription> _campaignSubscriptionRepository;
        protected CampaignSubscriptionService _campaignSubscriptionService;
        protected ICustomerService _customerService;

        public CampaignServiceTest()
        {
            #region Campaign Management
            _campaignSubscriptionRepository = _fakeDataStore.RegRepository<CampaignSubscription>();
            _fakeDataStore.RegRepository<ServiceSubscription>();
            _fakeDataStore.RegRepository<CampaignManagement>();
            _campaignSubscriptionService = new CampaignSubscriptionService(_campaignSubscriptionRepository);
            #endregion
        }

        protected static class CampaignMockData
        {
            public readonly static CampaignManagement activePayToApplyJobRegistrationOffer = new CampaignManagement
            {
                Title = "Launch Offer",
                Activity = (int)CampaignActivityIndividual.Registration,
                Beneficiary = (int)CampaignBeneficiary.Individual,
                Channel = (int)CampaignChannel.Pro,
                Type = (int)CampaignType.PayToApplyJobs,
                Value1 = 60,
                EngagementType = (int)CampaignEngagementType.Job,
                From = DateTime.Now.AddDays(-1),
                Until = DateTime.Now.AddDays(1),
                TransactionLimit = 0,
                Deleted = false,
                Id = 1
            };

            public readonly static CampaignManagement activePayToViewJobPostOffer = new CampaignManagement
            {
                Title = "Launch Offer",
                Activity = (int)CampaignActivityOrganization.NewJobPost,
                Beneficiary = (int)CampaignBeneficiary.Organization,
                Channel = (int)CampaignChannel.Pro,
                Type = (int)CampaignType.PayToViewAndInvite,
                Value1 = 60,
                EngagementType = (int)CampaignEngagementType.Job,
                From = DateTime.Now.AddDays(-1),
                Until = DateTime.Now.AddDays(1),
                TransactionLimit = 0,
                Deleted = false,
                Id = 0
            };

        }

        [SetUp]
        public override void SetUp()
        {
            var nopEngine = new Mock<NopEngine>();
            nopEngine.Setup(x => x.ServiceProvider).Returns(new TestServiceProvider());
            EngineContext.Replace(nopEngine.Object);

            _fakeDataStore.ResetStore();
        }

        [TearDown]
        public void TearDown()
        {
            EngineContext.Replace(null);
        }

        [Test]
        public void Ensure_PayToApplyJobSubscription_IndividualCampaign_RegistrationActivity()
        {
            var subsRepository = _fakeDataStore.GetRepository<ServiceSubscription>();
            var campaigns = new List<CampaignManagement>();
            campaigns.Add(CampaignMockData.activePayToApplyJobRegistrationOffer);
            
            var campRepository = _fakeDataStore.GetRepository<CampaignManagement>();
            campRepository.Insert(campaigns);

            var campaignManageService = new CampaignManagementService(campRepository);
            
            var customer = new Customer { Id = 1 };
            var customerService = new Mock<ICustomerService>();
            customerService.Setup(x => x.GetCustomerById(1)).Returns(customer);
            customerService.Setup(x => x.IsInCustomerRole(customer, NopCustomerDefaults.IndividualRoleName, It.IsAny<bool>())).Returns(true);
            _customerService = customerService.Object;

            var serviceSubscriptionService =
                new ServiceSubscriptionService(_mapper, _logger, _dateTimeHelper, subsRepository);
            var processors = new List<ICampaignProcessor>();
            var ptvProcessor = new PayToApplyJobsCampaign(_customerService, serviceSubscriptionService);
            processors.Add(ptvProcessor);

            var campaignProcessService = new CampaignProcessingService(campaignManageService, _campaignSubscriptionService, processors, _customerService);

            campaignProcessService.Process(CampaignActivityIndividual.Registration, CampaignChannel.Pro, customer.Id);

            var result = _campaignSubscriptionRepository.Table.Where(q => q.CustomerId == customer.Id).ToList();
            Assert.That(result.Count == 1);
            var subs = result.First();
            Assert.That(subs.CampaignId == CampaignMockData.activePayToApplyJobRegistrationOffer.Id);
            Assert.That(subs.CampaignActivity == (int)CampaignActivityIndividual.Registration);

            var subscription = subsRepository.Table.Where(q => q.CustomerId == customer.Id).ToList();
            Assert.That(subscription.Count == 1);
            var serviceSubs = subscription.First();
            Assert.That(serviceSubs.Id == subs.UsageRefId);
            Assert.That(serviceSubs.EndDate.Subtract(serviceSubs.StartDate).Days == CampaignMockData.activePayToApplyJobRegistrationOffer.Value1);
            Assert.That(serviceSubs.SubscriptionTypeId == (int)SubscriptionType.ApplyJob);
        }

        [Test]
        public void Ensure_PayToViewSubscription_OrganizationCampaign_RegistrationActivity()
        {
            #region Arrange
            var subsRepository = _fakeDataStore.GetRepository<ServiceSubscription>();
            var campaigns = new List<CampaignManagement>();
            campaigns.Add(CampaignMockData.activePayToViewJobPostOffer);
            
            var campRepository = _fakeDataStore.GetRepository<CampaignManagement>();
            campRepository.Insert(campaigns);

            var campaignManageService = new CampaignManagementService(campRepository);
            
            var customer = new Customer { Id = 1 };
            var customerService = new Mock<ICustomerService>();
            customerService.Setup(x => x.GetCustomerById(1)).Returns(customer);
            customerService.Setup(x => x.IsInCustomerRole(customer, NopCustomerDefaults.OrganizationRoleName, It.IsAny<bool>())).Returns(true);
            _customerService = customerService.Object;

            var jobPost = new JobProfile { Id = 1 };

            var serviceSubscriptionService =
                new ServiceSubscriptionService(_mapper, _logger, _dateTimeHelper, subsRepository);
            var processors = new List<ICampaignProcessor>();
            var ptvProcessor = new PayToViewInviteJobsCampaign(_customerService, serviceSubscriptionService);
            processors.Add(ptvProcessor);

            var campaignProcessService = new CampaignProcessingService(campaignManageService, _campaignSubscriptionService, processors, _customerService);
            #endregion

            campaignProcessService.Process(CampaignActivityOrganization.NewJobPost, CampaignChannel.Pro, customer.Id, jobPost.Id);

            var result = _campaignSubscriptionRepository.Table.Where(q => q.CustomerId == customer.Id).ToList();
            Assert.That(result.Count == 1);
            var subs = result.First();
            Assert.That(subs.CampaignId == CampaignMockData.activePayToViewJobPostOffer.Id);
            Assert.That(subs.CampaignActivity == (int)CampaignActivityOrganization.NewJobPost);

            var subscription = subsRepository.Table.Where(q => q.CustomerId == customer.Id).ToList();
            Assert.That(subscription.Count == 1);
            var serviceSubs = subscription.First();
            Assert.That(serviceSubs.Id == subs.UsageRefId);
            Assert.That(serviceSubs.EndDate.Subtract(serviceSubs.StartDate).Days == CampaignMockData.activePayToViewJobPostOffer.Value1);
            Assert.That(serviceSubs.SubscriptionTypeId == (int)SubscriptionType.ViewJobCandidateFulleProfile);
            Assert.That(serviceSubs.RefId == jobPost.Id);
        }
    }
}