using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Html;
using Nop.Core.Infrastructure;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Plugin.NopStation.WebApi.Factories;
using Nop.Plugin.NopStation.WebApi.Models.Checkout;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Plugin.NopStation.WebApi.Models.ShoppingCart;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.ShippingShuq;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Plugin.NopStation.WebApi.Controllers
{
    [Route("api/shoppingcart")]
    public partial class ShoppingCartApiController : BaseApiController
    {
        #region Fields

        private readonly ICacheKeyService _cacheKeyService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IDownloadService _downloadService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGiftCardService _giftCardService;
        private readonly ILocalizationService _localizationService;
        private readonly INopFileProvider _fileProvider;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly MediaSettings _mediaSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly IProductModelFactory _productModelFactory;
        private readonly CartCheckoutApiModelFactory _cartCheckoutApiModelFactory;
        private readonly IShipmentProcessor _shipmentProcessor;
        private readonly IVendorService _vendorService;

        #endregion

        #region Ctor

        public ShoppingCartApiController(ICacheKeyService cacheKeyService,
            IStaticCacheManager staticCacheManager,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDiscountService discountService,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            ILocalizationService localizationService,
            INopFileProvider fileProvider,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            ITaxService taxService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            MediaSettings mediaSettings,
            ShoppingCartSettings shoppingCartSettings,
            ShippingSettings shippingSettings,
            IProductModelFactory productModelFactory,
            CartCheckoutApiModelFactory cartCheckoutApiModelFactory,
            IShipmentProcessor shipmentProcessor,
            IVendorService vendorService)
        {
            _cacheKeyService = cacheKeyService;
            _staticCacheManager = staticCacheManager;
            _checkoutAttributeParser = checkoutAttributeParser;
            _checkoutAttributeService = checkoutAttributeService;
            _currencyService = currencyService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _discountService = discountService;
            _downloadService = downloadService;
            _genericAttributeService = genericAttributeService;
            _giftCardService = giftCardService;
            _localizationService = localizationService;
            _fileProvider = fileProvider;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _taxService = taxService;
            _webHelper = webHelper;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _mediaSettings = mediaSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _shippingSettings = shippingSettings;
            _productModelFactory = productModelFactory;
            _cartCheckoutApiModelFactory = cartCheckoutApiModelFactory;
            _shipmentProcessor = shipmentProcessor;
            _vendorService = vendorService;
        }

        #endregion

        #region Utilities

        protected ShoppingCartApiModel PrepareShoppingCartApiModel(ShoppingCartModel cartModel, IList<ShoppingCartItem> cart, bool isEditable)
        {
            var model = new ShoppingCartApiModel
            {
                Cart = _shoppingCartModelFactory.PrepareShoppingCartModel(cartModel, cart, getSupplierShippingDiscount: true),
                //OrderTotals = _shoppingCartModelFactory.PrepareOrderTotalsModel(cart, isEditable),
                SelectedCheckoutAttributes = _shoppingCartModelFactory.FormatSelectedCheckoutAttributes()
            };

            if (_shippingSettings.EstimateShippingCartPageEnabled)
                model.EstimateShipping = _shoppingCartModelFactory.PrepareEstimateShippingModel(cart);

            return model;
        }

        protected virtual void ParseAndSaveCheckoutAttributes(IList<ShoppingCartItem> cart, NameValueCollection form)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = string.Empty;
            var excludeShippableAttributes = !_shoppingCartService.ShoppingCartRequiresShipping(cart);
            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, excludeShippableAttributes);
            foreach (var attribute in checkoutAttributes)
            {
                var controlId = $"checkout_attribute_{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var date = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(date));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid.TryParse(form[controlId], out var downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                           attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            //validate conditional attributes (if specified)
            foreach (var attribute in checkoutAttributes)
            {
                var conditionMet = _checkoutAttributeParser.IsConditionMet(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                    attributesXml = _checkoutAttributeParser.RemoveCheckoutAttribute(attributesXml, attribute);
            }

            //save checkout attributes
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, NopCustomerDefaults.CheckoutAttributes, attributesXml, _storeContext.CurrentStore.Id);
        }

        protected virtual string ParseProductAttributes(Product product, NameValueCollection form, List<string> errors)
        {
            //product attributes
            var attributesXml = GetProductAttributesXml(product, form, errors);

            //gift cards
            AddGiftCardsAttributesXml(product, form, ref attributesXml);

            return attributesXml;
        }

        protected virtual void ParseRentalDates(Product product, NameValueCollection form,
            out DateTime? startDate, out DateTime? endDate)
        {
            startDate = null;
            endDate = null;

            var startControlId = $"rental_start_date_{product.Id}";
            var endControlId = $"rental_end_date_{product.Id}";
            var ctrlStartDate = form[startControlId];
            var ctrlEndDate = form[endControlId];
            try
            {
                //currently we support only this format (as in the \Views\Product\_RentalInfo.cshtml file)
                const string datePickerFormat = "MM/dd/yyyy";
                startDate = DateTime.ParseExact(ctrlStartDate, datePickerFormat, CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(ctrlEndDate, datePickerFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
            }
        }

        protected virtual void AddGiftCardsAttributesXml(Product product, NameValueCollection form, ref string attributesXml)
        {
            if (!product.IsGiftCard) return;

            var recipientName = "";
            var recipientEmail = "";
            var senderName = "";
            var senderEmail = "";
            var giftCardMessage = "";
            foreach (string formKey in form.Keys)
            {
                if (formKey.Equals($"giftcard_{product.Id}.RecipientName", StringComparison.InvariantCultureIgnoreCase))
                {
                    recipientName = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.RecipientEmail", StringComparison.InvariantCultureIgnoreCase))
                {
                    recipientEmail = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.SenderName", StringComparison.InvariantCultureIgnoreCase))
                {
                    senderName = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.SenderEmail", StringComparison.InvariantCultureIgnoreCase))
                {
                    senderEmail = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.Message", StringComparison.InvariantCultureIgnoreCase))
                {
                    giftCardMessage = form[formKey];
                }
            }

            attributesXml = _productAttributeParser.AddGiftCardAttribute(attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
        }

        protected virtual string GetProductAttributesXml(Product product, NameValueCollection form, List<string> errors)
        {
            var attributesXml = string.Empty;
            var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in productAttributes)
            {
                var controlId = $"{NopCatalogDefaults.ProductAttributePrefix}{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                {
                                    //get quantity entered by customer
                                    var quantity = 1;
                                    var quantityStr = form[$"{NopCatalogDefaults.ProductAttributePrefix}{attribute.Id}_{selectedAttributeId}_qty"];
                                    if (!StringValues.IsNullOrEmpty(quantityStr) &&
                                        (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                        errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                                }
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                foreach (var item in ctrlAttributes.ToString()
                                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                    {
                                        //get quantity entered by customer
                                        var quantity = 1;
                                        var quantityStr = form[$"{NopCatalogDefaults.ProductAttributePrefix}{attribute.Id}_{item}_qty"];
                                        if (!StringValues.IsNullOrEmpty(quantityStr) &&
                                            (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                            errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                                    }
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                //get quantity entered by customer
                                var quantity = 1;
                                var quantityStr = form[$"{NopCatalogDefaults.ProductAttributePrefix}{attribute.Id}_{selectedAttributeId}_qty"];
                                if (!StringValues.IsNullOrEmpty(quantityStr) &&
                                    (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                    errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var day = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                            }
                            catch
                            {
                            }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid.TryParse(form[controlId], out var downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            //validate conditional attributes (if specified)
            foreach (var attribute in productAttributes)
            {
                var conditionMet = _productAttributeParser.IsConditionMet(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                {
                    attributesXml = _productAttributeParser.RemoveProductAttribute(attributesXml, attribute);
                }
            }
            return attributesXml;
        }

        protected virtual void SaveItem(ShoppingCartItem updatecartitem, List<string> addToCartWarnings, Product product,
           ShoppingCartType cartType, string attributes, decimal customerEnteredPriceConverted, DateTime? rentalStartDate,
           DateTime? rentalEndDate, int quantity)
        {
            if (updatecartitem == null)
            {
                //add to the cart
                addToCartWarnings.AddRange(_shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                    product, cartType, _storeContext.CurrentStore.Id,
                    attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate, quantity, true));
            }
            else
            {
                var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, updatecartitem.ShoppingCartType, _storeContext.CurrentStore.Id);

                var otherCartItemWithSameParameters = _shoppingCartService.FindShoppingCartItemInTheCart(
                    cart, updatecartitem.ShoppingCartType, product, attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate);
                if (otherCartItemWithSameParameters != null &&
                    otherCartItemWithSameParameters.Id == updatecartitem.Id)
                {
                    //ensure it's some other shopping cart item
                    otherCartItemWithSameParameters = null;
                }
                //update existing item
                addToCartWarnings.AddRange(_shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                    updatecartitem.Id, attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate, quantity + (otherCartItemWithSameParameters?.Quantity ?? 0), true));
                if (otherCartItemWithSameParameters != null && !addToCartWarnings.Any())
                {
                    //delete the same shopping cart item (the other one)
                    _shoppingCartService.DeleteShoppingCartItem(otherCartItemWithSameParameters);
                }
            }
        }

        #endregion

        #region Shopping cart

        [HttpPost("addproducttocart/catalog/{productId:min(0)}/{shoppingCartTypeId:min(0)}")]
        public virtual IActionResult AddProductToCart_Catalog(int productId, int shoppingCartTypeId, [FromBody]BaseQueryModel<string> queryModel)
        {
            var cartType = (ShoppingCartType)shoppingCartTypeId;

            var product = _productService.GetProductById(productId);
            if (product == null)
                return NotFound(_localizationService.GetResource("NopStation.WebApi.Response.AddToCart.ProductNotFound"));

            var form = queryModel.FormValues == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();

            var quantity = 1;
            foreach (string formKey in form.Keys)
                if (formKey.Equals($"quantity", StringComparison.InvariantCultureIgnoreCase))
                {
                    int.TryParse(form[formKey], out quantity);
                    break;
                }

            var forceredirection = false;
            foreach (string formKey in form.Keys)
                if (formKey.Equals($"forceredirection", StringComparison.InvariantCultureIgnoreCase))
                {
                    bool.TryParse(form[formKey], out forceredirection);
                    break;
                }

            var response = new GenericResponseModel<AddToCartResponseModel>
            {
                Data = new AddToCartResponseModel()
            };

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                response.Data.RedirectToDetailsPage = true;
                return Ok(response);
            }

            //products with "minimum order quantity" more than a specified qty
            if (product.OrderMinimumQuantity > quantity)
            {
                response.Data.RedirectToDetailsPage = true;
                return Ok(response);
            }

            if (product.CustomerEntersPrice)
            {
                response.Data.RedirectToDetailsPage = true;
                return Ok(response);
            }

            if (product.IsRental)
            {
                response.Data.RedirectToDetailsPage = true;
                return Ok(response);
            }

            var allowedQuantities = _productService.ParseAllowedQuantities(product);
            if (allowedQuantities.Length > 0)
            {
                response.Data.RedirectToDetailsPage = true;
                return Ok(response);
            }

            //allow a product to be added to the cart when all attributes are with "read-only checkboxes" type
            var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            if (productAttributes.Any(pam => pam.AttributeControlType != AttributeControlType.ReadonlyCheckboxes))
            {
                response.Data.RedirectToDetailsPage = true;
                return Ok(response);
            }

            //creating XML for "read-only checkboxes" attributes
            var attXml = productAttributes.Aggregate(string.Empty, (attributesXml, attribute) =>
            {
                var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                foreach (var selectedAttributeId in attributeValues
                    .Where(v => v.IsPreSelected)
                    .Select(v => v.Id)
                    .ToList())
                {
                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                        attribute, selectedAttributeId.ToString());
                }
                return attributesXml;
            });

            //get standard warnings without attribute validations
            //first, try to find existing shopping cart item
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, cartType, _storeContext.CurrentStore.Id);
            var shoppingCartItem = _shoppingCartService.FindShoppingCartItemInTheCart(cart, cartType, product);
            //if we already have the same product in the cart, then use the total quantity to validate
            var quantityToValidate = shoppingCartItem != null ? shoppingCartItem.Quantity + quantity : quantity;
            var addToCartWarnings = _shoppingCartService
                .GetShoppingCartItemWarnings(_workContext.CurrentCustomer, cartType,
                product, _storeContext.CurrentStore.Id, string.Empty,
                decimal.Zero, null, null, quantityToValidate, false, shoppingCartItem?.Id ?? 0, true, false, false, false);
            if (addToCartWarnings.Any())
            {
                response.ErrorList.AddRange(addToCartWarnings);
                return BadRequest(response);
            }

            //now let's try adding product to the cart (now including product attribute validation, etc)
            addToCartWarnings = _shoppingCartService.AddToCart(customer: _workContext.CurrentCustomer,
                product: product,
                shoppingCartType: cartType,
                storeId: _storeContext.CurrentStore.Id,
                attributesXml: attXml,
                quantity: quantity);
            if (addToCartWarnings.Any())
            {
                response.Data.RedirectToDetailsPage = true;
                return Ok(response);
            }

            //added to the cart/wishlist
            switch (cartType)
            {
                case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToWishlist",
                            string.Format(_localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Name), product);

                        if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct || forceredirection)
                        {
                            response.Data.RedirectToWishListPage = true;
                            return Ok(response);
                        }
                        break;
                    }
                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart",
                            string.Format(_localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name), product);

                        if (_shoppingCartSettings.DisplayCartAfterAddingProduct || forceredirection)
                        {
                            response.Data.RedirectToShoppingCartPage = true;
                            return Ok(response);
                        }
                        break;
                    }
            }


            var updatedCart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, storeId: _storeContext.CurrentStore.Id);

            var model = new AddToCartResponseModel()
            {
                TotalShoppingCartProductsQuantity = updatedCart.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).Sum(item => item.Quantity),
                TotalShoppingCartProducts = updatedCart.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).Count(),
                TotalWishListProducts = updatedCart.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).Sum(item => item.Quantity)
            };
            response.Data = model;

            return Ok(response);
        }

        [HttpPost("addproducttocart/details/{productId:min(0)}/{shoppingCartTypeId:min(0)}")]
        public virtual IActionResult AddProductToCart_Details(int productId, int shoppingCartTypeId, [FromBody]BaseQueryModel<string> queryModel)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return NotFound(_localizationService.GetResource("NopStation.WebApi.Response.AddToCart.ProductNotFound"));

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.AddToCart.SimpleProductOnly"));

            var form = queryModel.FormValues == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();
            //update existing shopping cart item
            var updatecartitemid = 0;
            foreach (string formKey in form.Keys)
                if (formKey.Equals($"addtocart_{productId}.UpdatedShoppingCartItemId", StringComparison.InvariantCultureIgnoreCase))
                {
                    int.TryParse(form[formKey], out updatecartitemid);
                    break;
                }
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                //search with the same cart type as specified
                var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, (ShoppingCartType)shoppingCartTypeId, _storeContext.CurrentStore.Id);

                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //is it this product?
                if (updatecartitem != null && product.Id != updatecartitem.ProductId)
                    return BadRequest(_localizationService.GetResource("NopStation.WebApi.AddToCart.NotMatchingWithCartItem"));
            }

            //customer entered price
            var customerEnteredPriceConverted = decimal.Zero;
            if (product.CustomerEntersPrice)
            {
                foreach (string formKey in form.Keys)
                {
                    if (formKey.Equals($"addtocart_{productId}.CustomerEnteredPrice", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (decimal.TryParse(form[formKey], out var customerEnteredPrice))
                            customerEnteredPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(customerEnteredPrice, _workContext.WorkingCurrency);
                        break;
                    }
                }
            }

            var quantity = 1;
            foreach (string formKey in form.Keys)
                if (formKey.Equals($"addtocart_{productId}.EnteredQuantity", StringComparison.InvariantCultureIgnoreCase))
                {
                    int.TryParse(form[formKey], out quantity);
                    break;
                }

            var addToCartWarnings = new List<string>();

            //product and gift card attributes
            var attributes = ParseProductAttributes(product, form, addToCartWarnings);

            //rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (product.IsRental)
            {
                ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
            }

            var cartType = updatecartitem == null ? (ShoppingCartType)shoppingCartTypeId :
                //if the item to update is found, then we ignore the specified "shoppingCartTypeId" parameter
                updatecartitem.ShoppingCartType;

            SaveItem(updatecartitem, addToCartWarnings, product, cartType, attributes, customerEnteredPriceConverted, rentalStartDate, rentalEndDate, quantity);

            var response = new GenericResponseModel<AddToCartResponseModel>();
            if (addToCartWarnings.Any())
            {
                response.ErrorList.AddRange(addToCartWarnings);
                return BadRequest(response);
            }

            var updatedCart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, storeId: _storeContext.CurrentStore.Id);

            var model = new AddToCartResponseModel()
            {
                TotalShoppingCartProducts = updatedCart.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).Count(),
                TotalShoppingCartProductsQuantity = updatedCart.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).Sum(item => item.Quantity),
                TotalWishListProducts = updatedCart.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).Sum(item => item.Quantity)
            };
            response.Data = model;

            response.Message = (ShoppingCartType)shoppingCartTypeId == ShoppingCartType.ShoppingCart ?
                _localizationService.GetResource("Products.ProductHasBeenAddedToTheCart") :
                _localizationService.GetResource("Products.ProductHasBeenAddedToTheWishList");

            return Ok(response);
        }

        [HttpPost("productattributechange/{productId:min(0)}")]
        public virtual IActionResult ProductDetails_AttributeChange(int productId, [FromBody]BaseQueryModel<string> queryModel)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return NotFound();

            var form = queryModel.FormValues == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();
            var validateAttributeConditions = form["validateAttributeConditions"] != null ? bool.Parse(form["validateAttributeConditions"]) : false;
            var loadPicture = form["loadPicture"] != null ? bool.Parse(form["loadPicture"]) : false;

            var errors = new List<string>();
            var attributeXml = ParseProductAttributes(product, form, errors);

            //rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (product.IsRental)
            {
                ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
            }

            //sku, mpn, gtin
            var sku = _productService.FormatSku(product, attributeXml);
            var mpn = _productService.FormatMpn(product, attributeXml);
            var gtin = _productService.FormatGtin(product, attributeXml);

            // calculating weight adjustment
            var attributeValues = _productAttributeParser.ParseProductAttributeValues(attributeXml);
            var totalWeight = product.BasepriceAmount;

            foreach (var attributeValue in attributeValues)
            {
                switch (attributeValue.AttributeValueType)
                {
                    case AttributeValueType.Simple:
                        //simple attribute
                        totalWeight += attributeValue.WeightAdjustment;
                        break;
                    case AttributeValueType.AssociatedToProduct:
                        //bundled product
                        var associatedProduct = _productService.GetProductById(attributeValue.AssociatedProductId);
                        if (associatedProduct != null)
                            totalWeight += associatedProduct.BasepriceAmount * attributeValue.Quantity;
                        break;
                }
            }

            //price
            var price = "";
            //base price
            var basepricepangv = "";
            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices) && !product.CustomerEntersPrice)
            {
                //we do not calculate price of "customer enters price" option is enabled
                
                var finalPrice = _shoppingCartService.GetUnitPrice(product,
                    _workContext.CurrentCustomer,
                    ShoppingCartType.ShoppingCart,
                    1, attributeXml, 0,
                    rentalStartDate, rentalEndDate,
                    true, out var _, out _);
                var finalPriceWithDiscountBase = _taxService.GetProductPrice(product, finalPrice, out var _);
                var finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);
                price = _priceFormatter.FormatPrice(finalPriceWithDiscount);
                basepricepangv = _priceFormatter.FormatBasePrice(product, finalPriceWithDiscountBase, totalWeight);
            }

            //stock
            var stockAvailability = _productService.FormatStockMessage(product, attributeXml);

            //conditional attributes
            var enabledAttributeMappingIds = new List<int>();
            var disabledAttributeMappingIds = new List<int>();
            if (validateAttributeConditions)
            {
                var attributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                foreach (var attribute in attributes)
                {
                    var conditionMet = _productAttributeParser.IsConditionMet(attribute, attributeXml);
                    if (conditionMet.HasValue)
                    {
                        if (conditionMet.Value)
                            enabledAttributeMappingIds.Add(attribute.Id);
                        else
                            disabledAttributeMappingIds.Add(attribute.Id);
                    }
                }
            }

            //picture. used when we want to override a default product picture when some attribute is selected
            var pictureFullSizeUrl = string.Empty;
            var pictureDefaultSizeUrl = string.Empty;
            if (loadPicture)
            {
                //first, try to get product attribute combination picture
                var pictureId = _productAttributeParser.FindProductAttributeCombination(product, attributeXml)?.PictureId ?? 0;

                //then, let's see whether we have attribute values with pictures
                if (pictureId == 0)
                {
                    pictureId = _productAttributeParser.ParseProductAttributeValues(attributeXml)
                        .FirstOrDefault(attributeValue => attributeValue.PictureId > 0)?.PictureId ?? 0;
                }

                if (pictureId > 0)
                {
                    var productAttributePictureCacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductAttributePictureModelKey,
                        pictureId, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore);
                    var pictureModel = _staticCacheManager.Get(productAttributePictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPictureById(pictureId);
                        return picture == null ? new PictureModel() : new PictureModel
                        {
                            FullSizeImageUrl = _pictureService.GetPictureUrl(ref picture),
                            ImageUrl = _pictureService.GetPictureUrl(ref picture, _mediaSettings.ProductDetailsPictureSize)
                        };
                    });
                    pictureFullSizeUrl = pictureModel.FullSizeImageUrl;
                    pictureDefaultSizeUrl = pictureModel.ImageUrl;
                }

            }

            var isFreeShipping = product.IsFreeShipping;
            if (isFreeShipping && !string.IsNullOrEmpty(attributeXml))
            {
                isFreeShipping = _productAttributeParser.ParseProductAttributeValues(attributeXml)
                    .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                    .Select(attributeValue => _productService.GetProductById(attributeValue.AssociatedProductId))
                    .All(associatedProduct => associatedProduct == null || !associatedProduct.IsShipEnabled || associatedProduct.IsFreeShipping);
            }

            var response = new GenericResponseModel<CartAttributeChangeModel>
            {
                Data = new CartAttributeChangeModel()
                {
                    Gtin = gtin,
                    Mpn = mpn,
                    Sku = sku,
                    Price = price,
                    StockAvailability = stockAvailability,
                    BasePricePangv = basepricepangv,
                    DisabledAttributeMappingIds = disabledAttributeMappingIds,
                    EnabledAttributeMappingIds = enabledAttributeMappingIds,
                    IsFreeShipping = isFreeShipping,
                    PictureDefaultSizeUrl = pictureDefaultSizeUrl
                },
                ErrorList = errors
            };

            return Ok(response);
        }

        [HttpPost("checkoutattributechange")]
        public virtual IActionResult CheckoutAttributeChange([FromBody]BaseQueryModel<string> queryModel)
        {
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart,
                _storeContext.CurrentStore.Id, onlySelectedItems: true);

            _productService.GetProductsByIds(cart.Select(sci => sci.ProductId).ToArray());

            var form = queryModel.FormValues == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();
            var isEditable = form["isEditable"] != null ? bool.Parse(form["isEditable"]) : false;

            ParseAndSaveCheckoutAttributes(cart, form);
            var attributeXml = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
                NopCustomerDefaults.CheckoutAttributes, _storeContext.CurrentStore.Id);

            //conditions
            var enabledAttributeIds = new List<int>();
            var disabledAttributeIds = new List<int>();
            var excludeShippableAttributes = !_shoppingCartService.ShoppingCartRequiresShipping(cart);
            var attributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, excludeShippableAttributes);
            foreach (var attribute in attributes)
            {
                var conditionMet = _checkoutAttributeParser.IsConditionMet(attribute, attributeXml);
                if (conditionMet.HasValue)
                {
                    if (conditionMet.Value)
                        enabledAttributeIds.Add(attribute.Id);
                    else
                        disabledAttributeIds.Add(attribute.Id);
                }
            }

            var response = new GenericResponseModel<CheckoutConfirmOrderModel>();
            response.Data = _cartCheckoutApiModelFactory.PrepareCheckoutConfirmOrderModel(cart);

            return Ok(response);

            // return Ok(new CheckoutAttributeChangeModel
            // {
            //     Cart = _shoppingCartModelFactory.PrepareShoppingCartModel(new ShoppingCartModel(), cart, isEditable),
            //     OrderTotals = _shoppingCartModelFactory.PrepareOrderTotalsModel(cart, isEditable),
            //     SelectedCheckoutAttributess = _shoppingCartModelFactory.FormatSelectedCheckoutAttributes(),
            //     EnabledAttributeIds = enabledAttributeIds,
            //     DisabledAttributeIds = disabledAttributeIds
            // });
        }
        
        [HttpPost("uploadfileproductattribute/{attributeId:min(0)}")]
        public virtual IActionResult UploadFileProductAttribute(int attributeId)
        {
            var attribute = _productAttributeService.GetProductAttributeMappingById(attributeId);
            if (attribute == null)
                return NotFound();

            if (attribute.AttributeControlType != AttributeControlType.FileUpload)
                return BadRequest();

            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.ShoppingCart.NoFileUploaded"));
            }

            var fileBinary = _downloadService.GetDownloadBits(httpPostedFile);

            var qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = _fileProvider.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = _fileProvider.GetFileExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            if (attribute.ValidationFileMaximumSize.HasValue)
            {
                //compare in bytes
                var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    return BadRequest(string.Format(_localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value));
                }
            }

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = fileBinary,
                ContentType = contentType,
                Filename = _fileProvider.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            _downloadService.InsertDownload(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            var response = new GenericResponseModel<UploadFileModel>
            {
                Data = new UploadFileModel()
                {
                    DownloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                    DownloadGuid = download.DownloadGuid,
                },
                Message = _localizationService.GetResource("ShoppingCart.FileUploaded")
            };

            return Ok(response);
        }
        
        [HttpPost("uploadfilecheckoutattribute/{attributeId:min(0)}")]
        public virtual IActionResult UploadFileCheckoutAttribute(int attributeId)
        {
            var attribute = _checkoutAttributeService.GetCheckoutAttributeById(attributeId);
            if (attribute == null)
                return NotFound();

            if (attribute.AttributeControlType != AttributeControlType.FileUpload)
                return BadRequest();

            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.ShoppingCart.NoFileUploaded"));
            }

            var fileBinary = _downloadService.GetDownloadBits(httpPostedFile);

            var qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = _fileProvider.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = _fileProvider.GetFileExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            if (attribute.ValidationFileMaximumSize.HasValue)
            {
                //compare in bytes
                var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    //when returning JSON the mime-type must be set to text/plain
                    //otherwise some browsers will pop-up a "Save As" dialog.
                    return BadRequest(string.Format(_localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value));
                }
            }

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = _fileProvider.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            _downloadService.InsertDownload(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            var response = new GenericResponseModel<UploadFileModel>
            {
                Data = new UploadFileModel()
                {
                    DownloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                    DownloadGuid = download.DownloadGuid,
                },
                Message = _localizationService.GetResource("ShoppingCart.FileUploaded")
            };

            return Ok(response);
        }

        [HttpsRequirement]
        [HttpGet("cart")]
        public virtual IActionResult Cart(int shippingAddress = 0)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return BadRequest();

            var address = new Address();
            if (shippingAddress > 0)
            {
                //existing address
                address = _customerService.GetCustomerAddress(_workContext.CurrentCustomer.Id, shippingAddress)
                    ?? throw new Exception("Address can't be loaded");

                _workContext.CurrentCustomer.ShippingAddressId = address.Id;
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
            }

            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            var response = new GenericResponseModel<ShoppingCartApiModel>
            {
                Data = PrepareShoppingCartApiModel(new ShoppingCartModel(), cart, false)
            };

            if (!response.Data.Cart.Vendors.Any())
                response.ErrorList.Add(_localizationService.GetResource("ShoppingCart.CartIsEmpty"));

            if (shippingAddress > 0)
            {
                foreach (var v in response.Data.Cart.Vendors)
                {
                    var vendor = _vendorService.GetVendorById(v.Id);
                    var coverageChecking = _shipmentProcessor.CheckProductCoverage(vendor, _workContext.CurrentCustomer, address);
                    if (!coverageChecking.InsideCoverage)
                    {
                        v.NotInsideCoverage = true;
                    }
                }
            }

            return Ok(response);
        }
        
        [HttpPost("updatecart")]
        public virtual IActionResult UpdateCart([FromBody]BaseQueryModel<string> queryModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return BadRequest();

            var formValues = queryModel.FormValues;
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            var form = formValues == null ? new NameValueCollection() : formValues.ToNameValueCollection();
            var isEditable = form["isEditable"] != null ? bool.Parse(form["isEditable"]) : false;
            //get identifiers of items to remove
            var itemIdsToRemove = new List<int>();
            if (form["removefromcart"] != null)
            {
                itemIdsToRemove = form["removefromcart"]
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(idString => int.TryParse(idString, out var id) ? id : 0)
                    .Distinct().ToList();
            }

            //get order items with changed quantity
            var itemsWithNewQuantity = cart.Select(item => new
            {
                //try to get a new quantity for the item, set 0 for items to remove
                NewQuantity = itemIdsToRemove.Contains(item.Id) ? 0 : int.TryParse(form[$"itemquantity{item.Id}"], out var quantity) ? quantity : item.Quantity,
                Item = item
            }).Where(item => item.NewQuantity != item.Item.Quantity);

            //order cart items
            //first should be items with a reduced quantity and that require other products; or items with an increased quantity and are required for other products
            var orderedCart = itemsWithNewQuantity
                .OrderByDescending(cartItem =>
                    (cartItem.NewQuantity < cartItem.Item.Quantity && (_productService.GetProductById(cartItem.Item.ProductId)?.RequireOtherProducts ?? false)) ||
                    (cartItem.NewQuantity > cartItem.Item.Quantity &&
                        cart.Any(item => _productService.GetProductById(item.ProductId) != null && _productService.GetProductById(item.ProductId).RequireOtherProducts && _productService.ParseRequiredProductIds(_productService.GetProductById(item.ProductId)).Contains(cartItem.Item.ProductId))))
                .ToList();

            //try to update cart items with new quantities and get warnings
            var warnings = orderedCart.Select(cartItem => new
            {
                ItemId = cartItem.Item.Id,
                Warnings = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                    cartItem.Item.Id, cartItem.Item.AttributesXml, cartItem.Item.CustomerEnteredPrice,
                    cartItem.Item.RentalStartDateUtc, cartItem.Item.RentalEndDateUtc, cartItem.NewQuantity, true)
            }).ToList();

            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);

            //updated cart
            cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            //prepare model
            var model = PrepareShoppingCartApiModel(new ShoppingCartModel(), cart, isEditable);

            //update current warnings
            foreach (var warningItem in warnings.Where(warningItem => warningItem.Warnings.Any()))
            {
                //find shopping cart item model to display appropriate warnings
                var itemModel = model.Cart.Vendors.SelectMany(item => item.Items).FirstOrDefault(item => item.Id == warningItem.ItemId);
                if (itemModel != null)
                    itemModel.Warnings = warningItem.Warnings.Concat(itemModel.Warnings).Distinct().ToList();
            }

            var response = new GenericResponseModel<ShoppingCartApiModel>
            {
                Data = model
            };

            if (!response.Data.Cart.Vendors.Any())
                response.ErrorList.Add(_localizationService.GetResource("ShoppingCart.CartIsEmpty"));

            int.TryParse(form["shipping_address"], out var shippingAddressId);

            if (shippingAddressId > 0)
            {
                var address = _customerService.GetCustomerAddress(_workContext.CurrentCustomer.Id, shippingAddressId)
                    ?? throw new Exception("Address can't be loaded");

                foreach (var v in response.Data.Cart.Vendors)
                {
                    var vendor = _vendorService.GetVendorById(v.Id);
                    var coverageChecking = _shipmentProcessor.CheckProductCoverage(vendor, _workContext.CurrentCustomer, address);
                    if (!coverageChecking.InsideCoverage)
                    {
                        v.NotInsideCoverage = true;
                    }
                }
            }

            return Ok(response);
        }
        
        [HttpPost("applydiscountcoupon")]
        public virtual IActionResult ApplyDiscountCoupon([FromBody]BaseQueryModel<string> queryModel)
        {
            //cart
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            var form = queryModel == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();
            var isEditable = form["isEditable"] != null ? bool.Parse(form["isEditable"]) : false;
            var discountcouponcode = form["discountcouponcode"];

            //trim
            if (discountcouponcode != null)
                discountcouponcode = discountcouponcode.Trim();

            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);

            var cartModel = new ShoppingCartModel();
            if (!string.IsNullOrWhiteSpace(discountcouponcode))
            {
                //we find even hidden records here. this way we can display a user-friendly message if it's expired
                var discounts = _discountService.GetAllDiscounts(couponCode: discountcouponcode, showHidden: true)
                    .Where(d => d.RequiresCouponCode)
                    .ToList();
                if (discounts.Any())
                {
                    var userErrors = new List<string>();
                    var anyValidDiscount = discounts.Any(discount =>
                    {
                        var validationResult = _discountService.ValidateDiscount(discount, _workContext.CurrentCustomer, new[] { discountcouponcode });
                        userErrors.AddRange(validationResult.Errors);

                        return validationResult.IsValid;
                    });

                    if (anyValidDiscount)
                    {
                        //valid
                        _customerService.ApplyDiscountCouponCode(_workContext.CurrentCustomer, discountcouponcode);
                        cartModel.DiscountBox.Messages.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.Applied"));
                        cartModel.DiscountBox.IsApplied = true;
                    }
                    else
                    {
                        if (userErrors.Any())
                            //some user errors
                            cartModel.DiscountBox.Messages = userErrors;
                        else
                            //general error text
                            cartModel.DiscountBox.Messages.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));
                    }
                }
                else
                    //discount cannot be found
                    cartModel.DiscountBox.Messages.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));
            }
            else
                //empty coupon code
                cartModel.DiscountBox.Messages.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));

            var response = new GenericResponseModel<ShoppingCartApiModel>
            {
                Data = PrepareShoppingCartApiModel(cartModel, cart, isEditable)
            };

            if (!response.Data.Cart.Vendors.Any())
                response.ErrorList.Add(_localizationService.GetResource("ShoppingCart.CartIsEmpty"));

            return Ok(response);
        }

        [HttpPost("applygiftcard")]
        public virtual IActionResult ApplyGiftCard([FromBody]BaseQueryModel<string> queryModel)
        {
            //cart
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            var form = queryModel.FormValues == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();
            var isEditable = form["isEditable"] != null ? bool.Parse(form["isEditable"]) : false;

            var giftcardcouponcode = form["giftcardcouponcode"];
            //trim
            if (giftcardcouponcode != null)
                giftcardcouponcode = giftcardcouponcode.Trim();

            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);

            var cartModel = new ShoppingCartModel();
            if (!_shoppingCartService.ShoppingCartIsRecurring(cart))
            {
                if (!string.IsNullOrWhiteSpace(giftcardcouponcode))
                {
                    var giftCard = _giftCardService.GetAllGiftCards(giftCardCouponCode: giftcardcouponcode).FirstOrDefault();
                    var isGiftCardValid = giftCard != null && _giftCardService.IsGiftCardValid(giftCard);
                    if (isGiftCardValid)
                    {
                        _customerService.ApplyGiftCardCouponCode(_workContext.CurrentCustomer, giftcardcouponcode);
                        cartModel.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.Applied");
                        cartModel.GiftCardBox.IsApplied = true;
                    }
                    else
                    {
                        cartModel.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");
                        cartModel.GiftCardBox.IsApplied = false;
                    }
                }
                else
                {
                    cartModel.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");
                    cartModel.GiftCardBox.IsApplied = false;
                }
            }
            else
            {
                cartModel.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.DontWorkWithAutoshipProducts");
                cartModel.GiftCardBox.IsApplied = false;
            }

            var response = new GenericResponseModel<ShoppingCartApiModel>
            {
                Data = PrepareShoppingCartApiModel(cartModel, cart, isEditable)
            };

            if (!response.Data.Cart.Vendors.Any())
                response.ErrorList.Add(_localizationService.GetResource("ShoppingCart.CartIsEmpty"));

            return Ok(response);
        }
        
        [HttpPost("cart/estimateshipping")]
        public virtual IActionResult GetEstimateShipping([FromBody]BaseQueryModel<EstimateShippingModel> queryModel)
        {
            var response = new GenericResponseModel<EstimateShippingResultModel>();
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id,onlySelectedItems:true);

            var form = queryModel.FormValues == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();
            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);

            var data = queryModel.Data;
            if (string.IsNullOrEmpty(data.ZipPostalCode))
            {
                response.ErrorList.Add(_localizationService.GetResource("ShoppingCart.EstimateShipping.ZipPostalCode.Required"));
            }

            if (data.CountryId == null || data.CountryId == 0)
            {
                response.ErrorList.Add(_localizationService.GetResource("ShoppingCart.EstimateShipping.Country.Required"));
            }

            response.Data = _shoppingCartModelFactory.PrepareEstimateShippingResultModel(cart, data.CountryId, data.StateProvinceId, data.ZipPostalCode,true);

            return Ok(response);
        }
        
        [HttpPost("removediscountcoupon")]
        public virtual IActionResult RemoveDiscountCoupon([FromBody]BaseQueryModel<string> queryModel)
        {
            var form = queryModel.FormValues == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();

            //get discount identifier
            var discountId = 0;
            foreach (string formValue in form.Keys)
                if (formValue.StartsWith("removediscount-", StringComparison.InvariantCultureIgnoreCase))
                    discountId = Convert.ToInt32(formValue.Substring("removediscount-".Length));
            var discount = _discountService.GetDiscountById(discountId);
            if (discount != null)
                _customerService.RemoveDiscountCouponCode(_workContext.CurrentCustomer, discount.CouponCode);

            var isEditable = form["isEditable"] != null ? bool.Parse(form["isEditable"]) : false;
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            var response = new GenericResponseModel<ShoppingCartApiModel>
            {
                Data = PrepareShoppingCartApiModel(new ShoppingCartModel(), cart, isEditable)
            };

            if (!response.Data.Cart.Vendors.Any())
                response.ErrorList.Add(_localizationService.GetResource("ShoppingCart.CartIsEmpty"));

            return Ok(response);
        }

        [HttpPost("removegiftcardcode")]
        public virtual IActionResult RemoveGiftCardCode([FromBody]BaseQueryModel<string> queryModel)
        {
            var form = queryModel.FormValues == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();

            var isEditable = form["isEditable"] != null ? bool.Parse(form["isEditable"]) : false;

            //get gift card identifier
            var giftCardId = 0;
            foreach (string formValue in form.Keys)
                if (formValue.StartsWith("removegiftcard-", StringComparison.InvariantCultureIgnoreCase))
                    giftCardId = Convert.ToInt32(formValue.Substring("removegiftcard-".Length));
            var gc = _giftCardService.GetGiftCardById(giftCardId);
            if (gc != null)
                _customerService.RemoveGiftCardCouponCode(_workContext.CurrentCustomer, gc.GiftCardCouponCode);

            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            var response = new GenericResponseModel<ShoppingCartApiModel>
            {
                Data = PrepareShoppingCartApiModel(new ShoppingCartModel(), cart, isEditable)
            };

            if (!response.Data.Cart.Vendors.Any())
                response.ErrorList.Add(_localizationService.GetResource("ShoppingCart.CartIsEmpty"));

            return Ok(response);
        }

        //[HttpPost("selectcheckcartitems2")]
        //public virtual IActionResult SelectCheckItems([FromBody] BaseQueryModel<List<int>> queryModel)
        //{
        //    if ((queryModel?.Data?.Count ?? 0) == 0)
        //        return BadRequest(_localizationService.GetResource("ShoppingCart.AtleastOneItemToCheckout"));

        //    var selectedItems = queryModel.Data;

        //    var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

        //    if (cart.Count == 0)
        //        return BadRequest(_localizationService.GetResource("ShoppingCart.CartIsEmpty"));

        //    if (!selectedItems.All(q => cart.Select(c => c.Id).Contains(q)))
        //        return BadRequest(_localizationService.GetResource("ShoppingCart.SelectedItemNotInCart"));

        //    var form = queryModel.FormValues == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();

        //    int.TryParse(form["shipping_address_id"], out var shippingAddressId);

        //    var address = new Address();
        //    if (shippingAddressId > 0)
        //    {
        //        //existing address
        //        address = _customerService.GetCustomerAddress(_workContext.CurrentCustomer.Id, shippingAddressId)
        //            ?? throw new Exception("Address can't be loaded");

        //        _workContext.CurrentCustomer.ShippingAddressId = address.Id;
        //        _customerService.UpdateCustomer(_workContext.CurrentCustomer);
        //    }
        //    else
        //    {
        //        throw new ArgumentException($"{nameof(shippingAddressId)} cannot be 0");
        //    }

        //    var insideCoverage = true;
        //    var toUpdateSci = new List<int>();
        //    var outsideCoverageSci = new List<CartItemsOutsideCoverage>();
        //    foreach (var id in selectedItems)
        //    {
        //        var cartItem = _shoppingCartService.GetShoppingCartItemById(id);
        //        var product = _productService.GetProductById(cartItem.ProductId);
        //        var coverageChecking = _shipmentProcessor.CheckProductCoverage(product, _workContext.CurrentCustomer, address);
        //        if (!coverageChecking.InsideCoverage)
        //        {
        //            insideCoverage = false;
        //            var outCoverage = new CartItemsOutsideCoverage();
        //            outCoverage.Id = id;
        //            outCoverage.DeliveryModeId = coverageChecking.DeliveryModeId;
        //            outCoverage.VendorId = coverageChecking.VendorId;
        //            outsideCoverageSci.Add(outCoverage);
        //        }
        //        else
        //        {
        //            toUpdateSci.Add(id);
        //        }
        //    }
        //    _shoppingCartService.UpdateCustomerSelectedSci(_workContext.CurrentCustomer, toUpdateSci);

        //    if (insideCoverage)
        //    {
        //        return Ok();
        //    }
        //    else
        //    {
        //        return RedirectToAction("cart", new { isEditable = false, itemNotInCoverage  = outsideCoverageSci });
        //    }
        //}

        #endregion

        #region Wishlist

        [HttpGet("wishlist/{customerguid?}")]
        public virtual IActionResult Wishlist(Guid? customerGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return BadRequest();

            var customer = customerGuid.HasValue ?
                _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;
            if (customer == null)
                return NotFound();

            var cart = _shoppingCartService.GetShoppingCart(customer, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id);

            var response = new GenericResponseModel<WishlistModel>
            {
                Data = _shoppingCartModelFactory.PrepareWishlistModel(new WishlistModel(), cart, !customerGuid.HasValue)
            };

            foreach (var item in response.Data.Items)
            {
                var product = _productService.GetProductById(item.ProductId);

                var productReview = _productModelFactory.PrepareProductReviewOverviewModel(product);
                item.ProductReviewOverview = productReview;
            }
            return Ok(response);
        }
        
        [HttpPost("updatewishlist")]
        public virtual IActionResult UpdateWishlist([FromBody]BaseQueryModel<string> queryModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return BadRequest();

            var customer = _workContext.CurrentCustomer.Id;
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id);

            var form = queryModel.FormValues == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();

            var allIdsToRemove = form["removefromcart"] != null
                ? form["removefromcart"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList()
                : new List<int>();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                var remove = allIdsToRemove.Contains(sci.ProductId);
                var isCustomer = sci.CustomerId == customer ? true : false;
                if (remove && isCustomer)
                    _shoppingCartService.DeleteShoppingCartItem(sci);
                else
                {
                    foreach (string formKey in form.Keys)
                        if (formKey.Equals($"itemquantity{sci.Id}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (int.TryParse(form[formKey], out var newQuantity))
                            {
                                var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                                    sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
                                    sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                                    newQuantity, true);
                                innerWarnings.Add(sci.Id, currSciWarnings);
                            }
                            break;
                        }
                }
            }

            //updated wishlist
            cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id);
            var response = new GenericResponseModel<WishlistModel>
            {
                Data = _shoppingCartModelFactory.PrepareWishlistModel(new WishlistModel(), cart)
            };

            //update current warnings
            foreach (var kvp in innerWarnings)
            {
                //kvp = <cart item identifier, warnings>
                var sciId = kvp.Key;
                var warnings = kvp.Value;
                //find model
                var sciModel = response.Data.Items.FirstOrDefault(x => x.Id == sciId);
                if (sciModel != null)
                    foreach (var w in warnings)
                        if (!sciModel.Warnings.Contains(w))
                            sciModel.Warnings.Add(w);
            }
            return Ok(response);
        }
        
        [HttpPost("additemstocartfromwishlist")]
        public virtual IActionResult AddItemsToCartFromWishlist([FromBody]BaseQueryModel<string> queryModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return BadRequest();

            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return BadRequest();

            var form = queryModel.FormValues == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();
            var customerGuid = form["customerGuid"] != null ? Guid.Parse(form["customerGuid"]) : (Guid?)null;

            var pageCustomer = customerGuid.HasValue
                ? _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;
            if (pageCustomer == null)
                return NotFound();

            var pageCart = _shoppingCartService.GetShoppingCart(pageCustomer, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id);

            var allWarnings = new List<string>();
            var numberOfAddedItems = 0;

            var allIdsToAdd = form["addtocart"] != null
                ? form["addtocart"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
                : new List<int>();

            foreach (var sci in pageCart)
            {
                if (allIdsToAdd.Contains(sci.Id))
                {
                    var warnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                        _productService.GetProductById(sci.ProductId), ShoppingCartType.ShoppingCart,
                        _storeContext.CurrentStore.Id,
                        sci.AttributesXml, sci.CustomerEnteredPrice,
                        sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, true);
                    if (!warnings.Any())
                        numberOfAddedItems++;
                    if (_shoppingCartSettings.MoveItemsFromWishlistToCart && //settings enabled
                        !customerGuid.HasValue && //own wishlist
                        !warnings.Any()) //no warnings ( already in the cart)
                    {
                        //let's remove the item from wishlist
                        _shoppingCartService.DeleteShoppingCartItem(sci);
                    }
                    allWarnings.AddRange(warnings);
                }
            }

            var response = new GenericResponseModel<WishlistModel>();
            if (numberOfAddedItems > 0)
            {
                if (allWarnings.Any())
                {
                    response.ErrorList.AddRange(allWarnings);
                    return BadRequest(response);
                }

                response.Message = _localizationService.GetResource("Products.ProductHasBeenAddedToTheCart");
                return Ok(response);
            }
            //no items added. redisplay the wishlist page

            if (allWarnings.Any())
            {
                response.ErrorList.AddRange(allWarnings);
                return BadRequest(response);
            }

            var cart = _shoppingCartService.GetShoppingCart(pageCustomer, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id);
            response.Data = _shoppingCartModelFactory.PrepareWishlistModel(new WishlistModel(), cart, !customerGuid.HasValue);

            return Ok(response);
        }

        [HttpGet("emailwishlist")]
        public virtual IActionResult EmailWishlist()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist) || !_shoppingCartSettings.EmailWishlistEnabled)
                return BadRequest();

            var response = new GenericResponseModel<WishlistEmailAFriendModel>();
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id);

            if (!cart.Any())
            {
                response.ErrorList.Add(_localizationService.GetResource("WishList.CartIsEmpty"));
                return BadRequest(response);
            }

            response.Data = _shoppingCartModelFactory.PrepareWishlistEmailAFriendModel(new WishlistEmailAFriendModel(), false);
            return Ok(response);
        }

        [HttpPost("emailwishlistsend")]
        public virtual IActionResult EmailWishlistSend([FromBody]BaseQueryModel<WishlistEmailAFriendModel> queryModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist) || !_shoppingCartSettings.EmailWishlistEnabled)
                return BadRequest();

            var response = new GenericResponseModel<WishlistEmailAFriendModel>();
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id);

            if (!cart.Any())
                ModelState.AddModelError("", _localizationService.GetResource("WishList.CartIsEmpty"));

            //check whether the current customer is guest and ia allowed to email wishlist
            if (_customerService.IsGuest(_workContext.CurrentCustomer) && !_shoppingCartSettings.AllowAnonymousUsersToEmailWishlist)
                ModelState.AddModelError("", _localizationService.GetResource("Wishlist.EmailAFriend.OnlyRegisteredUsers"));

            var model = queryModel.Data;

            if (ModelState.IsValid)
            {
                //email
                _workflowMessageService.SendWishlistEmailAFriendMessage(_workContext.CurrentCustomer,
                        _workContext.WorkingLanguage.Id, model.YourEmailAddress,
                        model.FriendEmail, HtmlHelper.FormatText(model.PersonalMessage, false, true, false, false, false, false));

                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("Wishlist.EmailAFriend.SuccessfullySent");
                response.Data = model;

                return Ok(response);
            }

            //If we got this far, something failed, redisplay form
            response.Data = _shoppingCartModelFactory.PrepareWishlistEmailAFriendModel(model, true);

            return Ok(response);
        }

        #endregion
    }
}