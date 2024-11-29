using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Plugin.NopStation.WebApi.Infrastructure.Cache;
using Nop.Web.Models.ShoppingCart;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Plugin.NopStation.WebApi.Models.Order;
using Nop.Services.Media;
using Nop.Services.Vendors;
using Newtonsoft.Json;
using Nop.Web.Models.Common;

namespace Nop.Plugin.NopStation.WebApi.Controllers
{
    [Route("api/product")]
    public class ProductApiController : BaseApiController
    {
        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly ICompareProductsService _compareProductsService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IOrderReportService _orderReportService;
        private readonly ICustomerService _customerService;
        private readonly IReviewTypeService _reviewTypeService;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeApiParser _productAttributeApiParser;
        private readonly IProductApiService _productApiService;
        private readonly WebApiSettings _webApiSettings;
        private readonly IPictureService _pictureService;
        private readonly IVendorService _vendorService;

        #endregion

        #region Ctor

        public ProductApiController(CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            IAclService aclService,
            ICompareProductsService compareProductsService,
            ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IOrderService orderService,
            IPermissionService permissionService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IRecentlyViewedProductsService recentlyViewedProductsService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            ShoppingCartSettings shoppingCartSettings,
            IStaticCacheManager cacheManager,
            IOrderReportService orderReportService,
            ICustomerService customerService,
            IReviewTypeService reviewTypeService,
            ICacheKeyService cacheKeyService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IProductAttributeParser productAttributeParser,
            IProductAttributeApiParser productAttributeApiParser,
            IProductApiService productApiService,
            WebApiSettings webApiSettings,
            IPictureService pictureService,
            IVendorService vendorService)
        {
            _captchaSettings = captchaSettings;
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _compareProductsService = compareProductsService;
            _customerActivityService = customerActivityService;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _orderService = orderService;
            _permissionService = permissionService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _recentlyViewedProductsService = recentlyViewedProductsService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _cacheManager = cacheManager;
            _orderReportService = orderReportService;
            _customerService = customerService;
            _reviewTypeService = reviewTypeService;
            _cacheKeyService = cacheKeyService;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _productAttributeParser = productAttributeParser;
            _productAttributeApiParser = productAttributeApiParser;
            _productApiService = productApiService;
            _webApiSettings = webApiSettings;
            _pictureService = pictureService;
            _vendorService = vendorService;
        }

        #endregion

        #region Product details page

        [HttpGet("productdetails/{productId}/{updatecartitemid?}")]
        public virtual IActionResult ProductDetails(int productId, int updatecartitemid = 0)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                return NotFound(_localizationService.GetResource("NopStation.WebApi.Response.Product.ProductNotFound"));

            var notAvailable =
                //published?
                (!product.Published && !_catalogSettings.AllowViewUnpublishedProductPage) ||
                //ACL (access control list) 
                !_aclService.Authorize(product) ||
                //Store mapping
                !_storeMappingService.Authorize(product) ||
                //availability dates
                !_productService.ProductIsAvailable(product);
            //Check whether the current user has a "Manage products" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageProducts);
            if (notAvailable && !hasAdminAccess)
                return NotFound();

            //visible individually?
            if (!product.VisibleIndividually)
            {
                //is this one an associated products?
                var parentGroupedProduct = _productService.GetProductById(product.ParentGroupedProductId);
                if (parentGroupedProduct == null)
                    return BadRequest();

                return Redirect($"api/product/productdetails/{parentGroupedProduct.Id}");
            }

            //update existing shopping cart or wishlist  item?
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, storeId: _storeContext.CurrentStore.Id);
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    return Redirect($"api/product/productdetails/{product.Id}");
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    return Redirect($"api/product/productdetails/{product.Id}");
                }
            }

            //save as recently viewed
            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewProduct",
                string.Format(_localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name), product);

            var response = new GenericResponseModel<ProductDetailsModel>();
            //model
            response.Data = _productModelFactory.PrepareProductDetailsModel(product, updatecartitem, false);

            return Ok(response);
        }

        [HttpGet("getproductbybarcode/{productCode}")]
        public virtual IActionResult ProductDetails(string productCode)
        {
            if (string.IsNullOrEmpty(productCode))
                return NotFound(_localizationService.GetResource("NopStation.WebApi.Response.Product.ProductByBarCode.ProductNotFound"));

            Product product;
            if (_webApiSettings.ProductBarcodeScanKeyId == (int) BarcodeScanKeyType.Sku)
                product = _productService.GetProductBySku(productCode);
            else if (_webApiSettings.ProductBarcodeScanKeyId == (int) BarcodeScanKeyType.Gtin)
                product = _productApiService.GetProductByGtin(productCode);
            else
            {
                int.TryParse(productCode, out int productId);
                product = _productService.GetProductById(productId);
            }

            if (product == null || product.Deleted)
                return NotFound(_localizationService.GetResource("NopStation.WebApi.Response.Product.ProductByBarCode.ProductNotFound"));

            var notAvailable =
                //published?
                (!product.Published && !_catalogSettings.AllowViewUnpublishedProductPage) ||
                //ACL (access control list) 
                !_aclService.Authorize(product) ||
                //Store mapping
                !_storeMappingService.Authorize(product) ||
                //availability dates
                !_productService.ProductIsAvailable(product);
            //Check whether the current user has a "Manage products" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageProducts);
            if (notAvailable && !hasAdminAccess)
                return NotFound();

            //visible individually?
            if (!product.VisibleIndividually)
            {
                //is this one an associated products?
                var parentGroupedProduct = _productService.GetProductById(product.ParentGroupedProductId);
                if (parentGroupedProduct == null)
                    return BadRequest();

                return Redirect($"api/product/productdetails/{parentGroupedProduct.Id}");
            }

            //save as recently viewed
            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewProduct",
                string.Format(_localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name), product);

            var response = new GenericResponseModel<ProductDetailsModel>();
            //model
            response.Data = _productModelFactory.PrepareProductDetailsModel(product, null, false);

            return Ok(response);
        }

        [HttpPost("estimateshipping/{productId}/{updatecartitemid}")]
        public virtual IActionResult EstimateShipping(BaseQueryModel<ProductDetailsModel.ProductEstimateShippingModel> queryModel)
        {
            var model = queryModel.Data;
            var form = queryModel.FormValues.ToNameValueCollection();
            if (model == null)
                model = new ProductDetailsModel.ProductEstimateShippingModel();

            var errors = new List<string>();
            if (string.IsNullOrEmpty(model.ZipPostalCode))
                errors.Add(_localizationService.GetResource("Shipping.EstimateShipping.ZipPostalCode.Required"));

            if (model.CountryId == null || model.CountryId == 0)
                errors.Add(_localizationService.GetResource("Shipping.EstimateShipping.Country.Required"));

            if (errors.Count > 0)
                return BadRequest(errors);

            var product = _productService.GetProductById(model.ProductId);
            if (product == null || product.Deleted)
            {
                errors.Add(_localizationService.GetResource("Shipping.EstimateShippingPopUp.Product.IsNotFound"));
                return BadRequest(errors);
            }

            var wrappedProduct = new ShoppingCartItem()
            {
                StoreId = _storeContext.CurrentStore.Id,
                ShoppingCartTypeId = (int)ShoppingCartType.ShoppingCart,
                CustomerId = _workContext.CurrentCustomer.Id,
                ProductId = product.Id,
                CreatedOnUtc = DateTime.UtcNow
            };

            var addToCartWarnings = new List<string>();
            //customer entered price
            wrappedProduct.CustomerEnteredPrice = _productAttributeApiParser.ParseCustomerEnteredPrice(product, form);

            //entered quantity
            wrappedProduct.Quantity = _productAttributeApiParser.ParseEnteredQuantity(product, form);

            //product and gift card attributes
            wrappedProduct.AttributesXml = _productAttributeApiParser.ParseProductAttributes(product, form, addToCartWarnings);

            //rental attributes
            _productAttributeApiParser.ParseRentalDates(product, form, out var rentalStartDate, out var rentalEndDate);
            wrappedProduct.RentalStartDateUtc = rentalStartDate;
            wrappedProduct.RentalEndDateUtc = rentalEndDate;

            var response = new GenericResponseModel<EstimateShippingResultModel>();
            response.Data = _shoppingCartModelFactory.PrepareEstimateShippingResultModel(new[] { wrappedProduct }, model.CountryId, model.StateProvinceId, model.ZipPostalCode, false);

            return Ok(response);
        }

        [HttpGet("relatedproducts/{productId}/{productThumbPictureSize?}")]
        public IActionResult RelatedProducts(int productId, int? productThumbPictureSize)
        {
            var response = new GenericResponseModel<List<ProductOverviewModel>>();
            //load and cache report
            var productsRelatedIdsCacheKey = _cacheKeyService.PrepareKeyForDefaultCache(ApiModelCacheDefaults.ProductsRelatedIdsKey,
                       productId, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore);

            var productIds = _cacheManager.Get(productsRelatedIdsCacheKey,
                () => _productService.GetRelatedProductsByProductId1(productId).Select(x => x.ProductId2).ToArray());

            //load products
            var products = _productService.GetProductsByIds(productIds);
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();
            //visible individually
            products = products.Where(p => p.VisibleIndividually).ToList();

            if (!products.Any())
                return Ok(response);

            response.Data = _productModelFactory.PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();
            
            return Ok(response);
        }

        [HttpGet("productsalsopurchased/{productId}/{productThumbPictureSize?}")]
        public IActionResult ProductsAlsoPurchased(int productId, int? productThumbPictureSize)
        {
            var response = new GenericResponseModel<List<ProductOverviewModel>>();
            if (!_catalogSettings.ProductsAlsoPurchasedEnabled)
                return Ok(response);

            //load and cache report
            var productsAlsoPurchasedIdsCacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductsAlsoPurchasedIdsKey,
                        productId, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore);

            var productIds = _cacheManager.Get(productsAlsoPurchasedIdsCacheKey,
                () => _orderReportService.GetAlsoPurchasedProductsIds(_storeContext.CurrentStore.Id, productId, _catalogSettings.ProductsAlsoPurchasedNumber)
            );

            //load products
            var products = _productService.GetProductsByIds(productIds);
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();

            if (!products.Any())
                return Ok(response);

            response.Data = _productModelFactory.PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();

            return Ok(response);
        }

        [HttpGet("crosssellproducts/{productThumbPictureSize?}")]
        public IActionResult CrossSellProducts(int? productThumbPictureSize)
        {
            var response = new GenericResponseModel<List<ProductOverviewModel>>();
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            var products = _productService.GetCrosssellProductsByShoppingCart(cart, _shoppingCartSettings.CrossSellsNumber);
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();
            //visible individually
            products = products.Where(p => p.VisibleIndividually).ToList();

            if (!products.Any()) 
                return Ok(response);

            //Cross-sell products are displayed on the shopping cart page.
            //We know that the entire shopping cart page is not refresh
            //even if "ShoppingCartSettings.DisplayCartAfterAddingProduct" setting  is enabled.
            //That's why we force page refresh (redirect) in this case
            response.Data = _productModelFactory.PrepareProductOverviewModels(products,
                    productThumbPictureSize: productThumbPictureSize, forceRedirectionAfterAddingToCart: true)
                .ToList();

            return Ok(response);
        }

        #endregion

        #region Recently viewed products

        [HttpGet("recentlyviewedproducts")]
        public virtual IActionResult RecentlyViewedProducts()
        {
            if (!_catalogSettings.RecentlyViewedProductsEnabled)
                return BadRequest();

            var products = _recentlyViewedProductsService.GetRecentlyViewedProducts(_catalogSettings.RecentlyViewedProductsNumber);

            var response = new GenericResponseModel<List<ProductOverviewModel>>();
            response.Data = new List<ProductOverviewModel>();
            response.Data.AddRange(_productModelFactory.PrepareProductOverviewModels(products));

            return Ok(response);
        }

        #endregion

        #region New (recently added) products page

        [HttpGet("newproducts")]
        public virtual IActionResult NewProducts()
        {
            if (!_catalogSettings.NewProductsEnabled)
                return BadRequest();

            var products = _productService.SearchProducts(
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                markedAsNewOnly: true,
                orderBy: ProductSortingEnum.NameAsc,
                pageSize: _catalogSettings.NewProductsNumber);

            var response = new GenericResponseModel<List<ProductOverviewModel>>();
            response.Data = new List<ProductOverviewModel>();
            response.Data.AddRange(_productModelFactory.PrepareProductOverviewModels(products));

            return Ok(response);
        }

        #endregion

        #region Product reviews

        [HttpGet("productreviews/{productId}")]
        public virtual IActionResult ProductReviews(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published)
                return NotFound(_localizationService.GetResource("NopStation.WebApi.Response.Product.ProductNotFound"));

            if (!product.AllowCustomerReviews)
                return BadRequest();

            var response = new GenericResponseModel<ProductReviewsModel>();
            var model = new ProductReviewsModel();
            model = _productModelFactory.PrepareProductReviewsModel(model, product);
            //only registered users can leave reviews
            if (_customerService.IsGuest(_workContext.CurrentCustomer) && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
                response.ErrorList.Add(_localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));

            if (_catalogSettings.ProductReviewPossibleOnlyAfterPurchasing)
            {
                var hasCompletedOrders = _orderService.SearchOrders(customerId: _workContext.CurrentCustomer.Id,
                    productId: productId,
                    osIds: new List<int> { (int)OrderStatus.Complete },
                    pageSize: 1).Any();
                if (!hasCompletedOrders)
                    response.ErrorList.Add(_localizationService.GetResource("Reviews.ProductReviewPossibleOnlyAfterPurchasing"));
            }

            //default value
            model.AddProductReview.Rating = _catalogSettings.DefaultProductRatingValue;

            //default value for all additional review types
            if (model.ReviewTypeList.Count > 0)
                foreach (var additionalProductReview in model.AddAdditionalProductReviewList)
                {
                    additionalProductReview.Rating = additionalProductReview.IsRequired ? _catalogSettings.DefaultProductRatingValue : 0;
                }
            response.Data = model;
            return Ok(response);
        }

        [HttpGet("vendorproductreviews/{vendorId}")]
        public virtual IActionResult VendorProductReviews(int vendorId, 
            int pageNumber = 0,
            int pageSize = int.MaxValue)
        {
            var vendor = _vendorService.GetVendorById(vendorId);

            int pageIndex = 0;
            if (pageNumber > 0)
                pageIndex = pageNumber - 1;

            if (vendor == null)
                return NotFound();

            var products = _productService.SearchProducts(vendorId: vendorId);

            var response = new GenericResponseModel<VendorProductReviewModel>();

            var modelList = new VendorProductReviewModel();

            IList<ProductReviewModel> model = new List<ProductReviewModel>();

            model = _productModelFactory.PrepareProductReviewModel(model, products, pageIndex, pageSize);

            var modelEnum = model.AsEnumerable();
            var query = modelEnum.AsQueryable();
            var productReviewsPaged = new PagedList<ProductReviewModel>(query, pageIndex, pageSize);

            modelList.Items = productReviewsPaged;

            var pagerModel = new PagerModel
            {
                PageSize = pageSize,
                TotalRecords = modelEnum.Count(),
                PageIndex = pageIndex,
                ShowTotalSummary = false,
                RouteActionName = "ProductReviewsPaged",
                UseRouteLinks = true,
                RouteValues = new ProductReviewsModel.ProductReviewsRouteValues { pageNumber = pageIndex }
            };

            modelList.PagerModel = pagerModel;

            response.Data = modelList;
            return Ok(response);
        }

        [HttpPost("productreviewsadd/{orderId}/{productId}")]
        public virtual IActionResult ProductReviewsAdd(int orderId, int productId, ProductReviewsViewModel queryModel)
        {
            var product = _productService.GetProductById(productId);
            var order = new Order();
            if (product == null || product.Deleted || !product.Published)
                return NotFound(_localizationService.GetResource("NopStation.WebApi.Response.Product.ProductNotFound"));

            if (!product.AllowCustomerReviews)
                return BadRequest();

            var model = queryModel.Data;
            var response = new GenericResponseModel<ProductReviewsModel>();
            if (_customerService.IsGuest(_workContext.CurrentCustomer) && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
                ModelState.AddModelError("", _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));


            order = _orderService.GetOrderById(orderId);
            if (order.OrderStatus != OrderStatus.Complete)
                ModelState.AddModelError("", _localizationService.GetResource("Reviews.ProductReviewPossibleOnlyAfterPurchasing"));

            if (ModelState.IsValid)
            {
                List<int> pictureIds = new List<int>();
                //save review
                var rating = model.AddProductReview.Rating;
                if (rating < 1 || rating > 5)
                    rating = _catalogSettings.DefaultProductRatingValue;
                var isApproved = !_catalogSettings.ProductReviewsMustBeApproved;

                var productReview = new ProductReview
                {
                    ProductId = product.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    Title = model.AddProductReview.Title,
                    ReviewText = model.AddProductReview.ReviewText,
                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    IsApproved = isApproved,
                    CreatedOnUtc = DateTime.UtcNow,
                    StoreId = _storeContext.CurrentStore.Id,
                    OrderId = order.Id,
                };

                _productService.InsertProductReview(productReview);

                if (queryModel.Images != null)
                {
                    pictureIds = _pictureService.UploadPictureAndGetPictureId(queryModel.Images, true);

                    foreach (var item in pictureIds)
                    {
                        _productService.InsertProductReviewPictureMapping(new ProductReviewPictureMapping
                        {
                            PictureId = item,
                            ProductReviewId = productReview.Id,
                            DisplayOrder = 1
                        });
                    }
                }


                //add product review and review type mapping                
                foreach (var additionalReview in model.AddAdditionalProductReviewList)
                {
                    var additionalProductReview = new ProductReviewReviewTypeMapping
                    {
                        ProductReviewId = productReview.Id,
                        ReviewTypeId = additionalReview.ReviewTypeId,
                        Rating = additionalReview.Rating
                    };
                    _reviewTypeService.InsertProductReviewReviewTypeMappings(additionalProductReview);
                }

                //update product totals
                _productService.UpdateProductReviewTotals(product);

                //notify store owner
                if (_catalogSettings.NotifyStoreOwnerAboutNewProductReviews)
                    _workflowMessageService.SendProductReviewNotificationMessage(productReview, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                _customerActivityService.InsertActivity("PublicStore.AddProductReview",
                    string.Format(_localizationService.GetResource("ActivityLog.PublicStore.AddProductReview"), product.Name), product);

                //raise event
                if (productReview.IsApproved)
                    _eventPublisher.Publish(new ProductReviewApprovedEvent(productReview));

                model = _productModelFactory.PrepareProductReviewsModel(model, product);
                model.AddProductReview.Title = null;
                model.AddProductReview.ReviewText = null;

                model.AddProductReview.SuccessfullyAdded = true;
                if (!isApproved)
                    model.AddProductReview.Result = _localizationService.GetResource("Reviews.SeeAfterApproving");
                else
                    model.AddProductReview.Result = _localizationService.GetResource("Reviews.SuccessfullyAdded");

                response.Data = model;

                return Ok(response);
            }

            foreach (var modelState in ModelState.Values)
                foreach (var error in modelState.Errors)
                    response.ErrorList.Add(error.ErrorMessage);

            //If we got this far, something failed, redisplay form
            response.Data = _productModelFactory.PrepareProductReviewsModel(model, product);
            return BadRequest(response);
        }
        
        [HttpPost("setproductreviewhelpfulness/{productReviewId:min(0)}")]
        public virtual IActionResult SetProductReviewHelpfulness(int productReviewId, [FromBody]BaseQueryModel<object> queryModel)
        {
            var productReview = _productService.GetProductReviewById(productReviewId);
            if (productReview == null)
                return NotFound(_localizationService.GetResource("NopStation.WebApi.Response.Product.ProductReviewNotFound"));

            var form = queryModel.FormValues == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();
            var washelpful = form["washelpful"] != null ? bool.Parse(form["washelpful"]) : true;

            var response = new GenericResponseModel<ProductReviewHelpfulnessModel>();
            if (_customerService.IsGuest(_workContext.CurrentCustomer) && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
            {
                response.Data = new ProductReviewHelpfulnessModel()
                {
                    HelpfulYesTotal = productReview.HelpfulYesTotal,
                    HelpfulNoTotal = productReview.HelpfulNoTotal
                };
                response.ErrorList.Add(_localizationService.GetResource("Reviews.Helpfulness.OnlyRegistered"));
                return BadRequest(response);
            }

            //customers aren't allowed to vote for their own reviews
            if (productReview.CustomerId == _workContext.CurrentCustomer.Id)
            {
                response.Data = new ProductReviewHelpfulnessModel()
                {
                    HelpfulYesTotal = productReview.HelpfulYesTotal,
                    HelpfulNoTotal = productReview.HelpfulNoTotal
                };
                response.ErrorList.Add(_localizationService.GetResource("Reviews.Helpfulness.YourOwnReview"));
                return BadRequest(response);
            }


            _productService.SetProductReviewHelpfulness(productReview, washelpful);

            //new totals
            _productService.UpdateProductReviewHelpfulnessTotals(productReview);

           
            response.Data = new ProductReviewHelpfulnessModel()
            {
                ProductReviewId = productReview.Id,
                HelpfulYesTotal = productReview.HelpfulYesTotal,
                HelpfulNoTotal = productReview.HelpfulNoTotal
            };
            response.Message = _localizationService.GetResource("Reviews.Helpfulness.SuccessfullyVoted");

            return Ok(response);
        }

        [HttpGet("productreviews")]
        public virtual IActionResult CustomerProductReviews(int? pageNumber)
        {
            if (_customerService.IsGuest(_workContext.CurrentCustomer))
                return Unauthorized();

            if (!_catalogSettings.ShowProductReviewsTabOnAccountPage)
                return BadRequest();

            var response = new GenericResponseModel<CustomerProductReviewsModel>();
            response.Data = _productModelFactory.PrepareCustomerProductReviewsModel(pageNumber);
            return Ok(response);
        }

        #endregion

        #region Email a friend

        [HttpGet("productemailafriend/{productId:min(0)}")]
        public virtual IActionResult ProductEmailAFriend(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published)
                return NotFound();

            if (!product.AllowCustomerReviews)
                return BadRequest();

            var response = new GenericResponseModel<ProductEmailAFriendModel>();
            response.Data = _productModelFactory.PrepareProductEmailAFriendModel(new ProductEmailAFriendModel(), product, false);
            return Ok(response);
        }

        
        [HttpPost("productemailafriendsend")]
        public virtual IActionResult ProductEmailAFriendSend([FromBody]BaseQueryModel<ProductEmailAFriendModel> queryModel)
        {
            var model = queryModel.Data;

            var product = _productService.GetProductById(model.ProductId);
            if (product == null || product.Deleted || !product.Published)
                return NotFound();

            if (!product.AllowCustomerReviews)
                return BadRequest();

            //check whether the current customer is guest and ia allowed to email a friend
            if (_customerService.IsGuest(_workContext.CurrentCustomer) && !_catalogSettings.AllowAnonymousUsersToEmailAFriend)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Products.EmailAFriend.OnlyRegisteredUsers"));
            }

            var response = new GenericResponseModel<ProductEmailAFriendModel>();
            if (ModelState.IsValid)
            {
                //email
                _workflowMessageService.SendProductEmailAFriendMessage(_workContext.CurrentCustomer,
                        _workContext.WorkingLanguage.Id, product,
                        model.YourEmailAddress, model.FriendEmail,
                        Nop.Core.Html.HtmlHelper.FormatText(model.PersonalMessage, false, true, false, false, false, false));

                model = _productModelFactory.PrepareProductEmailAFriendModel(model, product, true);
                model.SuccessfullySent = true;
                response.Data = model;
                response.Message = _localizationService.GetResource("Products.EmailAFriend.SuccessfullySent");

                return Ok(response);
            }

            foreach (var modelState in ModelState.Values)
                foreach (var error in modelState.Errors)
                    response.ErrorList.Add(error.ErrorMessage);

            //If we got this far, something failed, redisplay form
            response.Data = _productModelFactory.PrepareProductEmailAFriendModel(model, product, true);
            return BadRequest(response);
        }

        #endregion

        #region Comparing products

        
        [HttpPost("compareproducts/add/{productId:min(0)}")]
        public virtual IActionResult AddProductToCompareList(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published)
                return NotFound();

            if (!product.AllowCustomerReviews)
                return BadRequest();

            if (!_catalogSettings.CompareProductsEnabled)
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.Catalog.ProductComparisonDisabled"));

            _compareProductsService.AddProductToCompareList(productId);

            //activity log
            _customerActivityService.InsertActivity("PublicStore.AddToCompareList",
                string.Format(_localizationService.GetResource("ActivityLog.PublicStore.AddToCompareList"), product.Name), product);

            return Ok(_localizationService.GetResource("Products.ProductHasBeenAddedToCompareList"));
        }

        [HttpGet("compareproducts/remove/{productId}")]
        public virtual IActionResult RemoveProductFromCompareList(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return NotFound();

            if (!_catalogSettings.CompareProductsEnabled)
                return BadRequest();

            _compareProductsService.RemoveProductFromCompareList(productId);

            return Ok();
        }

        [HttpGet("compareproducts")]
        public virtual IActionResult CompareProducts()
        {
            if (!_catalogSettings.CompareProductsEnabled)
                return BadRequest();

            var response = new GenericResponseModel<CompareProductsModel>();
            var model = new CompareProductsModel
            {
                IncludeShortDescriptionInCompareProducts = _catalogSettings.IncludeShortDescriptionInCompareProducts,
                IncludeFullDescriptionInCompareProducts = _catalogSettings.IncludeFullDescriptionInCompareProducts,
            };

            var products = _compareProductsService.GetComparedProducts();

            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();

            //prepare model
            _productModelFactory.PrepareProductOverviewModels(products, prepareSpecificationAttributes: true)
                .ToList()
                .ForEach(model.Products.Add);
            response.Data = model;

            return Ok(response);
        }

        [HttpGet("clearcomparelist")]
        public virtual IActionResult ClearCompareList()
        {
            if (!_catalogSettings.CompareProductsEnabled)
                return BadRequest();

            _compareProductsService.ClearCompareProducts();

            return Ok();
        }

        #endregion
    }
}
