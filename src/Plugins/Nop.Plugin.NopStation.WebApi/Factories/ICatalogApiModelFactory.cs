using System.Collections.Generic;
using Nop.Plugin.NopStation.WebApi.Models.Catalog;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.NopStation.WebApi.Factories
{
    public interface ICatalogApiModelFactory
    {
        IList<CategoryTreeModel> PrepareCategoryTreeModel();

        IList<HomepageCategoryModel> PrepareHomepageCategoriesWithProductsModel();

        IList<ManufacturerModel> PrepareHomepageManufacturerModels();
    }
}
