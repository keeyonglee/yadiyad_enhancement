using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.IPay88.Models;
using Nop.Plugin.Payments.IPay88.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading;
using Microsoft.Extensions.Primitives;
using Nop.Plugin.Payments.IPay88.Factories;
using Nop.Services.Customers;

namespace Nop.Plugin.Payments.IPay88.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class PaymentIPay88Controller : BasePaymentController
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly YadiYad.Pro.Services.Order.OrderService _orderService_Pro;
        private readonly YadiYad.Pro.Services.Order.OrderProcessingService  _orderProcessingService_Pro;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IPay88PaymentSettings _iPay88PaymentSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IIPay88Service _iPay88Service;
        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPay88PaymentModelFactory _iPay88PaymentModelFactory;

        #endregion

        #region Ctor

        public PaymentIPay88Controller(IGenericAttributeService genericAttributeService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            YadiYad.Pro.Services.Order.OrderService orderService_Pro,
            YadiYad.Pro.Services.Order.OrderProcessingService orderProcessingService_Pro,
            IPaymentPluginManager paymentPluginManager,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ILogger logger,
            INotificationService notificationService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext,
            IPay88PaymentSettings iPay88PaymentSettings,
            ShoppingCartSettings shoppingCartSettings,
            IIPay88Service iPay88Service,
            ICustomerService customerService,
            IShoppingCartService shoppingCartService,
            IPay88PaymentModelFactory iPay88PaymentModelFactory)
        {
            _genericAttributeService = genericAttributeService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _orderService_Pro = orderService_Pro;
            _orderProcessingService_Pro = orderProcessingService_Pro;
            _paymentPluginManager = paymentPluginManager;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _logger = logger;
            _notificationService = notificationService;
            _settingService = settingService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
            _iPay88PaymentSettings = iPay88PaymentSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _iPay88Service = iPay88Service;
            _customerService = customerService;
            _shoppingCartService = shoppingCartService;
            _iPay88PaymentModelFactory = iPay88PaymentModelFactory;
        }

        #endregion

        #region Utilities

        [NonAction]
        private string GetQueryIpayStatus(string merchantCode, string refNo, string amount)
        {
            var request = (HttpWebRequest)WebRequest.Create(_iPay88PaymentSettings.PaymentRequeryUrl);

            var postData = "MerchantCode=" + merchantCode + "&RefNo=" + refNo + "&Amount=" + amount;
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseString;
        }

        [NonAction]
        private bool verifyIsPaymentSucceed(IPay88PaymentResponseModel model)
        {
            bool isPaymentSucceed = false;

            if (model.Status == "1")//Payment Succeed
            {
                var paymentRecord = _iPay88Service.FindRecord(model.RefNo);

                if (paymentRecord == null)
                {
                    return isPaymentSucceed;
                }

                string ipayStatus = GetQueryIpayStatus(model.MerchantCode, model.RefNo, model.Amount);
                _logger.InsertLog(LogLevel.Information, "IPay88 Payment Result Query Status", ipayStatus);
                if (ipayStatus == "00")//Requery Succeed
                {
                    isPaymentSucceed = true;
                }
            }

            return isPaymentSucceed;
        }

        [NonAction]
        private IPay88PostPaymentModel GetIPay88PostPaymentModel(int? orderId, int? orderTypeId)
        {
            var ipay88Data = HttpContext.Session.GetString("IPay88PostPaymentModel");
            
            if(!string.IsNullOrWhiteSpace(ipay88Data) && ipay88Data.Trim().ToLower() != "null")
                return JsonConvert.DeserializeObject<IPay88PostPaymentModel>(ipay88Data);
            
            // Session usage wont work for mobile api
            // Get Payment Record using OrderId
            if(!orderId.HasValue || !orderTypeId.HasValue)
                return default;
                
            var iPay88PaymentRecord = _iPay88Service.GetByOrderId(orderId.GetValueOrDefault(), orderTypeId.GetValueOrDefault());
            return _iPay88PaymentModelFactory.GetIPay88PostPaymentModel(iPay88PaymentRecord);
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var iPay88PaymentSettings = _settingService.LoadSetting<IPay88PaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                MerchantCode = iPay88PaymentSettings.MerchantCode,
                MerchantKey = iPay88PaymentSettings.MerchantKey,
                CurrencyCode = iPay88PaymentSettings.CurrencyCode,
                ProductDesc = iPay88PaymentSettings.ProductDesc,
                ProxyPaymentURL = iPay88PaymentSettings.ProxyPaymentURL,
                PaymentUrl = iPay88PaymentSettings.PaymentUrl,
                PaymentRequeryUrl = iPay88PaymentSettings.PaymentRequeryUrl,
                InvoicePrefix = iPay88PaymentSettings.InvoicePrefix,
                PaymentPrefix = iPay88PaymentSettings.PaymentPrefix,

                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.MerchantCode_OverrideForStore = _settingService.SettingExists(iPay88PaymentSettings, x => x.MerchantCode, storeScope);
                model.MerchantKey_OverrideForStore = _settingService.SettingExists(iPay88PaymentSettings, x => x.MerchantKey, storeScope);
                model.CurrencyCode_OverrideForStore = _settingService.SettingExists(iPay88PaymentSettings, x => x.CurrencyCode, storeScope);
                model.ProductDesc_OverrideForStore = _settingService.SettingExists(iPay88PaymentSettings, x => x.ProductDesc, storeScope);
                model.ProxyPaymentURL_OverrideForStore = _settingService.SettingExists(iPay88PaymentSettings, x => x.ProxyPaymentURL, storeScope);
                model.PaymentUrl_OverrideForStore = _settingService.SettingExists(iPay88PaymentSettings, x => x.PaymentUrl, storeScope);
                model.PaymentRequeryUrl_OverrideForStore = _settingService.SettingExists(iPay88PaymentSettings, x => x.PaymentRequeryUrl, storeScope);
                model.InvoicePrefix_OverrideForStore = _settingService.SettingExists(iPay88PaymentSettings, x => x.InvoicePrefix, storeScope);
                model.PaymentPrefix_OverrideForStore = _settingService.SettingExists(iPay88PaymentSettings, x => x.PaymentPrefix, storeScope);
            }

            return View("~/Plugins/Payments.IPay88/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var iPay88PaymentSettings = _settingService.LoadSetting<IPay88PaymentSettings>(storeScope);

            //save settings
            iPay88PaymentSettings.MerchantCode = model.MerchantCode;
            iPay88PaymentSettings.MerchantKey = model.MerchantKey;
            iPay88PaymentSettings.CurrencyCode = model.CurrencyCode;
            iPay88PaymentSettings.ProductDesc = model.ProductDesc;
            iPay88PaymentSettings.ProxyPaymentURL = model.ProxyPaymentURL;
            iPay88PaymentSettings.PaymentUrl = model.PaymentUrl;
            iPay88PaymentSettings.PaymentRequeryUrl = model.PaymentRequeryUrl;
            iPay88PaymentSettings.InvoicePrefix = model.InvoicePrefix;
            iPay88PaymentSettings.PaymentPrefix = model.PaymentPrefix;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(iPay88PaymentSettings, x => x.MerchantCode, model.MerchantCode_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(iPay88PaymentSettings, x => x.MerchantKey, model.MerchantKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(iPay88PaymentSettings, x => x.CurrencyCode, model.CurrencyCode_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(iPay88PaymentSettings, x => x.ProductDesc, model.ProductDesc_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(iPay88PaymentSettings, x => x.ProxyPaymentURL, model.ProxyPaymentURL_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(iPay88PaymentSettings, x => x.PaymentUrl, model.PaymentUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(iPay88PaymentSettings, x => x.PaymentRequeryUrl, model.PaymentRequeryUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(iPay88PaymentSettings, x => x.InvoicePrefix, model.InvoicePrefix_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(iPay88PaymentSettings, x => x.PaymentPrefix, model.PaymentPrefix_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        //action displaying notification (warning) to a store owner about inaccurate IPay88 rounding
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult RoundingWarning(bool passProductNamesAndTotals)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //prices and total aren't rounded, so display warning
            if (passProductNamesAndTotals && !_shoppingCartSettings.RoundPricesDuringCalculation)
                return Json(new { Result = _localizationService.GetResource("Plugins.Payments.IPay88.RoundingWarning") });

            return Json(new { Result = string.Empty });
        }

        public IActionResult CancelOrder()
        {
            var order = _orderService.SearchOrders(_storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1).FirstOrDefault();

            if (order != null)
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });

            return RedirectToRoute("Homepage");
        }


        public IActionResult PaymentInfo()
        {
            return View("~/Plugins/Payments.IPay88/Views/PaymentInfo.cshtml");
        }

        [NonAction]
        public IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        [NonAction]
        public ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            return paymentInfo;
        }

        [IgnoreAntiforgeryToken]
        public IActionResult RedirectToPaymentUrl([FromQuery(Name = "o")] int? orderUniqueId, [FromQuery(Name = "ot")] int? orderTypeId)
        {
            var model = GetIPay88PostPaymentModel(orderUniqueId, orderTypeId);
            if (model == null)
            {
                throw new NopException("Payment not found, please try again.");
            }

            //After tested should uncomment
            HttpContext.Session.SetString("IPay88PostPaymentModel", JsonConvert.SerializeObject(null));

            //handle pay amount 0
            if(decimal.Parse(model.Amount) == 0)
            {
                var paymentRecord = _iPay88Service.FindRecord(model.RefNo);
                if (paymentRecord.OrderType == OrderType.Pro)
                {
                    _logger.InsertLog(LogLevel.Information, "skip IPay88 Payment cause by 0 amount", "");
                    // Pro Order
                    var proOrder = _orderService_Pro.GetCustomOrder(paymentRecord.UniqueId);

                    proOrder.OrderStatus = OrderStatus.Complete;
                    proOrder.PaymentStatus = PaymentStatus.Paid;

                    _orderProcessingService_Pro.ProcessOrder(proOrder);

                    _logger.InsertLog(LogLevel.Information, "Redirect to PRO payment response", "");
                    // Pro Order
                    return Redirect($"/api/pro/payment/response?orderId={paymentRecord.UniqueId}&status=1");
                }else if(paymentRecord.OrderType == OrderType.Shuq)
                {
                    //update order
                    var masterOrder = _orderService.GetMasterOrderById(paymentRecord.UniqueId);
                    var orders = _orderService.GetOrdersByMasterOrderId(paymentRecord.UniqueId);

                    if (masterOrder.PaymentStatus == PaymentStatus.Pending)
                    {
                        masterOrder.PaymentStatus = PaymentStatus.Paid;
                        masterOrder.PaidDateUtc = DateTime.UtcNow;
                        _orderService.UpdateMasterOrder(masterOrder);
                    }

                    foreach (var ord in orders)
                    {
                        if (ord.PaymentStatus == PaymentStatus.Pending)
                        {
                            ord.PaymentStatus = PaymentStatus.Paid;
                            //ord.PaidDateUtc = DateTime.UtcNow;
                            _orderService.UpdateOrder(ord);
                        }
                        _orderProcessingService.CheckOrderStatus(ord);
                    }

                    //process order
                    _orderProcessingService.PostPaymentProcessing(masterOrder);

                    return RedirectToRoute("CheckoutCompleted", new { orderId = paymentRecord.UniqueId });
                }
            }

            //proxy redirect
            if (string.IsNullOrWhiteSpace(_iPay88PaymentSettings.ProxyPaymentURL) == false)
            {
                model.PaymentURL = _iPay88PaymentSettings.ProxyPaymentURL;
            }

            return View("~/Plugins/Payments.IPay88/Views/RedirectToPaymentUrl.cshtml", model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult ProxyRedirectToPaymentUrl([FromForm] IPay88PostPaymentModel model = null)
        {
            if (model == null)
            {
                throw new NopException("Payment not found, please try again.");
            }

            //Proxy Payment Handling
            model.ResponseURL = _webHelper.GetStoreLocation(false) + "Plugins/PaymentIPay88/PaymentResult?t=1";
            model.BackendURL = _webHelper.GetStoreLocation(false) + "Plugins/PaymentIPay88/BackendUrl?t=1";
            model.PaymentURL = _iPay88PaymentSettings.PaymentUrl;

            return View("~/Plugins/Payments.IPay88/Views/RedirectToPaymentUrl.cshtml", model);
        }


        [IgnoreAntiforgeryToken]
        public IActionResult PaymentResult(
            IPay88PaymentResponseModel model,
            [FromQuery(Name = "t")] int transactionMode = 0)
        {
            if (transactionMode == 1)
            {
                model.ProxyResponseURL = "http://localhost:15536/Plugins/PaymentIPay88/PaymentResult";
                _logger.InsertLog(LogLevel.Information, "IPay88 Backend Url Proxy Redirection", JsonConvert.SerializeObject(model));
                return View("~/Plugins/Payments.IPay88/Views/ProxyPaymentResult.cshtml", model);
            }

            _logger.InsertLog(LogLevel.Information, "IPay88 PaymentResult", JsonConvert.SerializeObject(model));
            bool isPaymentSucceed = verifyIsPaymentSucceed(model);
            //bool isPaymentSucceed = false;

            // Check payment record is Custom Order

            if (isPaymentSucceed)
            {
                _logger.InsertLog(LogLevel.Information, "IPay88 Payment Success", "");
                // Cannot filter by completed status because it might be delay update from payment gateway
                //var successPaymentRecord = _iPay88Service.FindRecord(model.RefNo, IPay88Helper.PaymentStatusConstants.COMPLETED);

                var successPaymentRecord = _iPay88Service.FindRecord(model.RefNo);

                _logger.InsertLog(LogLevel.Information, "IPay88 Payment succeed record found.", "");
                if (successPaymentRecord.OrderType == OrderType.Pro)
                {
                    _logger.InsertLog(LogLevel.Information, "IPay88 Payment for PRO Order", "");
                    // Pro Order
                    var proOrder = _orderService_Pro.GetCustomOrder(successPaymentRecord.UniqueId);

                    proOrder.OrderStatus = OrderStatus.Complete;
                    proOrder.PaymentStatus = PaymentStatus.Paid;

                    _orderProcessingService_Pro.ProcessOrder(proOrder);

                    _logger.InsertLog(LogLevel.Information, "Redirect to PRO payment response", "");
                    // Pro Order
                    return Redirect($"/api/pro/payment/response?orderId={successPaymentRecord.UniqueId}&status=1");
                }
                else
                {
                    _logger.InsertLog(LogLevel.Information, "IPay88 Payment for SHUQ Order", "");

                    // Shuq Order

                    //update order
                    var masterOrder = _orderService.GetMasterOrderById(successPaymentRecord.UniqueId);
                    var orders = _orderService.GetOrdersByMasterOrderId(successPaymentRecord.UniqueId);

                    if (masterOrder.PaymentStatus == PaymentStatus.Pending)
                    {
                        masterOrder.PaymentStatus = PaymentStatus.Paid;
                        masterOrder.PaidDateUtc = DateTime.UtcNow;
                        _orderService.UpdateMasterOrder(masterOrder);
                    }

                    foreach (var ord in orders)
                    {
                        if (ord.PaymentStatus == PaymentStatus.Pending)
                        {
                            ord.PaymentStatus = PaymentStatus.Paid;
                            //ord.PaidDateUtc = DateTime.UtcNow;
                            _orderService.UpdateOrder(ord);
                        }
                        _orderProcessingService.CheckOrderStatus(ord);
                    }

                    //process order
                    _orderProcessingService.PostPaymentProcessing(masterOrder);

                    return RedirectToRoute("CheckoutCompleted", new { orderId = successPaymentRecord.UniqueId });
                }
            }

            var paymentRecord = _iPay88Service.FindRecord(model.RefNo);
            if (paymentRecord == null)
            {
                throw new NopException("No status payment record found with this payment refNo > " + model.RefNo);
            }

            var resultModel = new IPay88PaymentResultModel()
            {
                OrderId = paymentRecord != null ? paymentRecord.UniqueId : 0,//28
                IsPaymentSucceed = false,
                ResultMessage = "Payment Failure"
            };

            if (paymentRecord != null && paymentRecord.OrderType == OrderType.Pro)
            {
                return Redirect($"/pro/payment/response?orderId={paymentRecord.UniqueId}&status=0");
            }
            else if (paymentRecord != null && paymentRecord.OrderType == OrderType.Shuq)
            {
                ViewBag.Path = "~/Views/";
                ViewBag.Layout = "Shared/_Root.Head.cshtml";
                ViewBag.GotoURL = Url.RouteUrl("CustomerOrders");
                ViewBag.GotoPage = "Order History";
                return View("~/Plugins/Payments.IPay88/Views/PaymentResult.cshtml", resultModel);
            }
            else
            {
                //Payment Response URL
                return View("~/Plugins/Payments.IPay88/Views/PaymentResult.cshtml", resultModel);
            }
        }


        [IgnoreAntiforgeryToken]
        public void BackendUrl(IPay88PaymentResponseModel model,
            [FromQuery(Name = "t")] int transactionMode = 0)
        {
            if(transactionMode == 1)
            {
                _logger.InsertLog(LogLevel.Information, "IPay88 Backend Url Proxy Redirection", JsonConvert.SerializeObject(model));
                return;
            }

            _logger.InsertLog(LogLevel.Information, "IPay88 Backend Url", JsonConvert.SerializeObject(model));
            if (model == null)
            {
                throw new NopException("IPay88PaymentResponseModel is null at iPay88 payment backend Url");
            }

            var paymentRecord = _iPay88Service.FindRecord(model.RefNo, IPay88Helper.PaymentStatusConstants.PENDING);
            if (paymentRecord == null)
            {
                throw new NopException("No pending status payment record found with this payment refNo > " + model.RefNo);
            }

            if (model.Status == "1")//Payment Succeed
            {
                string ipayStatus = GetQueryIpayStatus(model.MerchantCode, model.RefNo, model.Amount);
                _logger.InsertLog(LogLevel.Information, "IPay88 Backend Url Query Status", ipayStatus);
                if (ipayStatus == "00")//Requery Succeed
                {
                    Thread.Sleep(2000);

                    if (paymentRecord.OrderType == OrderType.Pro)
                    {
                        // Pro Order
                        var proOrder = _orderService_Pro.GetCustomOrder(paymentRecord.UniqueId);

                        proOrder.OrderStatus = OrderStatus.Complete;
                        proOrder.PaymentStatus = PaymentStatus.Paid;

                        _orderProcessingService_Pro.ProcessOrder(proOrder);
                    }
                    else
                    {
                        // Shuq Order
                        var masterOrder = _orderService.GetMasterOrderById(paymentRecord.UniqueId);
                        var orders = _orderService.GetOrdersByMasterOrderId(paymentRecord.UniqueId);

                        if (masterOrder.PaymentStatus == PaymentStatus.Pending)
                        {
                            masterOrder.PaymentStatus = PaymentStatus.Paid;
                            masterOrder.PaidDateUtc = DateTime.UtcNow;
                            _orderService.UpdateMasterOrder(masterOrder);
                        }

                        foreach(var ord in orders)
                        {
                            if (ord.PaymentStatus == PaymentStatus.Pending)
                            {
                                ord.PaymentStatus = PaymentStatus.Paid;
                                //ord.PaidDateUtc = DateTime.UtcNow;
                                _orderService.UpdateOrder(ord);
                            }
                            _orderProcessingService.CheckOrderStatus(ord);
                        }
                        _orderProcessingService.PostPaymentProcessing(masterOrder);

                    }

                    paymentRecord.Status = IPay88Helper.PaymentStatusConstants.COMPLETED;
                    _iPay88Service.UpdateIPay88PaymentRecord(paymentRecord);

                    Response.WriteAsync("RECEIVEOK");
                }
                else//Requery failed
                {
                    paymentRecord.ErrorDesc = ipayStatus;
                    paymentRecord.Status = IPay88Helper.PaymentStatusConstants.FAILED;
                    _iPay88Service.UpdateIPay88PaymentRecord(paymentRecord);

                    throw new NopException(string.Format(@"Payment Failure with status(1) and requery status > {0}, details here > {1}", ipayStatus, JsonConvert.SerializeObject(model)));
                }

            }
            else
            {
                //Payment Failure
                throw new NopException("Payment Failure with status(not 1), details here > " + JsonConvert.SerializeObject(model));
            }
        }

        #endregion
    }
}