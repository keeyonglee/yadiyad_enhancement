using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.VendorApplications
{
    /// <summary>
    /// Represents a vendor attribute value search model
    /// </summary>
    public partial class VendorApplicationAttributeValueSearchModel : BaseSearchModel
    {
        #region Properties

        public int VendorApplicationAttributeId { get; set; }

        #endregion
    }
}