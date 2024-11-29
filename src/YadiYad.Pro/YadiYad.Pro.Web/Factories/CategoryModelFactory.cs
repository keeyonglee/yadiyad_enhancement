using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Services.Common;
using YadiYad.Pro.Web.Infrastructure;
using YadiYad.Pro.Web.Models.Category;

namespace YadiYad.Pro.Web.FactoriesPro
{
    public partial class CategoryModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly JobServiceCategoryService _categoryService;
        private readonly IDiscountService _discountService;
        private readonly IDiscountSupportedModelFactory _discountSupportedModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IProductService _productService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly BaseAdminModelFactory _baseAdminModelFactory;

        #endregion

        #region Ctor

        public CategoryModelFactory(CatalogSettings catalogSettings,
            IAclSupportedModelFactory aclSupportedModelFactory,
            JobServiceCategoryService categoryService,
            IDiscountService discountService,
            IDiscountSupportedModelFactory discountSupportedModelFactory,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IProductService productService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IUrlRecordService urlRecordService,
            BaseAdminModelFactory baseAdminModelFactory)
        {
            _catalogSettings = catalogSettings;
            _aclSupportedModelFactory = aclSupportedModelFactory;
            _categoryService = categoryService;
            _discountService = discountService;
            _discountSupportedModelFactory = discountSupportedModelFactory;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _productService = productService;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _urlRecordService = urlRecordService;
            _baseAdminModelFactory = baseAdminModelFactory;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare category product search model
        /// </summary>
        /// <param name="searchModel">Category product search model</param>
        /// <param name="category">Category</param>
        /// <returns>Category product search model</returns>
        protected virtual CategoryProductSearchModel PrepareCategoryProductSearchModel(CategoryProductSearchModel searchModel, Category category)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (category == null)
                throw new ArgumentNullException(nameof(category));

            searchModel.CategoryId = category.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare category search model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>Category search model</returns>
        public virtual CategorySearchModel PrepareCategorySearchModel(CategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare "published" filter (0 - all; 1 - published only; 2 - unpublished only)
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "0",
                Text = _localizationService.GetResource("Admin.Catalog.Categories.List.SearchPublished.All")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = _localizationService.GetResource("Admin.Catalog.Categories.List.SearchPublished.PublishedOnly")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = _localizationService.GetResource("Admin.Catalog.Categories.List.SearchPublished.UnpublishedOnly")
            });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged category list model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>Category list model</returns>
        public virtual JobServiceCategoryListModel PrepareCategoryListModel(CategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            //get categories
            var categories = _categoryService.GetAllJobServiceCategoris();

            //prepare grid model
            var model = new JobServiceCategoryListModel().PrepareToGrid(searchModel, categories, () =>
            {
                return categories.Select(category =>
                {
                    //fill in model values from the entity
                    var categoryModel = category.ToModel<JobServiceCategoryModel>();


                    return categoryModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="model">Category model</param>
        /// <param name="category">Category</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Category model</returns>
        public virtual JobServiceCategoryModel PrepareCategoryModel(JobServiceCategoryModel model, JobServiceCategory category, bool excludeProperties = false)
        {
            Action<CategoryLocalizedModel, int> localizedModelConfiguration = null;

            if (category != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = category.ToModel<JobServiceCategoryModel>();
                }
            }
            return model;
        }

        #endregion
    }
}
