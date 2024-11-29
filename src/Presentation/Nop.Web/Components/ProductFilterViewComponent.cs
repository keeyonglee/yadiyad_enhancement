using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Components
{
    public class ProductFilterViewComponent : NopViewComponent
    {
        private readonly ICatalogModelFactory _catalogModelFactory;

        public ProductFilterViewComponent(ICatalogModelFactory catalogModelFactory)
        {
            _catalogModelFactory = catalogModelFactory;
        }

        public IViewComponentResult Invoke(SearchModel searchModel, int filteredCategoryId, int startCategoryLevel)
        {
            var model = new ProductFilterViewModel
            {
                SearchModel = searchModel,
                CategoryNavigationModel = _catalogModelFactory.PrepareFilteredCategoryNavigationModel(filteredCategoryId, startCategoryLevel)
            };

            return View(model);
        }
    }
}
