using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.VendorApplications
{
    /// <summary>
    /// Represents a vendor search model
    /// </summary>
    public partial class VendorApplicationSearchModel : BaseSearchModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Vendors.List.SearchName")]
        public string SearchName { get; set; }

        [NopResourceDisplayName("Admin.Vendors.List.IsApproved")]
        public bool IsApproved { get; set; }

        #endregion
    }
}