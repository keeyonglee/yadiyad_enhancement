using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Pro.Models.AdminDashboard
{
    public class TopOrganizationsModel : BaseNopEntityModel
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ContactPersonPhone { get; set; }
        public int JobPostCount { get; set; }
        public decimal JobEngagementAmount { get; set; }
        public int TotalCandidateHired { get; set; }
        public string JobEngagementAmountText
        {
            get
            {
                return JobEngagementAmount.ToString("N");
            }
        }
    }
}
