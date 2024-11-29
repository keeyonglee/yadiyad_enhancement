using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Campaign
{
    public class CampaignManagement : BaseEntityExtension
    {
        public string Title { get; set; }
        public int Channel { get; set; }
        public int Activity { get; set; }
        public DateTime From { get; set; }
        public DateTime? Until { get; set; }
        public int TransactionLimit { get; set; }
        public int? EngagementType { get; set; }
        public int Beneficiary { get; set; }
        public int Type { get; set; }
        public decimal Value1 { get; set; }
        public decimal Value2 { get; set; }
    }
}
