using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Campaign;

namespace YadiYad.Pro.Services.DTO.Campaign
{
    public class CampaignManagementDTO 
    {
        public int Channel { get; set; }
        public int Activity { get; set; }
        public DateTime From { get; set; }
        public DateTime? Until { get; set; }
        public int TransactionLimit { get; set; }
        public int EngagementType { get; set; }
        public int Beneficiary { get; set; }
        public int Type { get; set; }
        public decimal Value1 { get; set; }
        public decimal Value2 { get; set; }
        public string ChannelText
        {
            get
            {
                var text = Channel != 0 ? ((CampaignChannel)Channel).GetDescription() : "";
                return text;
            }
        }
        //public string ActivityText
        //{
        //    get
        //    {
        //        var text = "";
        //        if (Beneficiary == 1)
        //        {
        //            text = Activity != 0 ? ((CampaignActivityOrganization)Activity).GetDescription() : "";
        //        }
        //        else
        //        {
        //            text = Activity != 0 ? ((CampaignActivityIndividual)Activity).GetDescription() : "";
        //        }
        //        return text;
        //    }
        //}
        public string EngagementTypeText
        {
            get
            {
                var text = (EngagementType != 0) ? ((CampaignEngagementType)EngagementType).GetDescription() : "";
                return text;
            }
        }
        public string BeneficiaryText
        {
            get
            {
                var text = Beneficiary != 0 ? ((CampaignBeneficiary)Beneficiary).GetDescription() : "" ;
                return text;
            }
        }
        public string TypeText
        {
            get
            {
                var text = Type != 0 ? ((CampaignBeneficiary)Beneficiary).GetDescription() : "" ;
                return text;
            }
        }
    }
}
