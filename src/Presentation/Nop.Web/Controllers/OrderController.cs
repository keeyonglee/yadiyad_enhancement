using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Order;
using Nop.Core.Domain.ShippingShuq;
using Nop.Services.ShippingShuq;
using Nop.Services.ShuqOrders;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Common;
using Nop.Services.Vendors;
using Nop.Core.Domain.ShippingShuq.DTO;
using Nop.Web.Models.Vendors;
using Nop.Web.Utilities;
using Nop.Services.Payout;

namespace Nop.Web.Controllers
{
    public partial class OrderController : BasePublicController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IOrderModelFactory _orderModelFactory;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IPdfService _pdfService;
        private readonly IShipmentService _shipmentService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ShipmentCarrierResolver _shipmentCarrierResolver;
        private readonly IShuqOrderService _shuqOrderService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IProductService _productService;
        private readonly IAddressService _addressService;
        private readonly IVendorService _vendorService;
        private readonly IVendorModelFactory _vendorModelFactory;
        private readonly ShippingMethodService _shippingMethodService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly OrderPayoutService _orderPayoutService;

        #endregion

        #region Ctor

        public OrderController(ICustomerService customerService,
            IOrderModelFactory orderModelFactory,
            IOrderProcessingService orderProcessingService, 
            IOrderService orderService, 
            IPaymentService paymentService, 
            IPdfService pdfService,
            IShipmentService shipmentService, 
            IWebHelper webHelper,
            IWorkContext workContext,
            RewardPointsSettings rewardPointsSettings,
            ShipmentCarrierResolver shipmentCarrierResolver,
            IShuqOrderService shuqOrderService,
            IReturnRequestService returnRequestService,
            IProductService productService,
            IAddressService addressService,
            IVendorService vendorService,
            IVendorModelFactory vendorModelFactory,
            ShippingMethodService shippingMethodService,
            ICheckoutAttributeService checkoutAttributeService,
            ICheckoutAttributeParser checkoutAttributeParser,
            OrderPayoutService orderPayoutService)
        {
            _customerService = customerService;
            _orderModelFactory = orderModelFactory;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentService = paymentService;
            _pdfService = pdfService;
            _shipmentService = shipmentService;
            _webHelper = webHelper;
            _workContext = workContext;
            _rewardPointsSettings = rewardPointsSettings;
            _shipmentCarrierResolver = shipmentCarrierResolver;
            _shuqOrderService = shuqOrderService;
            _returnRequestService = returnRequestService;
            _productService = productService;
            _addressService = addressService;
            _vendorService = vendorService;
            _vendorModelFactory = vendorModelFactory;
            _shippingMethodService = shippingMethodService;
            _checkoutAttributeService = checkoutAttributeService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _orderPayoutService = orderPayoutService;
        }

        #endregion

        #region Methods
        [HttpsRequirement]
        public IActionResult OrderCancellation(int orderId)
        {
            var model = new OrderCancellationModel();
            model.OrderId = orderId;

            return View(model);
        }

        //shuq/order/history
        //My account / Orders
        [HttpsRequirement]
        public virtual IActionResult CustomerOrders()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            var model = _orderModelFactory.PrepareCustomerOrderListModel();
            return View("ShuqCustomerOrders", model);
        }

        //My account / Orders / Cancel recurring order
        [HttpPost, ActionName("CustomerOrders")]
        [AutoValidateAntiforgeryToken]
        [FormValueRequired(FormValueRequirement.StartsWith, "cancelRecurringPayment")]
        public virtual IActionResult CancelRecurringPayment(IFormCollection form)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            //get recurring payment identifier
            var recurringPaymentId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("cancelRecurringPayment", StringComparison.InvariantCultureIgnoreCase))
                    recurringPaymentId = Convert.ToInt32(formValue.Substring("cancelRecurringPayment".Length));

            var recurringPayment = _orderService.GetRecurringPaymentById(recurringPaymentId);
            if (recurringPayment == null)
            {
                return RedirectToRoute("CustomerOrders");
            }

            if (_orderProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment))
            {
                var errors = _orderProcessingService.CancelRecurringPayment(recurringPayment);

                var model = _orderModelFactory.PrepareCustomerOrderListModel();
                model.RecurringPaymentErrors = errors;

                return View(model);
            }

            return RedirectToRoute("CustomerOrders");
        }

        //My account / Orders / Retry last recurring order
        [HttpPost, ActionName("CustomerOrders")]
        [AutoValidateAntiforgeryToken]
        [FormValueRequired(FormValueRequirement.StartsWith, "retryLastPayment")]
        public virtual IActionResult RetryLastRecurringPayment(IFormCollection form)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            //get recurring payment identifier
            var recurringPaymentId = 0;
            if (!form.Keys.Any(formValue => formValue.StartsWith("retryLastPayment", StringComparison.InvariantCultureIgnoreCase) &&
                int.TryParse(formValue.Substring(formValue.IndexOf('_') + 1), out recurringPaymentId)))
            {
                return RedirectToRoute("CustomerOrders");
            }

            var recurringPayment = _orderService.GetRecurringPaymentById(recurringPaymentId);
            if (recurringPayment == null)
                return RedirectToRoute("CustomerOrders");

            if (!_orderProcessingService.CanRetryLastRecurringPayment(_workContext.CurrentCustomer, recurringPayment))
                return RedirectToRoute("CustomerOrders");

            var errors = _orderProcessingService.ProcessNextRecurringPayment(recurringPayment);
            var model = _orderModelFactory.PrepareCustomerOrderListModel();
            model.RecurringPaymentErrors = errors.ToList();

            return View(model);
        }

        //My account / Reward points
        [HttpsRequirement]
        public virtual IActionResult CustomerRewardPoints(int? pageNumber)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            if (!_rewardPointsSettings.Enabled)
                return RedirectToRoute("CustomerInfo");

            var model = _orderModelFactory.PrepareCustomerRewardPoints(pageNumber);
            return View(model);
        }

        //My account / Order details page
        [HttpsRequirement]
        public virtual IActionResult Details(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return Challenge();
            var model = _orderModelFactory.PrepareOrderDetailsModel(order);
            var vendor = _vendorService.GetVendorByOrderId(order.Id);
            if (!String.IsNullOrEmpty(model.CheckoutAttributeXML))
            {
                model.DeliveryDate = _shuqOrderService.GetCheckoutDeliveryDate(order.CheckoutAttributesXml);
                model.DeliveryTimeslot = _shuqOrderService.GetCheckoutDeliveryTimeslot(order.CheckoutAttributesXml);
            }

            foreach (var item in model.Items)
            {
                item.HasReview = _shuqOrderService.CheckBuyerHasReviewProduct(orderId, item.ProductId);
            }
            var shipment = _shipmentService.GetShipmentsByOrderId(orderId).OrderByDescending(x => x.Id).FirstOrDefault();

            if (shipment?.ShippingMethodId != null)
            {
                var shippingCarrier = _shipmentCarrierResolver.ResolveByCourierSetting(vendor);
                if (shippingCarrier == null)
                    throw new ArgumentNullException("Cannot resolved any Shipping Carrier");
                model.TrackingNumberUrl = !String.IsNullOrEmpty(shipment.TrackingNumber) ? shippingCarrier.GetUrl(shipment.TrackingNumber, shipment.MarketCode) : "";
                model.ShippingMethod = shippingCarrier.Name;
            }

            var groupReturnRequest = _returnRequestService.GetGroupReturnRequestByOrderId(orderId).FirstOrDefault();
            ReturnOrder rOrder = new ReturnOrder();
            
            if (groupReturnRequest != null)
            {
                rOrder = _returnRequestService.GetReturnOrderByGroupReturnRequestId(groupReturnRequest.Id).FirstOrDefault();
                if (rOrder != null)
                {
                    var returnShipment = _shipmentService.GetShipmentsByReturnOrderId(rOrder.Id).FirstOrDefault();

                    var carrier = _shipmentCarrierResolver.ResolveByCourierSetting(vendor);
                    if (carrier == null)
                        throw new ArgumentNullException("Cannot resolved any Shipping Carrier");
                    model.RequireBarCode = carrier.RequireTrackingNumberBarCode;

                    if (model.RequireBarCode)
                    {
                        ViewBag.BarCode = NetBarCode.BarCodeGenerator(returnShipment.TrackingNumber);
                    }
                }
            }

            return View(model);
        }

        //My account / Order details page / Print
        [HttpsRequirement]
        public virtual IActionResult PrintOrderDetails(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return Challenge();

            var model = _orderModelFactory.PrepareOrderDetailsModel(order);
            model.PrintMode = true;

            return View("Details", model);
        }

        //My account / Order details page / PDF invoice
        public virtual IActionResult GetPdfInvoice(int orderId, int vendorId)
        {
            var order = _orderService.GetOrderById(orderId);
            var vendor = _vendorService.GetVendorById(vendorId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return Challenge();

            var orders = new List<Order>();
            orders.Add(order);
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintShuqOrdersToPdf(stream, orders, vendor, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, $"order_{order.Id}.pdf");
        }

        //My account / Order details page / re-order
        public virtual IActionResult ReOrder(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return Challenge();

            _orderProcessingService.ReOrder(order);
            return RedirectToRoute("ShoppingCart");
        }

        //My account / Order details page / Complete payment
        //[HttpPost, ActionName("Details")]
        //[AutoValidateAntiforgeryToken]
        //[FormValueRequired("repost-payment")]
        //public virtual IActionResult RePostPayment(int orderId)
        //{
        //    var order = _orderService.GetOrderById(orderId);
        //    if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
        //        return Challenge();

        //    if (!_paymentService.CanRePostProcessPayment(order))
        //        return RedirectToRoute("OrderDetails", new { orderId = orderId });

        //    var postProcessPaymentRequest = new PostProcessPaymentRequest
        //    {
        //        MasterOrder = order
        //    };
        //    _paymentService.PostProcessPayment(postProcessPaymentRequest);

        //    if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
        //    {
        //        //redirection or POST has been done in PostProcessPayment
        //        return Content("Redirected");
        //    }

        //    //if no redirection has been done (to a third-party payment page)
        //    //theoretically it's not possible
        //    return RedirectToRoute("OrderDetails", new { orderId = orderId });
        //}

        //My account / Order details page / Shipment details page
        [HttpsRequirement]
        public virtual IActionResult ShipmentDetails(int shipmentId)
        {
            var shipment = _shipmentService.GetShipmentById(shipmentId);
            if (shipment == null)
                return Challenge();

            var order = _orderService.GetOrderById(shipment.OrderId);
            
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return Challenge();

            var model = _orderModelFactory.PrepareShipmentDetailsModel(shipment);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult OrderReceived(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return Challenge();

            //shipments (only already shipped)
            var shipments = _shipmentService.GetShipmentsByOrderId(order.Id, true).OrderBy(x => x.CreatedOnUtc).ToList();

            _orderProcessingService.ReceivedOrder(shipments, order, true);

            var actorId = _workContext.CurrentCustomer.Id;

            _orderPayoutService.GenerateOrderPayoutRequest(actorId, DateTime.UtcNow, order.Id);

            return RedirectToRoute("OrderDetails", new { orderId = orderId });
        }
        
        #endregion
    }
}