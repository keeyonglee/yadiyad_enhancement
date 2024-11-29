using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Vendors
{
    public partial class VendorStatisticsModel : BaseNopModel
    {
        public int NumberOfOrdersLastMonth { get; set; }

        public int NumberOfOrdersThisMonth { get; set; }
        public int NumberOfOrdersThisWeek { get; set; }
        public int NumberOfOrdersLastWeek { get; set; }
    }
}