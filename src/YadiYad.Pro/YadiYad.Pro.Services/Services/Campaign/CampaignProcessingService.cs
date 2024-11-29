using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToDB;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Payout;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Messages.Caching;
using YadiYad.Pro.Core.Domain.Campaign;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Services.DTO.Campaign;
using YadiYad.Pro.Services.DTO.Payout;

namespace YadiYad.Pro.Services.Services.Campaign
{
    public class CampaignProcessingService : IConsumer<CustomerRegisteredEvent>, IConsumer<JobPublishedEvent>, IConsumer<ExtendPviEvent>
    {
        #region Fields
        private readonly CampaignManagementService _campaignManagementService;
        private readonly CampaignSubscriptionService _campaignSubscriptionService;
        private readonly IEnumerable<ICampaignProcessor> _campaignProcessors;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor
        public CampaignProcessingService(CampaignManagementService campaignManagementService,
            CampaignSubscriptionService campaignSubscriptionService,
            IEnumerable<ICampaignProcessor> campaignProcessors,
            ICustomerService customerService)
        {
            _campaignManagementService = campaignManagementService;
            _campaignSubscriptionService = campaignSubscriptionService;
            _campaignProcessors = campaignProcessors;
            _customerService = customerService;
        }
        #endregion

        #region public methods

        public void Process(CampaignActivityIndividual activity, CampaignChannel channel, int customerId, int refId = 0)
        {
            var campaigns = _campaignManagementService.GetActiveCampaignsForActivity(activity, channel);
            ProcessCampaigns(campaigns, customerId, refId);
        }

        public void Process(CampaignActivityOrganization activity, CampaignChannel channel, int customerId, int refId = 0)
        {
            var campaigns = _campaignManagementService.GetActiveCampaignsForActivity(activity, channel);
            ProcessCampaigns(campaigns, customerId, refId);
        }

        public void Apply(int customerId, PayoutRequestDTO refModel)
        {
            if (refModel == default)
                return;

            var campaigns = _campaignManagementService.GetActiveCampaignsForActivity(CampaignActivityIndividual.Payout,
                    CampaignChannel.Pro);

            ApplyCampaigns(customerId, campaigns, refModel);
        }
        
        public void Apply(int customerId, OrderPayoutRequest refModel)
        {
            if (refModel == default)
                return;

            var campaigns = _campaignManagementService.GetActiveCampaignsForActivity(CampaignActivityIndividual.Payout,
                CampaignChannel.Shuq);

            ApplyCampaigns(customerId, campaigns, refModel);
            
        }

        public void HandleEvent(CustomerRegisteredEvent eventMessage)
        {
            if (eventMessage?.Customer == null)
                return;
            // Campaign Channel can be ignored
            if (_customerService.IsInCustomerRole(eventMessage.Customer, NopCustomerDefaults.IndividualRoleName))
            {
                Process(CampaignActivityIndividual.Registration, CampaignChannel.Pro, eventMessage.Customer.Id);
                Process(CampaignActivityIndividual.Registration, CampaignChannel.Shuq, eventMessage.Customer.Id);
            }
        }
        
        public void HandleEvent(JobPublishedEvent eventMessage)
        {
            if (eventMessage?.JobProfileId == default)
                return;

            if (eventMessage?.OrgCustomerId == 0)
                return;
            
            Process(CampaignActivityOrganization.NewJobPost, CampaignChannel.Pro, eventMessage.OrgCustomerId, eventMessage.JobProfileId);
        }
        
        public void HandleEvent(ExtendPviEvent eventMessage)
        {
            if (eventMessage?.ServiceSubscriptionId == default)
                return;

            if (eventMessage?.OrgCustomerId == 0)
                return;
            
            Process(CampaignActivityOrganization.ExtendJobPost, CampaignChannel.Pro, eventMessage.OrgCustomerId, eventMessage.ServiceSubscriptionId);
        }
        #endregion
        
        #region private methods

        private void ProcessCampaigns(List<CampaignManagement> campaigns, int customerId, int refId = 0)
        {
            foreach (var campaign in campaigns)
            {
                ProcessCampaign(campaign, customerId, refId);
            }
        }

        private void ProcessCampaign(CampaignManagement campaign, int customerId, int refId = 0)
        {
            var processor = ResolveCampaign((CampaignType)campaign.Type);
            var campaignOffer = GetCampaignUsageParty(campaign);
            int currentUsage = 0;
            bool hasReferrer = false;

            // process referrer
            if (campaignOffer.NeedReferral)
            {
                // TODO : Change Method to retrieve Referrer for customer
                var referrer = _customerService.GetCustomerById(0);
                if (referrer == null)
                    return;

                hasReferrer = true;

                currentUsage =
                    _campaignSubscriptionService.CampaignUsageCountForCustomer(referrer.Id, campaign.Id);
                if (currentUsage < campaignOffer.ReferredByCustomerUsage.TransactionLimit)
                {
                    var action = GetCampaignSubscriptionSuccessAction(campaignOffer, referrer.Id, customerId);
                    processor.Process(referrer.Id, campaignOffer.ReferredByCustomerUsage.CampaignValue, refId,
                        action);
                }
            }

            // process self
            // Ignore Processing if Campaign Need Referral and No Referrer present for Customer
            if (campaignOffer.NeedReferral && !hasReferrer)
                return;

            currentUsage = _campaignSubscriptionService.CampaignUsageCountForCustomer(customerId, campaign.Id);
            if (currentUsage < campaignOffer.SelfCustomerUsage.TransactionLimit)
            {
                var action = GetCampaignSubscriptionSuccessAction(campaignOffer, customerId, customerId);
                processor.Process(customerId, campaignOffer.SelfCustomerUsage.CampaignValue, refId, action);
            }
        }

        private CampaignOffer GetCampaignUsageParty(CampaignManagement campaign)
        {
            // Limit = 0 // unlimited
            var limit = campaign.TransactionLimit == 0 ? int.MaxValue : campaign.TransactionLimit;
            
            var c = new CampaignOffer
            {
                CampaignActivity = campaign.Activity,
                CampaignId = campaign.Id,
                CampaignType = (CampaignType)campaign.Type,
            };
            if (campaign.Value2 != 0)
            {
                c.NeedReferral = true;
                c.ReferredByCustomerUsage = new CampaignCustomer
                {
                    TransactionLimit = limit,
                    CampaignValue = campaign.Value2
                };
            }
            c.SelfCustomerUsage = new CampaignCustomer
            {
                TransactionLimit = c.NeedReferral ? 1 : limit,
                CampaignValue = campaign.Value1
            };
            return c;
        }

        private ICampaignProcessor ResolveCampaign(CampaignType campaignType)
        {
            var processor = _campaignProcessors.LastOrDefault(s => s.CampaignType == campaignType);
            processor ??= new NoOpCampaignProcessor();
            return processor;
        }

        private Action<int> GetCampaignSubscriptionSuccessAction(CampaignOffer campaign, int customerId, int actorId)
        {
            return (int subscriptionRefId) =>
            {
                if (subscriptionRefId == 0)
                    return;
                
                _campaignSubscriptionService.Add(campaign.CampaignId, customerId, campaign.CampaignActivity, campaign.CampaignType, actorId, subscriptionRefId );
            };
        }

        private void ApplyCampaigns<T>(int customerId, IList<CampaignManagement> campaigns, T refModel)
            where T : class
        {
            foreach (var campaign in campaigns)
            {
                var count = 0;
                count = _campaignSubscriptionService.CampaignUsageCountForCustomer(customerId, campaign.Id);
                var campaignUsageParty = GetCampaignUsageParty(campaign);
                
                if (count >= campaignUsageParty.SelfCustomerUsage.TransactionLimit)
                    continue;

                var completeAction = GetCampaignSubscriptionSuccessAction(campaignUsageParty, customerId, customerId);
                
                // TODO : Enhance if need to have referral
                var processor = ResolveCampaign((CampaignType)campaign.Type);
                processor.Apply(customerId, refModel, campaignUsageParty.SelfCustomerUsage.CampaignValue, completeAction);
            }
        }

        #endregion
        
        #region subclasses
        private class CampaignOffer
        {
            public int CampaignId { get; set; }
            public int CampaignActivity { get; set; }
            public bool NeedReferral { get; set; }
            public CampaignType CampaignType { get; set; }
            public CampaignCustomer SelfCustomerUsage { get; set; }
            public CampaignCustomer ReferredByCustomerUsage { get; set; }
        }

        private class CampaignCustomer
        {
            public int TransactionLimit { get; set; }
            public decimal CampaignValue { get; set; }
        }
        #endregion
    }
}