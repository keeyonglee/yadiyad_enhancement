using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http.Extensions;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;
using Nop.Web.Models.Vendors;
using Nop.Core.Domain.ShippingShuq.DTO;
using Nop.Services.ShippingShuq;
using static Nop.Web.Models.ShoppingCart.OrderTotalsModel;
using Nop.Services.ShuqOrders;
using Newtonsoft.Json;
using Nop.Core.Domain.Discounts;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the shopping cart model factory
    /// </summary>
    public partial class ShoppingCartModelFactory : IShoppingCartModelFactory
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly CommonSettings _commonSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IVendorAttributeParser _vendorAttributeParser;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDiscountService _discountService;
        private readonly IDownloadService _downloadService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGiftCardService _giftCardService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductService _productService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ITaxService _taxService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorService _vendorService;
        private readonly IVendorAttributeService _vendorAttributeService;
        private readonly IShuqOrderService _shuqOrderService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly OrderSettings _orderSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly TaxSettings _taxSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly ICategoryService _categoryService;
        private readonly IAddressService _addressService;

        #endregion

        #region Ctor

        public ShoppingCartModelFactory(AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            CustomerSettings customerSettings,
            IAddressModelFactory addressModelFactory,
            ICacheKeyService cacheKeyService,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            ICheckoutAttributeParser checkoutAttributeParser,
            IVendorAttributeParser vendorAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IDiscountService discountService,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IProductService productService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IVendorAttributeService vendorAttributeService,
            IShuqOrderService shuqOrderService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            ShoppingCartSettings shoppingCartSettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings,
            ICategoryService categoryService,
            IAddressService addressService)
        {
            _addressSettings = addressSettings;
            _captchaSettings = captchaSettings;
            _catalogSettings = catalogSettings;
            _commonSettings = commonSettings;
            _customerSettings = customerSettings;
            _addressModelFactory = addressModelFactory;
            _cacheKeyService = cacheKeyService;
            _checkoutAttributeFormatter = checkoutAttributeFormatter;
            _checkoutAttributeParser = checkoutAttributeParser;
            _vendorAttributeParser = vendorAttributeParser;
            _checkoutAttributeService = checkoutAttributeService;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _discountService = discountService;
            _downloadService = downloadService;
            _genericAttributeService = genericAttributeService;
            _giftCardService = giftCardService;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _orderProcessingService = orderProcessingService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _productAttributeFormatter = productAttributeFormatter;
            _productService = productService;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _taxService = taxService;
            _urlRecordService = urlRecordService;
            _vendorService = vendorService;
            _vendorAttributeService = vendorAttributeService;
            _shuqOrderService = shuqOrderService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _orderSettings = orderSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _shippingSettings = shippingSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _taxSettings = taxSettings;
            _vendorSettings = vendorSettings;
            _categoryService = categoryService;
            _addressService = addressService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the checkout attribute models
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <returns>List of the checkout attribute model</returns>
        protected virtual IList<ShoppingCartModel.CheckoutAttributeModel> PrepareCheckoutAttributeModels(
            IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var model = new List<ShoppingCartModel.CheckoutAttributeModel>();

            var excludeShippableAttributes = !_shoppingCartService.ShoppingCartRequiresShipping(cart);
            var checkoutAttributes =
                _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id,
                    excludeShippableAttributes);
            foreach (var attribute in checkoutAttributes)
            {
                var attributeModel = new ShoppingCartModel.CheckoutAttributeModel
                {
                    Id = attribute.Id,
                    Name = _localizationService.GetLocalized(attribute, x => x.Name),
                    TextPrompt = _localizationService.GetLocalized(attribute, x => x.TextPrompt),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = _localizationService.GetLocalized(attribute, x => x.DefaultValue)
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
                    var attributeValues = _checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new ShoppingCartModel.CheckoutAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = _localizationService.GetLocalized(attributeValue, x => x.Name),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb,
                            IsPreSelected = attributeValue.IsPreSelected,
                        };
                        attributeModel.Values.Add(attributeValueModel);

                        //display price if allowed
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                        {
                            var priceAdjustmentBase = _taxService.GetCheckoutAttributePrice(attribute, attributeValue);
                            var priceAdjustment =
                                _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase,
                                    _workContext.WorkingCurrency);
                            if (priceAdjustmentBase > decimal.Zero)
                                attributeValueModel.PriceAdjustment =
                                    "+" + _priceFormatter.FormatPrice(priceAdjustment);
                            else if (priceAdjustmentBase < decimal.Zero)
                                attributeValueModel.PriceAdjustment =
                                    "-" + _priceFormatter.FormatPrice(-priceAdjustment);
                        }
                    }
                }

                //set already selected attributes
                var selectedCheckoutAttributes = _genericAttributeService.GetAttribute<string>(
                    _workContext.CurrentCustomer,
                    NopCustomerDefaults.CheckoutAttributes, _storeContext.CurrentStore.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    {
                        if (!string.IsNullOrEmpty(selectedCheckoutAttributes))
                        {
                            //clear default selection
                            foreach (var item in attributeModel.Values)
                                item.IsPreSelected = false;

                            //select new values
                            var selectedValues =
                                _checkoutAttributeParser.ParseCheckoutAttributeValues(selectedCheckoutAttributes);
                            foreach (var attributeValue in selectedValues.SelectMany(x => x.values))
                            foreach (var item in attributeModel.Values)
                                if (attributeValue.Id == item.Id)
                                    item.IsPreSelected = true;
                        }
                    }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                    {
                        //do nothing
                        //values are already pre-set
                    }

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                    {
                        if (!string.IsNullOrEmpty(selectedCheckoutAttributes))
                        {
                            var enteredText =
                                _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                            if (enteredText.Any())
                                attributeModel.DefaultValue = enteredText[0];
                        }
                    }

                        break;
                    case AttributeControlType.Datepicker:
                    {
                        //keep in mind my that the code below works only in the current culture
                        var selectedDateStr =
                            _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                        if (selectedDateStr.Any())
                        {
                            if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                DateTimeStyles.None, out var selectedDate))
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
                        if (!string.IsNullOrEmpty(selectedCheckoutAttributes))
                        {
                            var downloadGuidStr = _checkoutAttributeParser
                                .ParseValues(selectedCheckoutAttributes, attribute.Id).FirstOrDefault();
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

                model.Add(attributeModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the checkout attribute models
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <returns>List of the checkout attribute model</returns>
        protected virtual IList<ShoppingCartModel.CheckoutAttributeModel> PrepareCheckoutAttributeModelsForVendor(
            IList<ShoppingCartItem> cart, int vendorId, out bool deliveryDateClash)
        {
            deliveryDateClash = false;
            var model = new List<ShoppingCartModel.CheckoutAttributeModel>();
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));
            if (cart.Count == 0)
                return model;

            //check vendor is eats/lalamove
            var vendorAttributes = _vendorAttributeService.GetAllVendorAttributes();
            var businessNatureVendorAttribute = vendorAttributes.Where(x => x.Name == "Business Nature").FirstOrDefault();
            if (businessNatureVendorAttribute == null)
            {
                throw new NopException("Fail to identify vendor's business nature.");
            }
            var selectedVendorAttributes = _genericAttributeService.GetAttribute<string>(new Vendor { Id = vendorId }, NopVendorDefaults.VendorAttributes);
            var vendorAttributeValues = _vendorAttributeParser.ParseVendorAttributeValues(selectedVendorAttributes);
            var selectedBusinessNature = vendorAttributeValues.Where(x => x.VendorAttributeId == businessNatureVendorAttribute.Id).FirstOrDefault();
            if (selectedBusinessNature == null)
            {
                throw new NopException("Fail to identify vendor's business nature.");
            }
            if (selectedBusinessNature.Name != "Shuq Eats")
            {
                return model;
            }

            var excludeShippableAttributes = false; // !_shoppingCartService.ShoppingCartRequiresShipping(cart);
            var checkoutAttributes =
                _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id,
                    excludeShippableAttributes);
            foreach (var attribute in checkoutAttributes)
            {
                var attributeModel = new ShoppingCartModel.CheckoutAttributeModel
                {
                    Id = attribute.Id,
                    Name = _localizationService.GetLocalized(attribute, x => x.Name),
                    TextPrompt = _localizationService.GetLocalized(attribute, x => x.TextPrompt),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = _localizationService.GetLocalized(attribute, x => x.DefaultValue)
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
                    var attributeValues = _checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new ShoppingCartModel.CheckoutAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = _localizationService.GetLocalized(attributeValue, x => x.Name),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb,
                            IsPreSelected = attributeValue.IsPreSelected,
                        };
                        attributeModel.Values.Add(attributeValueModel);

                        //display price if allowed
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                        {
                            var priceAdjustmentBase = _taxService.GetCheckoutAttributePrice(attribute, attributeValue);
                            var priceAdjustment =
                                _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase,
                                    _workContext.WorkingCurrency);
                            if (priceAdjustmentBase > decimal.Zero)
                                attributeValueModel.PriceAdjustment =
                                    "+" + _priceFormatter.FormatPrice(priceAdjustment);
                            else if (priceAdjustmentBase < decimal.Zero)
                                attributeValueModel.PriceAdjustment =
                                    "-" + _priceFormatter.FormatPrice(-priceAdjustment);
                        }
                    }
                }

                if (attribute.Name == NopOrderDefaults.DeliveryDate)
                {
                    var operatingDaysVendorAttribute = vendorAttributes.Where(x => x.Name == NopVendorDefaults.VendorOperationDays).FirstOrDefault();
                    var selectedOperatingDays = vendorAttributeValues.Where(x => x.VendorAttributeId == operatingDaysVendorAttribute.Id).ToList();

                    var productIds = cart.Select(x => x.ProductId).ToArray();
                    var products = _productService.GetProductsByIds(productIds);
                    var itemToBeDelivered = _shuqOrderService.GetItemsToBeDeliveredByProductIds(productIds);
                    var advancedOrderDay = products.Max(x => x.AdvancedOrderDay!=null?x.AdvancedOrderDay.Value:1);
                    var numberOfSeasonalItem = products.Where(x => !String.IsNullOrEmpty(x.DeliveryDatesJson)).Count();
                    var numberOfNormalItem = products.Where(x => String.IsNullOrEmpty(x.DeliveryDatesJson)).Count();
                    var isContainSeasonalItems = numberOfSeasonalItem > 0;
                    var seasonalDeliveryDates = products
                        .Where(x => x.DeliveryDates.Count() > 0)
                        .SelectMany(x => x.DeliveryDates)
                        .GroupBy(x => x)
                        .Where(x => x.Count() == numberOfSeasonalItem)
                        .Select(x => Convert.ToDateTime(x.Key))
                        .ToList();
                    if (advancedOrderDay < 1)
                        advancedOrderDay = 1;
                    //var maxOrderDate = 14;
                    var maxOrderDate = _shoppingCartSettings.VendorMaxDeliveryDays != 0 ? _shoppingCartSettings.VendorMaxDeliveryDays : 5;
                    var numberOfDaysInPeriod = maxOrderDate - advancedOrderDay + 1;
                    if (numberOfDaysInPeriod < 0) 
                        numberOfDaysInPeriod = 0;
                    var daysInPeriod = new DateTime[numberOfDaysInPeriod]
                        .ToList()
                        .Select((x, idx) => DateTime.UtcNow.Date.AddDays(advancedOrderDay + idx))
                        .ToList();
                    var availableDays = daysInPeriod
                        .Where(x => numberOfNormalItem == 0 || (numberOfNormalItem > 0 && selectedOperatingDays.Select(y => y.Name.ToLower()).Contains(x.DayOfWeek.ToString().Substring(0, 3).ToLower())))
                        .Where(x =>
                        {
                            var dayOfWeek = x.DayOfWeek.ToString().Substring(0, 3);
                            return cart.Any(y =>
                            {
                                var quantityOrdered = itemToBeDelivered.Find(z => z.DeliveryDate == x && z.ProductId == y.ProductId)?.TotalQuantity ?? 0;
                                var product = products.Where(z => z.Id == y.ProductId).First();
                                int? maxNoOrderQty = product != null ? (int?)product.GetType().GetProperty($"{dayOfWeek}MaxNoOrderQty").GetValue(product, null) : null;
                                return (maxNoOrderQty == null || (y.Quantity + quantityOrdered) <= maxNoOrderQty);
                            });
                        })
                        // filter seasonal delivery date
                        .Where(x =>
                        {
                            return !isContainSeasonalItems || (isContainSeasonalItems && seasonalDeliveryDates.Contains(x));
                        }
                        )
                        .ToList();

                    deliveryDateClash = isContainSeasonalItems && availableDays.Count() == 0;

                    attributeModel.MinDate = DateTime.UtcNow.Date.AddDays(advancedOrderDay);
                    attributeModel.MaxDate = DateTime.UtcNow.Date.AddDays(maxOrderDate);
                    attributeModel.UnavailableDate = daysInPeriod.Where(x => !availableDays.Any(y => y == x)).ToList();
                }
                else if (attribute.Name == NopOrderDefaults.DeliveryTimeSlot)
                {
                    var operatingHoursVendorAttribute = vendorAttributes.Where(x => x.Name == NopVendorDefaults.VendorOperationHours).FirstOrDefault();
                    var selectedOperatingHours = vendorAttributeValues.Where(x => x.VendorAttributeId == operatingHoursVendorAttribute.Id).ToList();
                    attributeModel.Values = attributeModel.Values.Where(x => selectedOperatingHours.Select(y => y.Name).Contains(x.Name)).ToList();
                }

                model.Add(attributeModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the shopping cart item model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="sci">Shopping cart item</param>
        /// <returns>Shopping cart item model</returns>
        protected virtual ShoppingCartModel.ShoppingCartItemModel PrepareShoppingCartItemModel(IList<ShoppingCartItem> cart, ShoppingCartItem sci)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (sci == null)
                throw new ArgumentNullException(nameof(sci));

            var product = _productService.GetProductById(sci.ProductId);

            var productCategory = _categoryService.GetProductCategoriesByProductId(product.Id).FirstOrDefault();

            var vendor = _vendorService.GetVendorByProductId(product.Id);

            var cartItemModel = new ShoppingCartModel.ShoppingCartItemModel
            {
                Id = sci.Id,
                Sku = _productService.FormatSku(product, sci.AttributesXml),
                VendorId = vendor != null ? vendor.Id : 0,
                VendorName = vendor != null ? vendor.Name : string.Empty,
                VendorImage = _pictureService.GetPictureUrl(vendor?.PictureId ?? 0) ,
                VendorOnline = vendor != null ? vendor.Online : false,
                VendorActive = vendor != null ? vendor.Active : false,
                ProductId = sci.ProductId,
                ProductName = _localizationService.GetLocalized(product, x => x.Name),
                ProductSeName = _urlRecordService.GetSeName(product),
                Quantity = sci.Quantity,
                AttributeInfo = _productAttributeFormatter.FormatAttributes(product, sci.AttributesXml),
                IsSelected = sci.IsSelected,
                IsEats = _productService.CheckIfIsEats(productCategory.CategoryId),
                IsMart = _productService.CheckIfIsMart(productCategory.CategoryId),
                OriginalPrice = _priceFormatter.FormatPrice(product.Price),
                IsProductDeleted = product.Deleted,
                IsProductPublished = product.Published
        };

            //allow editing?
            //1. setting enabled?
            //2. simple product?
            //3. has attribute or gift card?
            //4. visible individually?
            cartItemModel.AllowItemEditing = _shoppingCartSettings.AllowCartItemEditing &&
                                             product.ProductType == ProductType.SimpleProduct &&
                                             (!string.IsNullOrEmpty(cartItemModel.AttributeInfo) ||
                                              product.IsGiftCard) &&
                                             product.VisibleIndividually;

            //disable removal?
            //1. do other items require this one?
            cartItemModel.DisableRemoval = _shoppingCartService.GetProductsRequiringProduct(cart, product).Any();

            //allowed quantities
            var allowedQuantities = _productService.ParseAllowedQuantities(product);
            foreach (var qty in allowedQuantities)
            {
                cartItemModel.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = sci.Quantity == qty
                });
            }

            //recurring info
            if (product.IsRecurring)
                cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"),
                        product.RecurringCycleLength, _localizationService.GetLocalizedEnum(product.RecurringCyclePeriod));

            //rental info
            if (product.IsRental)
            {
                var rentalStartDate = sci.RentalStartDateUtc.HasValue
                    ? _productService.FormatRentalDate(product, sci.RentalStartDateUtc.Value)
                    : string.Empty;
                var rentalEndDate = sci.RentalEndDateUtc.HasValue
                    ? _productService.FormatRentalDate(product, sci.RentalEndDateUtc.Value)
                    : string.Empty;
                cartItemModel.RentalInfo =
                    string.Format(_localizationService.GetResource("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
            }

            //unit prices
            if (product.CallForPrice &&
                //also check whether the current user is impersonated
                (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
            {
                cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
            }
            else
            {
                var shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(product, _shoppingCartService.GetUnitPrice(sci), out var _);
                var shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                cartItemModel.UnitPriceValue = shoppingCartUnitPriceWithDiscount;

            }
            //subtotal, discount
            if (product.CallForPrice &&
                //also check whether the current user is impersonated
                (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
            {
                cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
            }
            else
            {
                //sub total
                var shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(product, _shoppingCartService.GetSubTotal(sci, true, out var shoppingCartItemDiscountBase, out _, out var maximumDiscountQty), out _);
                var shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);
                cartItemModel.SubTotalValue = shoppingCartItemSubTotalWithDiscount;
                cartItemModel.MaximumDiscountedQty = maximumDiscountQty;

                //display an applied discount amount
                if (shoppingCartItemDiscountBase > decimal.Zero)
                {
                    shoppingCartItemDiscountBase = _taxService.GetProductPrice(product, shoppingCartItemDiscountBase, out _);
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        var shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, _workContext.WorkingCurrency);
                        cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                        cartItemModel.DiscountValue = shoppingCartItemDiscount;

                    }
                }
            }

            //picture
            if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
            {
                cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                    _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
            }

            //item warnings
            var itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
                _workContext.CurrentCustomer,
                sci.ShoppingCartType,
                product,
                sci.StoreId,
                sci.AttributesXml,
                sci.CustomerEnteredPrice,
                sci.RentalStartDateUtc,
                sci.RentalEndDateUtc,
                sci.Quantity,
                false,
                sci.Id);
            foreach (var warning in itemWarnings)
                cartItemModel.Warnings.Add(warning);

            return cartItemModel;
        }

        /// <summary>
        /// Prepare the wishlist item model
        /// </summary>
        /// <param name="sci">Shopping cart item</param>
        /// <returns>Shopping cart item model</returns>
        protected virtual WishlistModel.ShoppingCartItemModel PrepareWishlistItemModel(ShoppingCartItem sci)
        {
            if (sci == null)
                throw new ArgumentNullException(nameof(sci));

            var product = _productService.GetProductById(sci.ProductId);

            var cartItemModel = new WishlistModel.ShoppingCartItemModel
            {
                Id = sci.Id,
                Sku = _productService.FormatSku(product, sci.AttributesXml),
                ProductId = product.Id,
                ProductName = _localizationService.GetLocalized(product, x => x.Name),
                ProductSeName = _urlRecordService.GetSeName(product),
                Quantity = sci.Quantity,
                AttributeInfo = _productAttributeFormatter.FormatAttributes(product, sci.AttributesXml)
            };

            var vendor = _vendorService.GetVendorById(product.VendorId);


            if (vendor != null && !vendor.Deleted && vendor.Active)
            {
                if (vendor.AddressId != 0)
                {
                    var vendorAddress = _addressService.GetAddressById(vendor.AddressId);
                    cartItemModel.Address.Address = vendorAddress;
                    if (cartItemModel.Address.Address != null && cartItemModel.Address.Address.StateProvinceId != 0)
                    {
                        var state = _stateProvinceService.GetStateProvinceById(cartItemModel.Address.Address.StateProvinceId.Value);
                        cartItemModel.Address.State = state?.Name;
                    }
                }
            }

            //allow editing?
            //1. setting enabled?
            //2. simple product?
            //3. has attribute or gift card?
            //4. visible individually?
            cartItemModel.AllowItemEditing = _shoppingCartSettings.AllowCartItemEditing &&
                                             product.ProductType == ProductType.SimpleProduct &&
                                             (!string.IsNullOrEmpty(cartItemModel.AttributeInfo) ||
                                              product.IsGiftCard) &&
                                             product.VisibleIndividually;

            //allowed quantities
            var allowedQuantities = _productService.ParseAllowedQuantities(product);
            foreach (var qty in allowedQuantities)
            {
                cartItemModel.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = sci.Quantity == qty
                });
            }

            //recurring info
            if (product.IsRecurring)
                cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"),
                        product.RecurringCycleLength, _localizationService.GetLocalizedEnum(product.RecurringCyclePeriod));

            //rental info
            if (product.IsRental)
            {
                var rentalStartDate = sci.RentalStartDateUtc.HasValue
                    ? _productService.FormatRentalDate(product, sci.RentalStartDateUtc.Value)
                    : string.Empty;
                var rentalEndDate = sci.RentalEndDateUtc.HasValue
                    ? _productService.FormatRentalDate(product, sci.RentalEndDateUtc.Value)
                    : string.Empty;
                cartItemModel.RentalInfo =
                    string.Format(_localizationService.GetResource("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
            }

            //unit prices
            if (product.CallForPrice &&
                //also check whether the current user is impersonated
                (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
            {
                cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
            }
            else
            {
                var shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(product, _shoppingCartService.GetUnitPrice(sci), out var _);
                var shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
            }
            //subtotal, discount
            if (product.CallForPrice &&
                //also check whether the current user is impersonated
                (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
            {
                cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
            }
            else
            {
                //sub total
                var shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(product, _shoppingCartService.GetSubTotal(sci, true, out var shoppingCartItemDiscountBase, out _, out var maximumDiscountQty), out _);
                var shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);
                cartItemModel.MaximumDiscountedQty = maximumDiscountQty;

                //display an applied discount amount
                if (shoppingCartItemDiscountBase > decimal.Zero)
                {
                    shoppingCartItemDiscountBase = _taxService.GetProductPrice(product, shoppingCartItemDiscountBase, out _);
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        var shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, _workContext.WorkingCurrency);
                        cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                    }
                }
            }

            //picture
            if (_shoppingCartSettings.ShowProductImagesOnWishList)
            {
                cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                    _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
            }

            //item warnings
            var itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
                _workContext.CurrentCustomer,
                sci.ShoppingCartType,
                product,
                sci.StoreId,
                sci.AttributesXml,
                sci.CustomerEnteredPrice,
                sci.RentalStartDateUtc,
                sci.RentalEndDateUtc,
                sci.Quantity,
                false,
                sci.Id);
            foreach (var warning in itemWarnings)
                cartItemModel.Warnings.Add(warning);

            return cartItemModel;
        }

        /// <summary>
        /// Prepare the order review data model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <returns>Order review data model</returns>
        protected virtual ShoppingCartModel.OrderReviewDataModel PrepareOrderReviewDataModel(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var model = new ShoppingCartModel.OrderReviewDataModel
            {
                Display = true
            };

            //billing info
            var billingAddress = _customerService.GetCustomerBillingAddress(_workContext.CurrentCustomer);
            if (billingAddress != null)
            {
                _addressModelFactory.PrepareAddressModel(model.BillingAddress,
                        address: billingAddress,
                        excludeProperties: false,
                        addressSettings: _addressSettings);
            }

            //shipping info
            if (_shoppingCartService.ShoppingCartRequiresShipping(cart))
            {
                model.IsShippable = true;

                var pickupPoint = _genericAttributeService.GetAttribute<PickupPoint>(_workContext.CurrentCustomer,
                    NopCustomerDefaults.SelectedPickupPointAttribute, _storeContext.CurrentStore.Id);
                model.SelectedPickupInStore = _shippingSettings.AllowPickupInStore && pickupPoint != null;
                if (!model.SelectedPickupInStore)
                {
                    if (_customerService.GetCustomerShippingAddress(_workContext.CurrentCustomer) is Address address)
                    {
                        _addressModelFactory.PrepareAddressModel(model.ShippingAddress,
                            address: address,
                            excludeProperties: false,
                            addressSettings: _addressSettings);
                    }
                }
                else
                {
                    var country = _countryService.GetCountryByTwoLetterIsoCode(pickupPoint.CountryCode);
                    var state = _stateProvinceService.GetStateProvinceByAbbreviation(pickupPoint.StateAbbreviation, country?.Id);

                    model.PickupAddress = new AddressModel
                    {
                        Address1 = pickupPoint.Address,
                        City = pickupPoint.City,
                        County = pickupPoint.County,
                        CountryName = country?.Name ?? string.Empty,
                        StateProvinceName = state?.Name ?? string.Empty,
                        ZipPostalCode = pickupPoint.ZipPostalCode
                    };
                }

                //selected shipping method
                var shippingOption = _genericAttributeService.GetAttribute<ShippingOption>(_workContext.CurrentCustomer,
                    NopCustomerDefaults.SelectedShippingOptionAttribute, _storeContext.CurrentStore.Id);
                if (shippingOption != null)
                    model.ShippingMethod = shippingOption.Name;
            }

            //payment info
            var selectedPaymentMethodSystemName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);
            var paymentMethod = _paymentPluginManager
                .LoadPluginBySystemName(selectedPaymentMethodSystemName, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
            model.PaymentMethod = paymentMethod != null
                ? _localizationService.GetLocalizedFriendlyName(paymentMethod, _workContext.WorkingLanguage.Id)
                : string.Empty;

            //custom values
            var processPaymentRequest = _httpContextAccessor.HttpContext?.Session?.Get<ProcessPaymentRequest>("OrderPaymentInfo");
            if (processPaymentRequest != null)
                model.CustomValues = processPaymentRequest.CustomValues;

            return model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the estimate shipping model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="setEstimateShippingDefaultAddress">Whether to use customer default shipping address for estimating</param>
        /// <returns>Estimate shipping model</returns>
        public virtual EstimateShippingModel PrepareEstimateShippingModel(IList<ShoppingCartItem> cart, bool setEstimateShippingDefaultAddress = true)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var model = new EstimateShippingModel
            {
                Enabled = cart.Any() && _shoppingCartService.ShoppingCartRequiresShipping(cart)
            };
            if (model.Enabled)
            {
                var shippingAddress = _customerService.GetCustomerShippingAddress(_workContext.CurrentCustomer);
                if (shippingAddress == null)
                {
                    shippingAddress = _customerService.GetAddressesByCustomerId(_workContext.CurrentCustomer.Id)
                    //enabled for the current store
                    .FirstOrDefault(a => a.CountryId == null || _storeMappingService.Authorize(_countryService.GetCountryByAddress(a)));
                }

                //countries
                var defaultEstimateCountryId = (setEstimateShippingDefaultAddress && shippingAddress != null)
                    ? shippingAddress.CountryId
                    : model.CountryId;
                model.AvailableCountries.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Address.SelectCountry"),
                    Value = "0"
                });

                foreach (var c in _countryService.GetAllCountriesForShipping(_workContext.WorkingLanguage.Id))
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = _localizationService.GetLocalized(c, x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == defaultEstimateCountryId
                    });

                //states
                var defaultEstimateStateId = (setEstimateShippingDefaultAddress && shippingAddress != null)
                    ? shippingAddress.StateProvinceId
                    : model.StateProvinceId;
                var states = defaultEstimateCountryId.HasValue
                    ? _stateProvinceService.GetStateProvincesByCountryId(defaultEstimateCountryId.Value, _workContext.WorkingLanguage.Id).ToList()
                    : new List<StateProvince>();
                if (states.Any())
                {
                    foreach (var s in states)
                    {
                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = _localizationService.GetLocalized(s, x => x.Name),
                            Value = s.Id.ToString(),
                            Selected = s.Id == defaultEstimateStateId
                        });
                    }
                }
                else
                {
                    model.AvailableStates.Add(new SelectListItem
                    {
                        Text = _localizationService.GetResource("Address.Other"),
                        Value = "0"
                    });
                }

                if (setEstimateShippingDefaultAddress && shippingAddress != null)
                    model.ZipPostalCode = shippingAddress.ZipPostalCode;
            }

            return model;
        }

        /// <summary>
        /// Prepare the cart item picture model
        /// </summary>
        /// <param name="sci">Shopping cart item</param>
        /// <param name="pictureSize">Picture size</param>
        /// <param name="showDefaultPicture">Whether to show the default picture</param>
        /// <param name="productName">Product name</param>
        /// <returns>Picture model</returns>
        public virtual PictureModel PrepareCartItemPictureModel(ShoppingCartItem sci, int pictureSize, bool showDefaultPicture, string productName)
        {
            var pictureCacheKey = _cacheKeyService.PrepareKeyForShortTermCache(NopModelCacheDefaults.CartPictureModelKey
                , sci, pictureSize, true, _workContext.WorkingLanguage, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore);

            var model = _staticCacheManager.Get(pictureCacheKey, () =>
            {
                var product = _productService.GetProductById(sci.ProductId);

                //shopping cart item picture
                var sciPicture = _pictureService.GetProductPicture(product, sci.AttributesXml);

                return new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(ref sciPicture, pictureSize, showDefaultPicture),
                    Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), productName),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), productName),
                };
            });

            return model;
        }

        /// <summary>
        /// Prepare the shopping cart model
        /// </summary>
        /// <param name="model">Shopping cart model</param>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <param name="validateCheckoutAttributes">Whether to validate checkout attributes</param>
        /// <param name="prepareAndDisplayOrderReviewData">Whether to prepare and display order review data</param>
        /// <returns>Shopping cart model</returns>
        public virtual ShoppingCartModel PrepareShoppingCartModel(ShoppingCartModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true,
            bool validateCheckoutAttributes = false,
            bool prepareAndDisplayOrderReviewData = false,
            bool skipShippingCalculation = false,
            bool getSupplierShippingDiscount = false
            )
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //simple properties
            model.OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled;

            if (!cart.Any())
                return model;

            model.IsEditable = isEditable;
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnShoppingCart;
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
            model.ShowVendorName = _vendorSettings.ShowVendorOnOrderDetailsPage;
            var checkoutAttributesXml = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
                NopCustomerDefaults.CheckoutAttributes, _storeContext.CurrentStore.Id);
            //
            var minOrderSubtotalAmountOk = _orderProcessingService.ValidateMinOrderSubtotalAmount(cart);
            if (!minOrderSubtotalAmountOk)
            {
                var minOrderSubtotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderSubtotalAmount, _workContext.WorkingCurrency);
                model.MinOrderSubtotalWarning = string.Format(_localizationService.GetResource("Checkout.MinOrderSubtotalAmount"), _priceFormatter.FormatPrice(minOrderSubtotalAmount, true, false));
            }

            //min order amount validation
            var minOrderTotalAmountOk = _orderProcessingService.ValidateMinOrderTotalAmount(cart);
            if (!minOrderTotalAmountOk)
            {
                var minOrderTotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderTotalAmount, _workContext.WorkingCurrency);
                model.MinOrderTotalWarning = string.Format(_localizationService.GetResource("Checkout.MinOrderTotalAmount"), _priceFormatter.FormatPrice(minOrderTotalAmount, true, false));
            }

            model.TermsOfServiceOnShoppingCartPage = _orderSettings.TermsOfServiceOnShoppingCartPage;
            model.TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage;
            model.TermsOfServicePopup = _commonSettings.PopupForTermsOfServiceLinks;
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoShoppingCart;

            //discount and gift card boxes
            model.DiscountBox.Display = _shoppingCartSettings.ShowDiscountBox;
            var discountCouponCodes = _customerService.ParseAppliedDiscountCouponCodes(_workContext.CurrentCustomer);
            foreach (var couponCode in discountCouponCodes)
            {
                var discount = _discountService.GetAllDiscounts(couponCode: couponCode)
                    .FirstOrDefault(d => d.RequiresCouponCode && _discountService.ValidateDiscount(d, _workContext.CurrentCustomer).IsValid);

                if (discount != null)
                {
                    model.DiscountBox.AppliedDiscountsWithCodes.Add(new ShoppingCartModel.DiscountBoxModel.DiscountInfoModel()
                    {
                        Id = discount.Id,
                        CouponCode = discount.CouponCode
                    });
                }
            }

            model.GiftCardBox.Display = _shoppingCartSettings.ShowGiftCardBox;

            //cart warnings
            var cartWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, checkoutAttributesXml, validateCheckoutAttributes);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            //cart items
            var items = new List<ShoppingCartModel.ShoppingCartItemModel>();
            var todayDate = DateTime.Today;
            foreach (var sci in cart)
            {
                var cartItemModel = PrepareShoppingCartItemModel(cart, sci);
                var product = _productService.GetProductById(cartItemModel.ProductId);
                cartItemModel.DeliveryDates = product.DeliveryDates;
                if (product.DeliveryDates.Count > 0 && !product.DeliveryDates.Any(x => x >= todayDate))
                {
                    cartItemModel.IsTna = true;
                }
                items.Add(cartItemModel);
            }

            var vendorShippings = new List<VendorShipping>();
            var vendorDiscounts = new List<VendorDiscount>();
            if (!isEditable && !skipShippingCalculation && cart.Select(x=>x.IsSelected).Count() > 0 && _workContext.CurrentCustomer.ShippingAddressId != 0)
            {
                var cartWithSelectedItem = cart.Where(x => x.IsSelected).ToList();
                vendorShippings = _orderTotalCalculationService.GetShoppingCartShippingTotalPerVendor(cartWithSelectedItem);
                vendorDiscounts = _orderTotalCalculationService.GetShoppingCartDiscountTotalPerVendor(_workContext.CurrentCustomer);
            }

            model.Vendors = items.GroupBy(x => x.VendorId).Select(x =>
            {
                var vendor = x.First();
                var vendorShipping = vendorShippings.Where(y => y.VendorId == vendor.VendorId).FirstOrDefault();
                var vendorDiscount = vendorDiscounts.Where(y => y.VendorId == vendor.VendorId).FirstOrDefault();
                var checkoutAttributes = PrepareCheckoutAttributeModelsForVendor(
                    cart
                        .Where(y => items
                        .Where(z => z.VendorId == vendor.VendorId
                            && z.IsSelected)
                            .Select(z => z.ProductId)
                            .Contains(y.ProductId))
                        .ToList(),
                        vendor.VendorId,
                        out var deliveryDateClash
                    );

                var vendorDb = _vendorService.GetVendorById(vendor.VendorId);
                var vendorCategory = _categoryService.GetCategoryById(vendorDb.CategoryId.Value);
                var shortCategory = vendorCategory.Name.Split(" ");
                return new ShoppingCartModel.ShoppingCartVendorModel
                {
                    Id = vendor.VendorId,
                    Name = vendor.VendorName,
                    PictureUrl = vendor.VendorImage,
                    Items = x.ToList(),
                    Discount = vendorDiscount != null ? _priceFormatter.FormatPrice(vendorDiscount.DiscountTotal) : "",
                    DiscountValue = vendorDiscount != null ? vendorDiscount.DiscountTotal : 0,
                    ShippingCost = vendorShipping != null ? _priceFormatter.FormatPrice(vendorShipping.ShippingCost) : "",
                    NotInsideCoverage = (vendorShipping?.NotInsideCoverage) ?? false,
                    FailComputeShipping = (vendorShipping?.FailComputeShipping) ?? false,
                    NotInsideCoverageBike = (vendorShipping?.NotInsideCoverageBike) ?? false,
                    NotInsideCoverageCar = (vendorShipping?.NotInsideCoverageCar) ?? false,
                    ShipmentOverweight = (vendorShipping?.ShipmentOverweight) ?? false,
                    Online = _vendorService.GetVendorOnlineStatus(vendorDb),
                    Active = vendor.VendorActive,
                    //checkout attributes
                    CheckoutAttributes = checkoutAttributes,
                    DeliveryDateClash = deliveryDateClash,
                    Category = shortCategory[1],
                    MaxShipmentWeight = vendorShipping?.MaxShipmentWeight ?? 0
                };
            }).ToList();

            if (getSupplierShippingDiscount)
            {
                var messageDiscountOnShippingFee = _localizationService.GetResource("ShoppingCart.DiscountOnShippingFee");
                var messageDiscountOnOrderSubtotal = _localizationService.GetResource("ShoppingCart.DiscountOnOrderSubtotal");
                var messageDiscountOnShippingFeeNoMinSpend = _localizationService.GetResource("ShoppingCart.DiscountOnShippingFeeNoMinSpend");
                var messageDiscountOnOrderSubtotalNoMinSpend = _localizationService.GetResource("ShoppingCart.DiscountOnOrderSubtotalNoMinSpend");

                var vendorIds = model.Vendors.Select(x => x.Id).ToList();
                var potentialDiscounts = _discountService.GetPotentialDiscounts(vendorIds, new List<DiscountType> { 
                    DiscountType.AssignedToShipping,
                    DiscountType.AssignedToPlatformShipping,
                    DiscountType.AssignedToOrderSubTotal,
                    DiscountType.AssignedToPlatformOrderSubTotal
                }, _workContext.CurrentCustomer.Id);
                model.Vendors = model.Vendors.Select(x =>
                {
                    
                    var psd = potentialDiscounts
                        .Where(y => y.VendorId == x.Id
                        && y.DiscountType == DiscountType.AssignedToShipping)
                        .FirstOrDefault();
                    if (psd != null)
                    {
                        var msg = psd.MinimmumSubTotalAmount.HasValue ? messageDiscountOnShippingFee : messageDiscountOnShippingFeeNoMinSpend;
                        msg = string.Format(msg,
                            psd.MinimmumSubTotalAmount?.ToString("'RM' #,##0") ?? "",
                            (psd.UsePercentage ? $"{psd.DiscountPercentage.ToString("0")}%" : $"RM {psd.DiscountAmount.ToString("#,##0")}"),
                            (psd.MaximumDiscountAmount.HasValue ? $"(max RM {psd.MaximumDiscountAmount.Value.ToString("#,##0")})" : "")
                            );
                        msg = $"<span class='text-bold'><b>{x.Name}</b></span> Exclusive Promotion - " + msg;
                        x.PotentialShippingDiscount = psd;
                        x.PotentialShippingDiscountMsg = msg;
                    }

                    var pso = potentialDiscounts
                        .Where(y => y.VendorId == x.Id
                        && y.DiscountType == DiscountType.AssignedToOrderSubTotal)
                        .FirstOrDefault();
                    if (pso != null)
                    {
                        var msg = pso.MinimmumSubTotalAmount.HasValue ? messageDiscountOnOrderSubtotal : messageDiscountOnOrderSubtotalNoMinSpend;
                        msg = string.Format(msg,
                            pso.MinimmumSubTotalAmount?.ToString("'RM' #,##0") ?? "",
                            (pso.UsePercentage ? $"{pso.DiscountPercentage.ToString("0")}%" : $"RM {pso.DiscountAmount.ToString("#,##0")}"),
                            (pso.MaximumDiscountAmount.HasValue ? $"(max RM {pso.MaximumDiscountAmount.Value.ToString("#,##0")})" : "")
                            );
                        msg = $"<span class='text-bold'><b>{x.Name}</b></span> Exclusive Promotion - " + msg;
                        x.PotentialOrderSubTotalDiscount = pso;
                        x.PotentialOrderSubTotalDiscountMsg = msg;
                    }
                    return x;
                }).ToList();

                var psd = potentialDiscounts
                    .Where(y => y.VendorId == null
                    && y.DiscountType == DiscountType.AssignedToPlatformShipping)
                    .FirstOrDefault();
                if (psd != null)
                {
                    var msg = psd.MinimmumSubTotalAmount.HasValue ? messageDiscountOnShippingFee : messageDiscountOnShippingFeeNoMinSpend;
                    msg = string.Format(msg,
                    psd.MinimmumSubTotalAmount?.ToString("'RM' #,##0") ?? "",
                        (psd.UsePercentage ? $"{psd.DiscountPercentage.ToString("0")}%" : $"RM {psd.DiscountAmount.ToString("#,##0")}"),
                        (psd.MaximumDiscountAmount.HasValue ? $"(max RM {psd.MaximumDiscountAmount.Value.ToString("#,##0")})" : "")
                        );
                    model.PotentialPlatformShippingDiscount = psd;
                    model.PotentialPlatformShippingDiscountMsg = msg;
                }

                var pso = potentialDiscounts
                    .Where(y => y.VendorId == null
                    && y.DiscountType == DiscountType.AssignedToPlatformOrderSubTotal)
                    .FirstOrDefault();
                if (pso != null)
                {
                        var msg = pso.MinimmumSubTotalAmount.HasValue ? messageDiscountOnOrderSubtotal : messageDiscountOnOrderSubtotalNoMinSpend;
                        msg = string.Format(msg,
                        pso.MinimmumSubTotalAmount?.ToString("'RM' #,##0")??"",
                        (pso.UsePercentage ? $"{pso.DiscountPercentage.ToString("0")}%" : $"RM {pso.DiscountAmount.ToString("#,##0")}"),
                        (pso.MaximumDiscountAmount.HasValue ? $"(max RM {pso.MaximumDiscountAmount.Value.ToString("#,##0")})" : "")
                        );
                    model.PotentialPlatformOrderSubTotalDiscount = pso;
                    model.PotentialPlatformOrderSubTotalDiscountMsg = msg;
                }
            }

            //payment methods
            //all payment methods (do not filter by country here as it could be not specified yet)
            var paymentMethods = _paymentPluginManager
                .LoadActivePlugins(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id)
                .Where(pm => !pm.HidePaymentMethod(cart)).ToList();
            //payment methods displayed during checkout (not with "Button" type)
            var nonButtonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType != PaymentMethodType.Button)
                .ToList();
            //"button" payment methods(*displayed on the shopping cart page)
            var buttonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Button)
                .ToList();
            foreach (var pm in buttonPaymentMethods)
            {
                if (_shoppingCartService.ShoppingCartIsRecurring(cart) && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                var viewComponentName = pm.GetPublicViewComponentName();
                model.ButtonPaymentMethodViewComponentNames.Add(viewComponentName);
            }
            //hide "Checkout" button if we have only "Button" payment methods
            model.HideCheckoutButton = !nonButtonPaymentMethods.Any() && model.ButtonPaymentMethodViewComponentNames.Any();

            //order review data
            if (prepareAndDisplayOrderReviewData)
            {
                model.OrderReviewData = PrepareOrderReviewDataModel(cart);
            }

            return model;
        }

        /// <summary>
        /// Prepare the wishlist model
        /// </summary>
        /// <param name="model">Wishlist model</param>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <returns>Wishlist model</returns>
        public virtual WishlistModel PrepareWishlistModel(WishlistModel model, IList<ShoppingCartItem> cart, bool isEditable = true)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.EmailWishlistEnabled = _shoppingCartSettings.EmailWishlistEnabled;
            model.IsEditable = isEditable;
            model.DisplayAddToCart = _permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart);
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoWishlist;

            if (!cart.Any())
                return model;

            //simple properties
            var customer = _customerService.GetShoppingCartCustomer(cart);

            model.CustomerGuid = customer.CustomerGuid;
            model.CustomerFullname = _customerService.GetCustomerFullName(customer);
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnWishList;
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;

            //cart warnings
            var cartWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, string.Empty, false);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            //cart items
            foreach (var sci in cart)
            {
                var cartItemModel = PrepareWishlistItemModel(sci);
                model.Items.Add(cartItemModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the mini shopping cart model
        /// </summary>
        /// <returns>Mini shopping cart model</returns>
        public virtual MiniShoppingCartModel PrepareMiniShoppingCartModel()
        {
            var model = new MiniShoppingCartModel
            {
                ShowProductImages = _shoppingCartSettings.ShowProductImagesInMiniShoppingCart,
                //let's always display it
                DisplayShoppingCartButton = true,
                CurrentCustomerIsGuest = _customerService.IsGuest(_workContext.CurrentCustomer),
                AnonymousCheckoutAllowed = _orderSettings.AnonymousCheckoutAllowed,
            };

            //performance optimization (use "HasShoppingCartItems" property)
            if (_workContext.CurrentCustomer.HasShoppingCartItems)
            {
                var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

                if (cart.Any())
                {
                    model.TotalProducts = cart.Count();

                    //subtotal
                    var subTotalIncludingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                    _orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax, out var _, out var _, out var subTotalWithoutDiscountBase, out var _);
                    var subtotalBase = subTotalWithoutDiscountBase;
                    var subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, _workContext.WorkingCurrency);
                    model.SubTotal = _priceFormatter.FormatPrice(subtotal, false, _workContext.WorkingCurrency, _workContext.WorkingLanguage.Id, subTotalIncludingTax);

                    var requiresShipping = _shoppingCartService.ShoppingCartRequiresShipping(cart);
                    //a customer should visit the shopping cart page (hide checkout button) before going to checkout if:
                    //1. "terms of service" are enabled
                    //2. min order sub-total is OK
                    //3. we have at least one checkout attribute
                    var checkoutAttributesExist = _checkoutAttributeService
                        .GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, !requiresShipping)
                        .Any();

                    var minOrderSubtotalAmountOk = _orderProcessingService.ValidateMinOrderSubtotalAmount(cart);

                    var cartProductIds = cart.Select(ci => ci.ProductId).ToArray();

                    var downloadableProductsRequireRegistration =
                        _customerSettings.RequireRegistrationForDownloadableProducts && _productService.HasAnyDownloadableProduct(cartProductIds);

                    model.DisplayCheckoutButton = !_orderSettings.TermsOfServiceOnShoppingCartPage &&
                        minOrderSubtotalAmountOk &&
                        !checkoutAttributesExist &&
                        !(downloadableProductsRequireRegistration
                            && _customerService.IsGuest(_workContext.CurrentCustomer));

                    //products. sort descending (recently added products)
                    foreach (var sci in cart
                        .OrderByDescending(x => x.Id)
                        .Take(_shoppingCartSettings.MiniShoppingCartProductNumber)
                        .ToList())
                    {
                        var product = _productService.GetProductById(sci.ProductId);

                        var cartItemModel = new MiniShoppingCartModel.ShoppingCartItemModel
                        {
                            Id = sci.Id,
                            ProductId = sci.ProductId,
                            ProductName = _localizationService.GetLocalized(product, x => x.Name),
                            ProductSeName = _urlRecordService.GetSeName(product),
                            Quantity = sci.Quantity,
                            AttributeInfo = _productAttributeFormatter.FormatAttributes(product, sci.AttributesXml)
                        };

                        //unit prices
                        if (product.CallForPrice &&
                            //also check whether the current user is impersonated
                            (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
                        {
                            cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                        }
                        else
                        {
                            var shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(product, _shoppingCartService.GetUnitPrice(sci), out var _);
                            var shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                            cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                        }

                        //picture
                        if (_shoppingCartSettings.ShowProductImagesInMiniShoppingCart)
                        {
                            cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                                _mediaSettings.MiniCartThumbPictureSize, true, cartItemModel.ProductName);
                        }

                        model.Items.Add(cartItemModel);
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare selected checkout attributes
        /// </summary>
        /// <returns>Formatted attributes</returns>
        public virtual string FormatSelectedCheckoutAttributes()
        {
            var checkoutAttributesXml = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
                NopCustomerDefaults.CheckoutAttributes, _storeContext.CurrentStore.Id);
            return _checkoutAttributeFormatter.FormatAttributes(checkoutAttributesXml, _workContext.CurrentCustomer);
        }

        /// <summary>
        /// Prepare the order totals model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <returns>Order totals model</returns>
        public virtual OrderTotalsModel PrepareOrderTotalsModel(IList<ShoppingCartItem> cart, bool isEditable)
        {
            var model = new OrderTotalsModel
            {
                IsEditable = isEditable
            };

            if (cart.Any())
            {
                // filter selected item
                cart = cart.Where(c => c.IsSelected).ToList();

                //subtotal
                var subTotalIncludingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                _orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax, out var orderSubTotalDiscountAmountBase, out var _, out var subTotalWithoutDiscountBase, out var _);
                var subtotalBase = subTotalWithoutDiscountBase;
                var subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, _workContext.WorkingCurrency);
                model.SubTotal = _priceFormatter.FormatPrice(subtotal, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage.Id, subTotalIncludingTax);

                if (orderSubTotalDiscountAmountBase > decimal.Zero)
                {
                    var orderSubTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderSubTotalDiscountAmountBase, _workContext.WorkingCurrency);
                    model.SubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountAmount, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage.Id, subTotalIncludingTax);
                }

                //shipping info
                model.RequiresShipping = _shoppingCartService.ShoppingCartRequiresShipping(cart);
                if (model.RequiresShipping)
                {
                    var shippingPerVendor = _orderTotalCalculationService.GetShoppingCartShippingTotalPerVendor(cart);
                    model.ShippingPerVendors = shippingPerVendor;
                    var shoppingCartShippingBase = model.ShippingPerVendors.Sum(x => x.ShippingCost);

                    var shoppingCartShipping = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartShippingBase, _workContext.WorkingCurrency);
                    model.Shipping = _priceFormatter.FormatShippingPrice(shoppingCartShipping, true);

                    //selected shipping method
                    var shippingOption = _genericAttributeService.GetAttribute<ShippingOption>(_workContext.CurrentCustomer,
                        NopCustomerDefaults.SelectedShippingOptionAttribute, _storeContext.CurrentStore.Id);
                    if (shippingOption != null)
                        model.SelectedShippingMethod = shippingOption.Name;
                }
                else
                {
                    model.HideShippingTotal = _shippingSettings.HideShippingTotal;
                }

                //payment method fee
                var paymentMethodSystemName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);
                var paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, paymentMethodSystemName);
                var paymentMethodAdditionalFeeWithTaxBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                if (paymentMethodAdditionalFeeWithTaxBase > decimal.Zero)
                {
                    var paymentMethodAdditionalFeeWithTax = _currencyService.ConvertFromPrimaryStoreCurrency(paymentMethodAdditionalFeeWithTaxBase, _workContext.WorkingCurrency);
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeWithTax, true);
                }

                //tax
                bool displayTax;
                bool displayTaxRates;
                if (_taxSettings.HideTaxInOrderSummary && _workContext.TaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    var shoppingCartTaxBase = _orderTotalCalculationService.GetTaxTotal(cart, out var taxRates);
                    var shoppingCartTax = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTaxBase, _workContext.WorkingCurrency);

                    if (shoppingCartTaxBase == 0 && _taxSettings.HideZeroTax)
                    {
                        displayTax = false;
                        displayTaxRates = false;
                    }
                    else
                    {
                        displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                        displayTax = !displayTaxRates;

                        model.Tax = _priceFormatter.FormatPrice(shoppingCartTax, true, false);
                        foreach (var tr in taxRates)
                        {
                            model.TaxRates.Add(new OrderTotalsModel.TaxRate
                            {
                                Rate = _priceFormatter.FormatTaxRate(tr.Key),
                                Value = _priceFormatter.FormatPrice(_currencyService.ConvertFromPrimaryStoreCurrency(tr.Value, _workContext.WorkingCurrency), true, false),
                            });
                        }
                    }
                }

                model.DisplayTaxRates = displayTaxRates;
                model.DisplayTax = displayTax;

                ////discount
                //var discountPerVendor = _orderTotalCalculationService.GetShoppingCartDiscountTotalPerVendor(cart);
                //model.DiscountPerVendors = discountPerVendor;
                //var shoppingCartDiscountBase = model.DiscountPerVendors.Sum(x => x.DiscountTotal);

                //var shoppingCartDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartDiscountBase, _workContext.WorkingCurrency);
                //model.OrderTotalDiscount = _priceFormatter.FormatShippingPrice(-shoppingCartDiscount, true);

                //total
                var shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart, out var orderTotalDiscountAmountBase, out var _, out var appliedGiftCards, out var redeemedRewardPoints, out var redeemedRewardPointsAmount);
                if (shoppingCartTotalBase.HasValue)
                {
                    var shoppingCartTotal = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTotalBase.Value, _workContext.WorkingCurrency);
                    model.OrderTotal = _priceFormatter.FormatPrice(shoppingCartTotal, true, false);
                }

                //discount
                if (orderTotalDiscountAmountBase > decimal.Zero)
                {
                    var orderTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderTotalDiscountAmountBase, _workContext.WorkingCurrency);
                    model.OrderTotalDiscount = _priceFormatter.FormatPrice(-orderTotalDiscountAmount, true, false);
                }

                //gift cards
                if (appliedGiftCards != null && appliedGiftCards.Any())
                {
                    foreach (var appliedGiftCard in appliedGiftCards)
                    {
                        var gcModel = new OrderTotalsModel.GiftCard
                        {
                            Id = appliedGiftCard.GiftCard.Id,
                            CouponCode = appliedGiftCard.GiftCard.GiftCardCouponCode,
                        };
                        var amountCanBeUsed = _currencyService.ConvertFromPrimaryStoreCurrency(appliedGiftCard.AmountCanBeUsed, _workContext.WorkingCurrency);
                        gcModel.Amount = _priceFormatter.FormatPrice(-amountCanBeUsed, true, false);

                        var remainingAmountBase = _giftCardService.GetGiftCardRemainingAmount(appliedGiftCard.GiftCard) - appliedGiftCard.AmountCanBeUsed;
                        var remainingAmount = _currencyService.ConvertFromPrimaryStoreCurrency(remainingAmountBase, _workContext.WorkingCurrency);
                        gcModel.Remaining = _priceFormatter.FormatPrice(remainingAmount, true, false);

                        model.GiftCards.Add(gcModel);
                    }
                }

                //reward points to be spent (redeemed)
                if (redeemedRewardPointsAmount > decimal.Zero)
                {
                    var redeemedRewardPointsAmountInCustomerCurrency = _currencyService.ConvertFromPrimaryStoreCurrency(redeemedRewardPointsAmount, _workContext.WorkingCurrency);
                    model.RedeemedRewardPoints = redeemedRewardPoints;
                    model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-redeemedRewardPointsAmountInCustomerCurrency, true, false);
                }

                //reward points to be earned
                if (_rewardPointsSettings.Enabled && _rewardPointsSettings.DisplayHowMuchWillBeEarned && shoppingCartTotalBase.HasValue)
                {
                    //get shipping total
                    var shippingBaseInclTax = !model.RequiresShipping ? 0 : _orderTotalCalculationService.GetShoppingCartShippingTotal(cart, true) ?? 0;

                    //get total for reward points
                    var totalForRewardPoints = _orderTotalCalculationService
                        .CalculateApplicableOrderTotalForRewardPoints(shippingBaseInclTax, shoppingCartTotalBase.Value);
                    if (totalForRewardPoints > decimal.Zero)
                        model.WillEarnRewardPoints = _orderTotalCalculationService.CalculateRewardPoints(_workContext.CurrentCustomer, totalForRewardPoints);
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the estimate shipping result model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="countryId">Country identifier</param>
        /// <param name="stateProvinceId">State or province identifier</param>
        /// <param name="zipPostalCode">Zip postal code</param>
        /// <param name="cacheOfferedShippingOptions">Indicates whether to cache offered shipping options</param>
        /// <returns>Estimate shipping result model</returns>
        public virtual EstimateShippingResultModel PrepareEstimateShippingResultModel(IList<ShoppingCartItem> cart, int? countryId, int? stateProvinceId, string zipPostalCode, bool cacheShippingOptions)
        {
            var model = new EstimateShippingResultModel();

            if (_shoppingCartService.ShoppingCartRequiresShipping(cart))
            {
                var address = new Address
                {
                    CountryId = countryId,
                    StateProvinceId = stateProvinceId,
                    ZipPostalCode = zipPostalCode,
                };

                var rawShippingOptions = new List<ShippingOption>();

                var getShippingOptionResponse = _shippingService.GetShippingOptions(cart, address, _workContext.CurrentCustomer, storeId: _storeContext.CurrentStore.Id);
                if (getShippingOptionResponse.Success)
                {
                    if (getShippingOptionResponse.ShippingOptions.Any())
                    {
                        foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                        {
                            rawShippingOptions.Add(new ShippingOption()
                            {
                                Name = shippingOption.Name,
                                Description = shippingOption.Description,
                                Rate = shippingOption.Rate,
                                TransitDays = shippingOption.TransitDays,
                                ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName
                            });
                        }
                    }
                    else
                        foreach (var error in getShippingOptionResponse.Errors)
                            model.Warnings.Add(error);
                }

                var pickupPointsNumber = 0;
                if (_shippingSettings.AllowPickupInStore)
                {
                    var pickupPointsResponse = _shippingService.GetPickupPoints(address.Id, _workContext.CurrentCustomer, storeId: _storeContext.CurrentStore.Id);
                    if (pickupPointsResponse.Success)
                    {
                        if (pickupPointsResponse.PickupPoints.Any())
                        {
                            pickupPointsNumber = pickupPointsResponse.PickupPoints.Count();
                            var pickupPoint = pickupPointsResponse.PickupPoints.OrderBy(p => p.PickupFee).First();

                            rawShippingOptions.Add(new ShippingOption()
                            {
                                Name = _localizationService.GetResource("Checkout.PickupPoints"),
                                Description = _localizationService.GetResource("Checkout.PickupPoints.Description"),
                                Rate = pickupPoint.PickupFee,
                                TransitDays = pickupPoint.TransitDays,
                                ShippingRateComputationMethodSystemName = pickupPoint.ProviderSystemName,
                                IsPickupInStore = true
                            });
                        }
                    }
                    else
                        foreach (var error in pickupPointsResponse.Errors)
                            model.Warnings.Add(error);
                }

                ShippingOption selectedShippingOption = null;
                if (cacheShippingOptions)
                {
                    //performance optimization. cache returned shipping options.
                    //we'll use them later (after a customer has selected an option).
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                                                           NopCustomerDefaults.OfferedShippingOptionsAttribute,
                                                           rawShippingOptions,
                                                           _storeContext.CurrentStore.Id);

                    //find a selected (previously) shipping option
                    selectedShippingOption = _genericAttributeService.GetAttribute<ShippingOption>(_workContext.CurrentCustomer,
                            NopCustomerDefaults.SelectedShippingOptionAttribute, _storeContext.CurrentStore.Id);
                }

                if (rawShippingOptions.Any())
                {
                    foreach (var option in rawShippingOptions)
                    {
                        var shippingRate = _orderTotalCalculationService.AdjustShippingRate(option.Rate, cart, out var _, option.IsPickupInStore);
                        shippingRate = _taxService.GetShippingPrice(shippingRate, _workContext.CurrentCustomer);
                        shippingRate = _currencyService.ConvertFromPrimaryStoreCurrency(shippingRate, _workContext.WorkingCurrency);
                        var shippingRateString = _priceFormatter.FormatShippingPrice(shippingRate, true);

                        if (option.IsPickupInStore && pickupPointsNumber > 1)
                            shippingRateString = string.Format(_localizationService.GetResource("Shipping.EstimateShippingPopUp.Pickup.PriceFrom"), shippingRateString);

                        string deliveryDateFormat = null;
                        if (option.TransitDays.HasValue)
                        {
                            var currentCulture = CultureInfo.GetCultureInfo(_workContext.WorkingLanguage.LanguageCulture);
                            var customerDateTime = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
                            deliveryDateFormat = customerDateTime.AddDays(option.TransitDays.Value).ToString("d", currentCulture);
                        }

                        var selected = false;
                        if (selectedShippingOption != null &&
                        !string.IsNullOrEmpty(option.ShippingRateComputationMethodSystemName) &&
                               option.ShippingRateComputationMethodSystemName.Equals(selectedShippingOption.ShippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase) &&
                               (!string.IsNullOrEmpty(option.Name) &&
                               option.Name.Equals(selectedShippingOption.Name, StringComparison.InvariantCultureIgnoreCase) ||
                               (option.IsPickupInStore && option.IsPickupInStore == selectedShippingOption.IsPickupInStore))
                               )
                        {
                            selected = true;
                        }

                        model.ShippingOptions.Add(new EstimateShippingResultModel.ShippingOptionModel()
                        {
                            Name = option.Name,
                            ShippingRateComputationMethodSystemName = option.ShippingRateComputationMethodSystemName,
                            Description = option.Description,
                            Price = shippingRateString,
                            Rate = option.Rate,
                            DeliveryDateFormat = deliveryDateFormat,
                            Selected = selected
                        });
                    }

                    //if no option has been selected, let's do it for the first one
                    if (!model.ShippingOptions.Any(so => so.Selected))
                        model.ShippingOptions.First().Selected = true;
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the wishlist email a friend model
        /// </summary>
        /// <param name="model">Wishlist email a friend model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Wishlist email a friend model</returns>
        public virtual WishlistEmailAFriendModel PrepareWishlistEmailAFriendModel(WishlistEmailAFriendModel model, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnEmailWishlistToFriendPage;
            if (!excludeProperties)
            {
                model.YourEmailAddress = _workContext.CurrentCustomer.Email;
            }

            return model;
        }

        #endregion
    }
}