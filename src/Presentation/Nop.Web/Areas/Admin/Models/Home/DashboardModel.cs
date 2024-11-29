using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Models.Reports;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Home
{
    /// <summary>
    /// Represents a dashboard model
    /// </summary>
    public partial class DashboardModel : BaseNopModel
    {
        #region Ctor

        public DashboardModel()
        {
            PopularSearchTerms = new PopularSearchTermSearchModel();
            BestsellersByAmount = new BestsellerBriefSearchModel();
            BestsellersByQuantity = new BestsellerBriefSearchModel();
            OrderSearchModel = new OrderSearchModel();
        }

        #endregion

        #region Properties

        public bool IsLoggedInAsVendor { get; set; }

        public PopularSearchTermSearchModel PopularSearchTerms { get; set; }

        public BestsellerBriefSearchModel BestsellersByAmount { get; set; }

        public BestsellerBriefSearchModel BestsellersByQuantity { get; set; }
        public OrderSearchModel OrderSearchModel { get; set; }

        #endregion
    }
}