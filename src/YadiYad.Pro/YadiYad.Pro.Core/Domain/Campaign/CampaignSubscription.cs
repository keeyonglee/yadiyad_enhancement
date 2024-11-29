using System;
using Nop.Core;

namespace YadiYad.Pro.Core.Domain.Campaign
{
    public class CampaignSubscription : BaseEntity
    {
        public int CampaignId { get; set; }
        public int CustomerId { get; set; }
        public int CampaignActivity { get; set; }
        public CampaignType CampaignType { get; set; }
        public int UsageRefId { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public int ActorId { get; set; }
    }
}