using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Campaign;

namespace Nop.Web.Areas.Pro.Models.CampaignManagement
{
    public class CampaignManagementModel : BaseNopEntityModel
    {
        public string Title { get; set; }
        public int Channel { get; set; }
        public int Activity { get; set; }
        public int ActivityOrganization { get; set; }
        public int ActivityIndividual { get; set; }
        [UIHint("DateNullable")]
        public DateTime? From { get; set; }
        [UIHint("DateNullable")]
        public DateTime? Until { get; set; }
        public string FromText { get; set; }
        public string UntilText { get; set; }
        public int TransactionLimit { get; set; }
        public int? EngagementType { get; set; }
        public int Beneficiary { get; set; }
        public int Type { get; set; }
        public decimal Value1 { get; set; }
        public decimal Value2 { get; set; }
        public DateTime CreatedOnUTC { get; set; }

        public string ChannelText
        {
            get
            {
                var text = Channel != 0 ? ((CampaignChannel)Channel).GetDescription() : "";
                return text;
            }
        }
        public string ActivityText
        {
            get
            {
                var text = "";
                if (Beneficiary == 1)
                {
                    text = Activity != 0 ? ((CampaignActivityOrganization)Activity).GetDescription() : "";
                }
                else
                {
                    text = Activity != 0 ? ((CampaignActivityIndividual)Activity).GetDescription() : "";
                }
                return text;
            }
        }
        public string EngagementTypeText
        {
            get
            {
                var text = (EngagementType??0) != 0 ? ((CampaignEngagementType)EngagementType).GetDescription() : "";
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
                var text = Type != 0 ? ((CampaignType)Type).GetDescription() : "";
                return text;
            }
        }
    }
}
