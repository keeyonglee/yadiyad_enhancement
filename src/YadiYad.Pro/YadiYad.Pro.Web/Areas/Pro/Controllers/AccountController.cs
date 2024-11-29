using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Web.Models.Customer;
using YadiYad.Pro.Web.FactoriesPro;
using Nop.Services.Orders;
using Nop.Services.Logging;
using Nop.Services.Localization;
using Nop.Core.Domain;
using Nop.Core.Http;
using YadiYad.Pro.Services.Services.Customer;
using YadiYad.Pro.Services.Organization;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Services.Services.Messages;
using YadiYad.Pro.Services.DTO.Organization;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers
{
    [Area(AreaNames.Pro)]
    [AutoValidateAntiforgeryToken]
    public class AccountController : BaseController
    {
        #region Fields

        private readonly CustomerSettings _customerSettings;
        private readonly ICurrencyService _currencyService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IWorkContext _workContext;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly CustomerRegistrationRoleService _customerRegistrationRoleService;
        private readonly IStoreContext _storeContext;
        private readonly AccountContext _accountContext;
        private readonly ICustomerService _customerService;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ProWorkflowMessageService _proWorkflowMessageService;
        private readonly CustomerProModelFactory _customerModelFactory;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly OrganizationProfileService _organizationProfileService;
        private readonly IndividualProfileService _individualProfileService;
        private readonly BankAccountService _bankAccountService;

        #endregion

        #region Ctor

        public AccountController(
            CustomerSettings customerSettings,
            IAuthenticationService authenticationService,
            ICurrencyService currencyService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext,
            AccountContext accountContext,
            IGenericAttributeService genericAttributeService,
            ProWorkflowMessageService workflowMessageService,
            CustomerProModelFactory customerModelFactory,
            IShoppingCartService shoppingCartService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            StoreInformationSettings storeInformationSettings,
            CustomerRegistrationRoleService customerRegistrationRoleService,
            OrganizationProfileService organizationProfileService,
            IndividualProfileService individualProfileService,
            BankAccountService bankAccountService)
        {
            _customerSettings = customerSettings;
            _authenticationService = authenticationService;
            _currencyService = currencyService;
            _customerAttributeParser = customerAttributeParser;
            _customerAttributeService = customerAttributeService;
            _customerRegistrationService = customerRegistrationService;
            _customerService = customerService;
            _eventPublisher = eventPublisher;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
            _genericAttributeService = genericAttributeService;
            _proWorkflowMessageService = workflowMessageService;
            _customerModelFactory = customerModelFactory;
            _shoppingCartService = shoppingCartService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _storeInformationSettings = storeInformationSettings;
            _customerRegistrationRoleService = customerRegistrationRoleService;
            _organizationProfileService = organizationProfileService;
            _individualProfileService = individualProfileService;
            _accountContext = accountContext;
            _bankAccountService = bankAccountService;
        }

        #endregion

        #region Utilities

        protected virtual string ParseCustomCustomerAttributes(IFormCollection form)
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


        #endregion

        #region Methods

        #region Register

        [CheckAccessPublicStore(true)]
        public IActionResult Register()
        {
            var model = new RegisterProModel();
            model = _customerModelFactory.PrepareRegisterModel(model, false, setDefaultValues: true);
            model.FirstNameEnabled = false;
            model.FirstNameRequired = false;
            model.LastNameEnabled = false;
            model.LastNameRequired = false;

            return View(model);
        }

        [HttpPost]
        [CheckAccessPublicStore(true)]
        public virtual IActionResult Register(RegisterProModel model, string returnUrl, bool captchaValid, IFormCollection form)
        {
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

            //custom customer attributes
            var customerAttributesXml = ParseCustomCustomerAttributes(form);
            var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }

                var isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
                var registrationRequest = new CustomerRegistrationRequest(customer,
                    model.Email,
                    _customerSettings.UsernamesEnabled ? model.Username : model.Email,
                    model.Password,
                    _customerSettings.DefaultPasswordFormat,
                    _storeContext.CurrentStore.Id,
                    isApproved);

                registrationRequest.IsProOrganization = model.Type?.ToLower() == "Organization".ToLower();

                var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
                if (registrationResult.Success)
                {
                    //form fields
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.GenderAttribute, model.Gender);
                    if (_customerSettings.FirstNameEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FirstNameAttribute, model.FirstName);
                    if (_customerSettings.LastNameEnabled)
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
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StateProvinceIdAttribute,
                            model.StateProvinceId);
                    if (_customerSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PhoneAttribute, model.Phone);
                    if (_customerSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FaxAttribute, model.Fax);

                    //save customer attributes
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CustomCustomerAttributes, customerAttributesXml);

                    //add organization role
                    if (model.Type == "Organization")
                    {
                        var orgRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.OrganizationRoleName);
                        if (orgRole == null)
                            throw new NopException("'Organization' role could not be loaded");

                        _customerService.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = orgRole.Id });
                    }
                    //add individual role
                    //if (model.Type == "Individual")
                    //{
                    //    var indRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.IndividualRoleName);
                    //    if (indRole == null)
                    //        throw new NopException("'Individual' role could not be loaded");

                    //    _customerService.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = indRole.Id });
                    //}

                    //login customer now
                    if (isApproved)
                        _authenticationService.SignIn(customer, true);

                    //raise event       
                    _eventPublisher.Publish(new CustomerRegisteredEvent(customer));

                    switch (_customerSettings.UserRegistrationType)
                    {
                        case UserRegistrationType.EmailValidation:
                            {
                                //email validation message
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                                _proWorkflowMessageService.SendCustomerEmailValidationMessage(customer, _workContext.WorkingLanguage.Id);

                                //result
                                return RedirectToAction(
                                    "RegisterResult",
                                    "Account",
                                    new
                                    {
                                        area = "Pro",
                                        resultId = (int)UserRegistrationType.EmailValidation,
                                        type = model.Type.Substring(0, 3).ToLower()
                                    });
                            }
                        case UserRegistrationType.AdminApproval:
                            {
                                return RedirectToRoute("RegisterResult",
                                    new { resultId = (int)UserRegistrationType.AdminApproval });
                            }
                        case UserRegistrationType.Standard:
                            {
                                //send customer welcome message
                                _proWorkflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

                                //raise event       
                                _eventPublisher.Publish(new CustomerActivatedEvent(customer));

                                var redirectUrl = Url.RouteUrl("RegisterResult",
                                    new { resultId = (int)UserRegistrationType.Standard, returnUrl }, _webHelper.CurrentRequestProtocol);

                                var roles = _customerService.GetCustomerRoles(customer);
                                if (roles.Any(x => x.SystemName == NopCustomerDefaults.IndividualRoleName))
                                {
                                    var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(customer.Id);
                                    if (individualProfile == null)
                                    {
                                        return Redirect("~/pro/individual/details");
                                    }
                                }
                                if (roles.Any(x => x.SystemName == NopCustomerDefaults.OrganizationRoleName))
                                {
                                    var organizationProfile = _organizationProfileService.GetOrganizationProfileByCustomerId(customer.Id);
                                    if (organizationProfile == null)
                                    {
                                        return Redirect("~/pro/organization/details");
                                    }
                                }
                                return Redirect("~/Pro");
                            }
                        default:
                            {
                                return Redirect("~/Pro");
                            }
                    }
                }

                //errors
                foreach (var error in registrationResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            model = _customerModelFactory.PrepareRegisterModel(model, true, customerAttributesXml);
            model.FirstNameEnabled = false;
            model.FirstNameRequired = false;
            model.LastNameEnabled = false;
            model.LastNameRequired = false;
            return View(model);
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult RegisterResult(int resultId)
        {
            var model = _customerModelFactory.PrepareRegisterResultModel(resultId);
            return View(model);
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual IActionResult RegisterResult(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                return RedirectToRoute("ProHomepage");

            return Redirect(returnUrl);
        }

        [HttpsRequirement]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult AccountActivation(string token, string email, Guid guid)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                customer = _customerService.GetCustomerByGuid(guid);

            if (customer == null)
                return RedirectToRoute("ProHomepage");

            var cToken = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.AccountActivationTokenAttribute);
            if (string.IsNullOrEmpty(cToken))
                return
                    View(new AccountActivationModel
                    {
                        Result = _localizationService.GetResource("Account.AccountActivation.AlreadyActivated")
                    });

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("ProHomepage");

            //activate user account
            customer.Active = true;
            _customerService.UpdateCustomer(customer);
            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AccountActivationTokenAttribute, "");
            //send welcome message
            _proWorkflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

            //raise event       
            _eventPublisher.Publish(new CustomerActivatedEvent(customer));

            var model = new AccountActivationModel
            {
                Result = _localizationService.GetResource("Account.AccountActivation.Activated")
            };
            return View(model);
        }

        [HttpsRequirement]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult EmailRevalidation(string token, string email, Guid guid)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                customer = _customerService.GetCustomerByGuid(guid);

            if (customer == null)
                return RedirectToRoute("ProHomepage");

            var cToken = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute);
            if (string.IsNullOrEmpty(cToken))
                return View(new EmailRevalidationModel
                {
                    Result = _localizationService.GetResource("Account.EmailRevalidation.AlreadyChanged")
                });

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("ProHomepage");

            if (string.IsNullOrEmpty(customer.EmailToRevalidate))
                return RedirectToRoute("ProHomepage");

            if (_customerSettings.UserRegistrationType != UserRegistrationType.EmailValidation)
                return RedirectToRoute("ProHomepage");

            //change email
            try
            {
                _customerRegistrationService.SetEmail(customer, customer.EmailToRevalidate, false);
            }
            catch (Exception exc)
            {
                return View(new EmailRevalidationModel
                {
                    Result = _localizationService.GetResource(exc.Message)
                });
            }
            customer.EmailToRevalidate = null;
            _customerService.UpdateCustomer(customer);
            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute, "");

            //re-authenticate (if usernames are disabled)
            if (!_customerSettings.UsernamesEnabled)
            {
                _authenticationService.SignIn(customer, true);
            }

            var model = new EmailRevalidationModel()
            {
                Result = _localizationService.GetResource("Account.EmailRevalidation.Changed")
            };
            return View(model);
        }


        #endregion

        #region Login / Logout

        [HttpsRequirement]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult Login(bool? checkoutAsGuest)
        {
            var customer = _workContext.CurrentCustomer;
            if (customer != null)
            {
                var roles = _customerService.GetCustomerRoles(customer);
                if (roles.Any(x => x.SystemName == NopCustomerDefaults.IndividualRoleName))
                {

                    return Redirect("~/pro/individual/index");

                }
                if (roles.Any(x => x.SystemName == NopCustomerDefaults.OrganizationRoleName))
                {

                    //return Redirect("~/pro/organization/dashboard");
                    return Redirect("~/pro/organization/index");
                }
            }

            var model = _customerModelFactory.PrepareLoginModel(checkoutAsGuest);
            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult Login(LoginProModel model, string returnUrl, bool captchaValid)
        {
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

                            //migrate shopping cart
                            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

                            //sign in new customer
                            _authenticationService.SignIn(customer, model.RememberMe);

                            //raise event       
                            _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

                            //activity log
                            _customerActivityService.InsertActivity(customer, "PublicStore.Login",
                                _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);

                            var roles = _customerService.GetCustomerRoles(customer);

                            if (string.IsNullOrEmpty(returnUrl) == false && Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            if (roles.Any(x => x.SystemName == NopCustomerDefaults.IndividualRoleName))
                            {
                                var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(customer.Id);
                                if (individualProfile == null)
                                {
                                    return Redirect("~/pro/individual/details");
                                }
                            }
                            if (roles.Any(x => x.SystemName == NopCustomerDefaults.OrganizationRoleName))
                            {
                                var organizationProfile = _organizationProfileService.GetOrganizationProfileByCustomerId(customer.Id);
                                if (organizationProfile == null)
                                {
                                    return Redirect("~/pro/organization/details");
                                }
                                else
                                {
                                    //return Redirect("~/pro/organization/dashboard");
                                }
                            }
                            if (roles.Any(x => x.SystemName == NopCustomerDefaults.ModeratorRoleName))
                            {
                                return Redirect("~/pro/consultation/advs/review");
                            }
                            return Redirect("~/Pro");
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

            //If we got this far, something failed, redisplay form
            model = _customerModelFactory.PrepareLoginModel(model.CheckoutAsGuest);
            return View(model);
        }

        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult Logout()
        {
            if (_workContext.OriginalCustomerIfImpersonated != null)
            {
                //activity log
                _customerActivityService.InsertActivity(_workContext.OriginalCustomerIfImpersonated, "Impersonation.Finished",
                    string.Format(_localizationService.GetResource("ActivityLog.Impersonation.Finished.StoreOwner"),
                        _workContext.CurrentCustomer.Email, _workContext.CurrentCustomer.Id),
                    _workContext.CurrentCustomer);

                _customerActivityService.InsertActivity("Impersonation.Finished",
                    string.Format(_localizationService.GetResource("ActivityLog.Impersonation.Finished.Customer"),
                        _workContext.OriginalCustomerIfImpersonated.Email, _workContext.OriginalCustomerIfImpersonated.Id),
                    _workContext.OriginalCustomerIfImpersonated);

                //logout impersonated customer
                _genericAttributeService
                    .SaveAttribute<int?>(_workContext.OriginalCustomerIfImpersonated, NopCustomerDefaults.ImpersonatedCustomerIdAttribute, null);

                //redirect back to customer details page (admin area)
                return RedirectToAction("Edit", "Customer", new { id = _workContext.CurrentCustomer.Id, area = AreaNames.Admin });
            }

            //activity log
            _customerActivityService.InsertActivity(_workContext.CurrentCustomer, "PublicStore.Logout",
                _localizationService.GetResource("ActivityLog.PublicStore.Logout"), _workContext.CurrentCustomer);

            //standard logout 
            _authenticationService.SignOut();

            //raise logged out event       
            _eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

            //EU Cookie
            if (_storeInformationSettings.DisplayEuCookieLawWarning)
            {
                //the cookie law message should not pop up immediately after logout.
                //otherwise, the user will have to click it again...
                //and thus next visitor will not click it... so violation for that cookie law..
                //the only good solution in this case is to store a temporary variable
                //indicating that the EU cookie popup window should not be displayed on the next page open (after logout redirection to homepage)
                //but it'll be displayed for further page loads
                TempData[$"{NopCookieDefaults.Prefix}{NopCookieDefaults.IgnoreEuCookieLawWarning}"] = true;
            }

            _accountContext.ClearAccountSession();

            return Redirect("~/pro/home/info");
        }

        #endregion

        #region Password recovery

        [HttpsRequirement]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult PasswordRecovery()
        {
            var model = new PasswordRecoveryProModel();
            model = _customerModelFactory.PreparePasswordRecoveryModel(model);

            return View(model);
        }

        [HttpPost, ActionName("PasswordRecovery")]
        [FormValueRequired("send-email")]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult PasswordRecoverySend(PasswordRecoveryProModel model, bool captchaValid)
        {
            if (ModelState.IsValid)
            {
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
                    _proWorkflowMessageService.SendCustomerPasswordRecoveryMessage(customer,
                        _workContext.WorkingLanguage.Id);

                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailHasBeenSent");
                }
                else
                {
                    var error = _localizationService.GetResource("Account.PasswordRecovery.EmailNotFound");
                    ModelState.AddModelError("", error);
                }
            }

            model = _customerModelFactory.PreparePasswordRecoveryModel(model);
            return View(model);
        }

        [HttpsRequirement]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult PasswordRecoveryConfirm(string token, string email, Guid guid)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                customer = _customerService.GetCustomerByGuid(guid);

            if (customer == null)
                return Redirect("~/Pro/Account/Login");

            if (string.IsNullOrEmpty(_genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute)))
            {
                return base.View(new PasswordRecoveryConfirmProModel
                {
                    DisablePasswordChanging = true,
                    Result = _localizationService.GetResource("Account.PasswordRecovery.PasswordAlreadyHasBeenChanged")
                });
            }

            var model = _customerModelFactory.PreparePasswordRecoveryConfirmModel();

            //validate token
            if (!_customerService.IsPasswordRecoveryTokenValid(customer, token))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.WrongToken");
            }

            //validate token expiration date
            if (_customerService.IsPasswordRecoveryLinkExpired(customer))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.LinkExpired");
            }

            return View(model);
        }

        [HttpPost, ActionName("PasswordRecoveryConfirm")]
        [FormValueRequired("set-password")]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult PasswordRecoveryConfirmPOST(string token, string email, Guid guid, PasswordRecoveryConfirmProModel model)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                customer = _customerService.GetCustomerByGuid(guid);

            if (customer == null)
                return Redirect("~/Pro/Account/Login");

            //validate token
            if (!_customerService.IsPasswordRecoveryTokenValid(customer, token))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.WrongToken");
                return View(model);
            }

            //validate token expiration date
            if (_customerService.IsPasswordRecoveryLinkExpired(customer))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.LinkExpired");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var response = _customerRegistrationService.ChangePassword(new ChangePasswordRequest(customer.Email,
                    false, _customerSettings.DefaultPasswordFormat, model.NewPassword));
                if (response.Success)
                {
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute, "");

                    model.DisablePasswordChanging = true;
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.PasswordHasBeenChanged");
                }
                else
                {
                    model.Result = response.Errors.FirstOrDefault();
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Resent Activation email

        [HttpsRequirement]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult ResentActivation()
        {
            var model = new ResentActivationProModel();
            return View(model);
        }

        [HttpPost]
        [HttpsRequirement]
        [CheckAccessPublicStore(true)]
        public virtual IActionResult ResentActivation(ResentActivationProModel model)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var customer = _customerService.GetCustomerByEmail(model.Email);
            if (customer != null && !customer.Deleted)
            {
                var cToken = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.AccountActivationTokenAttribute);
                if (string.IsNullOrEmpty(cToken))
                {
                    model.Result = _localizationService.GetResource("Account.AccountActivation.AlreadyActivated");

                }
                else
                {
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                    _proWorkflowMessageService.SendCustomerEmailValidationMessage(customer, _workContext.WorkingLanguage.Id);

                    model.Result = _localizationService.GetResource("Admin.Customers.Customers.ReSendActivationMessage.Success");
                }

            }
            else
            {
                var error = _localizationService.GetResource("Account.PasswordRecovery.EmailNotFound");
                ModelState.AddModelError("", error);
            }
            return View(model);
        }

        #endregion

        #region My account / Change password

        [CheckAccessPublicStore(true)]
        public virtual IActionResult PasswordChange()
        {
            var customerId = _workContext.CurrentCustomer.Id;
            if (customerId == 0)
            {
                throw new NopException("User cannot be found");

            }
            var model = new PasswordChangeProModel();
            model.CustomerId = customerId;
            var roleList = _customerRegistrationRoleService.GetCustomerRolesByCustomerId(customerId);
            foreach (var role in roleList)
            {
                if (role == "Individual")
                {
                    var individual = _individualProfileService.GetIndividualProfileByCustomerId(_workContext.CurrentCustomer.Id);
                    model.IsIndividual = true;
                    model.IsOnline = individual.IsOnline;
                }

            }

            return View(model);
        }

        [HttpPost, ActionName("PasswordChange")]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult PasswordChange(PasswordChangeProModel model)
        {
            var customer = _customerService.GetCustomerByEmail(_workContext.CurrentCustomer.Email);
            if (customer == null)
                return Challenge();

            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(customer.Email,
                    true, _customerSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
                var changePasswordResult = _customerRegistrationService.ChangePassword(changePasswordRequest);
                if (changePasswordResult.Success)
                {
                    model.Result = _localizationService.GetResource("Account.ChangePassword.Success");
                    return Redirect("~/pro/");
                }

                //errors
                foreach (var error in changePasswordResult.Errors)
                    ModelState.AddModelError("", error);
            }
            return View(model);
        }

        #endregion

        #region My account / Change setting

        [HttpsRequirement]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult SettingChange()
        {
            var individual = _individualProfileService.GetIndividualProfileByCustomerId(_workContext.CurrentCustomer.Id);
            if (individual == null)
                throw new NopException("'Individual' setting could not be loaded");
            var model = new SettingChangeProModel();
            model.CustomerId = individual.CustomerId;
            model.IsOnline = individual.IsOnline;
            return View(model);
        }

        [HttpPost, ActionName("SettingChange")]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult SettingChange(SettingChangeProModel model)
        {
            var individual = _individualProfileService.GetIndividualProfileByCustomerId(_workContext.CurrentCustomer.Id);
            if (individual == null)
                throw new NopException("'Individual' setting could not be saved");
            individual.IsOnline = model.IsOnline;
            _individualProfileService.UpdateIndividualProfile(_workContext.CurrentCustomer.Id, individual);
            return Redirect("~/pro/");
        }

        #endregion

        #region Bank account
        [HttpGet]
        public IActionResult BankAccount()
        {
            var result = _bankAccountService.GetBankAccountByCustomerId(_workContext.CurrentCustomer.Id);

            return View(result);
        }
        #endregion

        #region Tax account
        [HttpGet]
        public IActionResult TaxAccount()
        {
            return View();
        }
        #endregion

        [HttpGet("/login")]
        public IActionResult Login([FromQuery]string returnUrl)
        {
            string referer = ((string)Request.Headers["Referer"])?.ToString().ToLower()??"";

            if (referer.Split("/", StringSplitOptions.RemoveEmptyEntries).Contains("pro"))
            {
                return RedirectToRoute("ProLogin", new { returnUrl = returnUrl });
            }
            else if (referer.Split("/", StringSplitOptions.RemoveEmptyEntries).Contains("shuq"))
            {
                return RedirectToRoute("Login", new { returnUrl = returnUrl });
            }

            return RedirectToRoute("Login", new { returnUrl = returnUrl });
        }

        #endregion
    }
}
