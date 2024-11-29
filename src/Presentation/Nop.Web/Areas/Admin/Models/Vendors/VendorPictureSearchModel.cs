using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Vendors
{
    /// <summary>
    /// Represents a vendor note search model
    /// </summary>
    public partial class VendorPictureSearchModel : BaseSearchModel
    {
        #region Properties

        public int VendorId { get; set; }
        
        #endregion
    }
}