using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.ExportImport;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Plugin.NopStation.WebApi.Models.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Web.Models.Order;

namespace Nop.Plugin.NopStation.WebApi.Controllers
{
    [Route("api/customer")]
    public partial class CustomerApiController : BaseApiController
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly IDownloadService _downloadService;
        private readonly ForumSettings _forumSettings;
        private readonly GdprSettings _gdprSettings;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAddressService _addressService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IExportManager _exportManager;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IGdprService _gdprService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGiftCardService _giftCardService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly TaxSettings _taxSettings;
        private readonly IApiDeviceService _deviceService;
        private readonly OrderSettings _orderSettings;
        private readonly IProductService _productService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly INopFileProvider _fileProvider;
        private readonly IOrderModelFactory _orderModelFactory;

        #endregion

        #region Ctor

        public CustomerApiController(AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            IDownloadService downloadService,
            ForumSettings forumSettings,
            GdprSettings gdprSettings,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            IAddressModelFactory addressModelFactory,
            IAddressService addressService,
            IAuthenticationService authenticationService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            ICustomerModelFactory customerModelFactory,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IExportManager exportManager,
            IExternalAuthenticationService externalAuthenticationService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            ILocalizationService localizationService,
            ILogger logger,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IOrderService orderService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            ITaxService taxService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            MediaSettings mediaSettings,
            StoreInformationSettings storeInformationSettings,
            TaxSettings taxSettings,
            IApiDeviceService deviceService,
            OrderSettings orderSettings,
            IProductService productService,
            IReturnRequestService returnRequestService,
            INopFileProvider fileProvider,
            IOrderModelFactory orderModelFactory)
        {
            _addressSettings = addressSettings;
            _captchaSettings = captchaSettings;
            _customerSettings = customerSettings;
            _dateTimeSettings = dateTimeSettings;
            _downloadService = downloadService;
            _forumSettings = forumSettings;
            _gdprSettings = gdprSettings;
            _addressAttributeParser = addressAttributeParser;
            _addressAttributeService = addressAttributeService;
            _addressModelFactory = addressModelFactory;
            _addressService = addressService;
            _authenticationService = authenticationService;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerActivityService = customerActivityService;
            _customerAttributeParser = customerAttributeParser;
            _customerAttributeService = customerAttributeService;
            _customerModelFactory = customerModelFactory;
            _customerRegistrationService = customerRegistrationService;
            _customerService = customerService;
            _eventPublisher = eventPublisher;
            _exportManager = exportManager;
            _externalAuthenticationService = externalAuthenticationService;
            _gdprService = gdprService;
            _genericAttributeService = genericAttributeService;
            _giftCardService = giftCardService;
            _localizationService = localizationService;
            _logger = logger;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _orderService = orderService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _taxService = taxService;
            _webHelper = webHelper;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _mediaSettings = mediaSettings;
            _storeInformationSettings = storeInformationSettings;
            _taxSettings = taxSettings;
            _deviceService = deviceService;
            _orderSettings = orderSettings;
            _productService = productService;
            _returnRequestService = returnRequestService;
            _fileProvider = fileProvider;
            _orderModelFactory = orderModelFactory;
        }

        #endregion

        #region Utilities

        protected string GetToken(Customer customer)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var now = Math.Round((DateTime.UtcNow.AddDays(180) - unixEpoch).TotalSeconds);

            var payload = new Dictionary<string, object>()
                {
                    { WebApiCustomerDefaults.CustomerId, customer.Id },
                    { "exp", now }
                };

            return JwtHelper.JwtEncoder.Encode(payload, WebApiCustomerDefaults.SecretKey);
        }

        protected string GetDeviceIdFromHeader()
        {
            StringValues headerValues;
            var secretKey = WebApiCustomerDefaults.SecretKey;
            var keyFound = Request.Headers.TryGetValue(WebApiCustomerDefaults.DeviceId, out headerValues);
            if (headerValues.Count > 0)
            {
                var device = headerValues.FirstOrDefault();
                if (device != null) return device;
            }
            return string.Empty;
        }

        protected virtual string ParseCustomCustomerAttributes(NameValueCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = "";
            var attributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in attributes)
            {
                var controlId = $"{NopCustomerServicesDefaults.CustomerAttributePrefix}{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
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
                                        attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _customerAttributeService.GetCustomerAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
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
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.FileUpload:
                    //not supported customer attributes
                    default:
                        break;
                }
            }

            return attributesXml;
        }

        protected virtual void LogGdpr(Customer customer, CustomerInfoModel oldCustomerInfoModel,
            CustomerInfoModel newCustomerInfoModel, NameValueCollection form)
        {
            try
            {
                //consents
                var consents = _gdprService.GetAllConsents().Where(consent => consent.DisplayOnCustomerInfoPage).ToList();
                foreach (var consent in consents)
                {
                    var previousConsentValue = _gdprService.IsConsentAccepted(consent.Id, _workContext.CurrentCustomer.Id);
                    var controlId = $"consent{consent.Id}";
                    var cbConsent = form[controlId];
                    if (!StringValues.IsNullOrEmpty(cbConsent) && cbConsent.ToString().Equals("on"))
                    {
                        //agree
                        if (!previousConsentValue.HasValue || !previousConsentValue.Value)
                        {
                            _gdprService.InsertLog(customer, consent.Id, GdprRequestType.ConsentAgree, consent.Message);
                        }
                    }
                    else
                    {
                        //disagree
                        if (!previousConsentValue.HasValue || previousConsentValue.Value)
                        {
                            _gdprService.InsertLog(customer, consent.Id, GdprRequestType.ConsentDisagree, consent.Message);
                        }
                    }
                }

                //newsletter subscriptions
                if (_gdprSettings.LogNewsletterConsent)
                {
                    if (oldCustomerInfoModel.Newsletter && !newCustomerInfoModel.Newsletter)
                        _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentDisagree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                    if (!oldCustomerInfoModel.Newsletter && newCustomerInfoModel.Newsletter)
                        _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                }

                //user profile changes
                if (!_gdprSettings.LogUserProfileChanges)
                    return;

                if (oldCustomerInfoModel.Gender != newCustomerInfoModel.Gender)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.Gender")} = {newCustomerInfoModel.Gender}");

                if (oldCustomerInfoModel.FirstName != newCustomerInfoModel.FirstName)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.FirstName")} = {newCustomerInfoModel.FirstName}");

                if (oldCustomerInfoModel.LastName != newCustomerInfoModel.LastName)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.LastName")} = {newCustomerInfoModel.LastName}");

                if (oldCustomerInfoModel.ParseDateOfBirth() != newCustomerInfoModel.ParseDateOfBirth())
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.DateOfBirth")} = {newCustomerInfoModel.ParseDateOfBirth().ToString()}");

                if (oldCustomerInfoModel.Email != newCustomerInfoModel.Email)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.Email")} = {newCustomerInfoModel.Email}");

                if (oldCustomerInfoModel.Company != newCustomerInfoModel.Company)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.Company")} = {newCustomerInfoModel.Company}");

                if (oldCustomerInfoModel.StreetAddress != newCustomerInfoModel.StreetAddress)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.StreetAddress")} = {newCustomerInfoModel.StreetAddress}");

                if (oldCustomerInfoModel.StreetAddress2 != newCustomerInfoModel.StreetAddress2)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.StreetAddress2")} = {newCustomerInfoModel.StreetAddress2}");

                if (oldCustomerInfoModel.ZipPostalCode != newCustomerInfoModel.ZipPostalCode)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.ZipPostalCode")} = {newCustomerInfoModel.ZipPostalCode}");

                if (oldCustomerInfoModel.City != newCustomerInfoModel.City)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.City")} = {newCustomerInfoModel.City}");

                if (oldCustomerInfoModel.County != newCustomerInfoModel.County)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.County")} = {newCustomerInfoModel.County}");

                if (oldCustomerInfoModel.CountryId != newCustomerInfoModel.CountryId)
                {
                    var countryName = _countryService.GetCountryById(newCustomerInfoModel.CountryId)?.Name;
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.Country")} = {countryName}");
                }

                if (oldCustomerInfoModel.StateProvinceId != newCustomerInfoModel.StateProvinceId)
                {
                    var stateProvinceName = _stateProvinceService.GetStateProvinceById(newCustomerInfoModel.StateProvinceId)?.Name;
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.StateProvince")} = {stateProvinceName}");
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception.Message, exception, customer);
            }
        }

        #endregion

        #region Methods

        #region Login / logout

        [HttpGet("login")]
        public virtual IActionResult Login(bool? checkoutAsGuest)
        {
            var response = new GenericResponseModel<LoginModel>();
            response.Data = _customerModelFactory.PrepareLoginModel(checkoutAsGuest);
            return Ok(response);
        }
        
        [HttpPost("login")]
        public virtual IActionResult Login([FromBody]BaseQueryModel<LoginModel> queryModel)
        {
            var model = queryModel.Data;
            var response = new GenericResponseModel<LogInResponseModel>();
            var responseData = new LogInResponseModel();

            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }
                var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled
                                ? _customerService.GetCustomerByUsername(model.Username)
                                : _customerService.GetCustomerByEmail(model.Email);

                            responseData.CustomerInfo = _customerModelFactory.PrepareCustomerInfoModel(responseData.CustomerInfo, customer, false);
                            responseData.Token = GetToken(customer);

                            //migrate shopping cart
                            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

                            //sign in new customer
                            _authenticationService.SignIn(customer, true);

                            //raise event       
                            _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

                            //activity log
                            _customerActivityService.InsertActivity(customer, "PublicStore.Login",
                                _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);

                            string deviceId = GetDeviceIdFromHeader();
                            var device = _deviceService.GetApiDeviceByDeviceId(deviceId, _storeContext.CurrentStore.Id);
                            if (device != null)
                            {
                                device.CustomerId = customer.Id;
                                device.IsRegistered = _customerService.IsRegistered(customer);
                                _deviceService.UpdateApiDevice(device);
                            }
                            
                            response.Data = responseData;
                            return Ok(response);
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
                        break;
                    case CustomerLoginResults.Deleted:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case CustomerLoginResults.NotActive:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case CustomerLoginResults.NotRegistered:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case CustomerLoginResults.LockedOut:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.LockedOut"));
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
                        break;
                }
            }

            foreach (var modelState in ModelState.Values)
                foreach (var error in modelState.Errors)
                    response.ErrorList.Add(error.ErrorMessage);
            
            return BadRequest(response);
        }

        [HttpGet("logout")]
        public virtual IActionResult Logout()
        {
            //activity log
            _customerActivityService.InsertActivity(_workContext.CurrentCustomer, "PublicStore.Logout",
                _localizationService.GetResource("ActivityLog.PublicStore.Logout"), _workContext.CurrentCustomer);

            //standard logout 
            _authenticationService.SignOut();

            //raise logged out event       
            _eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

            return Ok();
        }

        #endregion

        #region Password recovery

        [HttpGet("passwordrecovery")]
        public virtual IActionResult PasswordRecovery()
        {
            var response = new GenericResponseModel<PasswordRecoveryModel>();
            var model = new PasswordRecoveryModel();
            response.Data = _customerModelFactory.PreparePasswordRecoveryModel(model);

            return Ok(response);
        }

        [HttpPost("passwordrecovery")]
        public virtual IActionResult PasswordRecoverySend([FromBody]BaseQueryModel<PasswordRecoveryModel> queryModel)
        {
            var response = new GenericResponseModel<PasswordRecoveryModel>();
            if (ModelState.IsValid)
            {
                var model = queryModel.Data;
                var customer = _customerService.GetCustomerByEmail(model.Email);
                if (customer != null && customer.Active && !customer.Deleted)
                {
                    //save token and current date
                    var passwordRecoveryToken = Guid.NewGuid();
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute,
                        passwordRecoveryToken.ToString());
                    DateTime? generatedDateTime = DateTime.UtcNow;
                    _genericAttributeService.SaveAttribute(customer,
                        NopCustomerDefaults.PasswordRecoveryTokenDateGeneratedAttribute, generatedDateTime);

                    //send email
                    _workflowMessageService.SendCustomerPasswordRecoveryMessage(customer,
                        _workContext.WorkingLanguage.Id);

                    response.Data = model;
                    response.Message = _localizationService.GetResource("Account.PasswordRecovery.EmailHasBeenSent");
                    return Ok(response);
                }
                else
                {
                    response.Data = model;
                    response.ErrorList.Add(_localizationService.GetResource("Account.PasswordRecovery.EmailNotFound"));
                    return BadRequest(response);
                }
            }

            foreach (var modelState in ModelState.Values)
                foreach (var error in modelState.Errors)
                    response.ErrorList.Add(error.ErrorMessage);

            //If we got this far, something failed, redisplay form
            return BadRequest(response);
        }

        [HttpGet("passwordrecoveryconfirm/{token}/{email}")]
        public virtual IActionResult PasswordRecoveryConfirm(string token, string email)
        {
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                return NotFound(_localizationService.GetResource("NopStation.WebApi.Response.Customer.CustomerNotFound"));

            var response = new GenericResponseModel<PasswordRecoveryConfirmModel>();
            if (string.IsNullOrEmpty(_genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute)))
            {
                response.ErrorList.Add(_localizationService.GetResource("Account.PasswordRecovery.PasswordAlreadyHasBeenChanged"));
                return BadRequest(response);
            }

            var model = _customerModelFactory.PreparePasswordRecoveryConfirmModel();

            //validate token
            if (!_customerService.IsPasswordRecoveryTokenValid(customer, token))
            {
                model.DisablePasswordChanging = true;
                response.Data = model;
                response.ErrorList.Add(_localizationService.GetResource("Account.PasswordRecovery.WrongToken"));
                return BadRequest(response);
            }

            //validate token expiration date
            if (_customerService.IsPasswordRecoveryLinkExpired(customer))
            {
                model.DisablePasswordChanging = true;
                response.Data = model;
                response.ErrorList.Add(_localizationService.GetResource("Account.PasswordRecovery.LinkExpired"));
                return BadRequest(response);
            }

            response.Data = model;
            return Ok(response);
        }
       
        #endregion

        #region Register

        [HttpGet("register")]
        public virtual IActionResult Register()
        {
            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return BadRequest();

            var response = new GenericResponseModel<RegisterModel>();
            var model = new RegisterModel();
            response.Data = _customerModelFactory.PrepareRegisterModel(model, false, setDefaultValues: true);
            return Ok(response);
        }

     
        [HttpPost("register")]
        public virtual IActionResult Register([FromBody]BaseQueryModel<RegisterModel> model)
        {
            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return BadRequest();

            var response = new GenericResponseModel<RegisterModel>();

            if (_customerService.IsRegistered(_workContext.CurrentCustomer))
            {
                //Already registered customer. 
                _authenticationService.SignOut();

                //raise logged out event       
                _eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

                //Save a new record
                _workContext.CurrentCustomer = _customerService.InsertGuestCustomer();
            }
            var customer = _workContext.CurrentCustomer;
            customer.RegisteredInStoreId = _storeContext.CurrentStore.Id;

            var form = model.FormValues == null ? new NameValueCollection() : model.FormValues.ToNameValueCollection();
            //custom customer attributes
            var customerAttributesXml = ParseCustomCustomerAttributes(form);
            var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Data.Username != null)
                {
                    model.Data.Username = model.Data.Username.Trim();
                }

                var isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
                var registrationRequest = new CustomerRegistrationRequest(customer,
                    model.Data.Email,
                    _customerSettings.UsernamesEnabled ? model.Data.Username : model.Data.Email,
                    model.Data.Password,
                    _customerSettings.DefaultPasswordFormat,
                    _storeContext.CurrentStore.Id,
                    isApproved);
                var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
                if (registrationResult.Success)
                {
                    //properties
                    if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.TimeZoneIdAttribute, model.Data.TimeZoneId);
                    }
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberAttribute, model.Data.VatNumber);

                        var vatNumberStatus = _taxService.GetVatNumberStatus(model.Data.VatNumber, out string _, out string vatAddress);
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberStatusIdAttribute, (int)vatNumberStatus);
                        //send VAT number admin notification
                        if (!string.IsNullOrEmpty(model.Data.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                            _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, model.Data.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                    }

                    //form fields
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.GenderAttribute, model.Data.Gender);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FirstNameAttribute, model.Data.FirstName);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.LastNameAttribute, model.Data.LastName);
                    if (_customerSettings.DateOfBirthEnabled)
                    {
                        var dateOfBirth = model.Data.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.DateOfBirthAttribute, dateOfBirth);
                    }
                    if (_customerSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CompanyAttribute, model.Data.Company);
                    if (_customerSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddressAttribute, model.Data.StreetAddress);
                    if (_customerSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddress2Attribute, model.Data.StreetAddress2);
                    if (_customerSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.ZipPostalCodeAttribute, model.Data.ZipPostalCode);
                    if (_customerSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CityAttribute, model.Data.City);
                    if (_customerSettings.CountyEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountyAttribute, model.Data.County);
                    if (_customerSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountryIdAttribute, model.Data.CountryId);
                    if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StateProvinceIdAttribute,
                            model.Data.StateProvinceId);
                    if (_customerSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PhoneAttribute, model.Data.Phone);
                    if (_customerSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FaxAttribute, model.Data.Fax);

                    //newsletter
                    if (_customerSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(model.Data.Email, _storeContext.CurrentStore.Id);
                        if (newsletter != null)
                        {
                            if (model.Data.Newsletter)
                            {
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);

                                //GDPR
                                if (_gdprSettings.GdprEnabled && _gdprSettings.LogNewsletterConsent)
                                {
                                    _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                                }
                            }
                        }
                        else
                        {
                            if (model.Data.Newsletter)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = model.Data.Email,
                                    Active = true,
                                    StoreId = _storeContext.CurrentStore.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });

                                //GDPR
                                if (_gdprSettings.GdprEnabled && _gdprSettings.LogNewsletterConsent)
                                {
                                    _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                                }
                            }
                        }
                    }

                    if (_customerSettings.AcceptPrivacyPolicyEnabled)
                    {
                        //privacy policy is required
                        //GDPR
                        if (_gdprSettings.GdprEnabled && _gdprSettings.LogPrivacyPolicyConsent)
                        {
                            _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.PrivacyPolicy"));
                        }
                    }

                    //GDPR
                    if (_gdprSettings.GdprEnabled)
                    {
                        var consents = _gdprService.GetAllConsents().Where(consent => consent.DisplayDuringRegistration).ToList();
                        foreach (var consent in consents)
                        {
                            var controlId = $"consent{consent.Id}";
                            var cbConsent = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cbConsent) && cbConsent.ToString().Equals("on"))
                            {
                                //agree
                                _gdprService.InsertLog(customer, consent.Id, GdprRequestType.ConsentAgree, consent.Message);
                            }
                            else
                            {
                                //disagree
                                _gdprService.InsertLog(customer, consent.Id, GdprRequestType.ConsentDisagree, consent.Message);
                            }
                        }
                    }

                    //save customer attributes
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CustomCustomerAttributes, customerAttributesXml);

                    //login customer now
                    if (isApproved)
                        _authenticationService.SignIn(customer, true);

                    //insert default address (if possible)
                    var defaultAddress = new Address
                    {
                        FirstName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute),
                        LastName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute),
                        Email = customer.Email,
                        Company = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CompanyAttribute),
                        CountryId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CountryIdAttribute) > 0
                            ? (int?)_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CountryIdAttribute)
                            : null,
                        StateProvinceId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute) > 0
                            ? (int?)_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute)
                            : null,
                        County = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CountyAttribute),
                        City = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CityAttribute),
                        Address1 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddressAttribute),
                        Address2 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddress2Attribute),
                        ZipPostalCode = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute),
                        PhoneNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute),
                        FaxNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FaxAttribute),
                        CreatedOnUtc = customer.CreatedOnUtc
                    };
                    if (_addressService.IsAddressValid(defaultAddress))
                    {
                        //some validation
                        if (defaultAddress.CountryId == 0)
                            defaultAddress.CountryId = null;
                        if (defaultAddress.StateProvinceId == 0)
                            defaultAddress.StateProvinceId = null;

                        //set default address
                        _addressService.InsertAddress(defaultAddress);
                        _customerService.InsertCustomerAddress(customer, defaultAddress);

                        customer.BillingAddressId = defaultAddress.Id;
                        customer.ShippingAddressId = defaultAddress.Id;

                        _customerService.UpdateCustomer(customer);
                    }

                    //notifications
                    if (_customerSettings.NotifyNewCustomerRegistration)
                        _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer,
                            _localizationSettings.DefaultAdminLanguageId);

                    //raise event       
                    _eventPublisher.Publish(new CustomerRegisteredEvent(customer));

                    switch (_customerSettings.UserRegistrationType)
                    {
                        case UserRegistrationType.EmailValidation:
                            {
                                //email validation message
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                                _workflowMessageService.SendCustomerEmailValidationMessage(customer, _workContext.WorkingLanguage.Id);

                                response.Message = _localizationService.GetResource("Account.Register.Result.EmailValidation");
                                return Ok(response);
                            }
                        case UserRegistrationType.AdminApproval:
                            {
                                response.Message = _localizationService.GetResource("Account.Register.Result.AdminApproval");
                                return Ok(response);
                            }
                        case UserRegistrationType.Standard:
                            {
                                //send customer welcome message
                                _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);
                                response.Message = _localizationService.GetResource("Account.Register.Result.Standard");
                                return Ok(response);
                            }
                        default:
                            {
                                return BadRequest();
                            }
                    }
                }
                //errors
                response.ErrorList.AddRange(registrationResult.Errors);
            }

            foreach (var modelState in ModelState.Values)
                foreach (var error in modelState.Errors)
                    response.ErrorList.Add(error.ErrorMessage);

            //If we got this far, something failed, redisplay form
            response.Data = _customerModelFactory.PrepareRegisterModel(model.Data, true, customerAttributesXml);
            return BadRequest(response);
        }
        
        [HttpPost("checkusernameavailability")]
        public virtual IActionResult CheckUsernameAvailability(string username)
        {
            var usernameAvailable = false;
            var statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.NotAvailable");

            if (!UsernamePropertyValidator.IsValid(username, _customerSettings))
            {
                statusText = _localizationService.GetResource("Account.Fields.Username.NotValid");
            }
            else if (_customerSettings.UsernamesEnabled && !string.IsNullOrWhiteSpace(username))
            {
                if (_workContext.CurrentCustomer != null &&
                    _workContext.CurrentCustomer.Username != null &&
                    _workContext.CurrentCustomer.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                {
                    statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.CurrentUsername");
                }
                else
                {
                    var customer = _customerService.GetCustomerByUsername(username);
                    if (customer == null)
                    {
                        statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.Available");
                        usernameAvailable = true;
                    }
                }
            }

            var response = new GenericResponseModel<bool>
            {
                Data = usernameAvailable,
                Message = statusText
            };
            return Ok(response);
        }

        [HttpGet("activation/{token}/{email}")]
        public virtual IActionResult AccountActivation(string token, string email)
        {
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                return NotFound(_localizationService.GetResource("NopStation.WebApi.Response.Customer.CustomerNotFound"));

            var response = new GenericResponseModel<AccountActivationModel>();
            var cToken = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.AccountActivationTokenAttribute);
            if (string.IsNullOrEmpty(cToken))
            {
                response.ErrorList.Add(_localizationService.GetResource("Account.AccountActivation.AlreadyActivated"));
                return BadRequest(response);
            }

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return BadRequest();

            //activate user account
            customer.Active = true;
            _customerService.UpdateCustomer(customer);
            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AccountActivationTokenAttribute, "");
            //send welcome message
            _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

            response.Message = _localizationService.GetResource("Account.AccountActivation.Activated");
            return Ok(response);
        }

        #endregion

        #region My account / Info

        [HttpGet("info")]
        public virtual IActionResult Info()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            var response = new GenericResponseModel<CustomerInfoModel>();
            var model = new CustomerInfoModel();
            response.Data = _customerModelFactory.PrepareCustomerInfoModel(model, _workContext.CurrentCustomer, false);
            return Ok(response);
        }

        
        [HttpPost("info")]
        public virtual IActionResult Info([FromBody]BaseQueryModel<CustomerInfoModel> queryModel)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            var model = queryModel.Data;
            var response = new GenericResponseModel<CustomerInfoModel>();
            var oldCustomerModel = new CustomerInfoModel();

            var customer = _workContext.CurrentCustomer;

            //get customer info model before changes for gdpr log
            if (_gdprSettings.GdprEnabled & _gdprSettings.LogUserProfileChanges)
                oldCustomerModel = _customerModelFactory.PrepareCustomerInfoModel(oldCustomerModel, customer, false);
            
            var form = queryModel.FormValues == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();
            //custom customer attributes
            var customerAttributesXml = ParseCustomCustomerAttributes(form);
            var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //username 
                    if (_customerSettings.UsernamesEnabled && _customerSettings.AllowUsersToChangeUsernames)
                    {
                        if (
                            !customer.Username.Equals(model.Username.Trim(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            //change username
                            _customerRegistrationService.SetUsername(customer, model.Username.Trim());

                            //re-authenticate
                            //do not authenticate users in impersonation mode
                            if (_workContext.OriginalCustomerIfImpersonated == null)
                                _authenticationService.SignIn(customer, true);
                        }
                    }
                    //email
                    if (!customer.Email.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        //change email
                        var requireValidation = _customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation;
                        _customerRegistrationService.SetEmail(customer, model.Email.Trim(), requireValidation);

                        //do not authenticate users in impersonation mode
                        if (_workContext.OriginalCustomerIfImpersonated == null)
                        {
                            //re-authenticate (if usernames are disabled)
                            if (!_customerSettings.UsernamesEnabled && !requireValidation)
                                _authenticationService.SignIn(customer, true);
                        }
                    }

                    //properties
                    if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.TimeZoneIdAttribute,
                            model.TimeZoneId);
                    }
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        var prevVatNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.VatNumberAttribute);

                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberAttribute,
                            model.VatNumber);
                        if (prevVatNumber != model.VatNumber)
                        {
                            var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out string _, out string vatAddress);
                            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberStatusIdAttribute, (int)vatNumberStatus);
                            //send VAT number admin notification
                            if (!string.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                                _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer,
                                    model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                        }
                    }

                    //form fields
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.GenderAttribute, model.Gender);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FirstNameAttribute, model.FirstName);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.LastNameAttribute, model.LastName);
                    if (_customerSettings.DateOfBirthEnabled)
                    {
                        var dateOfBirth = model.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.DateOfBirthAttribute, dateOfBirth);
                    }
                    if (_customerSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CompanyAttribute, model.Company);
                    if (_customerSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddressAttribute, model.StreetAddress);
                    if (_customerSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddress2Attribute, model.StreetAddress2);
                    if (_customerSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.ZipPostalCodeAttribute, model.ZipPostalCode);
                    if (_customerSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CityAttribute, model.City);
                    if (_customerSettings.CountyEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountyAttribute, model.County);
                    if (_customerSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountryIdAttribute, model.CountryId);
                    if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StateProvinceIdAttribute, model.StateProvinceId);
                    if (_customerSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PhoneAttribute, model.Phone);
                    if (_customerSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FaxAttribute, model.Fax);

                    //newsletter
                    if (_customerSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, _storeContext.CurrentStore.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                var wasActive = newsletter.Active;
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
                            }
                            else
                            {
                                _newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                            }
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = customer.Email,
                                    Active = true,
                                    StoreId = _storeContext.CurrentStore.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                        }
                    }

                    if (_forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.SignatureAttribute, model.Signature);

                    //save customer attributes
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        NopCustomerDefaults.CustomCustomerAttributes, customerAttributesXml);

                    //GDPR
                    if (_gdprSettings.GdprEnabled)
                        LogGdpr(customer, oldCustomerModel, model, form);

                    response.Data = _customerModelFactory.PrepareCustomerInfoModel(model, _workContext.CurrentCustomer, false);
                    return Ok(response);
                }
                catch (Exception exc)
                {
                    return InternalServerError(exc.Message);
                }
            }

            foreach (var modelState in ModelState.Values)
                foreach (var error in modelState.Errors)
                    response.ErrorList.Add(error.ErrorMessage);

            //If we got this far, something failed, redisplay form
            response.Data = _customerModelFactory.PrepareCustomerInfoModel(model, customer, true, customerAttributesXml);
            return BadRequest(response);
        }

        
        [HttpPost("removeexternalassociation/{id}")]
        public virtual IActionResult RemoveExternalAssociation(int id)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            //ensure it's our record
            var ear = _externalAuthenticationService.GetExternalAuthenticationRecordById(id);
            if (ear != null)
                _externalAuthenticationService.DeleteExternalAuthenticationRecord(ear);

            return Ok();
        }

        [HttpGet("revalidateemail/{token}/{email}")]
        public virtual IActionResult EmailRevalidation(string token, string email)
        {
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                return NotFound(_localizationService.GetResource("NopStation.WebApi.Response.Customer.CustomerNotFound"));

            var response = new GenericResponseModel<EmailRevalidationModel>();
            var cToken = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute);
            if (string.IsNullOrEmpty(cToken))
            {
                response.ErrorList.Add(_localizationService.GetResource("Account.EmailRevalidation.AlreadyChanged"));
                return BadRequest(response);
            }

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return BadRequest();

            if (string.IsNullOrEmpty(customer.EmailToRevalidate))
                return BadRequest();

            if (_customerSettings.UserRegistrationType != UserRegistrationType.EmailValidation)
                return BadRequest();

            //change email
            try
            {
                _customerRegistrationService.SetEmail(customer, customer.EmailToRevalidate, false);
            }
            catch (Exception exc)
            {
                return InternalServerError(exc.Message);
            }

            customer.EmailToRevalidate = null;
            _customerService.UpdateCustomer(customer);
            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute, "");

            //re-authenticate (if usernames are disabled)
            if (!_customerSettings.UsernamesEnabled)
            {
                _authenticationService.SignIn(customer, true);
            }

            response.Message = _localizationService.GetResource("Account.EmailRevalidation.Changed");
            return Ok(response);
        }

        [HttpGet("menuvisibilitysettings")]
        public virtual IActionResult MenuVisibilitySettings(bool appStart)
        {
            var response = new GenericResponseModel<MenuVisibilityModel>();

            var model = new MenuVisibilityModel
            {
                HasReturnRequests = _orderSettings.ReturnRequestsEnabled &&
                    _returnRequestService.SearchReturnRequests(_storeContext.CurrentStore.Id,
                    _workContext.CurrentCustomer.Id, pageIndex: 0, pageSize: 1).Any(),
                HideDownloadableProducts = _customerSettings.HideDownloadableProductsTab
            };

            response.Data = model;

            return Ok(response);
        }

        #endregion

        #region My account / Addresses

        [HttpGet("addresses")]
        public virtual IActionResult Addresses()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            var response = new GenericResponseModel<CustomerAddressListModel>();
            response.Data = _customerModelFactory.PrepareCustomerAddressListModel();
            return Ok(response);
        }
        
        [HttpPost("addressdelete/{addressId:min(0)}")]
        public virtual IActionResult AddressDelete(int addressId)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            var customer = _workContext.CurrentCustomer;

            //find address (ensure that it belongs to the current customer)
            var address = _customerService.GetCustomerAddress(customer.Id, addressId);
            if (address != null)
            {
                _customerService.RemoveCustomerAddress(customer, address);
                _customerService.UpdateCustomer(customer);
                //now delete the address record
                _addressService.DeleteAddress(address);
            }

            return Ok(_localizationService.GetResource("NopStation.WebApi.Response.Customer.AddressDeleted"));
        }

        [HttpGet("addressadd")]
        public virtual IActionResult AddressAdd()
        {
            var response = new GenericResponseModel<CustomerAddressEditModel>();
            var model = new CustomerAddressEditModel();
            _addressModelFactory.PrepareAddressModel(model.Address,
                address: null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));

            response.Data = model;
            return Ok(response);
        }
        
        [HttpPost("addressadd")]
        public virtual IActionResult AddressAdd([FromBody]BaseQueryModel<CustomerAddressEditModel> queryModel)
        {
            try
            {
                if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                    return Unauthorized();

                var model = queryModel.Data;

                var form = queryModel.FormValues == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();
                //custom address attributes
                var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
                var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
                foreach (var error in customAttributeWarnings)
                {
                    ModelState.AddModelError("", error);
                }

                if (ModelState.IsValid)
                {
                    var address = model.Address.ToEntity();
                    address.CustomAttributes = customAttributes;
                    address.CreatedOnUtc = DateTime.UtcNow;
                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;
                    if (address.StateProvinceId <= 0)
                        address.StateProvinceId = null;

                    _addressService.InsertAddress(address);

                    _customerService.InsertCustomerAddress(_workContext.CurrentCustomer, address);

                    return Created(_localizationService.GetResource("NopStation.WebApi.Response.Customer.AddressUpdated"));
                }

                var response = new BaseResponseModel();
                foreach (var modelState in ModelState.Values)
                    foreach (var error in modelState.Errors)
                        response.ErrorList.Add(error.ErrorMessage);

                return BadRequest(response);
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc, _workContext.CurrentCustomer);
                return InternalServerError(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.SaveBillingFailed"));
            }
            
        }

        [HttpGet("addressedit/{addressId:min(0)}")]
        public virtual IActionResult AddressEdit(int addressId)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            var response = new GenericResponseModel<CustomerAddressEditModel>();
            var customer = _workContext.CurrentCustomer;
            //find address (ensure that it belongs to the current customer)
            var address = _customerService.GetCustomerAddress(customer.Id, addressId);
            if (address == null)
                //address is not found
                return NotFound(_localizationService.GetResource("NopStation.WebApi.Response.Customer.AddressNotFound"));

            var model = new CustomerAddressEditModel();
            _addressModelFactory.PrepareAddressModel(model.Address,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));

            response.Data = model;

            return Ok(response);
        }

        [HttpPost("addressedit/{addressId:min(0)}")]
        public virtual IActionResult AddressEdit([FromBody]BaseQueryModel<CustomerAddressEditModel> queryModel, int addressId)
        {
            try
            {
                if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                    return Unauthorized();

                var model = queryModel.Data;
                var customer = _workContext.CurrentCustomer;
                //find address (ensure that it belongs to the current customer)
                var address = _customerService.GetCustomerAddress(customer.Id, addressId);
                if (address == null)
                    //address is not found
                    return NotFound(_localizationService.GetResource("NopStation.WebApi.Response.Customer.AddressNotFound"));

                var form = queryModel.FormValues == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();
                //custom address attributes
                var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
                var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
                foreach (var error in customAttributeWarnings)
                {
                    ModelState.AddModelError("", error);
                }

                if (ModelState.IsValid)
                {
                    address = model.Address.ToEntity(address);
                    address.CustomAttributes = customAttributes;
                    _addressService.UpdateAddress(address);

                    return Ok(_localizationService.GetResource("NopStation.WebApi.Response.Customer.AddressUpdated"));
                }

                var response = new BaseResponseModel();
                foreach (var modelState in ModelState.Values)
                    foreach (var error in modelState.Errors)
                        response.ErrorList.Add(error.ErrorMessage);

                return BadRequest(response);
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc, _workContext.CurrentCustomer);
                return InternalServerError(_localizationService.GetResource("NopStation.WebApi.Response.Checkout.SaveBillingFailed"));
            }
            
        }

        #endregion

        #region My account / Downloadable products

        [HttpGet("downloadableproducts")]
        public virtual IActionResult DownloadableProducts()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            if (_customerSettings.HideDownloadableProductsTab)
                return BadRequest();

            var response = new GenericResponseModel<CustomerDownloadableProductsModel>();
            response.Data = _customerModelFactory.PrepareCustomerDownloadableProductsModel();
            return Ok(response);
        }

        [HttpGet("useragreement/{orderItemId:guid}")]
        public virtual IActionResult UserAgreement(Guid orderItemId)
        {
            var orderItem = _orderService.GetOrderItemByGuid(orderItemId);
            if (orderItem == null)
                return NotFound(_localizationService.GetResource("NopStation.WebApi.Response.Customer.OrderItemNotFound"));

            var product = _productService.GetProductById(orderItem.ProductId);

            if (product == null || !product.HasUserAgreement)
                return BadRequest();

            var response = new GenericResponseModel<UserAgreementModel>();
            response.Data = _customerModelFactory.PrepareUserAgreementModel(orderItem, product);
            return Ok(response);
        }

        #endregion

        #region My account / Change password

        [HttpGet("changepassword")]
        public virtual IActionResult ChangePassword()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            var response = new GenericResponseModel<ChangePasswordModel>();
            response.Data = _customerModelFactory.PrepareChangePasswordModel();

            //display the cause of the change password 
            if (_customerService.PasswordIsExpired(_workContext.CurrentCustomer))
            {
                response.ErrorList.Add(_localizationService.GetResource("Account.ChangePassword.PasswordIsExpired"));
                return BadRequest(response);
            }

            return Ok(response);
        }

        
        [HttpPost("changepassword")]
        public virtual IActionResult ChangePassword([FromBody]BaseQueryModel<ChangePasswordModel> queryModel)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            var model = queryModel.Data;
            var response = new BaseResponseModel();
            var customer = _workContext.CurrentCustomer;

            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(customer.Email,
                    true, _customerSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
                var changePasswordResult = _customerRegistrationService.ChangePassword(changePasswordRequest);
                if (changePasswordResult.Success)
                    return Ok(_localizationService.GetResource("Account.ChangePassword.Success"));

                //errors
                response.ErrorList.AddRange(changePasswordResult.Errors);
            }

            foreach (var modelState in ModelState.Values)
                foreach (var error in modelState.Errors)
                    response.ErrorList.Add(error.ErrorMessage);

            //If we got this far, something failed, redisplay form
            return BadRequest(response);
        }

        #endregion

        #region My account / Avatar

        [HttpGet("avatar")]
        public virtual IActionResult Avatar()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return MethodNotAllowed();

            var response = new GenericResponseModel<CustomerAvatarModel>();
            response.Data = _customerModelFactory.PrepareCustomerAvatarModel(new CustomerAvatarModel());
            return Ok(response);
        }
        
        [HttpPost("uploadavatar")]
        public virtual IActionResult UploadAvatar()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return MethodNotAllowed();

            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return BadRequest(_localizationService.GetResource("Account.Avatar.NoFileUploaded"));
            }

            var fileBinary = _downloadService.GetDownloadBits(httpPostedFile);

            var qqFileNameParameter = "cafilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = _fileProvider.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var avatarMaxSize = _customerSettings.AvatarMaximumSizeBytes;
            if (fileBinary.Length > avatarMaxSize)
                return BadRequest(string.Format(_localizationService.GetResource("Account.Avatar.MaximumUploadedFileSize"), avatarMaxSize));

            var customer = _workContext.CurrentCustomer;

            var customerAvatar = _pictureService.GetPictureById(_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute));
            if (customerAvatar != null)
                customerAvatar = _pictureService.UpdatePicture(customerAvatar.Id, fileBinary, contentType, null);
            else
                customerAvatar = _pictureService.InsertPicture(fileBinary, contentType, null);

            var customerAvatarId = 0;
            if (customerAvatar != null)
                customerAvatarId = customerAvatar.Id;

            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AvatarPictureIdAttribute, customerAvatarId);

            var model = new CustomerAvatarModel();
            model.AvatarUrl = _pictureService.GetPictureUrl(
                _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                _mediaSettings.AvatarPictureSize,
                false);

            var response = new GenericResponseModel<CustomerAvatarModel>();
            response.Data = model;
            return Ok(response);
        }
        
        [HttpPost("removeavatar")]
        public virtual IActionResult RemoveAvatar()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return MethodNotAllowed();

            var customer = _workContext.CurrentCustomer;

            var customerAvatar = _pictureService.GetPictureById(_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute));
            if (customerAvatar != null)
                _pictureService.DeletePicture(customerAvatar);
            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AvatarPictureIdAttribute, 0);

            return Ok(_localizationService.GetResource("NopStation.WebApi.Response.Customer.AvatarRemoved"));
        }

        #endregion

        #region GDPR tools

        [HttpGet("gdpr")]
        public virtual IActionResult GdprTools()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            if (!_gdprSettings.GdprEnabled)
                return MethodNotAllowed();

            var response = new GenericResponseModel<GdprToolsModel>();
            response.Data = _customerModelFactory.PrepareGdprToolsModel();
            return Ok(response);
        }

        
        [HttpPost("gdprexport")]
        public virtual IActionResult GdprToolsExport()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            if (!_gdprSettings.GdprEnabled)
                return MethodNotAllowed();

            //log
            _gdprService.InsertLog(_workContext.CurrentCustomer, 0, GdprRequestType.ExportData, _localizationService.GetResource("Gdpr.Exported"));

            //export
            var bytes = _exportManager.ExportCustomerGdprInfoToXlsx(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id);

            return File(bytes, MimeTypes.TextXlsx, "customerdata.xlsx");
        }

       
        [HttpPost("gdprdelete")]
        public virtual IActionResult GdprToolsDelete()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            if (!_gdprSettings.GdprEnabled)
                return MethodNotAllowed();

            //log
            _gdprService.InsertLog(_workContext.CurrentCustomer, 0, GdprRequestType.DeleteCustomer, _localizationService.GetResource("Gdpr.DeleteRequested"));

            var response = new GenericResponseModel<GdprToolsModel>();
            response.Data = _customerModelFactory.PrepareGdprToolsModel();
            response.Message = _localizationService.GetResource("Gdpr.DeleteRequested.Success");
            return Ok(response);
        }

        #endregion

        #region Check gift card balance

        [HttpGet("checkgiftcardbalance")]
        public virtual IActionResult CheckGiftCardBalance()
        {
            if (!(_captchaSettings.Enabled && _customerSettings.AllowCustomersToCheckGiftCardBalance))
                return MethodNotAllowed();

            var response = new GenericResponseModel<CheckGiftCardBalanceModel>();
            response.Data = _customerModelFactory.PrepareCheckGiftCardBalanceModel();
            return Ok(response);
        }

        
        [HttpPost("checkgiftcardbalance")]
        public virtual IActionResult CheckBalance([FromBody]BaseQueryModel<CheckGiftCardBalanceModel> queryModel)
        {
            var model = queryModel.Data;
            var response = new GenericResponseModel<CheckGiftCardBalanceModel>();

            if (ModelState.IsValid)
            {
                var giftCard = _giftCardService.GetAllGiftCards(giftCardCouponCode: model.GiftCardCode).FirstOrDefault();
                if (giftCard != null && _giftCardService.IsGiftCardValid(giftCard))
                {
                    var remainingAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_giftCardService.GetGiftCardRemainingAmount(giftCard), _workContext.WorkingCurrency);
                    model.Result = _priceFormatter.FormatPrice(remainingAmount, true, false);
                    response.Data = model;

                    return Ok(response);
                }
                else
                    response.ErrorList.Add(_localizationService.GetResource("CheckGiftCardBalance.GiftCardCouponCode.Invalid"));
            }

            foreach (var modelState in ModelState.Values)
                foreach (var error in modelState.Errors)
                    response.ErrorList.Add(error.ErrorMessage);

            return BadRequest(response);
        }

        #endregion

        #region Vendor Summary

        //My account / Order details page
        [HttpGet("vendorsummary")]
        public virtual IActionResult VendorSummary()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            var response = new GenericResponseModel<VendorSummaryModel>();
            response.Data = _orderModelFactory.PrepareVendorSummaryModel(0, 5);
            return Ok(response);
        }

        #endregion

        [HttpPost("deactivateaccount")]
        public virtual IActionResult DeactivateAccount([FromBody] BaseQueryModel<string> queryModel)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            var form = queryModel == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();

            var password = form["password"];

            var customer = _workContext.CurrentCustomer;

            var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? customer.Username : customer.Email, password);

            if (loginResult == CustomerLoginResults.WrongPassword)
                return BadRequest("Incorrect credentials!");

            _customerService.DeactivateCustomer(customer);

            return Ok(_localizationService.GetResource("NopStation.WebApi.Response.CustomerDeleted"));
        }

        #endregion
    }
}