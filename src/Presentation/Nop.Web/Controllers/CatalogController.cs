using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
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
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;

namespace Nop.Web.Controllers
{
    public partial class CatalogController : BasePublicController
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
        private readonly IPictureService _pictureService;
        private readonly IVendorAttributeService _vendorAttributeService;
        private readonly MediaSettings _mediaSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public CatalogController(CatalogSettings catalogSettings,
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
            IPictureService pictureService,
            IVendorAttributeService vendorAttributeService,
            MediaSettings mediaSettings,
            VendorSettings vendorSettings)
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
            _pictureService = pictureService;
            _mediaSettings = mediaSettings;
            _vendorSettings = vendorSettings;
            _vendorAttributeService = vendorAttributeService;
        }

        #endregion
        
        #region Categories
        
        public virtual IActionResult Category(int categoryId, CatalogPagingFilteringModel command)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null || category.Deleted)
                return InvokeHttp404();

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
                return InvokeHttp404();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, 
                NopCustomerDefaults.LastContinueShoppingPageAttribute, 
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                DisplayEditLink(Url.Action("Edit", "Category", new { id = category.Id, area = AreaNames.Admin }));

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewCategory",
                string.Format(_localizationService.GetResource("ActivityLog.PublicStore.ViewCategory"), category.Name), category);

            //model
            var model = _catalogModelFactory.PrepareCategoryModel(category, command);

            //template
            var templateViewPath = _catalogModelFactory.PrepareCategoryTemplateViewPath(category.CategoryTemplateId);
            return View(templateViewPath, model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual IActionResult GetCatalogRoot()
        {
            var model = _catalogModelFactory.PrepareRootCategories();

            return Json(model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual IActionResult GetCatalogSubCategories(int id)
        {
            var model = _catalogModelFactory.PrepareSubCategories(id);

            return Json(model);
        }

        #endregion

        #region Manufacturers

        public virtual IActionResult Manufacturer(int manufacturerId, CatalogPagingFilteringModel command)
        {
            var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);
            if (manufacturer == null || manufacturer.Deleted)
                return InvokeHttp404();

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
                return InvokeHttp404();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, 
                NopCustomerDefaults.LastContinueShoppingPageAttribute, 
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);
            
            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                DisplayEditLink(Url.Action("Edit", "Manufacturer", new { id = manufacturer.Id, area = AreaNames.Admin }));

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewManufacturer",
                string.Format(_localizationService.GetResource("ActivityLog.PublicStore.ViewManufacturer"), manufacturer.Name), manufacturer);

            //model
            var model = _catalogModelFactory.PrepareManufacturerModel(manufacturer, command);
            
            //template
            var templateViewPath = _catalogModelFactory.PrepareManufacturerTemplateViewPath(manufacturer.ManufacturerTemplateId);
            return View(templateViewPath, model);
        }

        public virtual IActionResult ManufacturerAll()
        {
            var model = _catalogModelFactory.PrepareManufacturerAllModels();
            return View(model);
        }
        
        #endregion

        #region Vendors

        public virtual IActionResult Vendor(int vendorId, CatalogPagingFilteringModel command)
        {
            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null || vendor.Deleted || !vendor.Active)
                return InvokeHttp404();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);
            
            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                DisplayEditLink(Url.Action("Edit", "Vendor", new { id = vendor.Id, area = AreaNames.Admin }));

            //model
            var model = _catalogModelFactory.PrepareVendorModel(vendor, command);

            return View("ShuqVendor", model);
        }

        public virtual IActionResult VendorAll()
        {
            //we don't allow viewing of vendors if "vendors" block is hidden
            if (_vendorSettings.VendorsBlockItemsToDisplay == 0)
                return RedirectToRoute("Homepage");

            var model = _catalogModelFactory.PrepareVendorAllModels();
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult PreviewVendor(Nop.Web.Areas.Admin.Models.Vendors.VendorModel model, IFormCollection form)
        {
            var command = new CatalogPagingFilteringModel();
            int vendorId = _workContext.CurrentVendor.Id;

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null || vendor.Deleted || !vendor.Active)
                return InvokeHttp404();

            //model
            var viewModel = _catalogModelFactory.PrepareVendorModel(vendor, command);

            viewModel.Name = model.Name;
            viewModel.Description = model.Description;

            var pictureURL = _pictureService.GetPictureUrl(model.PictureId);
            viewModel.PictureModel = new PictureModel
            {
                FullSizeImageUrl = pictureURL,
                ImageUrl = pictureURL
            };


            var vendorAttributes = _vendorAttributeService.GetAllVendorAttributes();
            var vendorAboutUsAttribute = vendorAttributes.Where(x => x.Name.ToLower() == "about us").FirstOrDefault();

            if (vendorAboutUsAttribute != null)
            {
                var controlId = $"{NopVendorDefaults.VendorAttributePrefix}{vendorAboutUsAttribute.Id}";
                var formField = form[controlId];

                var enteredText = formField.ToString().Trim();

                viewModel.AboutUs = enteredText;
            }

            return View("ShuqVendor", viewModel);
        }

        #endregion

        #region Product tags
        
        public virtual IActionResult ProductsByTag(int productTagId, CatalogPagingFilteringModel command)
        {
            var productTag = _productTagService.GetProductTagById(productTagId);
            if (productTag == null)
                return InvokeHttp404();

            var model = _catalogModelFactory.PrepareProductsByTagModel(productTag, command);
            return View(model);
        }

        public virtual IActionResult ProductTagsAll()
        {
            var model = _catalogModelFactory.PrepareProductTagsAllModel();
            return View(model);
        }

        #endregion

        #region Searching

        public virtual IActionResult Search(SearchModel model, CatalogPagingFilteringModel command)
        {
            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(true),
                _storeContext.CurrentStore.Id);

            if (model == null)
                model = new SearchModel();

            model = _catalogModelFactory.PrepareSearchModel(model, command);
            return View(model);
        }

        public virtual IActionResult SearchMartProduct(SearchModel model, CatalogPagingFilteringModel command)
        {
            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(true),
                _storeContext.CurrentStore.Id);

            if (model == null)
                model = new SearchModel();

            //remove location filter for shuq mart
            //model.lat = model.lat ?? _workContext.Latitude?.ToString();
            //model.lng = model.lng ?? _workContext.Longitude?.ToString();

            if(model.lat != null 
                && decimal.TryParse(model.lat, out var lat))
            {
                _workContext.Latitude = lat == 0 ? null : (decimal?)lat;
                model.lat = _workContext.Latitude?.ToString();
            }

            if (model.lng != null
                && decimal.TryParse(model.lng, out var lng))
            {
                _workContext.Longitude = lng == 0 ? null : (decimal?)lng;
                model.lng = _workContext.Longitude?.ToString();
            }

            model.cid = _vendorSettings.ShuqMartCategoryId;
            model.adv = true;
            model.isc = true;
            model.sid = true;
            model.ShowVendorState = false;
            model = _catalogModelFactory.PrepareSearchModel(model, command);
            model.PagingFilteringContext.AllowCustomersToSelectPageSize = false;
            model.TopSliderWidgetZone = PublicWidgetZones.ShuqMartPageTop;
            model.SideSliderWidgetZone = PublicWidgetZones.ShuqMartPageSide;
            return View("~/Views/Catalog/SearchProduct.cshtml", model);
        }

        public virtual IActionResult SearchEatsProduct(SearchModel model, CatalogPagingFilteringModel command)
        {
            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(true),
                _storeContext.CurrentStore.Id);

            if (model == null)
                model = new SearchModel();

            model.lat = model.lat ?? _workContext.Latitude?.ToString();
            model.lng = model.lng ?? _workContext.Longitude?.ToString();

            if (model.lat != null
                && decimal.TryParse(model.lat, out var lat))
            {
                _workContext.Latitude = lat == 0 ? null : (decimal?)lat;
                model.lat = _workContext.Latitude?.ToString();
            }

            if (model.lng != null
                && decimal.TryParse(model.lng, out var lng))
            {
                _workContext.Longitude = lng == 0 ? null : (decimal?)lng;
                model.lng = _workContext.Longitude?.ToString();
            }

            model.cid = _vendorSettings.ShuqEatsCategoryId;
            model.adv = true;
            model.isc = true;
            model.sid = true;
            model.ShowVendorState = true;
            model.CoverageDistance = _catalogSettings.ShuqEatProductSearchCoverageDistance;
            model = _catalogModelFactory.PrepareSearchModel(model, command);
            model.PagingFilteringContext.AllowCustomersToSelectPageSize = false;
            model.TopSliderWidgetZone = PublicWidgetZones.ShuqEatPageTop;
            model.SideSliderWidgetZone = PublicWidgetZones.ShuqEatPageSide;
            return View("~/Views/Catalog/SearchProduct.cshtml", model);
        }

        public virtual IActionResult SearchTermAutoComplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < _catalogSettings.ProductSearchTermMinimumLength)
                return Content("");

            //products
            var productNumber = _catalogSettings.ProductSearchAutoCompleteNumberOfProducts > 0 ?
                _catalogSettings.ProductSearchAutoCompleteNumberOfProducts : 10;            

            var products = _productService.SearchProducts(
                storeId: _storeContext.CurrentStore.Id,
                keywords: term,
                languageId: _workContext.WorkingLanguage.Id,
                visibleIndividuallyOnly: true,
                pageSize: productNumber);

            var showLinkToResultSearch = _catalogSettings.ShowLinkToAllResultInSearchAutoComplete && (products.TotalCount > productNumber);

            var models =  _productModelFactory.PrepareProductOverviewModels(products, false, _catalogSettings.ShowProductImagesInSearchAutoComplete, _mediaSettings.AutoCompleteSearchThumbPictureSize).ToList();
            var result = (from p in models
                    select new
                    {
                        label = p.Name,
                        producturl = Url.RouteUrl("Product", new {SeName = p.SeName}),
                        productpictureurl = p.DefaultPictureModel.ImageUrl,
                        showlinktoresultsearch = showLinkToResultSearch
                    })
                .ToList();
            return Json(result);
        }
        
        #endregion
    }
}