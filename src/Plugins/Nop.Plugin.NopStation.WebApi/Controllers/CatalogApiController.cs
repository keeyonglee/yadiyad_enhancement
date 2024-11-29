using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Services.Media;
using Nop.Web.Models.Media;
using YadiYad.Pro.Web.DTO.Base;
using Nop.Plugin.NopStation.WebApi.Models.Catalog;

namespace Nop.Plugin.NopStation.WebApi.Controllers
{
    [Route("api/catalog")]
    public class CatalogApiController : BaseApiController
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly ICatalogModelFactory _catalogModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPermissionService _permissionService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IVendorService _vendorService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly ICategoryIconService _categoryIconService;
        private readonly IPictureService _pictureService;

        #endregion

        #region Ctor

        public CatalogApiController(CatalogSettings catalogSettings,
            IAclService aclService,
            ICatalogModelFactory catalogModelFactory,
            ICategoryService categoryService,
            ICustomerActivityService customerActivityService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IPermissionService permissionService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IProductTagService productTagService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            VendorSettings vendorSettings,
            ICategoryIconService categoryIconService,
            IPictureService pictureService)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _catalogModelFactory = catalogModelFactory;
            _categoryService = categoryService;
            _customerActivityService = customerActivityService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _permissionService = permissionService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _productTagService = productTagService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _vendorService = vendorService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _vendorSettings = vendorSettings;
            _categoryIconService = categoryIconService;
            _pictureService = pictureService;
        }

        #endregion

        #region Action methods

        #region Categories

        [HttpGet("categories")]
        public IActionResult Categories(int currentCategoryId, int currentProductId)
        {
            var response = new GenericResponseModel<CategoryNavigationModel>();
            response.Data = _catalogModelFactory.PrepareCategoryNavigationModel(currentCategoryId, currentProductId);
            return Ok(response);
        }

        [HttpGet("homepagecategories")]
        public IActionResult HomepageCategories()
        {
            var response = new GenericResponseModel<List<CategoryModel>>();
            response.Data = _catalogModelFactory.PrepareHomepageCategoryModels();
            return Ok(response);
        }

        [HttpGet("category")]
        public IActionResult Category(int[] categoryId, CatalogPagingFilteringModel queryModel)
        {
            var response = new GenericResponseModel<CategoryModel>();

            List<int> categoryToSearch = new List<int>();

            if (categoryId.Length == 0)
                return NotFound();

            foreach (var cat in categoryId)
                categoryToSearch.Add(cat);

            var category = new Category();

            if (_productService.CheckIfIsEats(categoryToSearch[0]))
                category = _categoryService.GetCategoryById(_vendorSettings.ShuqEatsCategoryId);
            else if (_productService.CheckIfIsMart(categoryToSearch[0]))
                category = _categoryService.GetCategoryById(_vendorSettings.ShuqMartCategoryId);

            if (category == null || category.Deleted)
                return NotFound();

            var command = queryModel;

            var notAvailable =
                //published?
                !category.Published ||
                //ACL (access control list) 
                !_aclService.Authorize(category) ||
                //Store mapping
                !_storeMappingService.Authorize(category);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageCategories);
            if (notAvailable && !hasAdminAccess)
                return BadRequest();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewCategory",
                string.Format(_localizationService.GetResource("ActivityLog.PublicStore.ViewCategory"), category.Name), category);

            var categoryModel = _catalogModelFactory.PrepareCategoryModel(category, command, categoryToSearch);

            var categoryIcon = _categoryIconService.GetCategoryIconByCategoryId(category.Id);
            if (categoryIcon != null)
            {
                var banner = _pictureService.GetPictureById(categoryIcon.CategoryBannerId);
                var pictureModel = new PictureModel
                {
                    FullSizeImageUrl = _pictureService.GetPictureUrl(ref banner),
                    ImageUrl = _pictureService.GetPictureUrl(ref banner, _mediaSettings.CategoryThumbPictureSize),
                    Title = string.Format(
                                    _localizationService.GetResource("NopStation.WebApi.Category.CategoryIcons.ImageLinkTitleFormat"),
                                    category.Name),
                    AlternateText = string.Format(
                                        _localizationService.GetResource("NopStation.WebApi.Category.CategoryIcons.ImageAlternateTextFormat"),
                                        category.Name)
                };
                categoryModel.PictureModel = pictureModel;
            }

            response.Data = categoryModel;
            

            return Ok(response);
        }

        #endregion

        #region Manufacturers

        [HttpGet("manufacturer/{manufacturerId}")]
        public virtual IActionResult Manufacturer(int manufacturerId, CatalogPagingFilteringModel queryModel)
        {
            var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);
            if (manufacturer == null || manufacturer.Deleted)
                return NotFound();

            var command = queryModel;

            var notAvailable =
                //published?
                !manufacturer.Published ||
                //ACL (access control list) 
                !_aclService.Authorize(manufacturer) ||
                //Store mapping
                !_storeMappingService.Authorize(manufacturer);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers);
            if (notAvailable && !hasAdminAccess)
                return BadRequest();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewManufacturer",
                string.Format(_localizationService.GetResource("ActivityLog.PublicStore.ViewManufacturer"), manufacturer.Name), manufacturer);

            var response = new GenericResponseModel<ManufacturerModel>();
            response.Data = _catalogModelFactory.PrepareManufacturerModel(manufacturer, command);
            return Ok(response);
        }

        [HttpGet("manufacturer/all")]
        public virtual IActionResult ManufacturerAll()
        {
            var response = new GenericResponseModel<List<ManufacturerModel>>();
            response.Data = _catalogModelFactory.PrepareManufacturerAllModels();
            return Ok(response);
        }

        #endregion

        #region Vendors

        [HttpGet("vendor/{vendorId}")]
        public virtual IActionResult Vendor(int vendorId, CatalogPagingFilteringModel queryModel)
        {
            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null || vendor.Deleted || !vendor.Active)
                return NotFound();

            var command = queryModel;

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            var response = new GenericResponseModel<VendorModel>();
            //model
            response.Data = _catalogModelFactory.PrepareVendorModel(vendor, command);
            return Ok(response);
        }

        [HttpGet("vendor/all")]
        public virtual IActionResult VendorAll()
        {
            var response = new GenericResponseModel<List<VendorModel>>();
            //we don't allow viewing of vendors if "vendors" block is hidden
            if (_vendorSettings.VendorsBlockItemsToDisplay == 0)
                return Ok(response);

            response.Data = _catalogModelFactory.PrepareVendorAllModels();
            return Ok(response);
        }

        #endregion

        #region Product

        [HttpGet("homepageproducts")]
        public IActionResult HomepageProducts(int? productThumbPictureSize)
        {

            var products = _productService.GetAllProductsDisplayedOnHomepage();
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();

            products = products.Where(p => p.VisibleIndividually).ToList();

            if (!products.Any())
                return Content("");

            var model = _productModelFactory.PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();

            var response = new GenericResponseModel<List<ProductOverviewModel>>();
            response.Data = model;
            return Ok(response);
        }

        [HttpGet("producttag/{productTagId}")]
        public virtual IActionResult ProductsByTag(int productTagId, CatalogPagingFilteringModel queryModel)
        {
            var productTag = _productTagService.GetProductTagById(productTagId);
            if (productTag == null)
                return NotFound();

            var command = queryModel;

            var response = new GenericResponseModel<ProductsByTagModel>();
            response.Data = _catalogModelFactory.PrepareProductsByTagModel(productTag, command);
            return Ok(response);
        }

        [HttpGet("producttag/all")]
        public virtual IActionResult ProductTagsAll()
        {
            var response = new GenericResponseModel<PopularProductTagsModel>();
            response.Data = _catalogModelFactory.PrepareProductTagsAllModel();
            return Ok(response);
        }

        #endregion

        #region Searching

        [HttpGet("search")]
        public virtual IActionResult Search(SearchModel model, CatalogPagingFilteringModel command)
        {
            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(true),
                _storeContext.CurrentStore.Id);

            var response = new GenericResponseModel<SearchModel>();
            response.Data = _catalogModelFactory.PrepareSearchModel(model, command);
            return Ok(response);
        }

        [HttpGet("catalog/searchtermautocomplete")]
        public virtual IActionResult SearchTermAutoComplete(string term, ShuqBusinessNatureEnum category)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < _catalogSettings.ProductSearchTermMinimumLength)
                return LengthRequired();

            //products
            var productNumber = _catalogSettings.ProductSearchAutoCompleteNumberOfProducts > 0 ?
                _catalogSettings.ProductSearchAutoCompleteNumberOfProducts : 10;

            var products = _productService.SearchProducts(
                storeId: _storeContext.CurrentStore.Id,
                keywords: term,
                category: category,
                languageId: _workContext.WorkingLanguage.Id,
                visibleIndividuallyOnly: true,
                pageSize: productNumber);

            var showLinkToResultSearch = _catalogSettings.ShowLinkToAllResultInSearchAutoComplete && (products.TotalCount > productNumber);

            var models = _productModelFactory.PrepareProductOverviewModels(products, false, _catalogSettings.ShowProductImagesInSearchAutoComplete, _mediaSettings.AutoCompleteSearchThumbPictureSize).ToList();

            var response = new GenericResponseModel<object>();

            response.Data = (from p in models
                             select new
                             {
                                 label = p.Name,
                                 productid = p.Id,
                                 productpictureurl = p.DefaultPictureModel.ImageUrl,
                                 showlinktoresultsearch = showLinkToResultSearch,
                                 isEats = p.IsEats,
                                 isMart = p.IsMart
                             })
                .ToList();
            return Ok(response);
        }

        #endregion

        #endregion
    }
}
