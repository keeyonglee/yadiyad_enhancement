using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Services.DTO.Engagement
{
    public class EngagementDTO
    {
        public string EngagementNo { get; set; }
        public ProductType ProductType { get; set; }
        public int BuyerCustomerId { get; set; }
        public int SellerCustomerId { get; set; }
        public int? ModeratorCustomerId { get; set; }
        public bool IsProjectPayout { get; set; } = false;
        public int? EndMilestoneId { get; set; }
        public int? LastMilestoneId { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
