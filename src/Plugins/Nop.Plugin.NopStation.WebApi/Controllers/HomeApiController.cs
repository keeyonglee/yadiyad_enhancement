using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Services.Orders;
using Nop.Services.Caching;
using Nop.Core.Caching;
using Nop.Plugin.NopStation.WebApi.Factories;
using Nop.Plugin.NopStation.WebApi.Infrastructure.Cache;
using Nop.Services.Media;
using Nop.Services.Localization;
using System;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.NopStation.WebApi.Models.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Media;
using YadiYad.Pro.Web.DTO.Base;
using Nop.Web.Models.Blogs;

namespace Nop.Plugin.NopStation.WebApi.Controllers
{
    [Route("api/home")]
    public class HomeApiController : BaseApiController
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IOrderReportService _orderReportService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly WebApiSettings _webApiSettings;
        private readonly ICatalogApiModelFactory _catalogApiModelFactory;
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly ICommonApiModelFactory _commonApiModelFactory;
        private readonly IWorkContext _workContext;
        private readonly IPictureService _pictureService;
        private readonly ILanguageService _languageService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly VendorSettings _vendorSettings;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly OrderSettings _orderSettings;
        private readonly IOrderService _orderService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly CatalogSettings _catalogSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly IBlogModelFactory _blogModelFactory;
        private readonly MediaSettings _mediaSettings;
        private readonly ShuqHomepageSettings _shuqHomepageSettings;

        #endregion

        #region Ctor

        public HomeApiController(IAclService aclService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IStoreMappingService storeMappingService,
            IOrderReportService orderReportService,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            WebApiSettings webApiSettings,
            ICatalogApiModelFactory catalogApiModelFactory,
            ICommonModelFactory commonModelFactory,
            ICommonApiModelFactory commonApiModelFactory,
            IPictureService pictureService,
            ILanguageService languageService,
            IWorkContext workContext,
            IShoppingCartService shoppingCartService,
            VendorSettings vendorSettings,
            ICacheKeyService cacheKeyService,
            OrderSettings orderSettings,
            CustomerSettings customerSettings,
            IReturnRequestService returnRequestService,
            CatalogSettings catalogSettings,
            StoreInformationSettings storeInformationSettings,
            IBlogModelFactory blogModelFactory,
            MediaSettings mediaSettings,
            ShuqHomepageSettings shuqHomepageSettings)
        {
            _aclService = aclService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _storeMappingService = storeMappingService;
            _orderReportService = orderReportService;
            _cacheManager = cacheManager;
            _storeContext = storeContext;
            _webApiSettings = webApiSettings;
            _catalogApiModelFactory = catalogApiModelFactory;
            _commonModelFactory = commonModelFactory;
            _commonApiModelFactory = commonApiModelFactory;
            _pictureService = pictureService;
            _languageService = languageService;
            _workContext = workContext;
            _shoppingCartService = shoppingCartService;
            _vendorSettings = vendorSettings;
            _cacheKeyService = cacheKeyService;
            _orderSettings = orderSettings;
            _customerSettings = customerSettings;
            _returnRequestService = returnRequestService;
            _catalogSettings = catalogSettings;
            _storeInformationSettings = storeInformationSettings;
            _blogModelFactory = blogModelFactory;
            _mediaSettings = mediaSettings;
            _shuqHomepageSettings = shuqHomepageSettings;
        }

        #endregion

        #region Utilities

        protected virtual Language EnsureLanguageIsActive(Language language)
        {
            var storeId = _storeContext.CurrentStore.Id;

            if (language == null || !language.Published)
            {
                //load any language from the specified store
                language = _languageService.GetAllLanguages(storeId: storeId).FirstOrDefault();
            }

            if (language == null || !language.Published)
            {
                //load any language
                language = _languageService.GetAllLanguages().FirstOrDefault();
            }

            if (language == null)
                throw new Exception("No active language could be loaded");

            return language;
        }

        #endregion

        #region Methods

        [HttpGet("applandingsetting")]
        public virtual IActionResult AppLandingSetting(bool appStart)
        {
            var response = new GenericResponseModel<AppConfigurationModel>();

            var model = new AppConfigurationModel
            {
                PrimaryThemeColor = _webApiSettings.PrimaryThemeColor,
                BottomBarActiveColor = _webApiSettings.BottomBarActiveColor,
                BottomBarInactiveColor = _webApiSettings.BottomBarInactiveColor,
                BottomBarBackgroundColor = _webApiSettings.BottomBarBackgroundColor,
                TopBarTextColor = _webApiSettings.TopBarTextColor,
                TopBarBackgroundColor = _webApiSettings.TopBarBackgroundColor,
                GradientStartingColor = _webApiSettings.GradientStartingColor,
                GradientMiddleColor = _webApiSettings.GradientMiddleColor,
                GradientEndingColor = _webApiSettings.GradientEndingColor,
                GradientEnabled = _webApiSettings.GradientEnabled,
                IOSProductPriceTextSize = _webApiSettings.IOSProductPriceTextSize,
                AndroidProductPriceTextSize = _webApiSettings.AndroidProductPriceTextSize,
                IonicProductPriceTextSize = _webApiSettings.IonicProductPriceTextSize,
                ShowHomepageSlider = _webApiSettings.ShowHomepageSlider,
                SliderAutoPlay = _webApiSettings.SliderAutoPlay,
                SliderAutoPlayTimeout = _webApiSettings.SliderAutoPlayTimeout,
                ShowFeaturedProducts = _webApiSettings.ShowFeaturedProducts,
                ShowBestsellersOnHomepage = _webApiSettings.ShowBestsellersOnHomepage && _webApiSettings.NumberOfBestsellersOnHomepage > 0,
                ShowHomepageCategoryProducts = _webApiSettings.ShowHomepageCategoryProducts,
                ShowManufacturers = _webApiSettings.ShowManufacturers,
                AndriodForceUpdate = _webApiSettings.AndriodForceUpdate,
                AndroidVersion = _webApiSettings.AndroidVersion,
                PlayStoreUrl = _webApiSettings.PlayStoreUrl,
                IOSForceUpdate = _webApiSettings.IOSForceUpdate,
                IOSVersion = _webApiSettings.IOSVersion,
                AppStoreUrl = _webApiSettings.AppStoreUrl,
                ShowAllVendors = _vendorSettings.VendorsBlockItemsToDisplay > 0,
                LogoUrl = _pictureService.GetPictureUrl(_webApiSettings.LogoId, _webApiSettings.LogoSize),
                AnonymousCheckoutAllowed = _orderSettings.AnonymousCheckoutAllowed,
                ShowChangeBaseUrlPanel = _webApiSettings.ShowChangeBaseUrlPanel,
                HasReturnRequests = _orderSettings.ReturnRequestsEnabled &&
                    _returnRequestService.SearchReturnRequests(_storeContext.CurrentStore.Id,
                    _workContext.CurrentCustomer.Id, pageIndex: 0, pageSize: 1).Any(),
                HideDownloadableProducts = _customerSettings.HideDownloadableProductsTab,
                NewProductsEnabled = _catalogSettings.NewProductsEnabled,
                RecentlyViewedProductsEnabled = _catalogSettings.RecentlyViewedProductsEnabled,
                CompareProductsEnabled = _catalogSettings.CompareProductsEnabled,
                AllowCustomersToUploadAvatars = _customerSettings.AllowCustomersToUploadAvatars,
                AvatarMaximumSizeBytes = _customerSettings.AvatarMaximumSizeBytes,
                HideBackInStockSubscriptionsTab = _customerSettings.HideBackInStockSubscriptionsTab,
                StoreClosed = _storeInformationSettings.StoreClosed,
                MaxReviewImages = _mediaSettings.MaxReviewImages
            };
            
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, storeId: _storeContext.CurrentStore.Id);
            model.TotalShoppingCartQuantity = cart.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).Sum(item => item.Quantity);
            model.TotalShoppingCartProducts = cart.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).Count();
            model.TotalWishListProducts = cart.Where(x => x.ShoppingCartType == ShoppingCartType.Wishlist).Sum(item => item.Quantity);

            var language = EnsureLanguageIsActive(_workContext.WorkingLanguage);
            model.Rtl = language.Rtl;
            model.StringResources = _commonApiModelFactory.GetStringRsources(language.Id);

            model.LanguageNavSelector = _commonModelFactory.PrepareLanguageSelectorModel();
            model.CurrencyNavSelector = _commonModelFactory.PrepareCurrencySelectorModel();

            response.Data = model;

            return Ok(response);
        }

        [HttpGet("manufacturers")]
        public virtual IActionResult Manufacturers()
        {
            var response = new GenericResponseModel<List<ManufacturerModel>>
            {
                Data = _catalogApiModelFactory.PrepareHomepageManufacturerModels().ToList()
            };

            return Ok(response);
        }

        [HttpGet("blog")]
        public virtual IActionResult Blog()
        {
            var response = new GenericResponseModel<HomepageBlogPostModel>
            {
                Data = _blogModelFactory.PrepareHomepageBlogPostModel()
            };

            return Ok(response);
        }

        [HttpGet("categorytree")]
        public virtual IActionResult CategoryTree()
        {
            var response = new GenericResponseModel<List<CategoryTreeModel>>();
            var model = _catalogApiModelFactory.PrepareCategoryTreeModel();
            response.Data = model.ToList();
            return Ok(response);
        }

        [HttpGet("featureproducts")]
        public virtual IActionResult FeatureProducts(int? productThumbPictureSize)
        {
            var response = new GenericResponseModel<List<ProductOverviewModel>>();
            var model = new List<ProductOverviewModel>();
            var products = _productService.GetAllProductsDisplayedOnHomepage();
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();

            products = products.Where(p => p.VisibleIndividually).ToList();

            model = _productModelFactory.PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();
            response.Data = model;
            return Ok(response);
        }

        [HttpGet("featureproducts/eats")]
        public virtual IActionResult HomeEatsProduct(ListFilterDTO<ProductOverviewModel> searchDTO)
        {
            var response = new ResponseDTO();
            Random rng = new Random();

            var data = _productService.GetInitialEatsProductsDisplayedOnHomepage();
            data = data.Where(p => _aclService.Authorize(p)
                                   && _storeMappingService.Authorize(p)
                                   && _productService.ProductIsAvailable(p)
                                   && p.VisibleIndividually).ToList();

            if (!data.Any())
                return NotFound();

            var model = _productModelFactory.PrepareProductOverviewModels(data);
            model = model.OrderBy(a => rng.Next()).Take(_shuqHomepageSettings.EatsMaxFeaturedProducts); 
            
            response.SetResponse(model);

            return Ok(response);
        }

        [HttpGet("featureproducts/mart")]
        public virtual IActionResult HomeMartProduct(ListFilterDTO<ProductOverviewModel> searchDTO)
        {
            var response = new ResponseDTO();
            Random rng = new Random();

            var data = _productService.GetInitialMartProductsDisplayedOnHomepage();

            if (!data.Any())
                return NotFound();

            var model = _productModelFactory.PrepareProductOverviewModels(data);
            model = model.OrderBy(a => rng.Next()).Take(_shuqHomepageSettings.MartMaxFeaturedProducts); 

            response.SetResponse(model);

            return Ok(response);
        }

        [HttpGet("bestsellerproducts")]
        public virtual IActionResult BestSellerproducts(int? productThumbPictureSize)
        {
            var response = new GenericResponseModel<List<ProductOverviewModel>>();
            var model = new List<ProductOverviewModel>();

            if (!_webApiSettings.ShowBestsellersOnHomepage || _webApiSettings.NumberOfBestsellersOnHomepage <= 0)
            {
                response.Data = model;
                return BadRequest(response);
            }

            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(ApiModelCacheDefaults.HomepageBestsellersIdsKey,
                       _storeContext.CurrentStore.Id);

            //load and cache report
            var report = _cacheManager.Get(cacheKey,
                () => _orderReportService.BestSellersReport(
                        storeId: _storeContext.CurrentStore.Id,
                        pageSize: _webApiSettings.NumberOfBestsellersOnHomepage)
                    .ToList());

            //load products
            var products = _productService.GetProductsByIds(report.Select(x => x.ProductId).ToArray());
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();

            model = _productModelFactory.PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();
            response.Data = model;
            return Ok(response);
        }

        [HttpGet("homepagecategorieswithproducts")]
        public virtual IActionResult HomePageCategoriesWithProducts()
        {
            var response = new GenericResponseModel<List<HomepageCategoryModel>>();
            var model = _catalogApiModelFactory.PrepareHomepageCategoriesWithProductsModel();
            response.Data = model.ToList();
            return Ok(response);
        }

        #endregion
    }
}
