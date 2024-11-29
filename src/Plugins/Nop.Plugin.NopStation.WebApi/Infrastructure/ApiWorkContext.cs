using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using Microsoft.AspNetCore.Http;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Services.Tasks;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Core.Security;

namespace Nop.Plugin.NopStation.WebApi.Infrastructure
{
    public class ApiWorkContext : WebWorkContext
    {
        #region Fields

        private Customer _cachedCustomer;
        private Customer _originalCustomerIfImpersonated;
        private readonly CookieSettings _cookieSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomerService _customerService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IAuthenticationService _authenticationService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerApiService _customerApiService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public ApiWorkContext(CookieSettings cookieSettings,
            IHttpContextAccessor httpContextAccessor,
            ICustomerService customerService,
            IVendorService vendorService,
            IStoreContext storeContext,
            IAuthenticationService authenticationService,
            ILanguageService languageService,
            IGenericAttributeService genericAttributeService,
            TaxSettings taxSettings,
            CurrencySettings currencySettings,
            LocalizationSettings localizationSettings,
            IUserAgentHelper userAgentHelper,
            IStoreMappingService storeMappingService,
            ICurrencyService currencyService1,
            IGenericAttributeService genericAttributeService1,
            ICustomerApiService customerApiService,
            IWebHelper webHelper)
            : base(cookieSettings,
                  currencySettings,
                  authenticationService,
                  currencyService1,
                  customerService,
                  genericAttributeService,
                  httpContextAccessor,
                  languageService,
                  storeContext,
                  storeMappingService,
                  userAgentHelper,
                  vendorService,
                  webHelper,
                  localizationSettings,
                  taxSettings)
        {
            _cookieSettings = cookieSettings;
            _httpContextAccessor = httpContextAccessor;
            _customerService = customerService;
            _userAgentHelper = userAgentHelper;
            _authenticationService = authenticationService;
            _localizationSettings = localizationSettings;
            _genericAttributeService = genericAttributeService1;
            _customerApiService = customerApiService;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

        protected virtual void SetCustomerTokenCookie(string token)
        {
            if (_httpContextAccessor.HttpContext?.Response == null)
                return;

            //delete current cookie value
            var cookieName = $".Nop.Customer.Token";
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

            //get date of cookie expiration
            var cookieExpires = 24 * 365; //TODO make configurable
            var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);

            //if passed guid is empty set cookie as expired
            if (string.IsNullOrWhiteSpace(token))
                cookieExpiresDate = DateTime.Now.AddMonths(-1);

            //set new cookie value
            var options = new CookieOptions
            {
                HttpOnly = true,
                Expires = cookieExpiresDate
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, token, options);
        }

        protected virtual void SetCustomerDeviceIdCookie(string deviceId)
        {
            if (_httpContextAccessor.HttpContext?.Response == null)
                return;

            //delete current cookie value
            var cookieName = $".Nop.Customer.DeviceId";
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

            //get date of cookie expiration
            var cookieExpires = 24 * 365; //TODO make configurable
            var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);

            //if passed guid is empty set cookie as expired
            if (string.IsNullOrWhiteSpace(deviceId))
                cookieExpiresDate = DateTime.Now.AddMonths(-1);

            //set new cookie value
            var options = new CookieOptions
            {
                HttpOnly = true,
                Expires = cookieExpiresDate
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, deviceId, options);
        }

        protected bool TryGetCustomerByToken(out Customer customer)
        {
            customer = null;
            try
            {
                var token = "";
                if (_httpContextAccessor.HttpContext != null &&
                    _httpContextAccessor.HttpContext.Request.Headers.ContainsKey(WebApiCustomerDefaults.Token))
                {
                    _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(WebApiCustomerDefaults.Token, out var keyFound);
                    token = keyFound.FirstOrDefault();
                }
                else
                {
                    var cookieName = $".Nop.Customer.Token";
                    token = _httpContextAccessor.HttpContext?.Request?.Cookies[cookieName];

                    if (string.IsNullOrWhiteSpace(token))
                        token = _webHelper.QueryString<string>("customerToken");
                }

                if (!string.IsNullOrWhiteSpace(token))
                {
                    SetCustomerTokenCookie(token);
                    var load = JwtHelper.JwtDecoder.DecodeToObject(token, WebApiCustomerDefaults.SecretKey, true) as IDictionary<string, object>;
                    if (load != null)
                    {
                        var customerId = Convert.ToInt32(load[WebApiCustomerDefaults.CustomerId]);
                        customer = _customerService.GetCustomerById(customerId);
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                logger.Error(ex.Message, ex);
            }

            return customer != null;
        }

        protected bool TryGetCustomerByDeviceId(out Customer customer)
        {
            customer = null;
            try
            {
                var deviceId = "";
                if (_httpContextAccessor.HttpContext != null &&
                    _httpContextAccessor.HttpContext.Request.Headers.ContainsKey(WebApiCustomerDefaults.DeviceId))
                {
                    _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(WebApiCustomerDefaults.DeviceId, out var keyFound);
                    deviceId = keyFound.FirstOrDefault();
                }
                else
                {
                    var cookieName = $".Nop.Customer.DeviceId";
                    deviceId = _httpContextAccessor.HttpContext?.Request?.Cookies[cookieName];

                    if (string.IsNullOrWhiteSpace(deviceId))
                        deviceId = _webHelper.QueryString<string>("deviceId");
                }

                if (!string.IsNullOrWhiteSpace(deviceId))
                {
                    SetCustomerDeviceIdCookie(deviceId);
                    var customerGuid = HelperExtension.GetGuid(deviceId);
                    customer = _customerService.GetCustomerByGuid(customerGuid);
                    if (customer != null && _customerService.IsRegistered(customer))
                    {
                        customer.CustomerGuid = Guid.NewGuid();
                        _customerService.UpdateCustomer(customer);
                        customer = _customerApiService.InsertDeviceGuestCustomer(deviceId);
                    }
                    else if (customer == null)
                    {
                        customer = _customerApiService.InsertDeviceGuestCustomer(deviceId);
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                logger.Error(ex.Message, ex);
            }

            return customer != null;
        }

        #endregion

        #region Properties

        public override Customer CurrentCustomer
        {
            get
            {
                if (_cachedCustomer != null)
                    return _cachedCustomer;

                Customer customer = null;

                //check whether request is made by a background (schedule) task
                if (_httpContextAccessor.HttpContext == null ||
                    _httpContextAccessor.HttpContext.Request.Path.Equals(new PathString($"/{NopTaskDefaults.ScheduleTaskPath}"), StringComparison.InvariantCultureIgnoreCase))
                {
                    //in this case return built-in customer record for background task
                    customer = _customerService.GetCustomerBySystemName(NopCustomerDefaults.BackgroundTaskCustomerName);
                }

                if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                {
                    //check whether request is made by a search engine, in this case return built-in customer record for search engines
                    if (_userAgentHelper.IsSearchEngine())
                        customer = _customerService.GetCustomerBySystemName(NopCustomerDefaults.SearchEngineCustomerName);
                }

                if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                {
                    //check nop-station api user
                    if (TryGetCustomerByToken(out customer) || TryGetCustomerByDeviceId(out customer))
                        _cachedCustomer = customer;
                }

                if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                {
                    //try to get registered user
                    customer = _authenticationService.GetAuthenticatedCustomer();
                }

                if (customer != null && !customer.Deleted && customer.Active && !customer.RequireReLogin)
                {
                    //get impersonate user if required
                    var impersonatedCustomerId = _genericAttributeService
                        .GetAttribute<int?>(customer, NopCustomerDefaults.ImpersonatedCustomerIdAttribute);
                    if (impersonatedCustomerId.HasValue && impersonatedCustomerId.Value > 0)
                    {
                        var impersonatedCustomer = _customerService.GetCustomerById(impersonatedCustomerId.Value);
                        if (impersonatedCustomer != null && !impersonatedCustomer.Deleted && impersonatedCustomer.Active && !impersonatedCustomer.RequireReLogin)
                        {
                            //set impersonated customer
                            _originalCustomerIfImpersonated = customer;
                            customer = impersonatedCustomer;
                        }
                    }
                }

                if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                {
                    //get guest customer
                    var customerCookie = GetCustomerCookie();
                    if (!string.IsNullOrEmpty(customerCookie))
                    {
                        if (Guid.TryParse(customerCookie, out Guid customerGuid))
                        {
                            //get customer from cookie (should not be registered)
                            var customerByCookie = _customerService.GetCustomerByGuid(customerGuid);
                            if (customerByCookie != null && !_customerService.IsRegistered(customerByCookie))
                                customer = customerByCookie;
                        }
                    }
                }

                if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                {
                    //create guest if not exists
                    customer = _customerService.InsertGuestCustomer();
                }

                if (!customer.Deleted && customer.Active && !customer.RequireReLogin)
                {
                    //set customer cookie
                    SetCustomerCookie(customer.CustomerGuid);

                    //cache the found customer
                    _cachedCustomer = customer;
                }

                return _cachedCustomer;
            }
            set
            {
                SetCustomerCookie(value.CustomerGuid);
                _cachedCustomer = value;
            }
        }

        public override Customer OriginalCustomerIfImpersonated
        {
            get
            {
                return _originalCustomerIfImpersonated;
            }
        }

        #endregion
    }
}
