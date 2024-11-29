using Nop.Core.Domain.Vendors;
using Nop.Web.Areas.Admin.Models.VendorApplications;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the vendor model factory
    /// </summary>
    public partial interface IVendorApplicationModelFactory
    {
        /// <summary>
        /// Prepare vendor search model
        /// </summary>
        /// <param name="searchModel">Vendor search model</param>
        /// <returns>Vendor search model</returns>
        VendorApplicationSearchModel PrepareVendorApplicationSearchModel(VendorApplicationSearchModel searchModel);

        /// <summary>
        /// Prepare paged vendor list model
        /// </summary>
        /// <param name="searchModel">Vendor search model</param>
        /// <returns>Vendor list model</returns>
        VendorApplicationListModel PrepareVendorApplicationListModel(VendorApplicationSearchModel searchModel);

        /// <summary>
        /// Prepare vendor model
        /// </summary>
        /// <param name="model">Vendor model</param>
        /// <param name="vendor">Vendor</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Vendor model</returns>
        VendorApplicationModel PrepareVendorApplicationModel(VendorApplicationModel model, VendorApplication vendor, bool excludeProperties = false);


    }
}