using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Factories;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Models;
using Nop.Services.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Components
{
    public class WebApiCategoryIconAdminViewComponent : NopViewComponent
    {
        private readonly INopStationLicenseService _licenseService;
        private readonly ICategoryService _categoryService;
        private readonly ICategoryIconModelFactory _categoryIconModelFactory;

        public WebApiCategoryIconAdminViewComponent(INopStationLicenseService licenseService,
            ICategoryService categoryService,
            ICategoryIconModelFactory categoryIconModelFactory)
        {
            _licenseService = licenseService;
            _categoryService = categoryService;
            _categoryIconModelFactory = categoryIconModelFactory;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (!_licenseService.IsLicensed())
                return Content("");

            if (additionalData.GetType() != typeof(CategoryModel))
                return Content("");

            var categoryModel = additionalData as CategoryModel;

            var category = _categoryService.GetCategoryById(categoryModel.Id);
            if (category == null || category.Deleted)
                return View(new CategoryIconModel());

            var model = _categoryIconModelFactory.PrepareCategoryIconModel(null, category);
            return View(model);
        }
    }
}
