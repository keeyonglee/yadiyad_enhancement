using System;
using System.Linq;
using Nop.Data;
using YadiYad.Pro.Core.Domain.Campaign;

namespace YadiYad.Pro.Services.Services.Campaign
{
    public class CampaignSubscriptionService
    {
        private readonly IRepository<CampaignSubscription> _campaignSubsriptionRepository;

        public CampaignSubscriptionService(IRepository<CampaignSubscription> campaignSubsriptionRepository)
        {
            _campaignSubsriptionRepository = campaignSubsriptionRepository;
        }

        internal void Add(int campaignId, int customerId, int activityId, CampaignType campaignType, int actorId, int usageRefId = 0)
        {
            _campaignSubsriptionRepository.Insert(new CampaignSubscription
            {
                CampaignActivity = activityId,
                CustomerId = customerId,
                CampaignId = campaignId,
                CampaignType = campaignType,
                UsageRefId = usageRefId,
                CreatedOnUTC = DateTime.UtcNow,
                ActorId = actorId
            });
        }

        internal int CampaignUsageCountForCustomer(int customerId, int campaignId)
        {
            return _campaignSubsriptionRepository.Table
                .Count(s => s.CustomerId == customerId
                            && s.CampaignId == campaignId);
        }
    }
}