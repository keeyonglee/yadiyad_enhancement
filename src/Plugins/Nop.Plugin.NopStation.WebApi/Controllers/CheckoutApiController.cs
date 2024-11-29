using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Web.Factories;
using Nop.Web.Models.Checkout;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Nop.Web.Extensions;
using Nop.Core.Domain.Catalog;
using Microsoft.Extensions.Primitives;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Plugin.NopStation.WebApi.Models.Checkout;
using Nop.Web.Models.ShoppingCart;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.NopStation.WebApi.Factories;
using Nop.Services.Catalog;
using Nop.Services.ShuqOrders;

namespace Nop.Plugin.NopStation.WebApi.Controllers
{
    [Route("api/checkout")]
    public partial class CheckoutApiController : BaseApiController
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressService _addressService;
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly CartCheckoutApiModelFactory _cartCheckoutApiModelFactory;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IProductService _productService;
        private readonly IShuqOrderService _shuqOrderService;
        private readonly IShipmentService _shipmentService;

        #endregion

        #region Ctor

        public CheckoutApiController(AddressSettings addressSettings,
            CustomerSettings customerSettings,
            IAddressAttributeParser addressAttributeParser,
            IAddressService addressService,
            ICheckoutModelFactory checkoutModelFactory,
            IShoppingCartModelFactory shoppingCartModelFactory,
            ICountryService countryService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext,
            OrderSettings orderSettings,
            PaymentSettings paymentSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            IAddressAttributeService addressAttributeService,
            CartCheckoutApiModelFactory cartCheckoutApiModelFactory,
            ICheckoutAttributeService checkoutAttributeService,
            ICheckoutAttributeParser checkoutAttributeParser,
            IProductService productService,
            IShuqOrderService shuqOrderService,
            IShipmentService shipmentService)
        {
            _addressSettings = addressSettings;
            _customerSettings = customerSettings;
            _addressAttributeParser = addressAttributeParser;
            _addressService = addressService;
            _checkoutModelFactory = checkoutModelFactory;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _countryService = countryService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _paymentSettings = paymentSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _shippingSettings = shippingSettings;
            _addressAttributeService = addressAttributeService;
            _cartCheckoutApiModelFactory = cartCheckoutApiModelFactory;
            _checkoutAttributeService = checkoutAttributeService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _productService = productService;
            _shuqOrderService = shuqOrderService;
            _shipmentService = shipmentService;
        }

        #endregion
        #region Utilities

        protected virtual void GenerateOrderGuid(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest == null)
                return;

            //we should use the same GUID for multiple payment attempts
            //this way a payment gateway can prevent security issues such as credit card brute-force attacks
            //in order to avoid any possible limitations by payment gateway we reset GUID periodically
            var previousPaymentRequest = _genericAttributeService.GetPaymentRequestAttribute(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
            if (_paymentSettings.RegenerateOrderGuidInterval > 0 &&
                previousPaymentRequest != null &&
                previousPaymentRequest.OrderGuidGeneratedOnUtc.HasValue)
            {
                var interval = DateTime.UtcNow - previousPaymentRequest.OrderGuidGeneratedOnUtc.Value;
                if (interval.TotalSeconds < _paymentSettings.RegenerateOrderGuidInterval)
                {
                    processPaymentRequest.OrderGuid = previousPaymentRequest.OrderGuid;
                    processPaymentRequest.OrderGuidGeneratedOnUtc = previousPaymentRequest.OrderGuidGeneratedOnUtc;
                }
            }

            if (processPaymentRequest.OrderGuid == Guid.Empty)
            {
                processPaymentRequest.OrderGuid = Guid.NewGuid();
                processPaymentRequest.OrderGuidGeneratedOnUtc = DateTime.UtcNow;
            }
        }

        protected virtual bool IsMinimumOrderPlacementIntervalValid(Customer customer)
        {
            //prevent 2 orders being placed within an X seconds time frame
            if (_orderSettings.MinimumOrderPlacementInterval == 0)
                return true;

            var lastOrder = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                .FirstOrDefault();
            if (lastOrder == null)
                return true;

            var interval = DateTime.UtcNow - lastOrder.CreatedOnUtc;
            return interval.TotalSeconds > _orderSettings.MinimumOrderPlacementInterval;
        }

        protected string ParseCustomAddressAttributes(NameValueCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = string.Empty;

            foreach (var attribute in _addressAttributeService.GetAllAddressAttributes())
            {
                var controlId = string.Format(NopCommonDefaults.AddressAttributeControlName, attribute.Id);
                var attributeValues = form[controlId];
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        if (!StringValues.IsNullOrEmpty(attributeValues) && int.TryParse(attributeValues, out var value) && value > 0)
                            attributesXml = _addressAttributeParser.AddAddressAttribute(attributesXml, attribute, value.ToString());
                        break;

                    case AttributeControlType.Checkboxes:
                        foreach (var attributeValue in attributeValues.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (int.TryParse(attributeValue, out value) && value > 0)
                                attributesXml = _addressAttributeParser.AddAddressAttribute(attributesXml, attribute, value.ToString());
                        }

                        break;

                    case AttributeControlType.ReadonlyCheckboxes:
                        //load read-only (already server-side selected) values
                        var addressAttributeValues = _addressAttributeService.GetAddressAttributeValues(attribute.Id);
                        foreach (var addressAttributeValue in addressAttributeValues)
                        {
                            if (addressAttributeValue.IsPreSelected)
                                attributesXml = _addressAttributeParser.AddAddressAttribute(attributesXml, attribute, addressAttributeValue.Id.ToString());
                        }

                        break;

                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        if (!StringValues.IsNullOrEmpty(attributeValues))
                            attributesXml = _addressAttributeParser.AddAddressAttribute(attributesXml, attribute, attributeValues.ToString().Trim());
                        break;

                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.FileUpload:
                    default:
                        break;
                }
            }

            return attributesXml;
        }

        protected virtual IActionResult LoadStepAfterShippingAddress(IList<ShoppingCartItem> cart, GenericResponseModel<OpcStepResponseModel> response)
        {
            var shippingMethodModel = _checkoutModelFactory.PrepareShippingMethodModel(cart, _customerService.GetCustomerShippingAddress(_workContext.CurrentCustomer));
            if (_shippingSettings.BypassShippingMethodSelectionIfOnlyOne &&
                shippingMethodModel.ShippingMethods.Count == 1)
            {
                //if we have only one shipping method, then a customer doesn't have to choose a shipping method
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    NopCustomerDefaults.SelectedShippingOptionAttribute,
                    shippingMethodModel.ShippingMethods.First().ShippingOption,
                    _storeContext.CurrentStore.Id);

                //load next step
                return LoadStepAfterShippingMethod(cart, response);
            }

            response.Data = new OpcStepResponseModel();
            response.Data.NextStep = OpcStep.ShippingMethod;
            response.Data.ShippingMethodModel = shippingMethodModel;

            return Ok(response);
        }

        protected virtual IActionResult LoadStepAfterShippingMethod(IList<ShoppingCartItem> cart, GenericResponseModel<OpcStepResponseModel> response)
        {
            //Check whether payment workflow is required
            //we ignore reward points during cart total calculation
            var isPaymentWorkflowRequired = _orderProcessingService.IsPaymentWorkflowRequired(cart, false);
            if (isPaymentWorkflowRequired)
            {
                //filter by country
                var filterByCountryId = 0;

                if (_addressSettings.CountryEnabled)
                {
                    filterByCountryId = _customerService.GetCustomerBillingAddress(_workContext.CurrentCustomer)?.CountryId ?? 0;
                }

                //payment is required
                var paymentMethodModel = _checkoutModelFactory.PreparePaymentMethodModel(cart, filterByCountryId);

                if (_paymentSettings.BypassPaymentMethodSelectionIfOnlyOne &&
                    paymentMethodModel.PaymentMethods.Count == 1 && !paymentMethodModel.DisplayRewardPoints)
                {
                    //if we have only one payment method and reward points are disabled or the current customer doesn't have any reward points
                    //so customer doesn't have to choose a payment method

                    var selectedPaymentMethodSystemName = paymentMethodModel.PaymentMethods[0].PaymentMethodSystemName;
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        NopCustomerDefaults.SelectedPaymentMethodAttribute,
                        selectedPaymentMethodSystemName, _storeContext.CurrentStore.Id);

                    var paymentMethodInst = _paymentPluginManager
                        .LoadPluginBySystemName(selectedPaymentMethodSystemName, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
                    if (!_paymentPluginManager.IsPluginActive(paymentMethodInst))
                        throw new Exception("Selected payment method can't be parsed");

                    return LoadStepAfterPaymentMethod(paymentMethodInst, cart, response);
                }

                response.Data = new OpcStepResponseModel();
                response.Data.NextStep = OpcStep.PaymentMethod;
                response.Data.PaymentMethodModel = paymentMethodModel;
                
                return Ok(response);
            }

            //payment is not required
            _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, null, _storeContext.CurrentStore.Id);

            response.Data = new OpcStepResponseModel();
            response.Data.NextStep = OpcStep.ConfirmOrder;
            response.Data.ConfirmModel = _cartCheckoutApiModelFactory.PrepareCheckoutConfirmOrderModel(cart);

            return Ok(response);
        }

        protected virtual IActionResult LoadStepAfterPaymentMethod(IPaymentMethod paymentMethod, IList<ShoppingCartItem> cart, GenericResponseModel<OpcStepResponseModel> response)
        {
            response.Data = new OpcStepResponseModel();

            if (paymentMethod.SkipPaymentInfo ||
                (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection && _paymentSettings.SkipPaymentInfoStepForRedirectionPaymentMethods))
            {
                //skip payment info page
                var paymentInfo = new ProcessPaymentRequest();

                //session save
                _genericAttributeService.SavePaymentRequestAttribute(_workContext.CurrentCustomer, paymentInfo, _storeContext.CurrentStore.Id);

                response.Data.NextStep = OpcStep.ConfirmOrder;
                response.Data.ConfirmModel = _cartCheckoutApiModelFactory.PrepareCheckoutConfirmOrderModel(cart);

                return Ok(response);
            }

            //return payment info page
            var paymentInfoModel = _checkoutModelFactory.PreparePaymentInfoModel(paymentMethod);

            response.Data.NextStep = OpcStep.PaymentInfo;
            response.Data.PaymentInfoModel = paymentInfoModel;

            return Ok(response);
        }

        protected virtual bool ParsePickupInStore(NameValueCollection form)
        {
            var pickupInStore = false;

            var pickupInStoreParameter = form["PickupInStore"];
            if (!StringValues.IsNullOrEmpty(pickupInStoreParameter))
                bool.TryParse(pickupInStoreParameter, out pickupInStore);

            return pickupInStore;
        }

        #endregion

        #region Methods
        
        [HttpPost("selectcartitems")]
        public virtual IActionResult SelectItem([FromBody]BaseQueryModel<List<int>> queryModel)
        {
            if((queryModel?.Data?.Count ?? 0) == 0)
                return BadRequest(_localizationService.GetResource("ShoppingCart.AtleastOneItemToCheckout"));
            
            var selectedItems = queryModel.Data;
            
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);
            
            if(cart.Count == 0)
                return BadRequest(_localizationService.GetResource("ShoppingCart.CartIsEmpty"));
            
            if(!selectedItems.All(q => cart.Select(c => c.Id).Contains(q)))
                return BadRequest(_localizationService.GetResource("ShoppingCart.SelectedItemNotInCart"));

            _shoppingCartService.UpdateCustomerSelectedSci(_workContext.CurrentCustomer, selectedItems);
            cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            var response = new GenericResponseModel<ShoppingCartModel>();
            var scModel = new ShoppingCartModel();
            response.Data = _shoppingCartModelFactory.PrepareShoppingCartModel(scModel, cart, isEditable: false, skipShippingCalculation: false);

            return Ok(response);
        }

        [HttpGet("getbilling")]
        public virtual IActionResult GetBilling()
        {
            if (_orderSettings.CheckoutDisabled)
                return BadRequest(_localizationService.GetResource("Checkout.Disabled"));

            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart,
                _storeContext.CurrentStore.Id, onlySelectedItems: true);

            if (!cart.Any())
                return BadRequest(_localizationService.GetResource("ShoppingCart.CartIsEmpty"));

            if (_customerService.IsGuest(_workContext.CurrentCustomer) && !_orderSettings.AnonymousCheckoutAllowed)
                return Unauthorized(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.AnonymousCheckoutNotAllowed"));

            var response = new GenericResponseModel<OnePageCheckoutModel>();
            response.Data = _checkoutModelFactory.PrepareOnePageCheckoutModel(cart);

            return Ok(response);
        }

        [HttpPost("savebilling")]
        public virtual IActionResult SaveBilling([FromBody] BaseQueryModel<CheckoutBillingAddressModel> queryModel)
        {
            try
            {
                if (_orderSettings.CheckoutDisabled)
                    return BadRequest(_localizationService.GetResource("Checkout.Disabled"));

                var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer,
                    ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id, onlySelectedItems: true);

                if (!cart.Any())
                    return BadRequest(_localizationService.GetResource("ShoppingCart.CartIsEmpty"));

                if (_customerService.IsGuest(_workContext.CurrentCustomer) && !_orderSettings.AnonymousCheckoutAllowed)
                    return Unauthorized(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.AnonymousCheckoutNotAllowed"));

                var response = new GenericResponseModel<OpcStepResponseModel>();
                response.Data = new OpcStepResponseModel();

                var form = queryModel.FormValues.ToNameValueCollection();
                int.TryParse(form["billing_address_id"], out var billingAddressId);

                if (billingAddressId > 0)
                {
                    //existing address
                    var address = _customerService.GetCustomerAddress(_workContext.CurrentCustomer.Id, billingAddressId);
                    if (address == null)
                        return BadRequest(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.AddressCantBeLoaded"));

                    _workContext.CurrentCustomer.BillingAddressId = address.Id;
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                }
                else
                {
                    //new address
                    var newAddress = queryModel.Data.BillingNewAddress;

                    //custom address attributes
                    var customAttributes = ParseCustomAddressAttributes(form);
                    var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
                    foreach (var error in customAttributeWarnings)
                    {
                        ModelState.AddModelError("", error);
                    }

                    //validate model
                    if (!ModelState.IsValid)
                    {
                        //model is not valid. redisplay the form with errors
                        var billingAddressModel = _checkoutModelFactory.PrepareBillingAddressModel(cart,
                            selectedCountryId: newAddress.CountryId,
                            overrideAttributesXml: customAttributes);
                        billingAddressModel.NewAddressPreselected = true;

                        response.Data.NextStep = OpcStep.BillingAddress;
                        response.Data.BillingAddressModel = billingAddressModel;

                        foreach (var modelState in ModelState.Values)
                            foreach (var error in modelState.Errors)
                                response.ErrorList.Add(error.ErrorMessage);

                        return BadRequest(response);
                    }

                    //try to find an address with the same values (don't duplicate records)
                    var address = _addressService.FindAddress(_customerService.GetAddressesByCustomerId(_workContext.CurrentCustomer.Id).ToList(),
                        newAddress.FirstName, newAddress.LastName, newAddress.PhoneNumber,
                        newAddress.Email, newAddress.FaxNumber, newAddress.Company,
                        newAddress.Address1, newAddress.Address2, newAddress.City,
                        newAddress.County, newAddress.StateProvinceId, newAddress.ZipPostalCode,
                        newAddress.CountryId, customAttributes);
                    if (address == null)
                    {
                        //address is not found. let's create a new one
                        address = newAddress.ToEntity();
                        address.CustomAttributes = customAttributes;
                        address.CreatedOnUtc = DateTime.UtcNow;
                        //some validation
                        if (address.CountryId == 0)
                            address.CountryId = null;
                        if (address.StateProvinceId == 0)
                            address.StateProvinceId = null;

                        _addressService.InsertAddress(address);
                        _customerService.InsertCustomerAddress(_workContext.CurrentCustomer, address);

                    }
                    _workContext.CurrentCustomer.BillingAddressId = address.Id;
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                }

                if (_shoppingCartService.ShoppingCartRequiresShipping(cart))
                {
                    //shipping is required
                    var address = _customerService.GetCustomerBillingAddress(_workContext.CurrentCustomer);

                    //by default Shipping is available if the country is not specified
                    var shippingAllowed = _addressSettings.CountryEnabled ? _countryService.GetCountryByAddress(address)?.AllowsShipping ?? false : true;
                    if (_shippingSettings.ShipToSameAddress && queryModel.Data.ShipToSameAddress && shippingAllowed)
                    {
                        //ship to the same address
                        _workContext.CurrentCustomer.ShippingAddressId = address.Id;
                        _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                        //reset selected shipping method (in case if "pick up in store" was selected)
                        _genericAttributeService.SaveAttribute<ShippingOption>(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedShippingOptionAttribute, null, _storeContext.CurrentStore.Id);
                        _genericAttributeService.SaveAttribute<PickupPoint>(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedPickupPointAttribute, null, _storeContext.CurrentStore.Id);
                        //limitation - "Ship to the same address" doesn't properly work in "pick up in store only" case (when no shipping plugins are available) 
                        return LoadStepAfterShippingAddress(cart, response);
                    }

                    //do not ship to the same address
                    var shippingAddressModel = _checkoutModelFactory.PrepareShippingAddressModel(cart, prePopulateNewAddressWithCustomerFields: true);

                    response.Data.NextStep = OpcStep.ShippingAddress;
                    response.Data.ShippingAddressModel = shippingAddressModel;

                    return Ok(response);
                }

                //shipping is not required
                _workContext.CurrentCustomer.ShippingAddressId = null;
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);

                _genericAttributeService.SaveAttribute<ShippingOption>(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedShippingOptionAttribute, null, _storeContext.CurrentStore.Id);

                //load next step
                return LoadStepAfterShippingMethod(cart, response);
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc, _workContext.CurrentCustomer);
                return InternalServerError(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.SaveBillingFailed"));
            }
        }

        [HttpPost("saveshipping")]
        public virtual IActionResult SaveShipping([FromBody] BaseQueryModel<CheckoutShippingAddressModel> queryModel)
        {
            try
            {
                if (_orderSettings.CheckoutDisabled)
                    return BadRequest(_localizationService.GetResource("Checkout.Disabled"));

                var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer,
                    ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id, onlySelectedItems: false);

                if (!cart.Any())
                    return BadRequest(_localizationService.GetResource("ShoppingCart.CartIsEmpty"));

                if (_customerService.IsGuest(_workContext.CurrentCustomer) && !_orderSettings.AnonymousCheckoutAllowed)
                    return Unauthorized(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.AnonymousCheckoutNotAllowed"));

                if (!_shoppingCartService.ShoppingCartRequiresShipping(cart))
                    return BadRequest(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.ShippingNotRequired"));

                var response = new GenericResponseModel<OpcStepResponseModel>();
                response.Data = new OpcStepResponseModel();
                var model = queryModel.Data;

                var form = queryModel.FormValues == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();

                //pickup point
                if (_shippingSettings.AllowPickupInStore)
                {
                    var pickupInStore = ParsePickupInStore(form);

                    if (pickupInStore)
                    {
                        //no shipping address selected
                        _workContext.CurrentCustomer.ShippingAddressId = null;
                        _customerService.UpdateCustomer(_workContext.CurrentCustomer);

                        var pickupPoint = form["pickup-points-id"].ToString().Split(new[] { "___" }, StringSplitOptions.None);
                        var pickupPoints = _shippingService.GetPickupPoints(_customerService.GetCustomerBillingAddress(_workContext.CurrentCustomer).Id,
                            _workContext.CurrentCustomer, pickupPoint[1], _storeContext.CurrentStore.Id).PickupPoints.ToList();
                        var selectedPoint = pickupPoints.FirstOrDefault(x => x.Id.Equals(pickupPoint[0]));
                        if (selectedPoint == null)
                            throw new Exception("Pickup point is not allowed");

                        var pickUpInStoreShippingOption = new ShippingOption
                        {
                            Name = string.Format(_localizationService.GetResource("Checkout.PickupPoints.Name"), selectedPoint.Name),
                            Rate = selectedPoint.PickupFee,
                            Description = selectedPoint.Description,
                            ShippingRateComputationMethodSystemName = selectedPoint.ProviderSystemName
                        };
                        _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedShippingOptionAttribute, pickUpInStoreShippingOption, _storeContext.CurrentStore.Id);
                        _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedPickupPointAttribute, selectedPoint, _storeContext.CurrentStore.Id);

                        //load next step
                        return LoadStepAfterShippingMethod(cart, response);
                    }

                    //set value indicating that "pick up in store" option has not been chosen
                    _genericAttributeService.SaveAttribute<PickupPoint>(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedPickupPointAttribute, null, _storeContext.CurrentStore.Id);
                }

                int.TryParse(form["shipping_address_id"], out var shippingAddressId);

                if (shippingAddressId > 0)
                {
                    //existing address
                    var address = _customerService.GetCustomerAddress(_workContext.CurrentCustomer.Id, shippingAddressId)
                        ?? throw new Exception("Address can't be loaded");

                    _workContext.CurrentCustomer.ShippingAddressId = address.Id;

                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                }
                else
                {
                    //new address
                    var newAddress = model.ShippingNewAddress;

                    //custom address attributes
                    var customAttributes = ParseCustomAddressAttributes(form);
                    var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
                    foreach (var error in customAttributeWarnings)
                    {
                        ModelState.AddModelError("", error);
                    }

                    //validate model
                    if (!ModelState.IsValid)
                    {
                        //model is not valid. redisplay the form with errors
                        var shippingAddressModel = _checkoutModelFactory.PrepareShippingAddressModel(cart,
                            selectedCountryId: newAddress.CountryId,
                            overrideAttributesXml: customAttributes);

                        shippingAddressModel.NewAddressPreselected = true;

                        response.Data.NextStep = OpcStep.ShippingAddress;
                        response.Data.ShippingAddressModel = shippingAddressModel;

                        foreach (var modelState in ModelState.Values)
                            foreach (var error in modelState.Errors)
                                response.ErrorList.Add(error.ErrorMessage);

                        return BadRequest(response);
                    }

                    //try to find an address with the same values (don't duplicate records)
                    var address = _addressService.FindAddress(_customerService.GetAddressesByCustomerId(_workContext.CurrentCustomer.Id).ToList(),
                        newAddress.FirstName, newAddress.LastName, newAddress.PhoneNumber,
                        newAddress.Email, newAddress.FaxNumber, newAddress.Company,
                        newAddress.Address1, newAddress.Address2, newAddress.City,
                        newAddress.County, newAddress.StateProvinceId, newAddress.ZipPostalCode,
                        newAddress.CountryId, customAttributes);
                    if (address == null)
                    {
                        address = newAddress.ToEntity();
                        address.CustomAttributes = customAttributes;
                        address.CreatedOnUtc = DateTime.UtcNow;
                        //little hack here (TODO: find a better solution)
                        //EF does not load navigation properties for newly created entities (such as this "Address").
                        //we have to load them manually 
                        //otherwise, "Country" property of "Address" entity will be null in shipping rate computation methods
                       
                       
                        //other null validations
                        if (address.CountryId == 0)
                            address.CountryId = null;
                        if (address.StateProvinceId == 0)
                            address.StateProvinceId = null;

                        _addressService.InsertAddress(address);

                        _customerService.InsertCustomerAddress(_workContext.CurrentCustomer, address);
                    }
                    _workContext.CurrentCustomer.ShippingAddressId = address.Id;
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);

                    newAddress.StateProvinceName = _stateProvinceService.GetStateProvinceById(newAddress.StateProvinceId.Value).Name;
                    newAddress.Id = address.Id;
                    response.Data.ShippingAddressModel.ShippingNewAddress = newAddress;
                }

                //return LoadStepAfterShippingMethod(cart, response);

                return Ok(response);
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc, _workContext.CurrentCustomer);
                return InternalServerError(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.SaveShippingFailed"));
            }
        }

        [HttpPost("saveshippingmethod")]
        public virtual IActionResult SaveShippingMethod([FromBody] BaseQueryModel<string> queryModel)
        {
            try
            {
                if (_orderSettings.CheckoutDisabled)
                    return BadRequest(_localizationService.GetResource("Checkout.Disabled"));

                var form = queryModel.FormValues.ToNameValueCollection();

                var shippingoption = form["shippingoption"];
                var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer,
                    ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id, onlySelectedItems: true);

                if (!cart.Any())
                    return BadRequest(_localizationService.GetResource("ShoppingCart.CartIsEmpty"));

                if (_customerService.IsGuest(_workContext.CurrentCustomer) && !_orderSettings.AnonymousCheckoutAllowed)
                    return Unauthorized(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.AnonymousCheckoutNotAllowed"));

                if (!_shoppingCartService.ShoppingCartRequiresShipping(cart))
                    return BadRequest(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.ShippingNotRequired"));

                var response = new GenericResponseModel<OpcStepResponseModel>();
                response.Data = new OpcStepResponseModel();

                //parse selected method 
                if (string.IsNullOrEmpty(shippingoption))
                    return BadRequest(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.ShippingCannotBeParsed"));
                var splittedOption = shippingoption.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
                if (splittedOption.Length != 2)
                    return BadRequest(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.ShippingCannotBeParsed"));
                var selectedName = splittedOption[0];
                var shippingRateComputationMethodSystemName = splittedOption[1];

                //find it
                //performance optimization. try cache first
                var shippingOptions = _genericAttributeService.GetAttribute<List<ShippingOption>>(_workContext.CurrentCustomer,
                    NopCustomerDefaults.OfferedShippingOptionsAttribute, _storeContext.CurrentStore.Id);
                if (shippingOptions == null || !shippingOptions.Any())
                {
                    //not found? let's load them using shipping service
                    shippingOptions = _shippingService.GetShippingOptions(cart, _customerService.GetCustomerShippingAddress(_workContext.CurrentCustomer),
                        _workContext.CurrentCustomer, shippingRateComputationMethodSystemName, _storeContext.CurrentStore.Id).ShippingOptions.ToList();
                }
                else
                {
                    //loaded cached results. let's filter result by a chosen shipping rate computation method
                    shippingOptions = shippingOptions.Where(so => so.ShippingRateComputationMethodSystemName.Equals(shippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                        .ToList();
                }

                var shippingOption = shippingOptions
                    .Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(selectedName, StringComparison.InvariantCultureIgnoreCase));
                if (shippingOption == null)
                    return BadRequest(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.ShippingCannotBeLoaded"));

                //save
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedShippingOptionAttribute, shippingOption, _storeContext.CurrentStore.Id);

                //load next step
                return LoadStepAfterShippingMethod(cart, response);
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc, _workContext.CurrentCustomer);
                return InternalServerError(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.SaveShippingMethodFailed"));
            }
        }

        [HttpPost("savepaymentmethod")]
        public virtual IActionResult SavePaymentMethod([FromBody] BaseQueryModel<CheckoutPaymentMethodModel> queryModel)
        {
            try
            {
                if (_orderSettings.CheckoutDisabled)
                    return BadRequest(_localizationService.GetResource("Checkout.Disabled"));

                var form = queryModel.FormValues == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();

                var paymentmethod = form["paymentmethod"];
                var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer,
                    ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id, onlySelectedItems: true);

                if (!cart.Any())
                    return BadRequest(_localizationService.GetResource("ShoppingCart.CartIsEmpty"));

                if (_customerService.IsGuest(_workContext.CurrentCustomer) && !_orderSettings.AnonymousCheckoutAllowed)
                    return Unauthorized(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.AnonymousCheckoutNotAllowed"));

                //payment method 
                if (string.IsNullOrEmpty(paymentmethod))
                    return BadRequest(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.PaymentCannotBeParsed"));

                var response = new GenericResponseModel<OpcStepResponseModel>();
                response.Data = new OpcStepResponseModel();
                var model = queryModel.Data;

                //reward points
                if (_rewardPointsSettings.Enabled)
                {
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        NopCustomerDefaults.UseRewardPointsDuringCheckoutAttribute, model.UseRewardPoints,
                        _storeContext.CurrentStore.Id);
                }

                //Check whether payment workflow is required
                var isPaymentWorkflowRequired = _orderProcessingService.IsPaymentWorkflowRequired(cart);
                if (!isPaymentWorkflowRequired)
                {
                    //payment is not required
                    _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                        NopCustomerDefaults.SelectedPaymentMethodAttribute, null, _storeContext.CurrentStore.Id);

                    response.Data.NextStep = OpcStep.ConfirmOrder;
                    response.Data.ConfirmModel = _cartCheckoutApiModelFactory.PrepareCheckoutConfirmOrderModel(cart);

                    return Ok(response);
                }

                var paymentMethodInst = _paymentPluginManager
                    .LoadPluginBySystemName(paymentmethod, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
                if (!_paymentPluginManager.IsPluginActive(paymentMethodInst))
                    return BadRequest(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.PaymentMethodNotFound"));

                //save
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, paymentmethod, _storeContext.CurrentStore.Id);

                return LoadStepAfterPaymentMethod(paymentMethodInst, cart, response);
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc, _workContext.CurrentCustomer);
                return InternalServerError(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.SavePaymentMethodFailed"));
            }
        }

        [HttpGet("confirmorder")]
        public virtual IActionResult GetConfirmOrder()
        {
            if (_orderSettings.CheckoutDisabled)
                return BadRequest(_localizationService.GetResource("Checkout.Disabled"));

            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart,
                _storeContext.CurrentStore.Id, onlySelectedItems: true);

            if (!cart.Any())
                return BadRequest(_localizationService.GetResource("ShoppingCart.CartIsEmpty"));

            if (_customerService.IsGuest(_workContext.CurrentCustomer) && !_orderSettings.AnonymousCheckoutAllowed)
                return Unauthorized(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.AnonymousCheckoutNotAllowed"));

            //prevent 2 orders being placed within an X seconds time frame
            if (!IsMinimumOrderPlacementIntervalValid(_workContext.CurrentCustomer))
                return BadRequest(_localizationService.GetResource("Checkout.MinOrderPlacementInterval"));

            var response = new GenericResponseModel<CheckoutConfirmOrderModel>();
            response.Data = _cartCheckoutApiModelFactory.PrepareCheckoutConfirmOrderModel(cart);

            return Ok(response);
        }

        [HttpPost("confirmorder")]
        public virtual IActionResult ConfirmOrder([FromBody] List<CustomCheckoutAttributeModel> customCheckoutAttr)
        {
            try
            {
                if (_orderSettings.CheckoutDisabled)
                    return BadRequest(_localizationService.GetResource("Checkout.Disabled"));

                var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer,
                    ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id, onlySelectedItems: true);

                if (!cart.Any())
                    return BadRequest(_localizationService.GetResource("ShoppingCart.CartIsEmpty"));

                if (_customerService.IsGuest(_workContext.CurrentCustomer) && !_orderSettings.AnonymousCheckoutAllowed)
                    return Unauthorized(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.AnonymousCheckoutNotAllowed"));

                //prevent 2 orders being placed within an X seconds time frame
                if (!IsMinimumOrderPlacementIntervalValid(_workContext.CurrentCustomer))
                    return BadRequest(_localizationService.GetResource("Checkout.MinOrderPlacementInterval"));

                var response = new GenericResponseModel<OpcStepResponseModel>();
                response.Data = new OpcStepResponseModel();

                //place order
                var processPaymentRequest = _genericAttributeService.GetPaymentRequestAttribute(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id);

                if (processPaymentRequest == null)
                {
                    //Check whether payment workflow is required
                    if (_orderProcessingService.IsPaymentWorkflowRequired(cart))
                    {
                        throw new Exception("Payment information is not entered");
                    }

                    processPaymentRequest = new ProcessPaymentRequest();
                }
                GenerateOrderGuid(processPaymentRequest);
                processPaymentRequest.StoreId = _storeContext.CurrentStore.Id;
                processPaymentRequest.CustomerId = _workContext.CurrentCustomer.Id;
                processPaymentRequest.PaymentMethodSystemName = "Payments.IPay88";

                _genericAttributeService.SavePaymentRequestAttribute(_workContext.CurrentCustomer, processPaymentRequest, _storeContext.CurrentStore.Id);

                var products = _productService.GetProductsByIds(cart.Select(c => c.ProductId).ToArray());
                var vendorIds = products.Select(p => p.VendorId).Distinct();

                var orders = new List<Order>();
                var attributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id);
                var placeOrderResults = new List<PlaceOrderResult>();
                foreach (var vendorId in vendorIds)
                {
                    //attribute
                    var attributesXml = string.Empty;
                    if (customCheckoutAttr != null)
                    {
                        var vendorAttrs = customCheckoutAttr.Where(x => x.VendorId == vendorId);
                        if (vendorAttrs.Count() > 0)
                        {
                            foreach (var vendorAttr in vendorAttrs)
                            {
                                var attribute = attributes.Where(x => x.Id == vendorAttr.AttributeId).FirstOrDefault();
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml, attribute, vendorAttr.AttributeValue);
                            }
                        }
                    }


                    var cartItemIds = from c in cart
                        join p in products on c.ProductId equals p.Id
                        where p.VendorId == vendorId
                        select c.Id;
                    
                    if (cartItemIds.Any())
                    {
                        var placeOrderResult = _orderProcessingService.PlaceOrderWithVendor(vendorId,
                            processPaymentRequest, cartItemIds, attributesXml);

                        if (!string.IsNullOrEmpty(placeOrderResult.PlacedOrder.CheckoutAttributesXml))
                        {
                            var deliveryDateText = _shuqOrderService.GetCheckoutDeliveryDate(placeOrderResult.PlacedOrder.CheckoutAttributesXml);
                            var deliveryTimeText = _shuqOrderService.GetCheckoutDeliveryTimeslot(placeOrderResult.PlacedOrder.CheckoutAttributesXml);
                            placeOrderResult.PlacedOrder.OrderShippingDeliverySlot = _shipmentService.GetDeliverySlot(deliveryDateText, deliveryTimeText);
                        }

                        placeOrderResults.Add(placeOrderResult);
                        orders.Add(placeOrderResult.PlacedOrder);
                    }
                }

                if (placeOrderResults.All(s => s.Success))
                {
                    _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                        NopStationCustomerDefaults.OrderPaymentInfo, null, _storeContext.CurrentStore.Id);
                    //create master order according to order
                    var masterOrder = _orderService.CreateMasterOrderByOrders(orders);

                    //update order's master order id
                    foreach (var order in orders)
                    {
                        order.MasterOrderId = masterOrder.Id;
                        _orderService.UpdateOrder(order);
                    }

                    var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(processPaymentRequest.PaymentMethodSystemName);
                    if (paymentMethod == null)
                    {
                        response.Data.NextStep = OpcStep.Completed;
                        return Ok(response);
                    }

                    if (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection)
                    {
                        var postProcessPaymentRequest = new PostProcessPaymentRequest
                        {
                            MasterOrder = masterOrder
                        };
                    
                        _paymentService.PostProcessPayment(postProcessPaymentRequest);
                    
                        var paymentResponseURL = "/PaymentIPay88/RedirectToPaymentUrl";
                        var redirectURL = HttpUtility.UrlEncode("");
                        var paymentURL = $"{paymentResponseURL}?r={redirectURL}&o={masterOrder.Id}&ot={((int)OrderType.Shuq).ToString()}";
                        
                        response.Data.PaymentMethodModel.PaymentMethods.Add(new CheckoutPaymentMethodModel.PaymentMethodModel
                        {
                            PaymentMethodSystemName = processPaymentRequest.PaymentMethodSystemName,
                            Selected = true,
                            PaymentRedirectUrl = paymentURL
                        });
                        response.Data.NextStep = OpcStep.RedirectToGateway;
                        return Ok(response);
                    }
                    response.Data.NextStep = OpcStep.Completed;
                    return Ok(response);
                }

                //error
                foreach (var error in placeOrderResults.SelectMany(s => s.Errors))
                    response.ErrorList.Add(error);

                response.Data.NextStep = OpcStep.ConfirmOrder;

                return Ok(response);
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc, _workContext.CurrentCustomer);
                return InternalServerError(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.ConfirmOrderFailed"));
            }
        }

        [HttpGet("completed/{orderId?}")]
        public virtual IActionResult Completed(int? orderId)
        {
            //validation
            if (_customerService.IsGuest(_workContext.CurrentCustomer) && !_orderSettings.AnonymousCheckoutAllowed)
                return Unauthorized(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.AnonymousCheckoutNotAllowed"));

            Order order = null;
            if (orderId.HasValue)
            {
                //load order by identifier (if provided)
                order = _orderService.GetOrderById(orderId.Value);
            }
            if (order == null)
            {
                order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                    .FirstOrDefault();
            }
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
            {
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.OrderNotFound"));
            }

            var response = new GenericResponseModel<OpcStepResponseModel>();
            response.Data = new OpcStepResponseModel();

            //model
            var model = _checkoutModelFactory.PrepareCheckoutCompletedModel(order);
            response.Data.CompletedModel = model;

            return Ok(response);
        }

        #endregion
    }
}
