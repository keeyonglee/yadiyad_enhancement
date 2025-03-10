﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping.Date;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the product model factory
    /// </summary>
    public partial class ProductModelFactory : IProductModelFactory
    {
        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly ICategoryService _categoryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IDateRangeService _dateRangeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDownloadService _downloadService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IReviewTypeService _reviewTypeService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly ITaxService _taxService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorService _vendorService;
        private readonly IVendorAttributeService _vendorAttributeService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly OrderSettings _orderSettings;
        private readonly SeoSettings _seoSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IAddressService _addressService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShoppingCartService _shoppingCartService;

        #endregion

        #region Ctor

        public ProductModelFactory(CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CustomerSettings customerSettings,
            ICacheKeyService cacheKeyService,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IProductTagService productTagService,
            IProductTemplateService productTemplateService,
            IReviewTypeService reviewTypeService,
            ISpecificationAttributeService specificationAttributeService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IShoppingCartModelFactory shoppingCartModelFactory,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IVendorAttributeService vendorAttributeService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            SeoSettings seoSettings,
            ShippingSettings shippingSettings,
            VendorSettings vendorSettings,
            IAddressService addressService,
            IStateProvinceService stateProvinceService,
            IShoppingCartService shoppingCartService)
        {
            _captchaSettings = captchaSettings;
            _catalogSettings = catalogSettings;
            _customerSettings = customerSettings;
            _cacheKeyService = cacheKeyService;
            _categoryService = categoryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _dateRangeService = dateRangeService;
            _dateTimeHelper = dateTimeHelper;
            _downloadService = downloadService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _priceCalculationService = priceCalculationService;
            _priceFormatter = priceFormatter;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _productTagService = productTagService;
            _productTemplateService = productTemplateService;
            _reviewTypeService = reviewTypeService;
            _specificationAttributeService = specificationAttributeService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _taxService = taxService;
            _urlRecordService = urlRecordService;
            _vendorService = vendorService;
            _vendorAttributeService = vendorAttributeService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _orderSettings = orderSettings;
            _seoSettings = seoSettings;
            _shippingSettings = shippingSettings;
            _vendorSettings = vendorSettings;
            _addressService = addressService;
            _stateProvinceService = stateProvinceService;
            _shoppingCartService = shoppingCartService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the product review overview model
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Product review overview model</returns>
        public virtual ProductReviewOverviewModel PrepareProductReviewOverviewModel(Product product)
        {
            ProductReviewOverviewModel productReview;

            if (_catalogSettings.ShowProductReviewsPerStore)
            {
                var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductReviewsModelKey, product, _storeContext.CurrentStore);

                productReview = _staticCacheManager.Get(cacheKey, () =>
                {
                    var productReviews = _productService.GetAllProductReviews(productId: product.Id, approved: true, storeId: _storeContext.CurrentStore.Id);

                    return new ProductReviewOverviewModel
                    {
                        RatingSum = productReviews.Sum(pr => pr.Rating),
                        TotalReviews = productReviews.Count

                    };
                });
            }
            else
            {
                productReview = new ProductReviewOverviewModel
                {
                    RatingSum = product.ApprovedRatingSum,
                    TotalReviews = product.ApprovedTotalReviews
                };
            }

            if (productReview != null)
            {
                productReview.ProductId = product.Id;
                productReview.AllowCustomerReviews = product.AllowCustomerReviews;
            }

            if (productReview.TotalReviews != 0)
                productReview.RatingPercent = (productReview.RatingSum * 100) / productReview.TotalReviews / 5;
            else
                productReview.RatingPercent = 0;

            return productReview;
        }

        /// <summary>
        /// Prepare the product overview price model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>Product overview price model</returns>
        protected virtual ProductOverviewModel.ProductPriceModel PrepareProductOverviewPriceModel(Product product, bool forceRedirectionAfterAddingToCart = false)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var priceModel = new ProductOverviewModel.ProductPriceModel
            {
                ForceRedirectionAfterAddingToCart = forceRedirectionAfterAddingToCart
            };

            switch (product.ProductType)
            {
                case ProductType.GroupedProduct:
                    //grouped product
                    PrepareGroupedProductOverviewPriceModel(product, priceModel);

                    break;
                case ProductType.SimpleProduct:
                default:
                    //simple product
                    PrepareSimpleProductOverviewPriceModel(product, priceModel);

                    break;
            }

            return priceModel;
        }

        protected virtual void PrepareSimpleProductOverviewPriceModel(Product product, ProductOverviewModel.ProductPriceModel priceModel)
        {
            //add to cart button
            priceModel.DisableBuyButton = product.DisableBuyButton ||
                                          !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart) ||
                                          !_permissionService.Authorize(StandardPermissionProvider.DisplayPrices);

            //add to wishlist button
            priceModel.DisableWishlistButton = product.DisableWishlistButton ||
                                               !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist) ||
                                               !_permissionService.Authorize(StandardPermissionProvider.DisplayPrices);
            //compare products
            priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareProductsEnabled;

            //rental
            priceModel.IsRental = product.IsRental;

            //pre-order
            if (product.AvailableForPreOrder)
            {
                priceModel.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                                                  product.PreOrderAvailabilityStartDateTimeUtc.Value >=
                                                  DateTime.UtcNow;
                priceModel.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;
            }

            //prices
            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                if (product.CustomerEntersPrice)
                    return;

                if (product.CallForPrice &&
                    //also check whether the current user is impersonated
                    (!_orderSettings.AllowAdminsToBuyCallForPriceProducts ||
                     _workContext.OriginalCustomerIfImpersonated == null))
                {
                    //call for price
                    priceModel.OldPrice = null;
                    priceModel.Price = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    //prices
                    var minPossiblePriceWithoutDiscount = _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, includeDiscounts: false);
                    var minPossiblePriceWithDiscount = _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer);

                    if (product.HasTierPrices)
                    {
                        //calculate price for the maximum quantity if we have tier prices, and choose minimal
                        minPossiblePriceWithoutDiscount = Math.Min(minPossiblePriceWithoutDiscount,
                            _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, includeDiscounts: false, quantity: int.MaxValue));
                        minPossiblePriceWithDiscount = Math.Min(minPossiblePriceWithDiscount,
                            _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, includeDiscounts: true, quantity: int.MaxValue));
                    }

                    var oldPriceBase = _taxService.GetProductPrice(product, product.OldPrice, out var _);
                    var finalPriceWithoutDiscountBase = _taxService.GetProductPrice(product, minPossiblePriceWithoutDiscount, out _);
                    var finalPriceWithDiscountBase = _taxService.GetProductPrice(product, minPossiblePriceWithDiscount, out _);

                    var oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                    var finalPriceWithoutDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithoutDiscountBase, _workContext.WorkingCurrency);
                    var finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);

                    //do we have tier prices configured?
                    var tierPrices = new List<TierPrice>();
                    if (product.HasTierPrices)
                    {
                        tierPrices.AddRange(_productService.GetTierPrices(product, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id));
                    }
                    //When there is just one tier price (with  qty 1), there are no actual savings in the list.
                    var displayFromMessage = tierPrices.Any() && !(tierPrices.Count == 1 && tierPrices[0].Quantity <= 1);
                    if (displayFromMessage)
                    {
                        priceModel.OldPrice = null;
                        priceModel.Price = string.Format(_localizationService.GetResource("Products.PriceRangeFrom"), _priceFormatter.FormatPrice(finalPriceWithDiscount));
                        priceModel.PriceValue = finalPriceWithDiscount;
                    }
                    else
                    {
                        var strikeThroughPrice = decimal.Zero;

                        if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                            strikeThroughPrice = oldPrice;

                        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                            strikeThroughPrice = finalPriceWithoutDiscount;

                        if (strikeThroughPrice > decimal.Zero)
                            priceModel.OldPrice = _priceFormatter.FormatPrice(strikeThroughPrice);

                        priceModel.Price = _priceFormatter.FormatPrice(finalPriceWithDiscount);
                        priceModel.PriceValue = finalPriceWithDiscount;
                    }

                    if (product.IsRental)
                    {
                        //rental product
                        priceModel.OldPrice = _priceFormatter.FormatRentalProductPeriod(product, priceModel.OldPrice);
                        priceModel.Price = _priceFormatter.FormatRentalProductPeriod(product, priceModel.Price);
                    }

                    //property for German market
                    //we display tax/shipping info only with "shipping enabled" for this product
                    //we also ensure this it's not free shipping
                    priceModel.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoProductBoxes && product.IsShipEnabled && !product.IsFreeShipping;

                    //PAngV default baseprice (used in Germany)
                    priceModel.BasePricePAngV = _priceFormatter.FormatBasePrice(product, finalPriceWithDiscount);
                }
            }
            else
            {
                //hide prices
                priceModel.OldPrice = null;
                priceModel.Price = null;
            }
        }

        protected virtual void PrepareGroupedProductOverviewPriceModel(Product product, ProductOverviewModel.ProductPriceModel priceModel)
        {
            var associatedProducts = _productService.GetAssociatedProducts(product.Id,
                _storeContext.CurrentStore.Id);

            //add to cart button (ignore "DisableBuyButton" property for grouped products)
            priceModel.DisableBuyButton =
                !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart) ||
                !_permissionService.Authorize(StandardPermissionProvider.DisplayPrices);

            //add to wishlist button (ignore "DisableWishlistButton" property for grouped products)
            priceModel.DisableWishlistButton =
                !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist) ||
                !_permissionService.Authorize(StandardPermissionProvider.DisplayPrices);

            //compare products
            priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareProductsEnabled;
            if (!associatedProducts.Any())
                return;

            //we have at least one associated product
            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                //find a minimum possible price
                decimal? minPossiblePrice = null;
                Product minPriceProduct = null;
                foreach (var associatedProduct in associatedProducts)
                {
                    var tmpMinPossiblePrice = _priceCalculationService.GetFinalPrice(associatedProduct, _workContext.CurrentCustomer);

                    if (associatedProduct.HasTierPrices)
                    {
                        //calculate price for the maximum quantity if we have tier prices, and choose minimal
                        tmpMinPossiblePrice = Math.Min(tmpMinPossiblePrice,
                            _priceCalculationService.GetFinalPrice(associatedProduct, _workContext.CurrentCustomer, quantity: int.MaxValue));
                    }

                    if (minPossiblePrice.HasValue && tmpMinPossiblePrice >= minPossiblePrice.Value)
                        continue;
                    minPriceProduct = associatedProduct;
                    minPossiblePrice = tmpMinPossiblePrice;
                }

                if (minPriceProduct == null || minPriceProduct.CustomerEntersPrice)
                    return;

                if (minPriceProduct.CallForPrice &&
                    //also check whether the current user is impersonated
                    (!_orderSettings.AllowAdminsToBuyCallForPriceProducts ||
                     _workContext.OriginalCustomerIfImpersonated == null))
                {
                    priceModel.OldPrice = null;
                    priceModel.Price = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    //calculate prices
                    var finalPriceBase = _taxService.GetProductPrice(minPriceProduct, minPossiblePrice.Value, out var _);
                    var finalPrice = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, _workContext.WorkingCurrency);

                    priceModel.OldPrice = null;
                    priceModel.Price = string.Format(_localizationService.GetResource("Products.PriceRangeFrom"), _priceFormatter.FormatPrice(finalPrice));
                    priceModel.PriceValue = finalPrice;

                    //PAngV default baseprice (used in Germany)
                    priceModel.BasePricePAngV = _priceFormatter.FormatBasePrice(product, finalPriceBase);
                }
            }
            else
            {
                //hide prices
                priceModel.OldPrice = null;
                priceModel.Price = null;
            }
        }

        /// <summary>
        /// Prepare the product overview picture model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productThumbPictureSize">Product thumb picture size (longest side); pass null to use the default value of media settings</param>
        /// <returns>Picture model</returns>
        public virtual PictureModel PrepareProductOverviewPictureModel(Product product, int? productThumbPictureSize = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var productName = _localizationService.GetLocalized(product, x => x.Name);
            //If a size has been set in the view, we use it in priority
            var pictureSize = productThumbPictureSize ?? _mediaSettings.ProductThumbPictureSize;

            //prepare picture model
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductDefaultPictureModelKey,
                product, pictureSize, true, _workContext.WorkingLanguage, _webHelper.IsCurrentConnectionSecured(),
                _storeContext.CurrentStore);

            var defaultPictureModel = _staticCacheManager.Get(cacheKey, () =>
            {
                var picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
                var pictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(ref picture, pictureSize),
                    FullSizeImageUrl = _pictureService.GetPictureUrl(ref picture),
                    //"title" attribute
                    Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute))
                        ? picture.TitleAttribute
                        : string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"),
                            productName),
                    //"alt" attribute
                    AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute))
                        ? picture.AltAttribute
                        : string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"),
                            productName)
                };

                return pictureModel;
            });

            return defaultPictureModel;
        }

        /// <summary>
        /// Prepare the product breadcrumb model
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Product breadcrumb model</returns>
        protected virtual ProductDetailsModel.ProductBreadcrumbModel PrepareProductBreadcrumbModel(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var breadcrumbModel = new ProductDetailsModel.ProductBreadcrumbModel
            {
                Enabled = _catalogSettings.CategoryBreadcrumbEnabled,
                ProductId = product.Id,
                ProductName = _localizationService.GetLocalized(product, x => x.Name),
                ProductSeName = _urlRecordService.GetSeName(product)
            };
            var productCategories = _categoryService.GetProductCategoriesByProductId(product.Id);
            if (!productCategories.Any())
                return breadcrumbModel;

            var category = _categoryService.GetCategoryById(productCategories[0].CategoryId);
            if (category == null)
                return breadcrumbModel;

            foreach (var catBr in _categoryService.GetCategoryBreadCrumb(category))
            {
                breadcrumbModel.CategoryBreadcrumb.Add(new CategorySimpleModel
                {
                    Id = catBr.Id,
                    Name = _localizationService.GetLocalized(catBr, x => x.Name),
                    SeName = _urlRecordService.GetSeName(catBr),
                    IncludeInTopMenu = catBr.IncludeInTopMenu
                });
            }

            return breadcrumbModel;
        }

        /// <summary>
        /// Prepare the product tag models
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>List of product tag model</returns>
        protected virtual IList<ProductTagModel> PrepareProductTagModels(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model =
                _productTagService.GetAllProductTagsByProductId(product.Id)
                    //filter by store
                    .Where(x => _productTagService.GetProductCount(x.Id, _storeContext.CurrentStore.Id) > 0)
                    .Select(x => new ProductTagModel
                    {
                        Id = x.Id,
                        Name = _localizationService.GetLocalized(x, y => y.Name),
                        SeName = _urlRecordService.GetSeName(x),
                        ProductCount = _productTagService.GetProductCount(x.Id, _storeContext.CurrentStore.Id)
                    }).ToList();

            return model;
        }

        /// <summary>
        /// Prepare the product price model
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Product price model</returns>
        protected virtual ProductDetailsModel.ProductPriceModel PrepareProductPriceModel(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = new ProductDetailsModel.ProductPriceModel
            {
                ProductId = product.Id
            };

            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                model.HidePrices = false;
                if (product.CustomerEntersPrice)
                {
                    model.CustomerEntersPrice = true;
                }
                else
                {
                    if (product.CallForPrice &&
                        //also check whether the current user is impersonated
                        (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
                    {
                        model.CallForPrice = true;
                    }
                    else
                    {
                        var oldPriceBase = _taxService.GetProductPrice(product, product.OldPrice, out var _);
                        var finalPriceWithoutDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, includeDiscounts: false), out _);
                        var finalPriceWithDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer), out _);

                        var oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                        var finalPriceWithoutDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithoutDiscountBase, _workContext.WorkingCurrency);
                        var finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);

                        if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                            model.OldPrice = _priceFormatter.FormatPrice(oldPrice);
                        else
                            model.OldPrice = _priceFormatter.FormatPrice(finalPriceWithoutDiscount);

                        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                            model.Price = _priceFormatter.FormatPrice(finalPriceWithDiscount);

                        model.Price = _priceFormatter.FormatPrice(finalPriceWithDiscount);
                        if (model.Price == model.OldPrice)
                        {
                            model.OldPrice = null;
                        }

                        model.PriceValue = finalPriceWithDiscount;

                        //property for German market
                        //we display tax/shipping info only with "shipping enabled" for this product
                        //we also ensure this it's not free shipping
                        model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoProductDetailsPage
                            && product.IsShipEnabled &&
                            !product.IsFreeShipping;

                        //PAngV baseprice (used in Germany)
                        model.BasePricePAngV = _priceFormatter.FormatBasePrice(product, finalPriceWithDiscountBase);
                        //currency code
                        model.CurrencyCode = _workContext.WorkingCurrency.CurrencyCode;

                        //rental
                        if (product.IsRental)
                        {
                            model.IsRental = true;
                            var priceStr = _priceFormatter.FormatPrice(finalPriceWithDiscount);
                            model.RentalPrice = _priceFormatter.FormatRentalProductPeriod(product, priceStr);
                        }
                    }
                }
            }
            else
            {
                model.HidePrices = true;
                model.OldPrice = null;
                model.Price = null;
            }

            return model;
        }

        /// <summary>
        /// Prepare the product add to cart model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <returns>Product add to cart model</returns>
        protected virtual ProductDetailsModel.AddToCartModel PrepareProductAddToCartModel(Product product, ShoppingCartItem updatecartitem)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var vendor = _vendorService.GetVendorById(product.VendorId);

            var model = new ProductDetailsModel.AddToCartModel
            {
                ProductId = product.Id
            };

            model.VendorOnline = _vendorService.GetVendorOnlineStatus(vendor);
            if (product.DeliveryDates.Count > 0 && !product.DeliveryDates.Any(x => x >= DateTime.Today))
            {
                model.PastSeasonal = true;
            }

            if (updatecartitem != null)
            {
                model.UpdatedShoppingCartItemId = updatecartitem.Id;
                model.UpdateShoppingCartItemType = updatecartitem.ShoppingCartType;
            }

            //quantity
            model.EnteredQuantity = updatecartitem != null ? updatecartitem.Quantity : product.OrderMinimumQuantity == 0 ? 1 : product.OrderMinimumQuantity;
            //allowed quantities
            var allowedQuantities = _productService.ParseAllowedQuantities(product);
            foreach (var qty in allowedQuantities)
            {
                model.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = updatecartitem != null && updatecartitem.Quantity == qty
                });
            }
            //minimum quantity notification
            if (product.OrderMinimumQuantity > 1)
            {
                model.MinimumQuantityNotification = string.Format(_localizationService.GetResource("Products.MinimumQuantityNotification"), product.OrderMinimumQuantity);
            }

            //'add to cart', 'add to wishlist' buttons
            model.DisableBuyButton = product.DisableBuyButton || !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart);
            model.DisableWishlistButton = product.DisableWishlistButton || !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist);
            if (!_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                model.DisableBuyButton = true;
                model.DisableWishlistButton = true;
            }
            //pre-order
            if (product.AvailableForPreOrder)
            {
                model.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                    product.PreOrderAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow;
                model.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;

                if (model.PreOrderAvailabilityStartDateTimeUtc.HasValue && _catalogSettings.DisplayDatePreOrderAvailability)
                {
                    model.PreOrderAvailabilityStartDateTimeUserTime =
                        _dateTimeHelper.ConvertToUserTime(model.PreOrderAvailabilityStartDateTimeUtc.Value).ToString("D");
                }
            }
            //rental
            model.IsRental = product.IsRental;

            //customer entered price
            model.CustomerEntersPrice = product.CustomerEntersPrice;
            if (!model.CustomerEntersPrice)
                return model;

            var minimumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(product.MinimumCustomerEnteredPrice, _workContext.WorkingCurrency);
            var maximumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(product.MaximumCustomerEnteredPrice, _workContext.WorkingCurrency);

            model.CustomerEnteredPrice = updatecartitem != null ? updatecartitem.CustomerEnteredPrice : minimumCustomerEnteredPrice;
            model.CustomerEnteredPriceRange = string.Format(_localizationService.GetResource("Products.EnterProductPrice.Range"),
                _priceFormatter.FormatPrice(minimumCustomerEnteredPrice, false, false),
                _priceFormatter.FormatPrice(maximumCustomerEnteredPrice, false, false));

            return model;
        }

        /// <summary>
        /// Prepare the product attribute models
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <returns>List of product attribute model</returns>
        protected virtual IList<ProductDetailsModel.ProductAttributeModel> PrepareProductAttributeModels(Product product, ShoppingCartItem updatecartitem)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = new List<ProductDetailsModel.ProductAttributeModel>();

            var productAttributeMapping = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in productAttributeMapping)
            {
                var productAttrubute = _productAttributeService.GetProductAttributeById(attribute.ProductAttributeId);

                var attributeModel = new ProductDetailsModel.ProductAttributeModel
                {
                    Id = attribute.Id,
                    ProductId = product.Id,
                    ProductAttributeId = attribute.ProductAttributeId,
                    Name = !String.IsNullOrEmpty(attribute.TextPrompt) ? _localizationService.GetLocalized(attribute, x => x.TextPrompt) :  _localizationService.GetLocalized(productAttrubute, x => x.Name),
                    Description = _localizationService.GetLocalized(productAttrubute, x => x.Description),
                    TextPrompt = _localizationService.GetLocalized(attribute, x => x.TextPrompt),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = updatecartitem != null ? null : _localizationService.GetLocalized(attribute, x => x.DefaultValue),
                    HasCondition = !string.IsNullOrEmpty(attribute.ConditionAttributeXml)
                };
                if (!string.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                {
                    attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new ProductDetailsModel.ProductAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = _localizationService.GetLocalized(attributeValue, x => x.Name),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb, //used with "Color squares" attribute type
                            IsPreSelected = attributeValue.IsPreSelected,
                            CustomerEntersQty = attributeValue.CustomerEntersQty,
                            Quantity = attributeValue.Quantity
                        };
                        attributeModel.Values.Add(valueModel);

                        //display price if allowed
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                        {
                            var customer = updatecartitem?.CustomerId is null ? _workContext.CurrentCustomer : _customerService.GetCustomerById(updatecartitem.CustomerId);

                            var attributeValuePriceAdjustment = _priceCalculationService.GetProductAttributeValuePriceAdjustment(product, attributeValue, customer);
                            var priceAdjustmentBase = _taxService.GetProductPrice(product, attributeValuePriceAdjustment, out var _);
                            var priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);

                            if (attributeValue.PriceAdjustmentUsePercentage)
                            {
                                var priceAdjustmentStr = attributeValue.PriceAdjustment.ToString("G29");
                                if (attributeValue.PriceAdjustment > decimal.Zero)
                                    valueModel.PriceAdjustment = "+";
                                valueModel.PriceAdjustment += priceAdjustmentStr + "%";
                            }
                            else
                            {
                                if (priceAdjustmentBase > decimal.Zero)
                                    valueModel.PriceAdjustment = "+" + _priceFormatter.FormatPrice(priceAdjustment, false, false);
                                else if (priceAdjustmentBase < decimal.Zero)
                                    valueModel.PriceAdjustment = "-" + _priceFormatter.FormatPrice(-priceAdjustment, false, false);
                            }

                            valueModel.PriceAdjustmentValue = priceAdjustment;
                        }

                        //"image square" picture (with with "image squares" attribute type only)
                        if (attributeValue.ImageSquaresPictureId > 0)
                        {
                            var productAttributeImageSquarePictureCacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductAttributeImageSquarePictureModelKey
                                , attributeValue.ImageSquaresPictureId,
                                    _webHelper.IsCurrentConnectionSecured(),
                                    _storeContext.CurrentStore);
                            valueModel.ImageSquaresPictureModel = _staticCacheManager.Get(productAttributeImageSquarePictureCacheKey, () =>
                            {
                                var imageSquaresPicture = _pictureService.GetPictureById(attributeValue.ImageSquaresPictureId);

                                if (imageSquaresPicture != null)
                                {
                                    return new PictureModel
                                    {
                                        FullSizeImageUrl = _pictureService.GetPictureUrl(ref imageSquaresPicture),
                                        ImageUrl = _pictureService.GetPictureUrl(ref imageSquaresPicture, _mediaSettings.ImageSquarePictureSize)
                                    };
                                }

                                return new PictureModel();
                            });
                        }

                        //picture of a product attribute value
                        valueModel.PictureId = attributeValue.PictureId;
                    }
                }

                //set already selected attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.Checkboxes:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                            {
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    //clear default selection
                                    foreach (var item in attributeModel.Values)
                                        item.IsPreSelected = false;

                                    //select new values
                                    var selectedValues = _productAttributeParser.ParseProductAttributeValues(updatecartitem.AttributesXml);
                                    foreach (var attributeValue in selectedValues)
                                        foreach (var item in attributeModel.Values)
                                            if (attributeValue.Id == item.Id)
                                            {
                                                item.IsPreSelected = true;

                                                //set customer entered quantity
                                                if (attributeValue.CustomerEntersQty)
                                                    item.Quantity = attributeValue.Quantity;
                                            }
                                }
                            }

                            break;
                        case AttributeControlType.ReadonlyCheckboxes:
                            {
                                //values are already pre-set

                                //set customer entered quantity
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    foreach (var attributeValue in _productAttributeParser.ParseProductAttributeValues(updatecartitem.AttributesXml)
                                        .Where(value => value.CustomerEntersQty))
                                    {
                                        var item = attributeModel.Values.FirstOrDefault(value => value.Id == attributeValue.Id);
                                        if (item != null)
                                            item.Quantity = attributeValue.Quantity;
                                    }
                                }
                            }

                            break;
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                            {
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    var enteredText = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                                    if (enteredText.Any())
                                        attributeModel.DefaultValue = enteredText[0];
                                }
                            }

                            break;
                        case AttributeControlType.Datepicker:
                            {
                                //keep in mind my that the code below works only in the current culture
                                var selectedDateStr = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                                if (selectedDateStr.Any())
                                {
                                    if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture, DateTimeStyles.None, out var selectedDate))
                                    {
                                        //successfully parsed
                                        attributeModel.SelectedDay = selectedDate.Day;
                                        attributeModel.SelectedMonth = selectedDate.Month;
                                        attributeModel.SelectedYear = selectedDate.Year;
                                    }
                                }
                            }

                            break;
                        case AttributeControlType.FileUpload:
                            {
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    var downloadGuidStr = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id).FirstOrDefault();
                                    Guid.TryParse(downloadGuidStr, out var downloadGuid);
                                    var download = _downloadService.GetDownloadByGuid(downloadGuid);
                                    if (download != null)
                                        attributeModel.DefaultValue = download.DownloadGuid.ToString();
                                }
                            }

                            break;
                        default:
                            break;
                    }
                }

                model.Add(attributeModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the product tier price models
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>List of tier price model</returns>
        protected virtual IList<ProductDetailsModel.TierPriceModel> PrepareProductTierPriceModels(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = _productService.GetTierPrices(product, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id)
                   .Select(tierPrice =>
                {
                    var priceBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product,
                        _workContext.CurrentCustomer, decimal.Zero, _catalogSettings.DisplayTierPricesWithDiscounts,
                        tierPrice.Quantity), out var _);

                    var price = _currencyService.ConvertFromPrimaryStoreCurrency(priceBase, _workContext.WorkingCurrency);

                    return new ProductDetailsModel.TierPriceModel
                    {
                        Quantity = tierPrice.Quantity,
                        Price = _priceFormatter.FormatPrice(price, false, false)
                    };
                }).ToList();

            return model;
        }

        /// <summary>
        /// Prepare the product manufacturer models
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>List of manufacturer brief info model</returns>
        protected virtual IList<ManufacturerBriefInfoModel> PrepareProductManufacturerModels(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = _manufacturerService.GetProductManufacturersByProductId(product.Id)
                .Select(pm =>
                {
                    var manufacturer = _manufacturerService.GetManufacturerById(pm.ManufacturerId);
                    var modelMan = new ManufacturerBriefInfoModel
                    {
                        Id = manufacturer.Id,
                        Name = _localizationService.GetLocalized(manufacturer, x => x.Name),
                        SeName = _urlRecordService.GetSeName(manufacturer)
                    };

                    return modelMan;
                }).ToList();

            return model;
        }

        /// <summary>
        /// Prepare the product details picture model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="isAssociatedProduct">Whether the product is associated</param>
        /// <param name="allPictureModels">All picture models</param>
        /// <returns>Picture model for the default picture</returns>
        public virtual PictureModel PrepareProductDetailsPictureModel(Product product, bool isAssociatedProduct, out IList<PictureModel> allPictureModels)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //default picture size
            var defaultPictureSize = isAssociatedProduct ?
                _mediaSettings.AssociatedProductPictureSize :
                _mediaSettings.ProductDetailsPictureSize;

            //prepare picture models
            var productPicturesCacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductDetailsPicturesModelKey
                , product, defaultPictureSize, isAssociatedProduct,
                _workContext.WorkingLanguage, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore);
            var cachedPictures = _staticCacheManager.Get(productPicturesCacheKey, () =>
            {
                var productName = _localizationService.GetLocalized(product, x => x.Name);

                var pictures = _pictureService.GetPicturesByProductId(product.Id);
                var defaultPicture = pictures.FirstOrDefault();
                var defaultPictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(ref defaultPicture, defaultPictureSize, !isAssociatedProduct),
                    FullSizeImageUrl = _pictureService.GetPictureUrl(ref defaultPicture, 0, !isAssociatedProduct)
                };
                //"title" attribute
                defaultPictureModel.Title = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.TitleAttribute)) ?
                    defaultPicture.TitleAttribute :
                    string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat.Details"), productName);
                //"alt" attribute
                defaultPictureModel.AlternateText = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.AltAttribute)) ?
                    defaultPicture.AltAttribute :
                    string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat.Details"), productName);

                //all pictures
                var pictureModels = new List<PictureModel>();
                for (var i = 0; i < pictures.Count(); i++)
                {
                    var picture = pictures[i];
                    var pictureModel = new PictureModel
                    {
                        ImageUrl = _pictureService.GetPictureUrl(ref picture, defaultPictureSize, !isAssociatedProduct),
                        ThumbImageUrl = _pictureService.GetPictureUrl(ref picture, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(ref picture),
                        Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat.Details"), productName),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat.Details"), productName),
                    };
                    //"title" attribute
                    pictureModel.Title = !string.IsNullOrEmpty(picture.TitleAttribute) ?
                        picture.TitleAttribute :
                        string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat.Details"), productName);
                    //"alt" attribute
                    pictureModel.AlternateText = !string.IsNullOrEmpty(picture.AltAttribute) ?
                        picture.AltAttribute :
                        string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat.Details"), productName);

                    pictureModels.Add(pictureModel);
                }

                return new { DefaultPictureModel = defaultPictureModel, PictureModels = pictureModels };
            });

            allPictureModels = cachedPictures.PictureModels;
            return cachedPictures.DefaultPictureModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the product template view path
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>View path</returns>
        public virtual string PrepareProductTemplateViewPath(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var template = _productTemplateService.GetProductTemplateById(product.ProductTemplateId) ??
                           _productTemplateService.GetAllProductTemplates().FirstOrDefault();

            if (template == null)
                throw new Exception("No default template could be loaded");

            return template.ViewPath;
        }

        /// <summary>
        /// Prepare the product overview models
        /// </summary>
        /// <param name="products">Collection of products</param>
        /// <param name="preparePriceModel">Whether to prepare the price model</param>
        /// <param name="preparePictureModel">Whether to prepare the picture model</param>
        /// <param name="productThumbPictureSize">Product thumb picture size (longest side); pass null to use the default value of media settings</param>
        /// <param name="prepareSpecificationAttributes">Whether to prepare the specification attribute models</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>Collection of product overview model</returns>
        public virtual IEnumerable<ProductOverviewModel> PrepareProductOverviewModels(IEnumerable<Product> products,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false,
            bool showState = false)
        {
            if (products == null)
                throw new ArgumentNullException(nameof(products));

            var models = new List<ProductOverviewModel>();

            var vendorIds = products.Select(x => x.VendorId).ToList();
            var vendors = _vendorService.GetVendorsByIds(vendorIds.ToArray());

            IList<Address> addresses = new List<Address>();
            IList<AddressModel> addresseModels = new List<AddressModel>();

            if (showState)
            {
                addresses = _addressService.GetAddressByIds(vendors.Where(x => x.AddressId != 0).Select(x => x.AddressId).ToArray());
                var states = _stateProvinceService.GetStateProvinces();

                foreach (var address in addresses)
                {
                    addresseModels.Add(new AddressModel
                    {
                        Id = address.Id,
                        StateProvinceName = states
                            .Where(x => x.Id == address.StateProvinceId)
                            .Select(x => x.Name)
                            .FirstOrDefault()
                    });
                }
            }

            foreach (var product in products)
            {
                var vendor = vendors.Where(x => x.Id == product.VendorId).FirstOrDefault();
                var addressModel = addresseModels.Where(x => x.Id == vendor.AddressId).FirstOrDefault();

                var breadcrumbModel = new ProductDetailsModel.ProductBreadcrumbModel
                {
                    Enabled = _catalogSettings.CategoryBreadcrumbEnabled,
                    ProductId = product.Id,
                    ProductName = _localizationService.GetLocalized(product, x => x.Name),
                    ProductSeName = _urlRecordService.GetSeName(product)
                };
                var productCategories = _categoryService.GetProductCategoriesByProductId(product.Id);

                if (productCategories.Count > 0)
                {
                    var category = _categoryService.GetCategoryById(productCategories[0].CategoryId);

                    foreach (var catBr in _categoryService.GetCategoryBreadCrumb(category))
                    {
                        breadcrumbModel.CategoryBreadcrumb.Add(new CategorySimpleModel
                        {
                            Id = catBr.Id,
                            Name = _localizationService.GetLocalized(catBr, x => x.Name),
                            SeName = _urlRecordService.GetSeName(catBr),
                            IncludeInTopMenu = catBr.IncludeInTopMenu
                        });
                    }
                }

                var productCategory = _categoryService.GetProductCategoriesByProductId(product.Id).FirstOrDefault();
                var model = new ProductOverviewModel
                {
                    Id = product.Id,
                    Name = _localizationService.GetLocalized(product, x => x.Name),
                    ShortDescription = _localizationService.GetLocalized(product, x => x.ShortDescription),
                    FullDescription = _localizationService.GetLocalized(product, x => x.FullDescription),
                    SeName = _urlRecordService.GetSeName(product),
                    Sku = product.Sku,
                    ProductType = product.ProductType,
                    MarkAsNew = product.MarkAsNew &&
                        (!product.MarkAsNewStartDateTimeUtc.HasValue || product.MarkAsNewStartDateTimeUtc.Value < DateTime.UtcNow) &&
                        (!product.MarkAsNewEndDateTimeUtc.HasValue || product.MarkAsNewEndDateTimeUtc.Value > DateTime.UtcNow),
                    VendorName = vendor?.Name,
                    VendorStateName = addressModel?.StateProvinceName,
                    ProductBusinessNature = breadcrumbModel.CategoryBreadcrumb[0].Name.Contains("Eat") ? "Eat" : "Mart",
                    ProductSpecifications = PrepareProductSpecificationModel(product),
                    IsEats = _productService.CheckIfIsEats(productCategory.CategoryId),
                    IsMart = _productService.CheckIfIsMart(productCategory.CategoryId),
                    VendorOnline = vendor != null && _vendorService.GetVendorOnlineStatus(vendor)
                };

                if (product.DeliveryDates.Count > 0 && !product.DeliveryDates.Any(x => x >= DateTime.Today))
                {
                    model.PastSeasonal = true;
                }

                model.SpecificationAttributeModels = PrepareProductSpecificationModel(product);

                if (vendor != null && !vendor.Deleted && vendor.Active)
                {
                    if (vendor.AddressId != 0)
                    {
                        var vendorAddress = _addressService.GetAddressById(vendor.AddressId);
                        model.Address.Address = vendorAddress;
                        if (model.Address.Address != null && model.Address.Address.StateProvinceId != 0)
                        {
                            var state = _stateProvinceService.GetStateProvinceById(model.Address.Address.StateProvinceId.Value);
                            model.Address.State = state?.Name;
                        }
                    }
                }
                

                //price
                if (preparePriceModel)
                {
                    model.ProductPrice = PrepareProductOverviewPriceModel(product, forceRedirectionAfterAddingToCart);
                }

                //picture
                if (preparePictureModel)
                {
                    model.DefaultPictureModel = PrepareProductOverviewPictureModel(product, productThumbPictureSize);
                }

                //specs
                if (prepareSpecificationAttributes)
                {
                    model.SpecificationAttributeModels = PrepareProductSpecificationModel(product);
                }

                //reviews
                model.ReviewOverviewModel = PrepareProductReviewOverviewModel(product);
                model.ReviewOverviewModel.ProductBusinessNature = model.ProductBusinessNature;
                models.Add(model);
            }

            return models;
        }

        /// <summary>
        /// Prepare the product details model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <param name="isAssociatedProduct">Whether the product is associated</param>
        /// <returns>Product details model</returns>
        public virtual ProductDetailsModel PrepareProductDetailsModel(Product product,
            ShoppingCartItem updatecartitem = null, bool isAssociatedProduct = false, bool isAdditionalProduct = false)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //standard properties
            var model = new ProductDetailsModel
            {
                Id = product.Id,
                Name = _localizationService.GetLocalized(product, x => x.Name),
                ShortDescription = _localizationService.GetLocalized(product, x => x.ShortDescription),
                FullDescription = _localizationService.GetLocalized(product, x => x.FullDescription),
                MetaKeywords = _localizationService.GetLocalized(product, x => x.MetaKeywords),
                MetaDescription = _localizationService.GetLocalized(product, x => x.MetaDescription),
                MetaTitle = _localizationService.GetLocalized(product, x => x.MetaTitle),
                SeName = _urlRecordService.GetSeName(product),
                ProductType = product.ProductType,
                ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage,
                Sku = product.Sku,
                ShowManufacturerPartNumber = _catalogSettings.ShowManufacturerPartNumber,
                FreeShippingNotificationEnabled = _catalogSettings.ShowFreeShippingNotification,
                ManufacturerPartNumber = product.ManufacturerPartNumber,
                ShowGtin = _catalogSettings.ShowGtin,
                Gtin = product.Gtin,
                ManageInventoryMethod = product.ManageInventoryMethod,
                StockAvailability = _productService.FormatStockMessage(product, string.Empty),
                HasSampleDownload = product.IsDownload && product.HasSampleDownload,
                DisplayDiscontinuedMessage = !product.Published && _catalogSettings.DisplayDiscontinuedMessageForUnpublishedProducts,
                AvailableEndDate = product.AvailableEndDateTimeUtc,
                ProductWeight = product.Weight,
            };

            var customer = _workContext.CurrentCustomer;

            if (customer != null)
            {
                var wishList = _shoppingCartService.GetShoppingCart(customer, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id);

                model.IsInWishList = wishList.Any(x => x.ProductId == product.Id);
            }


            //automatically generate product description?
            if (_seoSettings.GenerateProductMetaDescription && string.IsNullOrEmpty(model.MetaDescription))
            {
                //based on short description
                model.MetaDescription = model.ShortDescription;
            }

            //shipping info
            model.IsShipEnabled = product.IsShipEnabled;
            if (product.IsShipEnabled)
            {
                model.IsFreeShipping = product.IsFreeShipping;
                //delivery date
                var deliveryDate = _dateRangeService.GetDeliveryDateById(product.DeliveryDateId);
                if (deliveryDate != null)
                {
                    model.DeliveryDate = _localizationService.GetLocalized(deliveryDate, dd => dd.Name);
                }
            }

            //email a friend
            model.EmailAFriendEnabled = _catalogSettings.EmailAFriendEnabled;
            //compare products
            model.CompareProductsEnabled = _catalogSettings.CompareProductsEnabled;
            //store name
            model.CurrentStoreName = _localizationService.GetLocalized(_storeContext.CurrentStore, x => x.Name);

            //vendor details
            if (_vendorSettings.ShowVendorOnProductDetailsPage)
            {
                var vendor = _vendorService.GetVendorById(product.VendorId);
                if (vendor != null && !vendor.Deleted && vendor.Active)
                {
                    model.ShowVendor = true;
                    model.VendorModel = new VendorBriefInfoModel
                    {
                        Id = vendor.Id,
                        Name = _localizationService.GetLocalized(vendor, x => x.Name),
                        SeName = _urlRecordService.GetSeName(vendor),
                    };

                    if (vendor.PictureId != 0)
                    {
                        var vendorPicture = _pictureService.GetPictureUrl(vendor.PictureId);
                        model.VendorModel.PictureUrl = vendorPicture;
                    }

                    if (vendor.AddressId != 0)
                    {
                        var vendorAddress = _addressService.GetAddressById(vendor.AddressId);
                        model.VendorModel.Address = vendorAddress;
                        if (model.VendorModel.Address != null && model.VendorModel.Address.StateProvinceId != 0)
                        {
                            var state = _stateProvinceService.GetStateProvinceById(model.VendorModel.Address.StateProvinceId.Value);
                            model.VendorModel.StateProvince = state?.Name;
                        }
                    }

                    model.VendorModel.ProductQty = _productService.GetNumberOfProductsByVendorId(vendor.Id);
                    model.VendorModel.RatingSum = 3;
                    model.VendorModel.TotalReviews = 1;
                    model.VendorModel.Online = _vendorService.GetVendorOnlineStatus(vendor); 
                }
            }

            //page sharing
            if (_catalogSettings.ShowShareButton && !string.IsNullOrEmpty(_catalogSettings.PageShareCode))
            {
                var shareCode = _catalogSettings.PageShareCode;
                if (_webHelper.IsCurrentConnectionSecured())
                {
                    //need to change the add this link to be https linked when the page is, so that the page doesn't ask about mixed mode when viewed in https...
                    shareCode = shareCode.Replace("http://", "https://");
                }

                model.PageShareCode = shareCode;
            }

            //back in stock subscriptions
            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                _productService.GetTotalStockQuantity(product) <= 0)
            {
                //out of stock
                model.DisplayBackInStockSubscription = true;
            }

            //breadcrumb
            //do not prepare this model for the associated products. anyway it's not used
            if (_catalogSettings.CategoryBreadcrumbEnabled && !isAssociatedProduct && !isAdditionalProduct)
            {
                model.Breadcrumb = PrepareProductBreadcrumbModel(product);
            }

            //product tags
            //do not prepare this model for the associated products. anyway it's not used
            if (!isAssociatedProduct && !isAdditionalProduct)
            {
                model.ProductTags = PrepareProductTagModels(product);
            }

            //pictures
            model.DefaultPictureZoomEnabled = _mediaSettings.DefaultPictureZoomEnabled;
            model.DefaultPictureModel = PrepareProductDetailsPictureModel(product, isAssociatedProduct, out var allPictureModels);
            model.PictureModels = allPictureModels;

            //price
            model.ProductPrice = PrepareProductPriceModel(product);

            //'Add to cart' model
            model.AddToCart = PrepareProductAddToCartModel(product, updatecartitem);

            //gift card
            if (product.IsGiftCard)
            {
                model.GiftCard.IsGiftCard = true;
                model.GiftCard.GiftCardType = product.GiftCardType;

                if (updatecartitem == null)
                {
                    model.GiftCard.SenderName = _customerService.GetCustomerFullName(_workContext.CurrentCustomer);
                    model.GiftCard.SenderEmail = _workContext.CurrentCustomer.Email;
                }
                else
                {
                    _productAttributeParser.GetGiftCardAttribute(updatecartitem.AttributesXml,
                        out var giftCardRecipientName, out var giftCardRecipientEmail,
                        out var giftCardSenderName, out var giftCardSenderEmail, out var giftCardMessage);

                    model.GiftCard.RecipientName = giftCardRecipientName;
                    model.GiftCard.RecipientEmail = giftCardRecipientEmail;
                    model.GiftCard.SenderName = giftCardSenderName;
                    model.GiftCard.SenderEmail = giftCardSenderEmail;
                    model.GiftCard.Message = giftCardMessage;
                }
            }

            //product attributes
            model.ProductAttributes = PrepareProductAttributeModels(product, updatecartitem);

            //product specifications
            //do not prepare this model for the associated products. anyway it's not used
            if (!isAssociatedProduct && !isAdditionalProduct)
            {
                model.ProductSpecifications = PrepareProductSpecificationModel(product);
            }

            //product review overview
            model.ProductReviewOverview = PrepareProductReviewOverviewModel(product);

            //tier prices
            if (product.HasTierPrices && _permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                model.TierPrices = PrepareProductTierPriceModels(product);
            }

            //manufacturers
            model.ProductManufacturers = PrepareProductManufacturerModels(product);

            //rental products
            if (product.IsRental)
            {
                model.IsRental = true;
                //set already entered dates attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    model.RentalStartDate = updatecartitem.RentalStartDateUtc;
                    model.RentalEndDate = updatecartitem.RentalEndDateUtc;
                }
            }

            //estimate shipping
            if (_shippingSettings.EstimateShippingProductPageEnabled && !model.IsFreeShipping)
            {
                var wrappedProduct = new ShoppingCartItem()
                {
                    StoreId = _storeContext.CurrentStore.Id,
                    ShoppingCartTypeId = (int)ShoppingCartType.ShoppingCart,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    ProductId = product.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };

                var estimateShippingModel = _shoppingCartModelFactory.PrepareEstimateShippingModel(new[] { wrappedProduct });

                model.ProductEstimateShipping.ProductId = product.Id;
                model.ProductEstimateShipping.Enabled = estimateShippingModel.Enabled;
                model.ProductEstimateShipping.CountryId = estimateShippingModel.CountryId;
                model.ProductEstimateShipping.StateProvinceId = estimateShippingModel.StateProvinceId;
                model.ProductEstimateShipping.ZipPostalCode = estimateShippingModel.ZipPostalCode;
                model.ProductEstimateShipping.AvailableCountries = estimateShippingModel.AvailableCountries;
                model.ProductEstimateShipping.AvailableStates = estimateShippingModel.AvailableStates;
            }
            else
                model.ProductEstimateShipping.Enabled = false;

            //associated products
            if (product.ProductType == ProductType.GroupedProduct)
            {
                //ensure no circular references
                if (!isAssociatedProduct)
                {
                    var associatedProducts = _productService.GetAssociatedProducts(product.Id, _storeContext.CurrentStore.Id);
                    foreach (var associatedProduct in associatedProducts)
                        model.AssociatedProducts.Add(PrepareProductDetailsModel(associatedProduct, null, true, false));
                }
            }

            if (!isAdditionalProduct)
            {
                var additionalProducts = _productService.GetAdditionalProductsByVendorId(product.Id, product.VendorId);
                foreach (var additionalProduct in additionalProducts)
                    model.AdditionalProducts.Add(PrepareProductDetailsModel(additionalProduct, null, false, true));

            }
            return model;

        }

        public virtual IList<ProductReviewModel> PrepareProductReviewModel(IList<ProductReviewModel> model, 
            IList<Product> product, 
            int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            IEnumerable<ProductReview> list = Enumerable.Empty<ProductReview>();
            foreach (var item in product)
            {
                var review = _productService.GetAllProductReviews(
                    approved: true,
                    productId: item.Id,
                    storeId: _catalogSettings.ShowProductReviewsPerStore ? _storeContext.CurrentStore.Id : 0);

                var reviewEnumerable = review.AsEnumerable();
                list = list.Concat(reviewEnumerable);
            }

            var productReviews = list.AsEnumerable();
            productReviews = _catalogSettings.ProductReviewsSortByCreatedDateAscending
                ? productReviews.OrderBy(pr => pr.CreatedOnUtc)
                : productReviews.OrderByDescending(pr => pr.CreatedOnUtc);

            //var query = productReviews.AsQueryable();
            //var productReviewsPaged = new PagedList<ProductReview>(query, pageIndex, pageSize);
            //filling data from db
            foreach (var pr in productReviews)
            {
                var customer = _customerService.GetCustomerById(pr.CustomerId);
                var pictureIds = _productService.GetProducReviewsPicturesById(pr.Id);
                var productReviewModel = new ProductReviewModel
                {
                    Id = pr.Id,
                    CustomerId = pr.CustomerId,
                    CustomerName = _customerService.FormatUsername(customer),
                    AllowViewingProfiles = _customerSettings.AllowViewingProfiles && customer != null && !_customerService.IsGuest(customer),
                    Title = pr.Title,
                    ReviewText = pr.ReviewText,
                    ReplyText = pr.ReplyText,
                    Rating = pr.Rating,
                    Helpfulness = new ProductReviewHelpfulnessModel
                    {
                        ProductReviewId = pr.Id,
                        HelpfulYesTotal = pr.HelpfulYesTotal,
                        HelpfulNoTotal = pr.HelpfulNoTotal,
                    },
                    WrittenOnStr = _dateTimeHelper.ConvertToUserTime(pr.CreatedOnUtc, DateTimeKind.Utc).ToString("g"),
                    PictureUrl = pr.PictureId != null ? _pictureService.GetPictureUrl(pr.PictureId.Value) : ""
                };

                if (pictureIds != null)
                {
                    foreach (var p in pictureIds)
                    {
                        var pictureUrl = _pictureService.GetPictureUrl(p);
                        productReviewModel.PictureUrls.Add(pictureUrl);
                    }
                }


                if (_customerSettings.AllowCustomersToUploadAvatars)
                {
                    productReviewModel.CustomerAvatarUrl = _pictureService.GetPictureUrl(
                        _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                        _mediaSettings.AvatarPictureSize, _customerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
                }

                foreach (var q in _reviewTypeService.GetProductReviewReviewTypeMappingsByProductReviewId(pr.Id))
                {
                    var reviewType = _reviewTypeService.GetReviewTypeById(q.ReviewTypeId);

                    productReviewModel.AdditionalProductReviewList.Add(new ProductReviewReviewTypeMappingModel
                    {
                        ReviewTypeId = q.ReviewTypeId,
                        ProductReviewId = pr.Id,
                        Rating = q.Rating,
                        Name = _localizationService.GetLocalized(reviewType, x => x.Name),
                        VisibleToAllCustomers = reviewType.VisibleToAllCustomers || _workContext.CurrentCustomer.Id == pr.CustomerId,
                    });
                }

                model.Add(productReviewModel);
            }
            
            return model;
        }

        /// <summary>
        /// Prepare the product reviews model
        /// </summary>
        /// <param name="model">Product reviews model</param>
        /// <param name="product">Product</param>
        /// <returns>Product reviews model</returns>
        public virtual ProductReviewsModel PrepareProductReviewsModel(ProductReviewsModel model, Product product, int? page = 0)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var pageSize = _catalogSettings.ProductReviewsPageSizeOnAccountPage;
            var pageIndex = 0;

            if (page > 0)
            {
                pageIndex = page.Value - 1;
            }

            model.ProductId = product.Id;
            model.ProductName = _localizationService.GetLocalized(product, x => x.Name);
            model.ProductSeName = _urlRecordService.GetSeName(product);

            var list = _productService.GetAllProductReviews(
                approved: true,
                productId: product.Id,
                storeId: _catalogSettings.ShowProductReviewsPerStore ? _storeContext.CurrentStore.Id : 0,
                pageIndex: pageIndex,
                pageSize: pageSize);

            var productReviews = list.AsEnumerable();
            productReviews = _catalogSettings.ProductReviewsSortByCreatedDateAscending
                ? productReviews.OrderBy(pr => pr.CreatedOnUtc)
                : productReviews.OrderByDescending(pr => pr.CreatedOnUtc);

            //get all review types
            foreach (var reviewType in _reviewTypeService.GetAllReviewTypes())
            {
                model.ReviewTypeList.Add(new ReviewTypeModel
                {
                    Id = reviewType.Id,
                    Name = _localizationService.GetLocalized(reviewType, entity => entity.Name),
                    Description = _localizationService.GetLocalized(reviewType, entity => entity.Description),
                    VisibleToAllCustomers = reviewType.VisibleToAllCustomers,
                    DisplayOrder = reviewType.DisplayOrder,
                    IsRequired = reviewType.IsRequired,
                });
            }

            //filling data from db
            foreach (var pr in productReviews)
            {
                var customer = _customerService.GetCustomerById(pr.CustomerId);
                var pictureIds = _productService.GetProducReviewsPicturesById(pr.Id);
                var productReviewModel = new ProductReviewModel
                {
                    Id = pr.Id,
                    CustomerId = pr.CustomerId,
                    CustomerName = _customerService.FormatUsername(customer),
                    AllowViewingProfiles = _customerSettings.AllowViewingProfiles && customer != null && !_customerService.IsGuest(customer),
                    Title = pr.Title,
                    ReviewText = pr.ReviewText,
                    ReplyText = pr.ReplyText,
                    Rating = pr.Rating,
                    Helpfulness = new ProductReviewHelpfulnessModel
                    {
                        ProductReviewId = pr.Id,
                        HelpfulYesTotal = pr.HelpfulYesTotal,
                        HelpfulNoTotal = pr.HelpfulNoTotal,
                        CheckBuyerVoted = _productService.CheckBuyerVotedHelpfulness(pr.Id, _workContext.CurrentCustomer.Id)
                    },
                    WrittenOnStr = _dateTimeHelper.ConvertToUserTime(pr.CreatedOnUtc, DateTimeKind.Utc).ToString("g"),
                    PictureUrl = pr.PictureId != null ? _pictureService.GetPictureUrl(pr.PictureId.Value) : ""
                };

                if (pictureIds != null)
                {
                    foreach (var p in pictureIds)
                    {
                        var pictureUrl = _pictureService.GetPictureUrl(p);
                        productReviewModel.PictureUrls.Add(pictureUrl);
                    }
                }


                if (_customerSettings.AllowCustomersToUploadAvatars)
                {
                    productReviewModel.CustomerAvatarUrl = _pictureService.GetPictureUrl(
                        _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                        _mediaSettings.AvatarPictureSize, _customerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
                }

                foreach (var q in _reviewTypeService.GetProductReviewReviewTypeMappingsByProductReviewId(pr.Id))
                {
                    var reviewType = _reviewTypeService.GetReviewTypeById(q.ReviewTypeId);

                    productReviewModel.AdditionalProductReviewList.Add(new ProductReviewReviewTypeMappingModel
                    {
                        ReviewTypeId = q.ReviewTypeId,
                        ProductReviewId = pr.Id,
                        Rating = q.Rating,
                        Name = _localizationService.GetLocalized(reviewType, x => x.Name),
                        VisibleToAllCustomers = reviewType.VisibleToAllCustomers || _workContext.CurrentCustomer.Id == pr.CustomerId,
                    });
                }

                model.Items.Add(productReviewModel);
            }

            foreach (var rt in model.ReviewTypeList)
            {
                if (model.ReviewTypeList.Count <= model.AddAdditionalProductReviewList.Count)
                    continue;
                var reviewType = _reviewTypeService.GetReviewTypeById(rt.Id);
                var reviewTypeMappingModel = new AddProductReviewReviewTypeMappingModel
                {
                    ReviewTypeId = rt.Id,
                    Name = _localizationService.GetLocalized(reviewType, entity => entity.Name),
                    Description = _localizationService.GetLocalized(reviewType, entity => entity.Description),
                    DisplayOrder = rt.DisplayOrder,
                    IsRequired = rt.IsRequired,
                };

                model.AddAdditionalProductReviewList.Add(reviewTypeMappingModel);
            }

            //Average rating
            foreach (var rtm in model.ReviewTypeList)
            {
                var totalRating = 0;
                var totalCount = 0;
                foreach (var item in model.Items)
                {
                    foreach (var q in item.AdditionalProductReviewList.Where(w => w.ReviewTypeId == rtm.Id))
                    {
                        totalRating += q.Rating;
                        totalCount = ++totalCount;
                    }
                }

                rtm.AverageRating = (double)totalRating / (totalCount > 0 ? totalCount : 1);
            }

            model.AddProductReview.CanCurrentCustomerLeaveReview = _catalogSettings.AllowAnonymousUsersToReviewProduct || !_customerService.IsGuest(_workContext.CurrentCustomer);
            model.AddProductReview.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnProductReviewPage;


            //var pagerModel = new PagerModel
            //{
            //    PageSize = list.PageSize,
            //    TotalRecords = list.TotalCount,
            //    PageIndex = list.PageIndex,
            //    ShowTotalSummary = false,
            //    RouteActionName = "ProductReviewsPaged",
            //    UseRouteLinks = true,
            //    RouteValues = new ProductReviewsModel.ProductReviewsRouteValues { pageNumber = pageIndex }
            //};

            //model.PagerModel = pagerModel;


            return model;
        }

        /// <summary>
        /// Prepare the customer product reviews model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>Customer product reviews model</returns>
        public virtual CustomerProductReviewsModel PrepareCustomerProductReviewsModel(int? page)
        {
            var pageSize = _catalogSettings.ProductReviewsPageSizeOnAccountPage;
            var pageIndex = 0;

            if (page > 0)
            {
                pageIndex = page.Value - 1;
            }

            var list = _productService.GetAllProductReviews(customerId: _workContext.CurrentCustomer.Id,
                approved: null,
                storeId: _storeContext.CurrentStore.Id,
                pageIndex: pageIndex,
                pageSize: pageSize);

            var productReviews = new List<CustomerProductReviewModel>();

            foreach (var review in list)
            {
                var product = _productService.GetProductById(review.ProductId);

                var productReviewModel = new CustomerProductReviewModel
                {
                    Title = review.Title,
                    ProductId = product.Id,
                    ProductName = _localizationService.GetLocalized(product, p => p.Name),
                    ProductSeName = _urlRecordService.GetSeName(product),
                    Rating = review.Rating,
                    ReviewText = review.ReviewText,
                    ReplyText = review.ReplyText,
                    WrittenOnStr = _dateTimeHelper.ConvertToUserTime(review.CreatedOnUtc, DateTimeKind.Utc).ToString("g")
                };

                if (_catalogSettings.ProductReviewsMustBeApproved)
                {
                    productReviewModel.ApprovalStatus = review.IsApproved
                        ? _localizationService.GetResource("Account.CustomerProductReviews.ApprovalStatus.Approved")
                        : _localizationService.GetResource("Account.CustomerProductReviews.ApprovalStatus.Pending");
                }

                foreach (var q in _reviewTypeService.GetProductReviewReviewTypeMappingsByProductReviewId(review.Id))
                {
                    var reviewType = _reviewTypeService.GetReviewTypeById(q.ReviewTypeId);

                    productReviewModel.AdditionalProductReviewList.Add(new ProductReviewReviewTypeMappingModel
                    {
                        ReviewTypeId = q.ReviewTypeId,
                        ProductReviewId = review.Id,
                        Rating = q.Rating,
                        Name = _localizationService.GetLocalized(reviewType, x => x.Name),
                    });
                }

                productReviews.Add(productReviewModel);
            }

            var pagerModel = new PagerModel
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "CustomerProductReviewsPaged",
                UseRouteLinks = true,
                RouteValues = new CustomerProductReviewsModel.CustomerProductReviewsRouteValues { pageNumber = pageIndex }
            };

            var model = new CustomerProductReviewsModel
            {
                ProductReviews = productReviews,
                PagerModel = pagerModel
            };

            return model;
        }

        /// <summary>
        /// Prepare the product email a friend model
        /// </summary>
        /// <param name="model">Product email a friend model</param>
        /// <param name="product">Product</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>product email a friend model</returns>
        public virtual ProductEmailAFriendModel PrepareProductEmailAFriendModel(ProductEmailAFriendModel model, Product product, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            model.ProductId = product.Id;
            model.ProductName = _localizationService.GetLocalized(product, x => x.Name);
            model.ProductSeName = _urlRecordService.GetSeName(product);
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnEmailProductToFriendPage;
            if (!excludeProperties)
            {
                model.YourEmailAddress = _workContext.CurrentCustomer.Email;
            }

            return model;
        }

        /// <summary>
        /// Prepare the product specification models
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>List of product specification model</returns>
        public virtual IList<ProductSpecificationModel> PrepareProductSpecificationModel(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            return _specificationAttributeService.GetProductSpecificationAttributes(product.Id, 0, null, true)
                .Select(psa =>
                {
                    var specAttributeOption =
                        _specificationAttributeService.GetSpecificationAttributeOptionById(
                            psa.SpecificationAttributeOptionId);
                    var specAttribute =
                        _specificationAttributeService.GetSpecificationAttributeById(specAttributeOption
                            .SpecificationAttributeId);

                    var m = new ProductSpecificationModel
                    {
                        SpecificationAttributeId = specAttribute.Id,
                        SpecificationAttributeName = _localizationService.GetLocalized(specAttribute, x => x.Name),
                        ColorSquaresRgb = specAttributeOption.ColorSquaresRgb,
                        AttributeTypeId = psa.AttributeTypeId,
                        PictureId = specAttributeOption.PictureId,
                        PictureUrl = specAttributeOption.PictureId.HasValue ? _pictureService.GetPictureUrl(specAttributeOption.PictureId.Value) : null,
                        DisplayOrder = psa.DisplayOrder
                    };

                    switch (psa.AttributeType)
                    {
                        case SpecificationAttributeType.Option:
                            m.ValueRaw =
                                WebUtility.HtmlEncode(
                                    _localizationService.GetLocalized(specAttributeOption, x => x.Name));
                            break;
                        case SpecificationAttributeType.CustomText:
                            m.ValueRaw =
                                WebUtility.HtmlEncode(_localizationService.GetLocalized(psa, x => x.CustomValue));
                            m.PictureUrl = null;
                            break;
                        case SpecificationAttributeType.CustomHtmlText:
                            m.ValueRaw = _localizationService.GetLocalized(psa, x => x.CustomValue);
                            m.PictureUrl = null;
                            break;
                        case SpecificationAttributeType.Hyperlink:
                            m.ValueRaw = $"<a href='{psa.CustomValue}' target='_blank'>{psa.CustomValue}</a>";
                            m.PictureUrl = null;
                            break;
                        default:
                            break;
                    }

                    return m;
                }).ToList();
        }

        #endregion
    }
}