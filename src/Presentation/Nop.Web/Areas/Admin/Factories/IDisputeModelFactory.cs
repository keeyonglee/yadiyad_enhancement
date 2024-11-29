using Nop.Core.Domain.Orders;
using Nop.Web.Areas.Admin.Models.Orders;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the return request model factory
    /// </summary>
    public partial interface IDisputeModelFactory
    {
        DisputeSearchModel PrepareDisputeSearchModel(DisputeSearchModel searchModel);
        DisputeListModel PrepareDisputeListModel(DisputeSearchModel searchModel);
        SellerDisputePictureListModel PrepareSellerDisputePictureListModel(SellerDisputePictureSearchModel searchModel);
    }
}