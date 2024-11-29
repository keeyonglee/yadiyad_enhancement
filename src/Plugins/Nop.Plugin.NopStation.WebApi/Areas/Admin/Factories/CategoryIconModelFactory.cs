using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Models;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Services.Catalog;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Factories
{
    public class CategoryIconModelFactory : ICategoryIconModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ICategoryIconService _categoryIconService;
        private readonly ICategoryService _categoryService;
        private readonly IPictureService _pictureService;
        private readonly WebApiSettings _webApiSettings;

        #endregion

        #region Ctor

        public CategoryIconModelFactory(CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            IDateTimeHelper dateTimeHelper,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ICategoryIconService categoryIconService,
            ICategoryService categoryService,
            IPictureService pictureService,
            WebApiSettings webApiSettings)
        {
            _catalogSettings = catalogSettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _dateTimeHelper = dateTimeHelper;
            _languageService = languageService;
            _localizationService = localizationService;
            _categoryIconService = categoryIconService;
            _categoryService = categoryService;
            _pictureService = pictureService;
            _webApiSettings = webApiSettings;
        }

        #endregion

        #region Utilities

        protected void PrepareAvailableCategories(IList<SelectListItem> items, bool excludeDefaultItem = false)
        {
            var categories = _categoryService.GetAllCategories();
            foreach (var category in categories)
            {
                items.Add(new SelectListItem()
                {
                    Text = _categoryService.GetFormattedBreadCrumb(category, categories),
                    Value = category.Id.ToString()
                });
            }

            if (!excludeDefaultItem)
                items.Insert(0, new SelectListItem()
                {
                    Text = _localizationService.GetResource("Admin.Common.All"),
                    Value = "0"
                });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare categoryIcon search model
        /// </summary>
        /// <param name="searchModel">CategoryIcon search model</param>
        /// <returns>CategoryIcon search model</returns>
        public virtual CategoryIconSearchModel PrepareCategoryIconSearchModel(CategoryIconSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged categoryIcon list model
        /// </summary>
        /// <param name="searchModel">CategoryIcon search model</param>
        /// <returns>CategoryIcon list model</returns>
        public virtual CategoryIconListModel PrepareCategoryIconListModel(CategoryIconSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get categoryIcons
            var categories = _categoryService.GetAllCategories(categoryName: searchModel.SearchCategoryName,
                showHidden: true,
                storeId: searchModel.SearchStoreId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var categoryIcons = _categoryIconService.GetAllCategoryIcons(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new CategoryIconListModel().PrepareToGrid(searchModel, categories, () =>
            {
                return categories.Select(category =>
                {
                    //fill in model values from the entity
                    return PrepareCategoryIconModel(null, category);
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare categoryIcon model
        /// </summary>
        /// <param name="model">CategoryIcon model</param>
        /// <param name="categoryIcon">CategoryIcon</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>CategoryIcon model</returns>
        public virtual CategoryIconModel PrepareCategoryIconModel(CategoryIconModel model, Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (model == null)
            {
                model = new CategoryIconModel();
                var categoryIcon = _categoryIconService.GetCategoryIconByCategoryId(category.Id);
                Picture picture = null;
                Picture banner = null;
                if (categoryIcon != null)
                {
                    model = categoryIcon.ToModel<CategoryIconModel>();
                    picture = _pictureService.GetPictureById(categoryIcon.PictureId);
                    banner = _pictureService.GetPictureById(categoryIcon.CategoryBannerId);
                }

                if(picture == null)
                    picture = _pictureService.GetPictureById(_webApiSettings.DefaultCategoryIcon);
                if(banner == null)
                    banner = _pictureService.GetPictureById(_webApiSettings.DefaultCategoryIcon);

                model.CategoryId = category.Id;
                model.CategoryName = _categoryService.GetFormattedBreadCrumb(category);
                model.PictureUrl = _pictureService.GetPictureUrl(ref picture, 25);
                model.CategoryBannerUrl = _pictureService.GetPictureUrl(ref banner, 25);
            }

            return model;
        }

        #endregion
    }
}
