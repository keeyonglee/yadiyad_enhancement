using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a product picture search model
    /// </summary>
    public partial class SellerDisputePictureSearchModel : BaseSearchModel
    {
        #region Properties

        public int GroupReturnRequestId { get; set; }
        
        #endregion
    }
}