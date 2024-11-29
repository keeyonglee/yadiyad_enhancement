using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Plugin.NopStation.WebApi.Models.Order;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Models.Order;
using YadiYad.Pro.Web.DTO.Base;

namespace Nop.Plugin.NopStation.WebApi.Controllers
{
    [Route("api/returnrequest")]
    public class ReturnRequestApiController : BaseApiController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly ICustomNumberFormatter _customNumberFormatter;
        private readonly IDownloadService _downloadService;
        private readonly ILocalizationService _localizationService;
        private readonly INopFileProvider _fileProvider;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IReturnRequestModelFactory _returnRequestModelFactory;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly OrderSettings _orderSettings;
        private readonly IShipmentService _shipmentService;
        private readonly IPictureService _pictureService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IVendorService _vendorService;

        #endregion

        #region Ctor

        public ReturnRequestApiController(ICustomerService customerService,
            ICustomNumberFormatter customNumberFormatter,
            IDownloadService downloadService,
            ILocalizationService localizationService,
            INopFileProvider fileProvider,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IReturnRequestModelFactory returnRequestModelFactory,
            IReturnRequestService returnRequestService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            OrderSettings orderSettings,
            IShipmentService shipmentService,
            IPictureService pictureService,
            IEventPublisher eventPublisher,
            IVendorService vendorService)
        {
            _customerService = customerService;
            _customNumberFormatter = customNumberFormatter;
            _downloadService = downloadService;
            _localizationService = localizationService;
            _fileProvider = fileProvider;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _returnRequestModelFactory = returnRequestModelFactory;
            _returnRequestService = returnRequestService;
            _storeContext = storeContext;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _orderSettings = orderSettings;
            _shipmentService = shipmentService;
            _pictureService = pictureService;
            _eventPublisher = eventPublisher;
            _vendorService = vendorService;
        }

        #endregion

        #region Methods

        //[HttpGet("history")]
        //public virtual IActionResult CustomerReturnRequests()
        //{
        //    if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
        //        return Unauthorized();

        //    var response = new GenericResponseModel<CustomerReturnRequestsModel>();
        //    response.Data = _returnRequestModelFactory.PrepareCustomerReturnRequestsModel();
        //    return Ok(response);
        //}        
        
        [HttpGet("history")]
        public virtual IActionResult CustomerReturnRequests()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Unauthorized();

            var response = new GenericResponseModel<GroupReturnRequestModel>();
            response.Data = _returnRequestModelFactory.PrepareCustomerGroupReturnRequestsModel();
            return Ok(response);
        }

        [HttpGet("returnRequestDetail/{orderId}")]
        public virtual IActionResult GetReturnRequestDetail(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted)
                return NotFound();

            if (_workContext.CurrentCustomer.Id != order.CustomerId)
                return Unauthorized();

            var response = new GenericResponseModel<GroupReturnRequestModel>();
            response.Data = _returnRequestModelFactory.PrepareGroupReturnRequestModel(orderId);
            return Ok(response);
        }

        [HttpGet("returnrequest/{orderId}")]
        public virtual IActionResult ReturnRequest(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted)
                return BadRequest();

            if (_workContext.CurrentCustomer.Id != order.CustomerId)
                return Unauthorized();

            var response = new GenericResponseModel<SubmitReturnRequestModel>();
            response.Data = _returnRequestModelFactory.PrepareSubmitReturnRequestModel(new SubmitReturnRequestModel(), order);
            return Ok(response);
        }
        
        [HttpPost("createreturnrequest")]
        public IActionResult ReturnRequestSubmit(SubmitReturnRequestViewModel model)
        {
            
            var order = _orderService.GetOrderById(model.Data.OrderId);
            if (order == null || order.Deleted)
                return NotFound();
            if (_workContext.CurrentCustomer.Id != order.CustomerId)
                return Unauthorized();

            var customer = _workContext.CurrentCustomer;
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
                return Ok(response);
            }

            var vendor = _vendorService.GetVendorByOrderId(order.Id);

            var count = 0;

            var existingReturn = _returnRequestService.GetGroupReturnRequestByOrderId(order.Id).FirstOrDefault();
            var orderShipment = _shipmentService.GetShipmentsByOrderId(model.Data.OrderId);
            bool hasInsurance = false;
            var insuranceAmt = 0.0m;

            foreach (var shipItem in orderShipment)
            {
                insuranceAmt += shipItem.Insurance;
            }

            if (orderShipment.Count > 0)
            {
                if (orderShipment[0].Insurance > 0)
                    hasInsurance = true;
                else
                    hasInsurance = false;
            }

            var groupReturn = new GroupReturnRequest
            {
                ApproveStatusId = (int)ApproveStatusEnum.Pending,
                HasInsuranceCover = hasInsurance,
                InsuranceClaimAmt = insuranceAmt,
                CreatedById = _workContext.CurrentCustomer.Id,
                UpdatedById = _workContext.CurrentCustomer.Id,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                CustomerId = customer.Id
            };

            if (existingReturn == null)
                _returnRequestService.InsertGroupReturnRequest(groupReturn);
            else
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.Response.ReturnRequest.AlreadyExist"));

            model.Data.ReturnRequestImageId = _pictureService.UploadPictureAndGetPictureId(model.Images, false);

            //returnable products
            var orderItems = _orderService.GetOrderItems(order.Id, isNotReturnable: false);

            List<int> rrId = new List<int>();

            for (int i = 0; i < model.Data.Items.Count; i++)
            {
                orderItems[i].Quantity = model.Data.Items[i].Quantity;
            }

            foreach (var orderItem in orderItems)
            {

                if (orderItem.Quantity > 0)
                {
                    var rrr = _returnRequestService.GetReturnRequestReasonById(model.Data.ReturnRequestReasonId);
                    var rra = _returnRequestService.GetReturnRequestActionById(model.Data.ReturnRequestActionId);

                    var rr = new ReturnRequest
                    {
                        GroupReturnRequestId = groupReturn.Id,
                        CustomNumber = "",
                        StoreId = _storeContext.CurrentStore.Id,
                        OrderItemId = orderItem.Id,
                        Quantity = orderItem.Quantity,
                        CustomerId = _workContext.CurrentCustomer.Id,
                        ReasonForReturn = rrr != null ? _localizationService.GetLocalized(rrr, x => x.Name) : "not available",
                        RequestedAction = rra != null ? _localizationService.GetLocalized(rra, x => x.Name) : "not available",
                        CustomerComments = model.Data.Comments,
                        StaffNotes = string.Empty,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow
                    };

                    _returnRequestService.InsertReturnRequest(rr);

                    rrId.Add(rr.Id);


                    //set return request custom number
                    rr.CustomNumber = _customNumberFormatter.GenerateReturnRequestCustomNumber(rr);
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                    _returnRequestService.UpdateReturnRequest(rr);

                    //notify store owner
                    // _workflowMessageService.SendNewReturnRequestStoreOwnerNotification(rr, orderItem, order, _localizationSettings.DefaultAdminLanguageId);
                    //notify customer
                    _workflowMessageService.SendNewReturnRequestCustomerNotification(rr, orderItem, order);
                    //notify vendor
                    _workflowMessageService.SendOrderReturnRefundVendorNotification(order, vendor, order.CustomerLanguageId);
                    //notify vendor app
                    _eventPublisher.Publish(new ReturnRequestNewEvent(order));

                    count++;
                }
            }

            for (int i = 0; i < model.Data.ReturnRequestImageId.Count; i++)
            {
                _returnRequestService.InsertReturnRequestImage(new ReturnRequestImage
                {
                    GroupReturnRequestId = groupReturn.Id,
                    PictureId = model.Data.ReturnRequestImageId[i],
                    DisplayOrder = 2,
                });
            }

            order.OrderStatus = OrderStatus.ReturnRefundProcessing;
            _orderService.UpdateOrder(order);

            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

        #endregion
    }
}
