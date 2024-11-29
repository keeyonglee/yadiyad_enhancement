using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Factories;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Plugin.NopStation.WebApi.Infrastructure.Cache;
using Nop.Plugin.NopStation.WebApi.Models.Catalog;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;

namespace Nop.Plugin.NopStation.WebApi.Factories
{
    public class CatalogApiModelFactory : ICatalogApiModelFactory
    {
        private readonly IManufacturerService _manufacturerService;
        private readonly ICategoryService _categoryService;
        private readonly MediaSettings _mediaSettings;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;
        private readonly IStoreContext _storeContext;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly WebApiSettings _webApiSettings;
        private readonly IProductService _productService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly ICategoryIconService _categoryIconService;
        private readonly ICacheKeyService _cacheKeyService;

        public CatalogApiModelFactory(IManufacturerService manufacturerService,
            ICategoryService categoryService,
            MediaSettings mediaSettings,
            IWorkContext workContext,
            IWebHelper webHelper,
            IStoreContext storeContext,
            IStaticCacheManager cacheManager,
            ILocalizationService localizationService,
            IUrlRecordService urlRecordService,
            IPictureService pictureService,
            WebApiSettings webApiSettings,
            IProductService productService,
            IProductModelFactory productModelFactory,
            ICategoryIconService categoryIconService,
            ICacheKeyService cacheKeyService)
        {
            _manufacturerService = manufacturerService;
            _categoryService = categoryService;
            _mediaSettings = mediaSettings;
            _workContext = workContext;
            _webHelper = webHelper;
            _storeContext = storeContext;
            _cacheManager = cacheManager;
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            _pictureService = pictureService;
            _webApiSettings = webApiSettings;
            _productService = productService;
            _productModelFactory = productModelFactory;
            _categoryIconService = categoryIconService;
            _cacheKeyService = cacheKeyService;
        }

        #region Utilities

        protected IList<Product> GetProductsByCategoryId(int categoryId)
        {
            var categoryIds = new List<int> { categoryId };
            if (_webApiSettings.ShowSubCategoryProducts)
            {
                categoryIds.AddRange(_categoryService.GetAllCategoriesByParentCategoryId(categoryId).Select(x => x.Id).ToList());
            }

            //products
            var products = _productService.SearchProducts(out _, false,
                categoryIds: categoryIds,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                pageSize: _webApiSettings.NumberOfHomepageCategoryProducts);

            return products;
        }

        protected IList<HomepageCategoryModel> GetSubCategories(int categoryId)
        {
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;
            var subcategories = _categoryService.GetAllCategoriesByParentCategoryId(categoryId)
                .Select(subcategory =>
                {
                    var subCategoryModel = new HomepageCategoryModel
                    {
                        Name = _localizationService.GetLocalized(subcategory, x => x.Name),
                        SeName = _urlRecordService.GetSeName(subcategory),
                        Id = subcategory.Id
                    };
                    return subCategoryModel;
                })
                .ToList();
            return subcategories;
        }

        protected IList<CategoryTreeModel> PrepareCategoryModel(int categoryId, IList<ApiCategoryIcon> categoryIcons)
        {
            var models = new List<CategoryTreeModel>();
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(categoryId);

            foreach (var category in categories)
            {
                var iconId = categoryIcons.FirstOrDefault(x => x.CategoryId == category.Id)?.PictureId ?? _webApiSettings.DefaultCategoryIcon;
                var picture = _pictureService.GetPictureById(category.PictureId);

                models.Add(new CategoryTreeModel()
                {
                    CategoryId = category.Id,
                    Name = _localizationService.GetLocalized(category, x => x.Name),
                    SeName = _urlRecordService.GetSeName(category),
                    IconUrl = _pictureService.GetPictureUrl(ref picture),
                    SubCategories = PrepareCategoryModel(category.Id, categoryIcons)
                });
            }

            return models;
        }

        #endregion

        public IList<CategoryTreeModel> PrepareCategoryTreeModel()
        {
            var categoryIcons = _categoryIconService.GetAllCategoryIcons();
            var categories = PrepareCategoryModel(0, categoryIcons);
            return categories;
        }

        public IList<HomepageCategoryModel> PrepareHomepageCategoriesWithProductsModel()
        {
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;

            var model = _categoryService.GetAllCategoriesDisplayedOnHomepage()
                .Select(category =>
                {
                    var catModel = new HomepageCategoryModel
                    {
                        Id = category.Id,
                        Name = _localizationService.GetLocalized(category, x => x.Name),
                        SeName = _urlRecordService.GetSeName(category)
                    };

                    var products = GetProductsByCategoryId(category.Id);
                    catModel.Products = _productModelFactory.PrepareProductOverviewModels(products, true, true, null).ToList();
                    catModel.SubCategories = GetSubCategories(category.Id);
                    return catModel;
                })
                .ToList();

            return model;
        }

        public IList<ManufacturerModel> PrepareHomepageManufacturerModels()
        {
            var model = new List<ManufacturerModel>();
            var manufacturers = _manufacturerService.GetAllManufacturers(storeId: _storeContext.CurrentStore.Id, pageSize: _webApiSettings.NumberOfManufacturers);
            foreach (var manufacturer in manufacturers)
            {
                var modelMan = new ManufacturerModel
                {
                    Id = manufacturer.Id,
                    Name = _localizationService.GetLocalized(manufacturer, x => x.Name),
                    Description = _localizationService.GetLocalized(manufacturer, x => x.Description),
                    MetaKeywords = _localizationService.GetLocalized(manufacturer, x => x.MetaKeywords),
                    MetaDescription = _localizationService.GetLocalized(manufacturer, x => x.MetaDescription),
                    MetaTitle = _localizationService.GetLocalized(manufacturer, x => x.MetaTitle),
                    SeName = _urlRecordService.GetSeName(manufacturer),
                };

                //prepare picture model
                var pictureSize = _mediaSettings.ManufacturerThumbPictureSize;

                var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(ApiModelCacheDefaults.ManufacturerPictureModelKey,
                       manufacturer.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), 
                       _storeContext.CurrentStore.Id);
                
                modelMan.PictureModel = _cacheManager.Get(cacheKey, () =>
                {
                    var picture = _pictureService.GetPictureById(manufacturer.PictureId);
                    var pictureModel = new PictureModel
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(ref picture),
                        ImageUrl = _pictureService.GetPictureUrl(ref picture, pictureSize),
                        Title = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageLinkTitleFormat"), modelMan.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageAlternateTextFormat"), modelMan.Name)
                    };
                    return pictureModel;
                });
                model.Add(modelMan);
            }

            return model;
        }
    }
}
