using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Payout;
using Nop.Services.Shipping;
using Nop.Services.ShippingShuq;
using Nop.Services.ShuqOrders;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Models.Order;

namespace Nop.Plugin.NopStation.WebApi.Controllers
{
    [Route("api/order")]
    public class OrderApiController : BaseApiController
    {
        #region Fields

        private readonly IOrderModelFactory _orderModelFactory;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IPdfService _pdfService;
        private readonly IShipmentService _shipmentService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly IVendorService _vendorService;
        private readonly IShuqOrderProcessingService _shuqOrderProcessingService;
        private readonly IShipmentProcessor _shipmentProcessor;
        private readonly IShuqOrderService _shuqOrderService;
        private readonly IReturnRequestModelFactory _returnRequestModelFactory;
        private readonly ILogger _logger;
        private readonly OrderPayoutService _orderPayoutService;

        #endregion

        #region Ctor

        public OrderApiController(IOrderModelFactory orderModelFactory,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentService paymentService,
            IPdfService pdfService,
            IShipmentService shipmentService,
            IWebHelper webHelper,
            IWorkContext workContext,
            RewardPointsSettings rewardPointsSettings,
            ILocalizationService localizationService,
            ICustomerService customerService,
            IVendorService vendorService,
            IShuqOrderProcessingService shuqOrderProcessingService,
            IShipmentProcessor shipmentProcessor,
            IShuqOrderService shuqOrderService,
            IReturnRequestModelFactory returnRequestModelFactory,
            ILogger logger,
            OrderPayoutService orderPayoutService)
        {
            _orderModelFactory = orderModelFactory;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentService = paymentService;
            _pdfService = pdfService;
            _shipmentService = shipmentService;
            _webHelper = webHelper;
            _workContext = workContext;
            _rewardPointsSettings = rewardPointsSettings;
            _localizationService = localizationService;
            _customerService = customerService;
            _vendorService = vendorService;
            _shuqOrderProcessingService = shuqOrderProcessingService;
            _shipmentProcessor = shipmentProcessor;
            _shuqOrderService = shuqOrderService;
            _returnRequestModelFactory = returnRequestModelFactory;
            _logger = logger;
            _orderPayoutService = orderPayoutService;
        }

        #endregion

        #region Utilities

        private string CreateLalamoveScheduleAt(string deliveryDate, string deliveryTime)
        {
            if (string.IsNullOrEmpty(deliveryDate))
                throw new ArgumentNullException($"{nameof(deliveryDate)} cannot be null");

            if (string.IsNullOrEmpty(deliveryTime))
                throw new ArgumentNullException($"{nameof(deliveryTime)} cannot be null");

            string[] time = deliveryTime.Split(' ');

            var dateString = $"{deliveryDate} {time[0]}";
            //var dateValue = DateTime.Parse(dateString).ToUniversalTime().ToString("o");
            var checking = DateTime.TryParse(dateString, out var correctDateString);
            if (!checking)
                throw new ArgumentException($"{nameof(dateString)} not a proper date");

            //check whether schedule date is past?
            var scheduleAtDate = correctDateString.ToUniversalTime();
            var scheduleNow = DateTime.UtcNow;
            if (scheduleAtDate > scheduleNow)
            {
                return scheduleAtDate.ToString("o");
            }
            else
            {
                return null;
            }
        }


        #endregion

        #region Methods

        //My account / Orders
        [HttpGet("history")]
        public virtual IActionResult CustomerOrders(
            int pageNumber = 0,
            int pageSize = int.MaxValue)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            int pageIndex = 0;
            if (pageNumber > 0)
                pageIndex = pageNumber - 1;

            var response = new GenericResponseModel<CustomerOrderListModel>();
            response.Data = _orderModelFactory.PrepareCustomerOrderListModel(pageIndex, pageSize);
            return Ok(response);
        }

        //My account / Orders
        [HttpGet("vendorhistory")]
        public virtual IActionResult VendorOrders(
            OrderSortBy? orderSortBy = null,
            int pageNumber = 0,
            int pageSize = int.MaxValue)
        {
            if (!_customerService.IsVendor(_workContext.CurrentCustomer))
                return Unauthorized();

            int pageIndex = 0;
            if (pageNumber > 0)
                pageIndex = pageNumber - 1;

            var response = new GenericResponseModel<CustomerOrderListModel>();
            response.Data = _orderModelFactory.PrepareCustomerOrderListModel(pageIndex, pageSize, _workContext.CurrentVendor.Id, orderSortBy);
            return Ok(response);
        }

        //My account / Orders / Cancel recurring order
        [HttpPost("cancelrecurringpayment")]
        public virtual IActionResult CancelRecurringPayment([FromBody]List<KeyValueApi> formValues)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            var form = formValues == null ? new NameValueCollection() : formValues.ToNameValueCollection();
            //get recurring payment identifier
            var recurringPaymentId = 0;
            foreach (string formValue in form.Keys)
                if (formValue.StartsWith("cancelRecurringPayment", StringComparison.InvariantCultureIgnoreCase))
                    recurringPaymentId = Convert.ToInt32(formValue.Substring("cancelRecurringPayment".Length));

            var recurringPayment = _orderService.GetRecurringPaymentById(recurringPaymentId);
            if (recurringPayment == null)
                return NotFound();

            if (_orderProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment))
            {
                var errors = _orderProcessingService.CancelRecurringPayment(recurringPayment);

                var response = new GenericResponseModel<CustomerOrderListModel>();
                response.Data = _orderModelFactory.PrepareCustomerOrderListModel();
                response.Data.RecurringPaymentErrors = errors;
                response.ErrorList = errors.ToList();

                return Ok(response);
            }

            return BadRequest();
        }

        //My account / Reward points
        [HttpGet("customerrewardpoints/{pageNumber?}")]
        public virtual IActionResult CustomerRewardPoints(int? pageNumber)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            var response = new GenericResponseModel<CustomerRewardPointsModel>();
            if (!_rewardPointsSettings.Enabled)
            {
                response.ErrorList.Add(_localizationService.GetResource("NopStation.WebApi.Response.RewardPointsNotAvailable"));
                return BadRequest(response);
            }
            
            response.Data = _orderModelFactory.PrepareCustomerRewardPoints(pageNumber);
            return Ok(response);
        }

        //My account / Order details page
        [HttpGet("orderdetails/{orderId:min(0)}")]
        public virtual IActionResult Details(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted)
                return NotFound();

            var vendorCustomerId = _vendorService.GetVendorCustomerIdByOrderId(orderId);
            if (_workContext.CurrentCustomer.Id != order.CustomerId && _workContext.CurrentCustomer.Id != vendorCustomerId)
                return Unauthorized();

            var response = new GenericResponseModel<VendorOrderDetailsModel>();
            response.Data = _orderModelFactory.PrepareCustomerOrderModel(orderId);
            response.Data.GroupReturnRequest = _returnRequestModelFactory.PrepareGroupReturnRequestModel(orderId);
            return Ok(response);
        }

        //My account / Order details page / Print
        [HttpGet("orderdetails/print/{orderId}")]
        public virtual IActionResult PrintOrderDetails(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted)
                return NotFound();

            if (_workContext.CurrentCustomer.Id != order.CustomerId)
                return Unauthorized();

            var response = new GenericResponseModel<OrderDetailsModel>();
            response.Data = _orderModelFactory.PrepareOrderDetailsModel(order);
            response.Data.PrintMode = true;

            return Ok(response);
        }

        [HttpPost("cancelOrder/{orderId:min(0)}")]
        public IActionResult OrderCancellation(int orderId, [FromBody] BaseQueryModel<string> queryModel)
        {
            var form = queryModel == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();
            var isEditable = form["isEditable"] != null ? bool.Parse(form["isEditable"]) : false;
            var cancellationReasonId = form["cancellationreasonid"];

            if (cancellationReasonId == null)
                return NotFound();

            var intCancellationReasonId = int.Parse(cancellationReasonId);

            var order = _orderService.GetOrderById(orderId);

            if (order == null || order.Deleted)
                return NotFound();

            if (_workContext.CurrentCustomer.Id != order.CustomerId)
                return Unauthorized();

            order.CancellationReason = intCancellationReasonId;
            _orderProcessingService.CancelOrder(order, true);

            return Ok();
        }

        [HttpPost("receiveOrder")]
        public IActionResult OrderReceived([FromBody] BaseQueryModel<string> queryModel)
        {
            var form = queryModel == null ? new NameValueCollection() : queryModel.FormValues.ToNameValueCollection();
            var isEditable = form["isEditable"] != null ? bool.Parse(form["isEditable"]) : false;
            var orderId = form["orderId"];

            if (orderId == null)
                return NotFound();

            var intOrderId = int.Parse(orderId);

            var order = _orderService.GetOrderById(intOrderId);

            if (order == null || order.Deleted)
                return NotFound();

            if (_workContext.CurrentCustomer.Id != order.CustomerId)
                return Unauthorized();

            //shipments (only already shipped)
            var shipments = _shipmentService.GetShipmentsByOrderId(order.Id, true).OrderBy(x => x.CreatedOnUtc).ToList();
            _orderProcessingService.ReceivedOrder(shipments, order, true);

            var actorId = _workContext.CurrentCustomer.Id;
            _orderPayoutService.GenerateOrderPayoutRequest(actorId, DateTime.UtcNow, order.Id);

            return Ok();
        }

        //My account / Order details page / PDF invoice
        [HttpGet("orderdetails/pdf/{orderId}")]
        public virtual IActionResult GetPdfInvoice(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted)
                return NotFound();

            if (_workContext.CurrentCustomer.Id != order.CustomerId)
                return Unauthorized();

            var orders = new List<Order>();
            orders.Add(order);
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintOrdersToPdf(stream, orders, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, $"order_{order.Id}.pdf");
        }

        //My account / Order details page / re-order
        [HttpGet("reorder/{orderId:min(0)}")]
        public virtual IActionResult ReOrder(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted)
                return NotFound();

            if (_workContext.CurrentCustomer.Id != order.CustomerId)
                return Unauthorized();

            _orderProcessingService.ReOrder(order);
            return Ok();
        }

        //My account / Order details page / Complete payment
        
        [HttpPost("orderdetails/repostpayment/{orderId:min(0)}")]
        public virtual IActionResult RePostPayment(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted)
                return NotFound();

            if (_workContext.CurrentCustomer.Id != order.CustomerId)
                return Unauthorized();

            if (!_paymentService.CanRePostProcessPayment(order))
                return BadRequest();

            var postProcessPaymentRequest = new PostProcessPaymentRequest
            {
                Order = order
            };
            _paymentService.PostProcessPayment(postProcessPaymentRequest);

            if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
            {
                //redirection or POST has been done in PostProcessPayment
                return Ok();
            }

            //if no redirection has been done (to a third-party payment page)
            //theoretically it's not possible
            return Redirect($"api/orderdetails/{orderId}");
        }

        //My account / Order details page / Shipment details page
        [HttpGet("orderdetails/shipment/{shipmentId}")]
        public virtual IActionResult ShipmentDetails(int shipmentId)
        {
            var shipment = _shipmentService.GetShipmentById(shipmentId);
            if (shipment == null)
                return Unauthorized();

            var order = _orderService.GetOrderById(shipment.OrderId);

            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return Unauthorized();

            var response = new GenericResponseModel<ShipmentDetailsModel>();
            response.Data = _orderModelFactory.PrepareShipmentDetailsModel(shipment);

            return Ok(response);
        }

        //My account / Order details page / set-preparing
        [HttpGet("setPreparing/{orderId}")]
        public virtual IActionResult SetOrderToPreparing(int orderId)
        {
            try
            {
                var order = _orderService.GetOrderById(orderId);
                if (order == null || order.Deleted)
                    return NotFound();

                var vendorCustomerId = _vendorService.GetVendorCustomerIdByOrderId(orderId);
                if (_workContext.CurrentCustomer.Id != vendorCustomerId)
                    return Unauthorized();

                var deliveryDateText = _shuqOrderService.GetCheckoutDeliveryDate(order.CheckoutAttributesXml);
                var deliveryTimeText = _shuqOrderService.GetCheckoutDeliveryTimeslot(order.CheckoutAttributesXml);
                var deliveryScheduleAt = _shipmentService.CreateLalamoveScheduleAt(deliveryDateText, deliveryTimeText);
                _shuqOrderProcessingService.StartPreparing(order, deliveryScheduleAt);
                return Ok();
            }
            catch (Exception exc)
            {

                _logger.Error(exc.Message, exc, _workContext.CurrentCustomer);
                return InternalServerError(_localizationService.GetResource("NopStation.WebApi.Response.Shipment.SetPreparingFailed"));
            }
        }

        //My account / Order details page / arrange-shipping
        [HttpPost("arrangeShipping")]
        public virtual IActionResult SetOrderToArrangeShipping([FromBody] BaseQueryModel<ArrangeShippingModel> queryModel)
        {
            try
            {
                var order = _orderService.GetOrderById(queryModel.Data.OrderId);
                if (order == null || order.Deleted)
                    return NotFound();

                var vendorCustomerId = _vendorService.GetVendorCustomerIdByOrderId(queryModel.Data.OrderId);
                if (_workContext.CurrentCustomer.Id != vendorCustomerId)
                    return Unauthorized();

                _shuqOrderProcessingService.ArrangeShipment(order, queryModel.Data.RequireInsurance);
                return Ok();
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc, _workContext.CurrentCustomer);
                return InternalServerError(_localizationService.GetResource("NopStation.WebApi.Response.Shipment.ArrangeShipmentFailed"));
            }
            
        }

        //My account / Order details page / retry-find-driver
        [HttpGet("retryFindDriver/{orderId}")]
        public virtual IActionResult SetOrderToRetryFindDriver(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted)
                return NotFound();

            var vendorCustomerId = _vendorService.GetVendorCustomerIdByOrderId(orderId);
            if (_workContext.CurrentCustomer.Id != vendorCustomerId)
                return Unauthorized();

            _shuqOrderProcessingService.RetryShipping(order);
            return Ok();
        }

        //My account / Order details page / retry-find-driver
        [HttpGet("setshipmentshipped/{orderId}")]
        public virtual IActionResult SetShipmentShipped(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted)
                return NotFound();

            var vendorCustomerId = _vendorService.GetVendorCustomerIdByOrderId(orderId);
            if (_workContext.CurrentCustomer.Id != vendorCustomerId)
                return Unauthorized();

            var shipment = _shipmentService.GetShipmentsByOrderId(orderId)[0];
            if (shipment == null)
                return BadRequest("Shipment not found");

            _shuqOrderProcessingService.Ship(shipment, true);
            return Ok();
        }

        #endregion
    }
}
