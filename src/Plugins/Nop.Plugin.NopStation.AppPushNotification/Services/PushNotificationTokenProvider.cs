using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Payout;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Html;
using Nop.Core.Infrastructure;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.ShippingShuq;
using Nop.Services.ShuqOrders;
using Nop.Services.Stores;
using Nop.Services.Vendors;

namespace Nop.Plugin.NopStation.AppPushNotification.Services
{
    public class AppPushNotificationTokenProvider : IPushNotificationTokenProvider
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerAttributeFormatter _customerAttributeFormatter;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorAttributeFormatter _vendorAttributeFormatter;
        private readonly IWorkContext _workContext;
        private readonly PaymentSettings _paymentSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly IAddressService _addressService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICountryService _countryService;
        private readonly IProductService _productService;
        private readonly OrderSettings _orderSettings;
        private readonly IShuqOrderService _shuqOrderService;
        private readonly IVendorService _vendorService;

        private Dictionary<string, IEnumerable<string>> _allowedTokens;

        #endregion

        #region Ctor

        public AppPushNotificationTokenProvider(CurrencySettings currencySettings,
            IActionContextAccessor actionContextAccessor,
            IAddressAttributeFormatter addressAttributeFormatter,
            ICurrencyService currencyService,
            ICustomerAttributeFormatter customerAttributeFormatter,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPriceFormatter priceFormatter,
            IStoreContext storeContext,
            IStoreService storeService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IVendorAttributeFormatter vendorAttributeFormatter,
            IWorkContext workContext,
            PaymentSettings paymentSettings,
            StoreInformationSettings storeInformationSettings,
            IAddressService addressService,
            IStateProvinceService stateProvinceService,
            ICountryService countryService,
            IProductService productService,
            OrderSettings orderSettings,
            IShuqOrderService shuqOrderService,
            IVendorService vendorService)
        {
            _currencySettings = currencySettings;
            _actionContextAccessor = actionContextAccessor;
            _addressAttributeFormatter = addressAttributeFormatter;
            _currencyService = currencyService;
            _customerAttributeFormatter = customerAttributeFormatter;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _languageService = languageService;
            _localizationService = localizationService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _priceFormatter = priceFormatter;
            _storeContext = storeContext;
            _storeService = storeService;
            _urlHelperFactory = urlHelperFactory;
            _urlRecordService = urlRecordService;
            _vendorAttributeFormatter = vendorAttributeFormatter;
            _workContext = workContext;
            _paymentSettings = paymentSettings;
            _storeInformationSettings = storeInformationSettings;
            _addressService = addressService;
            _stateProvinceService = stateProvinceService;
            _countryService = countryService;
            _productService = productService;
            _orderSettings = orderSettings;
            _shuqOrderService = shuqOrderService;
            _vendorService = vendorService;
        }

        #endregion

        protected Dictionary<string, IEnumerable<string>> AllowedTokens
        {
            get
            {
                if (_allowedTokens != null)
                    return _allowedTokens;

                _allowedTokens = new Dictionary<string, IEnumerable<string>>();

                //store tokens
                _allowedTokens.Add(AppPushNotificationTokenGroupNames.StoreTokens, new[]
                {
                    "%Store.Name%",
                    "%Store.URL%",
                    "%Store.CompanyName%",
                    "%Store.CompanyAddress%",
                    "%Store.CompanyPhoneNumber%",
                    "%Store.CompanyVat%",
                    "%Facebook.URL%",
                    "%Twitter.URL%",
                    "%YouTube.URL%"
                });

                //customer tokens
                _allowedTokens.Add(AppPushNotificationTokenGroupNames.CustomerTokens, new[]
                {
                    "%Customer.Email%",
                    "%Customer.Username%",
                    "%Customer.FullName%",
                    "%Customer.FirstName%",
                    "%Customer.LastName%",
                    "%Customer.VatNumber%",
                    "%Customer.VatNumberStatus%",
                    "%Customer.CustomAttributes%",
                    "%Customer.PasswordRecoveryURL%",
                    "%Customer.AccountActivationURL%",
                    "%Customer.EmailRevalidationURL%",
                    "%Wishlist.URLForCustomer%"
                });

                //order tokens
                _allowedTokens.Add(AppPushNotificationTokenGroupNames.OrderTokens, new[]
                {
                    "%Order.OrderNumber%",
                    "%Order.CustomerFullName%",
                    "%Order.CustomerEmail%",
                    "%Order.BillingFirstName%",
                    "%Order.BillingLastName%",
                    "%Order.BillingPhoneNumber%",
                    "%Order.BillingEmail%",
                    "%Order.BillingFaxNumber%",
                    "%Order.BillingCompany%",
                    "%Order.BillingAddress1%",
                    "%Order.BillingAddress2%",
                    "%Order.BillingCity%",
                    "%Order.BillingCounty%",
                    "%Order.BillingStateProvince%",
                    "%Order.BillingZipPostalCode%",
                    "%Order.BillingCountry%",
                    "%Order.BillingCustomAttributes%",
                    "%Order.Shippable%",
                    "%Order.ShippingMethod%",
                    "%Order.ShippingFirstName%",
                    "%Order.ShippingLastName%",
                    "%Order.ShippingPhoneNumber%",
                    "%Order.ShippingEmail%",
                    "%Order.ShippingFaxNumber%",
                    "%Order.ShippingCompany%",
                    "%Order.ShippingAddress1%",
                    "%Order.ShippingAddress2%",
                    "%Order.ShippingCity%",
                    "%Order.ShippingCounty%",
                    "%Order.ShippingStateProvince%",
                    "%Order.ShippingZipPostalCode%",
                    "%Order.ShippingCountry%",
                    "%Order.ShippingCustomAttributes%",
                    "%Order.PaymentMethod%",
                    "%Order.VatNumber%",
                    "%Order.CustomValues%",
                    "%Order.CreatedOn%",
                    "%Order.OrderURLForCustomer%",
                    "%Order.PickupInStore%",
                    "%Order.OrderId%",
                    "%Order.DeliveryDate%",
                    "%Order.DeliveryTime%"
                });

                //shipment tokens
                _allowedTokens.Add(AppPushNotificationTokenGroupNames.ShipmentTokens, new[]
                {
                    "%Shipment.ShipmentNumber%",
                    "%Shipment.TrackingNumber%",
                    "%Shipment.TrackingNumberURL%",
                    "%Shipment.URLForCustomer%"
                });

                //refunded order tokens
                _allowedTokens.Add(AppPushNotificationTokenGroupNames.RefundedOrderTokens, new[]
                {
                    "%Order.AmountRefunded%"
                });

                //order note tokens
                _allowedTokens.Add(AppPushNotificationTokenGroupNames.OrderNoteTokens, new[]
                {
                    "%Order.NewNoteText%",
                    "%Order.OrderNoteAttachmentUrl%"
                });

                //recurring payment tokens
                _allowedTokens.Add(AppPushNotificationTokenGroupNames.RecurringPaymentTokens, new[]
                {
                    "%RecurringPayment.ID%",
                    "%RecurringPayment.CancelAfterFailedPayment%",
                    "%RecurringPayment.RecurringPaymentType%"
                });

                //newsletter subscription tokens
                _allowedTokens.Add(AppPushNotificationTokenGroupNames.SubscriptionTokens, new[]
                {
                    "%NewsLetterSubscription.Email%",
                    "%NewsLetterSubscription.ActivationUrl%",
                    "%NewsLetterSubscription.DeactivationUrl%"
                });

                //product tokens
                _allowedTokens.Add(AppPushNotificationTokenGroupNames.ProductTokens, new[]
                {
                    "%Product.ID%",
                    "%Product.Name%",
                    "%Product.ShortDescription%",
                    "%Product.SKU%",
                    "%Product.StockQuantity%"
                });

                //return request tokens
                _allowedTokens.Add(AppPushNotificationTokenGroupNames.ReturnRequestTokens, new[]
                {
                    "%ReturnRequest.CustomNumber%",
                    "%ReturnRequest.OrderId%",
                    "%ReturnRequest.Product.Quantity%",
                    "%ReturnRequest.Product.Name%",
                    "%ReturnRequest.Reason%",
                    "%ReturnRequest.RequestedAction%",
                    "%ReturnRequest.CustomerComment%",
                    "%ReturnRequest.StaffNotes%",
                    "%ReturnRequest.Status%"
                });

                //vendor tokens
                _allowedTokens.Add(AppPushNotificationTokenGroupNames.VendorTokens, new[]
                {
                    "%Vendor.Name%",
                    "%Vendor.Email%",
                    "%Vendor.VendorAttributes%"
                });

                //gift card tokens
                _allowedTokens.Add(AppPushNotificationTokenGroupNames.GiftCardTokens, new[]
                {
                    "%GiftCard.SenderName%",
                    "%GiftCard.SenderEmail%",
                    "%GiftCard.RecipientName%",
                    "%GiftCard.RecipientEmail%",
                    "%GiftCard.Amount%",
                    "%GiftCard.CouponCode%",
                    "%GiftCard.Message%"
                });

                //product review tokens
                _allowedTokens.Add(AppPushNotificationTokenGroupNames.ProductReviewTokens, new[]
                {
                    "%ProductReview.ProductName%",
                    "%ProductReview.Title%",
                    "%ProductReview.IsApproved%",
                    "%ProductReview.ReviewText%",
                    "%ProductReview.ReplyText%"
                });

                //attribute combination tokens
                _allowedTokens.Add(AppPushNotificationTokenGroupNames.AttributeCombinationTokens, new[]
                {
                    "%AttributeCombination.Formatted%",
                    "%AttributeCombination.SKU%",
                    "%AttributeCombination.StockQuantity%"
                });

                //product back in stock tokens
                _allowedTokens.Add(AppPushNotificationTokenGroupNames.ProductBackInStockTokens, new[]
                {
                    "%BackInStockSubscription.ProductName%",
                    "%BackInStockSubscription.ProductUrl%"
                });

                //VAT validation tokens
                _allowedTokens.Add(AppPushNotificationTokenGroupNames.VatValidation, new[]
                {
                    "%VatValidationResult.Name%",
                    "%VatValidationResult.Address%"
                });

                //contact vendor tokens
                _allowedTokens.Add(AppPushNotificationTokenGroupNames.ContactVendor, new[]
                {
                    "%ContactUs.SenderEmail%",
                    "%ContactUs.SenderName%",
                    "%ContactUs.Body%"
                });

                //payout vendor tokens
                _allowedTokens.Add(AppPushNotificationTokenGroupNames.OrderPayoutRequestTokens, new[]
                {
                    "%OrderPayoutRequest.OrderId%",
                    "%OrderPayoutRequest.OrderTotal%",
                    "%OrderPayoutRequest.CreatedOn%"
                });

                return _allowedTokens;
            }
        }

        public IEnumerable<string> GetListOfAllowedTokens(IEnumerable<string> tokenGroups)
        {
            var allowedTokens = AllowedTokens.Where(x => tokenGroups == null || tokenGroups.Contains(x.Key))
                .SelectMany(x => x.Value).ToList();

            return allowedTokens.Distinct();
        }

        public IEnumerable<string> GetTokenGroups(AppPushNotificationTemplate pushNotificationTemplate)
        {
            switch (pushNotificationTemplate?.Name)
            {
                case AppPushNotificationTemplateSystemNames.OrderPaidCustomerNotification:
                case AppPushNotificationTemplateSystemNames.OrderPlacedCustomerNotification:
                case AppPushNotificationTemplateSystemNames.OrderCompletedCustomerNotification:
                case AppPushNotificationTemplateSystemNames.OrderCancelledCustomerNotification:
                    return new[] { AppPushNotificationTokenGroupNames.StoreTokens, AppPushNotificationTokenGroupNames.OrderTokens, AppPushNotificationTokenGroupNames.CustomerTokens };

                case AppPushNotificationTemplateSystemNames.ShipmentSentCustomerNotification:
                case AppPushNotificationTemplateSystemNames.ShipmentDeliveredCustomerNotification:
                    return new[] { AppPushNotificationTokenGroupNames.StoreTokens, AppPushNotificationTokenGroupNames.ShipmentTokens, AppPushNotificationTokenGroupNames.OrderTokens, AppPushNotificationTokenGroupNames.CustomerTokens };

                case AppPushNotificationTemplateSystemNames.OrderRefundedCustomerNotification:
                    return new[] { AppPushNotificationTokenGroupNames.StoreTokens, AppPushNotificationTokenGroupNames.OrderTokens, AppPushNotificationTokenGroupNames.RefundedOrderTokens, AppPushNotificationTokenGroupNames.CustomerTokens };

                case AppPushNotificationTemplateSystemNames.CustomerRegisteredNotification:
                case AppPushNotificationTemplateSystemNames.CustomerWelcomeNotification:
                case AppPushNotificationTemplateSystemNames.CustomerEmailValidationNotification:
                case AppPushNotificationTemplateSystemNames.CustomerRegisteredWelcomeNotification:
                default:
                    return new[] { AppPushNotificationTokenGroupNames.StoreTokens, AppPushNotificationTokenGroupNames.CustomerTokens };
            }
        }

        public virtual void AddCustomerTokens(IList<Token> tokens, Customer customer)
        {
            tokens.Add(new Token("Customer.Email", customer.Email));
            tokens.Add(new Token("Customer.Username", customer.Username));
            tokens.Add(new Token("Customer.FullName", _customerService.GetCustomerFullName(customer)));
            tokens.Add(new Token("Customer.FirstName", _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute)));
            tokens.Add(new Token("Customer.LastName", _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute)));
            tokens.Add(new Token("Customer.VatNumber", _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.VatNumberAttribute)));
            tokens.Add(new Token("Customer.VatNumberStatus", ((VatNumberStatus)_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.VatNumberStatusIdAttribute)).ToString()));

            var customAttributesXml = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CustomCustomerAttributes);
            tokens.Add(new Token("Customer.CustomAttributes", _customerAttributeFormatter.FormatAttributes(customAttributesXml), true));

            //note: we do not use SEO friendly URLS for these links because we can get errors caused by having .(dot) in the URL (from the email address)
            var passwordRecoveryUrl = RouteUrl(routeName: "PasswordRecoveryConfirm", routeValues: new { token = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute), email = customer.Email });
            var accountActivationUrl = RouteUrl(routeName: "AccountActivation", routeValues: new { token = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.AccountActivationTokenAttribute), email = customer.Email });
            var emailRevalidationUrl = RouteUrl(routeName: "EmailRevalidation", routeValues: new { token = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute), email = customer.Email });
            var wishlistUrl = RouteUrl(routeName: "Wishlist", routeValues: new { customerGuid = customer.CustomerGuid });
            tokens.Add(new Token("Customer.PasswordRecoveryURL", passwordRecoveryUrl, true));
            tokens.Add(new Token("Customer.AccountActivationURL", accountActivationUrl, true));
            tokens.Add(new Token("Customer.EmailRevalidationURL", emailRevalidationUrl, true));
            tokens.Add(new Token("Wishlist.URLForCustomer", wishlistUrl, true));

            //event notification
            _eventPublisher.EntityTokensAdded(customer, tokens);
        }

        public virtual void AddStoreTokens(IList<Token> tokens, Store store)
        {
            tokens.Add(new Token("Store.Name", _localizationService.GetLocalized(store, x => x.Name)));
            tokens.Add(new Token("Store.URL", store.Url, true));
            tokens.Add(new Token("Store.CompanyName", store.CompanyName));
            tokens.Add(new Token("Store.CompanyAddress", store.CompanyAddress));
            tokens.Add(new Token("Store.CompanyPhoneNumber", store.CompanyPhoneNumber));
            tokens.Add(new Token("Store.CompanyVat", store.CompanyVat));

            tokens.Add(new Token("Facebook.URL", _storeInformationSettings.FacebookLink));
            tokens.Add(new Token("Twitter.URL", _storeInformationSettings.TwitterLink));
            tokens.Add(new Token("YouTube.URL", _storeInformationSettings.YoutubeLink));

            //event notification
            _eventPublisher.EntityTokensAdded(store, tokens);
        }

        public virtual void AddOrderTokens(IList<Token> tokens, Order order, int languageId, int vendorId = 0)
        {
            var masterOrder = _orderService.GetMasterOrderById(order.MasterOrderId);
            var orderTotalPrice = _priceFormatter.FormatPrice(order.OrderTotal);

            //lambda expression for choosing correct order address
            Address orderAddress(Order o) => _addressService.GetAddressById((o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) ?? 0);

            tokens.Add(new Token("Order.MasterOrderId", masterOrder?.Id ?? 0));
            tokens.Add(new Token("Order.OrderId", order.Id));
            tokens.Add(new Token("Order.OrderNumber", order.CustomOrderNumber));
            tokens.Add(new Token("Order.Price", orderTotalPrice));

            var billingAddress = _addressService.GetAddressById(order.BillingAddressId);
            tokens.Add(new Token("Order.CustomerFullName", $"{billingAddress.FirstName} {billingAddress.LastName}"));
            tokens.Add(new Token("Order.CustomerEmail", billingAddress.Email));

            tokens.Add(new Token("Order.BillingFirstName", billingAddress.FirstName));
            tokens.Add(new Token("Order.BillingLastName", billingAddress.LastName));
            tokens.Add(new Token("Order.BillingPhoneNumber", billingAddress.PhoneNumber));
            tokens.Add(new Token("Order.BillingEmail", billingAddress.Email));
            tokens.Add(new Token("Order.BillingFaxNumber", billingAddress.FaxNumber));
            tokens.Add(new Token("Order.BillingCompany", billingAddress.Company));
            tokens.Add(new Token("Order.BillingAddress1", billingAddress.Address1));
            tokens.Add(new Token("Order.BillingAddress2", billingAddress.Address2));
            tokens.Add(new Token("Order.BillingCity", billingAddress.City));
            tokens.Add(new Token("Order.BillingCounty", billingAddress.County));
            tokens.Add(new Token("Order.BillingStateProvince", _stateProvinceService.GetStateProvinceByAddress(billingAddress) is StateProvince billingStateProvince ? _localizationService.GetLocalized(billingStateProvince, x => x.Name) : string.Empty));
            tokens.Add(new Token("Order.BillingZipPostalCode", billingAddress.ZipPostalCode));
            tokens.Add(new Token("Order.BillingCountry", _countryService.GetCountryByAddress(billingAddress) is Country billingCountry ? _localizationService.GetLocalized(billingCountry, x => x.Name) : string.Empty));
            tokens.Add(new Token("Order.BillingCustomAttributes", _addressAttributeFormatter.FormatAttributes(billingAddress.CustomAttributes), true));

            tokens.Add(new Token("Order.Shippable", !string.IsNullOrEmpty(order.ShippingMethod)));
            tokens.Add(new Token("Order.ShippingMethod", order.ShippingMethod));
            tokens.Add(new Token("Order.PickupInStore", order.PickupInStore));
            tokens.Add(new Token("Order.ShippingFirstName", orderAddress(order)?.FirstName ?? string.Empty));
            tokens.Add(new Token("Order.ShippingLastName", orderAddress(order)?.LastName ?? string.Empty));
            tokens.Add(new Token("Order.ShippingPhoneNumber", orderAddress(order)?.PhoneNumber ?? string.Empty));
            tokens.Add(new Token("Order.ShippingEmail", orderAddress(order)?.Email ?? string.Empty));
            tokens.Add(new Token("Order.ShippingFaxNumber", orderAddress(order)?.FaxNumber ?? string.Empty));
            tokens.Add(new Token("Order.ShippingCompany", orderAddress(order)?.Company ?? string.Empty));
            tokens.Add(new Token("Order.ShippingAddress1", orderAddress(order)?.Address1 ?? string.Empty));
            tokens.Add(new Token("Order.ShippingAddress2", orderAddress(order)?.Address2 ?? string.Empty));
            tokens.Add(new Token("Order.ShippingCity", orderAddress(order)?.City ?? string.Empty));
            tokens.Add(new Token("Order.ShippingCounty", orderAddress(order)?.County ?? string.Empty));
            tokens.Add(new Token("Order.ShippingStateProvince", _stateProvinceService.GetStateProvinceByAddress(orderAddress(order)) is StateProvince shippingStateProvince ? _localizationService.GetLocalized(shippingStateProvince, x => x.Name) : string.Empty));
            tokens.Add(new Token("Order.ShippingZipPostalCode", orderAddress(order)?.ZipPostalCode ?? string.Empty));
            tokens.Add(new Token("Order.ShippingCountry", _countryService.GetCountryByAddress(orderAddress(order)) is Country orderCountry ? _localizationService.GetLocalized(orderCountry, x => x.Name) : string.Empty));
            tokens.Add(new Token("Order.ShippingCustomAttributes", _addressAttributeFormatter.FormatAttributes(orderAddress(order)?.CustomAttributes ?? string.Empty), true));

            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(order.PaymentMethodSystemName);
            var paymentMethodName = paymentMethod != null ? _localizationService.GetLocalizedFriendlyName(paymentMethod, _workContext.WorkingLanguage.Id) : order.PaymentMethodSystemName;
            tokens.Add(new Token("Order.PaymentMethod", paymentMethodName));
            tokens.Add(new Token("Order.VatNumber", order.VatNumber));
            var sbCustomValues = new StringBuilder();
            var customValues = _paymentService.DeserializeCustomValues(order);
            if (customValues != null)
            {
                foreach (var item in customValues)
                {
                    sbCustomValues.AppendFormat("{0}: {1}", WebUtility.HtmlEncode(item.Key), WebUtility.HtmlEncode(item.Value != null ? item.Value.ToString() : string.Empty));
                    sbCustomValues.Append("\\n");
                }
            }

            tokens.Add(new Token("Order.CustomValues", sbCustomValues.ToString(), true));

            var language = _languageService.GetLanguageById(languageId);
            if (language != null && !string.IsNullOrEmpty(language.LanguageCulture))
            {
                var customer = _customerService.GetCustomerById(order.CustomerId);
                var createdOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, TimeZoneInfo.Utc, _dateTimeHelper.GetCustomerTimeZone(customer));
                tokens.Add(new Token("Order.CreatedOn", createdOn.ToString("D", new CultureInfo(language.LanguageCulture))));
            }
            else
            {
                tokens.Add(new Token("Order.CreatedOn", order.CreatedOnUtc.ToString("D")));
            }

            var orderUrl = RouteUrl(order.StoreId, "OrderDetails", new { orderId = order.Id });
            tokens.Add(new Token("Order.OrderURLForCustomer", orderUrl, true));

            var deliveryDate = string.Empty;
            var deliveryTime = string.Empty;
            if (!string.IsNullOrEmpty(order.CheckoutAttributesXml))
            {
                deliveryDate = _shuqOrderService.GetCheckoutDeliveryDate(order.CheckoutAttributesXml);
                deliveryTime = _shuqOrderService.GetCheckoutDeliveryTimeslot(order.CheckoutAttributesXml);
            }
            tokens.Add(new Token("Order.DeliveryDate", deliveryDate));
            tokens.Add(new Token("Order.DeliveryTime", deliveryTime));

            //event notification
            _eventPublisher.EntityTokensAdded(order, tokens);
        }

        protected virtual string RouteUrl(int storeId = 0, string routeName = null, object routeValues = null)
        {
            //try to get a store by the passed identifier
            var store = _storeService.GetStoreById(storeId) ?? _storeContext.CurrentStore
                ?? throw new Exception("No store could be loaded");

            //ensure that the store URL is specified
            if (string.IsNullOrEmpty(store.Url))
                throw new Exception("URL cannot be null");

            //generate a URL with an absolute path
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var url = new PathString(urlHelper.RouteUrl(routeName, routeValues));

            //remove the application path from the generated URL if exists
            var pathBase = _actionContextAccessor.ActionContext?.HttpContext?.Request?.PathBase ?? PathString.Empty;
            url.StartsWithSegments(pathBase, out url);

            //compose the result
            return Uri.EscapeUriString(WebUtility.UrlDecode($"{store.Url.TrimEnd('/')}{url}"));
        }

        public virtual void AddShipmentTokens(IList<Token> tokens, Shipment shipment, int languageId)
        {
            tokens.Add(new Token("Shipment.ShipmentNumber", shipment.Id));
            tokens.Add(new Token("Shipment.TrackingNumber", shipment.TrackingNumber));
            var trackingNumberUrl = string.Empty;
            var vendor = _vendorService.GetVendorByOrderId(shipment.OrderId);
            if (!string.IsNullOrEmpty(shipment.TrackingNumber))
            {
                var shipmentService = EngineContext.Current.Resolve<ShipmentCarrierResolver>();
                var shipmentTracker = shipmentService.ResolveByCourierSetting(vendor);
                if (shipmentTracker != null)
                    trackingNumberUrl = shipmentTracker.GetUrl(shipment.TrackingNumber);
            }

            tokens.Add(new Token("Shipment.TrackingNumberURL", trackingNumberUrl, true));
            var shipmentUrl = RouteUrl(_orderService.GetOrderById(shipment.OrderId).StoreId, "ShipmentDetails", new { shipmentId = shipment.Id });
            tokens.Add(new Token("Shipment.URLForCustomer", shipmentUrl, true));

            //event notification
            _eventPublisher.EntityTokensAdded(shipment, tokens);
        }

        public virtual void AddOrderRefundedTokens(IList<Token> tokens, Order order, decimal refundedAmount)
        {
            //should we convert it to customer currency?
            //most probably, no. It can cause some rounding or legal issues
            //furthermore, exchange rate could be changed
            //so let's display it the primary store currency

            var primaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            var refundedAmountStr = _priceFormatter.FormatPrice(refundedAmount, true, primaryStoreCurrencyCode, false, _workContext.WorkingLanguage.Id);

            tokens.Add(new Token("Order.AmountRefunded", refundedAmountStr));

            //event notification
            _eventPublisher.EntityTokensAdded(order, tokens);
        }

        public virtual void AddOrderNoteTokens(IList<Token> tokens, OrderNote orderNote)
        {
            var order = _orderService.GetOrderById(orderNote.OrderId);

            tokens.Add(new Token("Order.NewNoteText", _orderService.FormatOrderNoteText(orderNote), true));
            var orderNoteAttachmentUrl = RouteUrl(order.StoreId, "GetOrderNoteFile", new { ordernoteid = orderNote.Id });
            tokens.Add(new Token("Order.OrderNoteAttachmentUrl", orderNoteAttachmentUrl, true));

            //event notification
            _eventPublisher.EntityTokensAdded(orderNote, tokens);
        }

        public void AddRecurringPaymentTokens(IList<Token> tokens, RecurringPayment recurringPayment)
        {
            tokens.Add(new Token("RecurringPayment.ID", recurringPayment.Id));
            tokens.Add(new Token("RecurringPayment.CancelAfterFailedPayment",
                recurringPayment.LastPaymentFailed && _paymentSettings.CancelRecurringPaymentsAfterFailedPayment));
            if (_orderService.GetOrderById(recurringPayment.InitialOrderId) is Order order)
                tokens.Add(new Token("RecurringPayment.RecurringPaymentType", _paymentService.GetRecurringPaymentType(order.PaymentMethodSystemName).ToString()));

            //event notification
            _eventPublisher.EntityTokensAdded(recurringPayment, tokens);
        }

        public void AddNewsLetterSubscriptionTokens(IList<Token> tokens, NewsLetterSubscription subscription)
        {
            tokens.Add(new Token("NewsLetterSubscription.Email", subscription.Email));

            var activationUrl = RouteUrl(routeName: "NewsletterActivation", routeValues: new { token = subscription.NewsLetterSubscriptionGuid, active = "true" });
            tokens.Add(new Token("NewsLetterSubscription.ActivationUrl", activationUrl, true));

            var deactivationUrl = RouteUrl(routeName: "NewsletterActivation", routeValues: new { token = subscription.NewsLetterSubscriptionGuid, active = "false" });
            tokens.Add(new Token("NewsLetterSubscription.DeactivationUrl", deactivationUrl, true));

            //event notification
            _eventPublisher.EntityTokensAdded(subscription, tokens);
        }

        public void AddProductTokens(IList<Token> tokens, Product product, int languageId)
        {
            var productService = EngineContext.Current.Resolve<IProductService>();
            tokens.Add(new Token("Product.ID", product.Id));
            tokens.Add(new Token("Product.Name", _localizationService.GetLocalized(product, x => x.Name, languageId)));
            tokens.Add(new Token("Product.ShortDescription", _localizationService.GetLocalized(product, x => x.ShortDescription, languageId), true));
            tokens.Add(new Token("Product.SKU", product.Sku));
            tokens.Add(new Token("Product.StockQuantity", productService.GetTotalStockQuantity(product)));

            var productUrl = RouteUrl(routeName: "Product", routeValues: new { SeName = _urlRecordService.GetSeName(product) });

            //event notification
            _eventPublisher.EntityTokensAdded(product, tokens);
        }

        public void AddReturnRequestTokens(IList<Token> tokens, ReturnRequest returnRequest, OrderItem orderItem)
        {
            var product = _productService.GetProductById(orderItem.ProductId);

            tokens.Add(new Token("ReturnRequest.CustomNumber", returnRequest.CustomNumber));
            tokens.Add(new Token("ReturnRequest.OrderId", orderItem.OrderId));
            tokens.Add(new Token("ReturnRequest.Product.Quantity", returnRequest.Quantity));
            tokens.Add(new Token("ReturnRequest.Product.Name", product.Name));
            tokens.Add(new Token("ReturnRequest.Reason", returnRequest.ReasonForReturn));
            tokens.Add(new Token("ReturnRequest.RequestedAction", returnRequest.RequestedAction));
            tokens.Add(new Token("ReturnRequest.CustomerComment", HtmlHelper.FormatText(returnRequest.CustomerComments, false, true, false, false, false, false), true));
            tokens.Add(new Token("ReturnRequest.StaffNotes", HtmlHelper.FormatText(returnRequest.StaffNotes, false, true, false, false, false, false), true));
            tokens.Add(new Token("ReturnRequest.Status", _localizationService.GetLocalizedEnum(returnRequest.ReturnRequestStatus)));

            //event notification
            _eventPublisher.EntityTokensAdded(returnRequest, tokens);
        }
        
        public void AddOrderSettingsTokens(IList<Token> tokens)
        {
            tokens.Add(new Token("OrderSettings.DaysForSellerToRespondReturnRequest", _orderSettings.DaysForSellerToRespondReturnRequest));
            tokens.Add(new Token("OrderSettings.DaysForSellerToShipOrder", _orderSettings.DaysForSellerToShipOrder));
            tokens.Add(new Token("OrderSettings.DaysForBuyerToShipOrder", _orderSettings.DaysForSellerToShipOrder));
            tokens.Add(new Token("OrderSettings.DaysForSellerToCompleteInspection", _orderSettings.DaysForSellerToCompleteInspection));
            tokens.Add(new Token("OrderSettings.NumberOfDaysReturnRequestAvailable", _orderSettings.NumberOfDaysReturnRequestAvailable));
        }

        public void AddDisputeTokens(List<Token> tokens, Dispute dispute)
        {
            var partialAmountPrice = dispute.PartialAmount != null ? _priceFormatter.FormatPrice(dispute.PartialAmount.Value) : string.Empty;

            tokens.Add(new Token("Dispute.PartialAmount", partialAmountPrice));
            tokens.Add(new Token("Dispute.Reason", dispute.DisputeReason));
        }

        public void AddForumTopicTokens(IList<Token> tokens, ForumTopic forumTopic,
            int? friendlyForumTopicPageIndex = null, int? appendedPostIdentifierAnchor = null)
        {
            var forumService = EngineContext.Current.Resolve<IForumService>();
            string topicUrl;
            if (friendlyForumTopicPageIndex.HasValue && friendlyForumTopicPageIndex.Value > 1)
                topicUrl = RouteUrl(routeName: "TopicSlugPaged", routeValues: new { id = forumTopic.Id, slug = forumService.GetTopicSeName(forumTopic), pageNumber = friendlyForumTopicPageIndex.Value });
            else
                topicUrl = RouteUrl(routeName: "TopicSlug", routeValues: new { id = forumTopic.Id, slug = forumService.GetTopicSeName(forumTopic) });
            if (appendedPostIdentifierAnchor.HasValue && appendedPostIdentifierAnchor.Value > 0)
                topicUrl = $"{topicUrl}#{appendedPostIdentifierAnchor.Value}";
            tokens.Add(new Token("Forums.TopicURL", topicUrl, true));
            tokens.Add(new Token("Forums.TopicName", forumTopic.Subject));

            //event notification
            _eventPublisher.EntityTokensAdded(forumTopic, tokens);
        }

        public void AddForumTokens(IList<Token> tokens, Forum forum)
        {
            var forumService = EngineContext.Current.Resolve<IForumService>();
            var forumUrl = RouteUrl(routeName: "ForumSlug", routeValues: new { id = forum.Id, slug = forumService.GetForumSeName(forum) });
            tokens.Add(new Token("Forums.ForumURL", forumUrl, true));
            tokens.Add(new Token("Forums.ForumName", forum.Name));

            //event notification
            _eventPublisher.EntityTokensAdded(forum, tokens);
        }

        public void AddAttributeCombinationTokens(IList<Token> tokens, ProductAttributeCombination combination, int languageId)
        {
            var product = _productService.GetProductById(combination.ProductId);

            var productAttributeFormatter = EngineContext.Current.Resolve<IProductAttributeFormatter>();
            var productService = EngineContext.Current.Resolve<IProductService>();
            var attributes = productAttributeFormatter.FormatAttributes(product,
                combination.AttributesXml,
                _workContext.CurrentCustomer,
                renderPrices: false);

            tokens.Add(new Token("AttributeCombination.Formatted", attributes, true));
            tokens.Add(new Token("AttributeCombination.SKU", productService.FormatSku(product, combination.AttributesXml)));
            tokens.Add(new Token("AttributeCombination.StockQuantity", combination.StockQuantity));

            //event notification
            _eventPublisher.EntityTokensAdded(combination, tokens);
        }

        public void AddBackInStockTokens(IList<Token> tokens, BackInStockSubscription subscription)
        {
            var product = _productService.GetProductById(subscription.ProductId);

            tokens.Add(new Token("BackInStockSubscription.ProductName", product.Name));
            var productUrl = RouteUrl(subscription.StoreId, "Product", new { SeName = _urlRecordService.GetSeName(product) });
            tokens.Add(new Token("BackInStockSubscription.ProductUrl", productUrl, true));

            //event notification
            _eventPublisher.EntityTokensAdded(subscription, tokens);
        }

        public void AddForumPostTokens(IList<Token> tokens, ForumPost forumPost)
        {
            var customer = _customerService.GetCustomerById(forumPost.CustomerId);

            var forumService = EngineContext.Current.Resolve<IForumService>();
            tokens.Add(new Token("Forums.PostAuthor", _customerService.FormatUsername(customer)));
            tokens.Add(new Token("Forums.PostBody", forumService.FormatPostText(forumPost), true));

            //event notification
            _eventPublisher.EntityTokensAdded(forumPost, tokens);
        }

        public void AddPrivateMessageTokens(IList<Token> tokens, PrivateMessage privateMessage)
        {
            var forumService = EngineContext.Current.Resolve<IForumService>();
            tokens.Add(new Token("PrivateMessage.Subject", privateMessage.Subject));
            tokens.Add(new Token("PrivateMessage.Text", forumService.FormatPrivateMessageText(privateMessage), true));

            //event notification
            _eventPublisher.EntityTokensAdded(privateMessage, tokens);
        }

        public void AddVendorTokens(IList<Token> tokens, Vendor vendor)
        {
            tokens.Add(new Token("Vendor.Name", vendor.Name));
            tokens.Add(new Token("Vendor.Email", vendor.Email));

            var vendorAttributesXml = _genericAttributeService.GetAttribute<string>(vendor, NopVendorDefaults.VendorAttributes);
            tokens.Add(new Token("Vendor.VendorAttributes", _vendorAttributeFormatter.FormatAttributes(vendorAttributesXml), true));

            //event notification
            _eventPublisher.EntityTokensAdded(vendor, tokens);
        }

        public void AddGiftCardTokens(IList<Token> tokens, GiftCard giftCard)
        {
            tokens.Add(new Token("GiftCard.SenderName", giftCard.SenderName));
            tokens.Add(new Token("GiftCard.SenderEmail", giftCard.SenderEmail));
            tokens.Add(new Token("GiftCard.RecipientName", giftCard.RecipientName));
            tokens.Add(new Token("GiftCard.RecipientEmail", giftCard.RecipientEmail));
            tokens.Add(new Token("GiftCard.Amount", _priceFormatter.FormatPrice(giftCard.Amount, true, false)));
            tokens.Add(new Token("GiftCard.CouponCode", giftCard.GiftCardCouponCode));

            var giftCardMesage = !string.IsNullOrWhiteSpace(giftCard.Message) ?
                HtmlHelper.FormatText(giftCard.Message, false, true, false, false, false, false) : string.Empty;

            tokens.Add(new Token("GiftCard.Message", giftCardMesage, true));

            //event notification
            _eventPublisher.EntityTokensAdded(giftCard, tokens);
        }

        public void AddProductReviewTokens(IList<Token> tokens, ProductReview productReview)
        {
            var product = _productService.GetProductById(productReview.ProductId);

            tokens.Add(new Token("ProductReview.ProductName", product.Name));
            tokens.Add(new Token("ProductReview.Title", productReview.Title));
            tokens.Add(new Token("ProductReview.IsApproved", productReview.IsApproved));
            tokens.Add(new Token("ProductReview.ReviewText", productReview.ReviewText));
            tokens.Add(new Token("ProductReview.ReplyText", productReview.ReplyText));

            //event notification
            _eventPublisher.EntityTokensAdded(productReview, tokens);
        }

        public void AddOrderPayoutRequest(List<Token> tokens, OrderPayoutRequest orderPayoutRequest, int languageId)
        {
            var orderTotalPrice = _priceFormatter.FormatPrice(orderPayoutRequest.OrderTotal);
            var order = _orderService.GetOrderById(orderPayoutRequest.OrderId);
            var language = _languageService.GetLanguageById(languageId);
            if (language != null && !string.IsNullOrEmpty(language.LanguageCulture))
            {
                var customer = _customerService.GetCustomerById(order.CustomerId);
                var createdOn = _dateTimeHelper.ConvertToUserTime(orderPayoutRequest.CreatedOnUTC, TimeZoneInfo.Utc, _dateTimeHelper.GetCustomerTimeZone(customer));
                tokens.Add(new Token("OrderPayoutRequest.CreatedOn", createdOn.ToString("D", new CultureInfo(language.LanguageCulture))));
            }
            else
            {
                tokens.Add(new Token("OrderPayoutRequest.CreatedOn", orderPayoutRequest.CreatedOnUTC.ToString("D")));
            }

            tokens.Add(new Token("OrderPayoutRequest.OrderId", orderPayoutRequest.OrderId));
            tokens.Add(new Token("OrderPayoutRequest.Price​", orderTotalPrice));
        }

        public void AddPayoutTokens(List<Token> tokens, PayoutAndGroupShuqDTO payoutAndGroupDTO)
        {
            tokens.Add(new Token("PayoutGroup.PayoutGroupId​", payoutAndGroupDTO.PayoutGroupId));
        }
    }
}
