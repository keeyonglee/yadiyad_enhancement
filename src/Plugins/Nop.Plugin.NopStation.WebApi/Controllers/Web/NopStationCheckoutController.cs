using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Factories;
using System;
using System.Linq;
using Nop.Web.Controllers;
using Nop.Plugin.NopStation.WebApi.Domains;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.NopStation.WebApi.Models.Checkout;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Services.Customers;
using Nop.Plugin.NopStation.Core.Infrastructure;

namespace Nop.Plugin.NopStation.WebApi.Controllers.Web
{
    [NopStationPublicLicense]
    public class NopStationCheckoutController : BasePublicController
    {
        #region Fields

        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public NopStationCheckoutController(ICheckoutModelFactory checkoutModelFactory,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService,
            IPaymentPluginManager paymentPluginManager,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IWorkContext workContext,
            OrderSettings orderSettings,
            PaymentSettings paymentSettings,
            IOrderService  orderService,
            IPaymentService paymentService,
            IWebHelper webHelper,
            ICustomerService customerService)
        {
            _checkoutModelFactory = checkoutModelFactory;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _orderProcessingService = orderProcessingService;
            _paymentPluginManager = paymentPluginManager;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _paymentSettings = paymentSettings;
            _orderService = orderService;
            _paymentService = paymentService;
            _webHelper = webHelper;
            _customerService = customerService;
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

        #endregion

        #region Methods

        public IActionResult Step(int nextStep)
        {
            return Content("");
        }

        public IActionResult PaymentInfo()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("NopStationCheckoutStep", new { nextStep = (int)OpcStep.CartPage });

            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id, onlySelectedItems: true);

            if (!cart.Any())
                return RedirectToRoute("NopStationCheckoutStep", new { nextStep = (int)OpcStep.CartPage });

            if (_customerService.IsGuest(_workContext.CurrentCustomer) && !_orderSettings.AnonymousCheckoutAllowed)
                return RedirectToRoute("NopStationCheckoutStep", new { nextStep = (int)OpcStep.CartPage });

            //Check whether payment workflow is required
            var isPaymentWorkflowRequired = _orderProcessingService.IsPaymentWorkflowRequired(cart);
            if (!isPaymentWorkflowRequired)
            {
                return RedirectToRoute("NopStationCheckoutStep", new { nextStep = (int)OpcStep.ConfirmOrder });
            }

            //load payment method
            var paymentMethodSystemName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);
            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(paymentMethodSystemName);
            if (paymentMethod == null)
                return RedirectToRoute("NopStationCheckoutStep", new { nextStep = (int)OpcStep.PaymentMethod });

            //Check whether payment info should be skipped
            if (paymentMethod.SkipPaymentInfo ||
                (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection && _paymentSettings.SkipPaymentInfoStepForRedirectionPaymentMethods))
            {
                //skip payment info page
                var paymentInfo = new ProcessPaymentRequest();

                //session save
                _genericAttributeService.SavePaymentRequestAttribute(_workContext.CurrentCustomer, paymentInfo, _storeContext.CurrentStore.Id);

                return RedirectToRoute("NopStationCheckoutStep", new { nextStep = (int)OpcStep.ConfirmOrder });
            }

            //model
            var model = _checkoutModelFactory.PreparePaymentInfoModel(paymentMethod);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult PaymentInfo(IFormCollection form)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("NopStationCheckoutStep", new { nextStep = (int)OpcStep.CartPage });

            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id, onlySelectedItems:true);

            if (!cart.Any())
                return RedirectToRoute("NopStationCheckoutStep", new { nextStep = (int)OpcStep.CartPage });

            if (_customerService.IsGuest(_workContext.CurrentCustomer) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //Check whether payment workflow is required
            var isPaymentWorkflowRequired = _orderProcessingService.IsPaymentWorkflowRequired(cart);
            if (!isPaymentWorkflowRequired)
            {
                return RedirectToRoute("NopStationCheckoutStep", new { nextStep = (int)OpcStep.ConfirmOrder });
            }

            //load payment method
            var paymentMethodSystemName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);
            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(paymentMethodSystemName);
            if (paymentMethod == null)
                return RedirectToRoute("NopStationCheckoutStep", new { nextStep = (int)OpcStep.PaymentMethod });

            var warnings = paymentMethod.ValidatePaymentForm(form);
            foreach (var warning in warnings)
                ModelState.AddModelError("", warning);
            if (ModelState.IsValid)
            {
                //get payment info
                var paymentInfo = paymentMethod.GetPaymentInfo(form);
                //set previous order GUID (if exists)
                GenerateOrderGuid(paymentInfo);
                //session save
                _genericAttributeService.SavePaymentRequestAttribute(_workContext.CurrentCustomer, paymentInfo, _storeContext.CurrentStore.Id);

                return RedirectToRoute("NopStationCheckoutStep", new { nextStep = (int)OpcStep.ConfirmOrder });
            }

            //If we got this far, something failed, redisplay form
            //model
            var model = _checkoutModelFactory.PreparePaymentInfoModel(paymentMethod);
            return View(model);
        }

        public IActionResult Redirect(int? orderId)
        {
            //validation
            if (_customerService.IsGuest(_workContext.CurrentCustomer) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();
            Order order;
            //get the order
            if (orderId.HasValue)
            {
                order = _orderService.GetOrderById(orderId.Value);
            }
            else
            {
                order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                    customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                .FirstOrDefault();
            }

            if (order == null)
                return RedirectToRoute("Homepage");

            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(order.PaymentMethodSystemName);
            if (paymentMethod == null)
                return RedirectToRoute("Homepage");
            if (paymentMethod.PaymentMethodType != PaymentMethodType.Redirection)
                return RedirectToRoute("Homepage");

            //ensure that order has been just placed
            if (orderId == null && (DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes > 3)
                return RedirectToRoute("Homepage");

            //Redirection will not work on one page checkout page because it's AJAX request.
            //That's why we process it here
            var postProcessPaymentRequest = new PostProcessPaymentRequest
            {
                Order = order
            };

            _paymentService.PostProcessPayment(postProcessPaymentRequest);

            if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
            {
                //redirection or POST has been done in PostProcessPayment
                return Content("Redirected");
            }

            //if no redirection has been done (to a third-party payment page)
            //theoretically it's not possible
            return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
        }

        #endregion
    }
}
