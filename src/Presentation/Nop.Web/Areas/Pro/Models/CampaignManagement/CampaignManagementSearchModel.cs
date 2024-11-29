using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Pro.Models.CampaignManagement
{
    public class CampaignManagementSearchModel : BaseSearchModel
    {
        public int Channel { get; set; }
        public int Activity { get; set; }
        [UIHint("DateNullable")]
        public DateTime? From { get; set; }
        public DateTime? Until { get; set; }
        public int TransactionLimit { get; set; }
        public int? EngagementType { get; set; }
        public int Beneficiary { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
    }
}
