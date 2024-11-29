using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.VendorApplications
{
    /// <summary>
    /// Represents a vendor associated customer model
    /// </summary>
    public partial class VendorApplicationAssociatedCustomerModel : BaseNopEntityModel
    {
        #region Properties

        public string Email { get; set; }

        #endregion
    }
}