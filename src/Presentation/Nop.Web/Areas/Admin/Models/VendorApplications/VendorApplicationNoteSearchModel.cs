using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.VendorApplications
{
    /// <summary>
    /// Represents a vendor note search model
    /// </summary>
    public partial class VendorApplicationNoteSearchModel : BaseSearchModel
    {
        #region Properties

        public int VendorApplicationId { get; set; }
        
        #endregion
    }
}