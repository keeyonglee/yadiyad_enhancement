using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.IPay88.Controllers;
using Nop.Plugin.Payments.IPay88.Domain;
using Nop.Plugin.Payments.IPay88.Models;
using Nop.Plugin.Payments.IPay88.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Tax;
using Newtonsoft.Json;
using Nop.Plugin.Payments.IPay88.Factories;

namespace Nop.Plugin.Payments.IPay88
{
    /// <summary>
    /// IPay88 payment processor
    /// </summary>
    public class IPay88PaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Constants

        /// <summary>
        /// nopCommerce partner code
        /// </summary>
        private const string BN_CODE = "nopCommerce_SP";

        #endregion

        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ISettingService _settingService;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly IPay88PaymentSettings _iPay88PaymentSettings;
        private readonly IIPay88Service _iPay88Service;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IPay88PaymentModelFactory _iPay88PaymentModelFactory;

        #endregion

        #region Ctor

        public IPay88PaymentProcessor(CurrencySettings currencySettings,
            IHttpContextAccessor httpContextAccessor,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ISettingService settingService,
            ITaxService taxService,
            IWebHelper webHelper,
            IPay88PaymentSettings iPay88PaymentSettings,
            IIPay88Service iPay88Service,
            IWorkContext workContext,
            IStoreContext storeContext,
            IPay88PaymentModelFactory iPay88PaymentModelFactory)
        {
            this._currencySettings = currencySettings;
            this._httpContextAccessor = httpContextAccessor;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._currencyService = currencyService;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._settingService = settingService;
            this._taxService = taxService;
            this._webHelper = webHelper;
            this._iPay88PaymentSettings = iPay88PaymentSettings;
            this._iPay88Service = iPay88Service;
            this._workContext = workContext;
            this._storeContext = storeContext;
            _iPay88PaymentModelFactory = iPay88PaymentModelFactory;
            //this._objectContext = objectContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets IPay88 Payment URL
        /// </summary>
        /// <returns></returns>
        private string GetPaymentUrl()
        {
            return _iPay88PaymentSettings.PaymentUrl;
        }

        private string GeneratePaymentNo()
        {
            // format eg. PAY150703541000001
            // PAY: prefix
            // 150703: yyMMdd
            // 354: random number, prevent duplicate order no
            // 000001: total payment count
            try
            {
                string paymentPrefix = _iPay88PaymentSettings.PaymentPrefix;
                string today = DateTime.Now.ToString("yyMMdd");

                using var random = new SecureRandomNumberGenerator();
                String randomNum = random.Next(0, 999).ToString("000");

                string strPaymentCount = "";
                int iPay88PaymentsCount = _iPay88Service.GetAll().Count + 1;
                strPaymentCount = iPay88PaymentsCount.ToString("000000");

                return paymentPrefix + today + randomNum + strPaymentCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GenerateSHA256SecureSignature(string inputString)
        {
            var sha256 = SHA256Managed.Create();
            Byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            Byte[] hash = sha256.ComputeHash(bytes);

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                stringBuilder.Append(hash[i].ToString("X2"));
            }

            return stringBuilder.ToString().ToLower();
        }

        private IPay88PaymentRecord PrepareIPay88PaymentRecord(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            bool isYadiyadPro = postProcessPaymentRequest.CustomOrder != null;

            //Save Payment Details
            IPay88PaymentRecord iPay88PaymentRecord = new IPay88PaymentRecord();
            iPay88PaymentRecord.PaymentNo = GeneratePaymentNo();
            iPay88PaymentRecord.Amount = isYadiyadPro ? postProcessPaymentRequest.CustomOrder.OrderTotal :  postProcessPaymentRequest.MasterOrder.OrderTotal;
            iPay88PaymentRecord.UniqueId = isYadiyadPro ? postProcessPaymentRequest.CustomOrder.Id : postProcessPaymentRequest.MasterOrder.Id;
            iPay88PaymentRecord.CurrencyCode = _iPay88PaymentSettings.CurrencyCode;//postProcessPaymentRequest.Order.CustomerCurrencyCode;
            iPay88PaymentRecord.ProdDesc = isYadiyadPro ? "Yadiyad Service" : "Products";
            iPay88PaymentRecord.UserName = GetFullName(_workContext.CurrentCustomer);
            iPay88PaymentRecord.UserEmail = _workContext.CurrentCustomer.Email;
            iPay88PaymentRecord.UserContact = GetPhoneNumber(_workContext.CurrentCustomer);// _workContext.CurrentCustomer.GetAttribute<>
            iPay88PaymentRecord.StoreId = _storeContext.CurrentStore.Id;
            iPay88PaymentRecord.CreatedBy = _workContext.CurrentCustomer.Id;
            iPay88PaymentRecord.CreatedDateTime = DateTime.UtcNow;
            iPay88PaymentRecord.OrderType = isYadiyadPro ? OrderType.Pro : OrderType.Shuq;
            iPay88PaymentRecord.Status = IPay88Helper.PaymentStatusConstants.PENDING;

            //IMPORTANT: If debug mode, payment amount will always be RM1.00
            bool isDebugPaymentGateway = _iPay88PaymentSettings.IsEnableTestPayment;
            if (isDebugPaymentGateway && iPay88PaymentRecord.Amount > 0)
            {
                iPay88PaymentRecord.Amount = 1.00m;
            }

            string strAmountClear = iPay88PaymentRecord.Amount.ToString("0.00").Replace(",", string.Empty).Replace(".", string.Empty);
            string strSign = _iPay88PaymentSettings.MerchantKey + _iPay88PaymentSettings.MerchantCode + iPay88PaymentRecord.PaymentNo + strAmountClear + iPay88PaymentRecord.CurrencyCode;
            iPay88PaymentRecord.Signature = GenerateSHA256SecureSignature(strSign);
            iPay88PaymentRecord.SignatureType = "SHA256";

            return iPay88PaymentRecord;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();

            result.NewPaymentStatus = PaymentStatus.Pending;

            return result;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            _httpContextAccessor.HttpContext.Session.SetString("IPay88PostPaymentModel", JsonConvert.SerializeObject(null));
            
            var iPay88PaymentRecord = PrepareIPay88PaymentRecord(postProcessPaymentRequest);

            _iPay88Service.InsertIPay88PaymentRecord(iPay88PaymentRecord);

            IPay88PostPaymentModel iPay88PostPaymentModel = _iPay88PaymentModelFactory.GetIPay88PostPaymentModel(iPay88PaymentRecord);
            _httpContextAccessor.HttpContext.Session.SetString("IPay88PostPaymentModel", JsonConvert.SerializeObject(iPay88PostPaymentModel));

            string redirectPaymentUrl = null;
            bool isDebugPaymentGateway = _iPay88PaymentSettings.IsEnableTestPayment;
            if (isDebugPaymentGateway)
            {
                redirectPaymentUrl = _webHelper.GetStoreLocation(false) + "Plugins/PaymentIPay88/RedirectToPaymentUrl";
            }
            else
            {
                redirectPaymentUrl = _webHelper.GetStoreLocation(true) + "Plugins/PaymentIPay88/RedirectToPaymentUrl";
            }

            _httpContextAccessor.HttpContext.Response.Redirect(redirectPaymentUrl);
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            //var result = this.CalculateAdditionalFee(_orderTotalCalculationService, cart,
            //    _iPay88PaymentSettings.AdditionalFee, _iPay88PaymentSettings.AdditionalFeePercentage);
            //return result;
            return decimal.Zero;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return result;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return result;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");


            return false;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentIPay88";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.IPay88.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for payment info
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentIPay88";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.IPay88.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Get type of controller
        /// </summary>
        /// <returns>Type</returns>
        public Type GetControllerType()
        {
            return typeof(PaymentIPay88Controller);
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentIPay88/Configure";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new IPay88PaymentSettings
            {
                PaymentUrl = "https://payment.ipay88.com.my/epayment/entry.asp",
                PaymentRequeryUrl = "https://payment.ipay88.com.my/epayment/enquiry.asp",
                PaymentPrefix = "PAY",
                InvoicePrefix = "INV",
                MerchantCode = "",
                MerchantKey = "",
                ProductDesc = "Product(s)",
                CurrencyCode = "MYR",
                PaymentId = 538, //Paypal
                IsEnableTestPayment = true,
            });

            //locales
            _localizationService.AddPluginLocaleResource(new Dictionary<string, string>
            {
                ["Plugins.Payments.IPay88.PaymentMethodDescription"] = "Credit or Debit Card",
                ["Plugins.Payments.IPay88.Fields.MerchantCode"] = "Merchant Code",
                ["Plugins.Payments.IPay88.Fields.MerchantKey"] = "Merchant Key",
                ["Plugins.Payments.IPay88.Fields.CurrencyCode"] = "Currency Code",
                ["Plugins.Payments.IPay88.Fields.ProductDesc"] = "Product Description",
                ["Plugins.Payments.IPay88.Fields.PaymentId"] = "Payment Id",
                ["Plugins.Payments.IPay88.Fields.ProxyPaymentURL"] = "Proxy Payment URL"
            });

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<IPay88PaymentSettings>();

            //locales
            _localizationService.DeletePluginLocaleResources("Plugins.Payments.IPay88");

            base.Uninstall();
        }

#endregion

        /// <summary>
        /// Get full name
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>Customer full name</returns>
        private string GetFullName(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");
            var firstName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute);
            var lastName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute);

            string fullName = "";
            if (!String.IsNullOrWhiteSpace(firstName) && !String.IsNullOrWhiteSpace(lastName))
                fullName = string.Format("{0} {1}", firstName, lastName);
            else
            {
                if (!String.IsNullOrWhiteSpace(firstName))
                    fullName = firstName;

                if (!String.IsNullOrWhiteSpace(lastName))
                    fullName = lastName;
            }
            return fullName;
        }

        /// <summary>
        /// Get Phone number
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>Customer Phone number</returns>
        private string GetPhoneNumber(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");
            var phone = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute);

            return phone;
        }


#region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get { return RecurringPaymentType.NotSupported; }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get { return PaymentMethodType.Redirection; }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        public string PaymentMethodDescription
        {
            //return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
            //for example, for a redirection payment method, description may be like this: "You will be redirected to PayPal site to complete the payment"
            get { return _localizationService.GetResource("Plugins.Payments.IPay88.PaymentMethodDescription"); }
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>List of validating errors</returns>
        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            return new List<string>();
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>Payment info holder</returns>
        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            return new ProcessPaymentRequest();
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <returns>View component name</returns>
        public string GetPublicViewComponentName()
        {
            return "PaymentIPay88";
        }

#endregion

    }
}