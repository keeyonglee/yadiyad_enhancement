using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payout;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Data;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using Nop.Plugin.NopStation.AppPushNotification.Services;
using Nop.Plugin.NopStation.WebApi.Controllers;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payout;
using Nop.Services.Shipping;
using Nop.Services.ShuqOrders;

namespace Nop.Plugin.NopStation.AppPushNotification.Controllers
{
    [Route("api/testpushnotification")]
    
    public class TestAppPushNotificationController : BaseApiController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly WorkflowNotificationService _notificationService;
        private readonly IPushNotificationTemplateService _templateService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IRepository<ApiDevice> _apiDeviceRepository;
        private readonly IPictureService _pictureService;
        private readonly IShuqOrderService _orderService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IShipmentService _shipmentService;
        private readonly OrderPayoutService _orderPayoutService;

        public TestAppPushNotificationController(IWebHostEnvironment webHostEnvironment,
            WorkflowNotificationService notificationService,
            IPushNotificationTemplateService templateService,
            ICustomerService customerService,
            IWorkContext workContext,
            IRepository<ApiDevice> apiDeviceRepository,
            IPictureService pictureService,
            IShuqOrderService orderService,
            IReturnRequestService returnRequestService,
            IEventPublisher eventPublisher,
            IShipmentService shipmentService,
            OrderPayoutService orderPayoutService)
        {
            _webHostEnvironment = webHostEnvironment;
            _notificationService = notificationService;
            _templateService = templateService;
            _customerService = customerService;
            _workContext = workContext;
            _apiDeviceRepository = apiDeviceRepository;
            _pictureService = pictureService;
            _orderService = orderService;
            _returnRequestService = returnRequestService;
            _eventPublisher = eventPublisher;
            _shipmentService = shipmentService;
            _orderPayoutService = orderPayoutService;
        }

        [HttpPost("send")]
        public IActionResult Send([FromBody]NotificationRequest request)
        {
            // this api controller is only for testing purpose
            // dont let anonymous insert in production environment
            if(_webHostEnvironment.IsProduction())
                return MethodNotAllowed();

            var templates = _templateService.GetPushNotificationTemplatesByName(request.Template);
            if (!templates.Any())
                return BadRequest("No Template Found with Name");

            // Target the Customer from Request parameter else target requesting customer
            var customer = request.CustomerId == default ?
                _workContext.CurrentCustomer : _customerService.GetCustomerById(request.CustomerId);

            if (customer == default)
                return BadRequest("Customer Not Found");

            var template = templates.First();
            var devices = _apiDeviceRepository.Table.Where(q => q.CustomerId == customer.Id && q.AppTypeId == template.AppTypeId);
            if (!string.IsNullOrWhiteSpace(request.SubscriptionId))
                devices = devices.Where(q => q.SubscriptionId == request.SubscriptionId);
            
            if (!devices.Any())
                return BadRequest("No Device Found");
            
            var device =  devices.OrderByDescending(q => q.CreatedOnUtc).First();
            var picture =  _pictureService.GetPictureUrl(template.ImageId, showDefaultPicture: false);

            _notificationService.SendNotification(
                new List<ApiDevice>() {device},
                template.Title,
                template.Body,
                picture,
                template.ActionType,
                string.Empty,
                1,
                template.AppTypeId,
                null);

            return Ok($"Notification will be sent to device with subscription id [{device.SubscriptionId}]");
        }
        
        [HttpPost("trigger")]
        public IActionResult Trigger([FromBody]TriggerNotificationRequest request)
        {
            // this api controller is only for testing purpose
            // dont let anonymous insert in production environment
            if(_webHostEnvironment.IsProduction())
                return MethodNotAllowed();

            var templates = _templateService.GetPushNotificationTemplatesByName(request.Template);
            if (!templates.Any())
                return BadRequest("No Template Found with Name");

            var success = TriggerTemplate(request);

            if (!success)
                return Ok("Template not found");
            
            return Ok();
        }

        [NonAction]
        private bool TriggerTemplate(TriggerNotificationRequest request)
        {
            switch(request.Template)
            {
                case AppPushNotificationTemplateSystemNames.ShipmentDeliveredCustomerNotification:
                    _eventPublisher.PublishShipmentDelivered(GetShipment(request.TemplateEntityId));
                    break;
                case AppPushNotificationTemplateSystemNames.ShipmentSentCustomerNotification:
                    _eventPublisher.PublishShipmentSent(GetShipment(request.TemplateEntityId));
                    break;
                case AppPushNotificationTemplateSystemNames.OrderCompletedCustomerNotification:
                    _eventPublisher.Publish(new OrderCompletedEvent(GetOrder(request.TemplateEntityId)));
                    break;
                case AppPushNotificationTemplateSystemNames.OrderPreparingCustomerNotification:
                    _eventPublisher.Publish(new OrderPreparedEvent(GetOrder(request.TemplateEntityId)));
                    break;
                case AppPushNotificationTemplateSystemNames.OrderReturnApproveCustomerNotification:
                    _eventPublisher.Publish(new ReturnRequestApprovedEvent(GetGroupReturnRequest(request.TemplateEntityId)));
                    break;
                case AppPushNotificationTemplateSystemNames.OrderRefundApproveCustomerNotification:
                    _eventPublisher.Publish(new ReturnRequestApprovedEvent(GetGroupReturnRequest(request.TemplateEntityId)));
                    break;
                case AppPushNotificationTemplateSystemNames.OrderDisputeRaisedCustomerNotification: 
                    _eventPublisher.Publish(new EntityInsertedEvent<Dispute>(GetDispute(request.TemplateEntityId)));
                    break;
                case AppPushNotificationTemplateSystemNames.OrderDisputeOutcomeCustomerNotification:
                    _eventPublisher.Publish(new OrderDisputeSettlementOutcomeEvent(GetDispute(request.TemplateEntityId), GetGroupReturnRequest(GetDispute(request.TemplateEntityId).GroupReturnRequestId)));
                    break;
                case AppPushNotificationTemplateSystemNames.OrderDisputeOutcomeNoRefundAppCustomerNotification:
                    _eventPublisher.Publish(new OrderDisputeSettlementOutcomeEvent(GetDispute(request.TemplateEntityId), GetGroupReturnRequest(GetDispute(request.TemplateEntityId).GroupReturnRequestId)));
                    break;
                case AppPushNotificationTemplateSystemNames.OrderDisputeOutcomeFullRefundAppCustomerNotification:
                    _eventPublisher.Publish(new OrderDisputeSettlementOutcomeEvent(GetDispute(request.TemplateEntityId), GetGroupReturnRequest(GetDispute(request.TemplateEntityId).GroupReturnRequestId)));
                    break;
                case AppPushNotificationTemplateSystemNames.OrderDisputeOutcomeFullRefundAndReturnAppCustomerNotification:
                    _eventPublisher.Publish(new OrderDisputeSettlementOutcomeEvent(GetDispute(request.TemplateEntityId), GetGroupReturnRequest(GetDispute(request.TemplateEntityId).GroupReturnRequestId)));
                    break;
                case AppPushNotificationTemplateSystemNames.OrderDisputeOutcomePartialRefundAndReturnAppCustomerNotification:
                    _eventPublisher.Publish(new OrderDisputeSettlementOutcomeEvent(GetDispute(request.TemplateEntityId), GetGroupReturnRequest(GetDispute(request.TemplateEntityId).GroupReturnRequestId)));
                    break;
                case AppPushNotificationTemplateSystemNames.OrderDisputeOutcomePartialRefundAppCustomerNotification:
                    _eventPublisher.Publish(new OrderDisputeSettlementOutcomeEvent(GetDispute(request.TemplateEntityId), GetGroupReturnRequest(GetDispute(request.TemplateEntityId).GroupReturnRequestId)));
                    break;
                case AppPushNotificationTemplateSystemNames.OrderPlacedVendorNotification:
                    _eventPublisher.Publish(new OrderPlacedEvent(GetOrder(request.TemplateEntityId)));
                    break;
                case AppPushNotificationTemplateSystemNames.ReturnRequestVendorNotification:
                    _eventPublisher.Publish(new ReturnRequestNewEvent(GetOrder(request.TemplateEntityId)));
                    break;
                //case AppPushNotificationTemplateSystemNames.NewPayoutVendorNotification:
                //    _eventPublisher.Publish(new OrderPayoutRequestEvent(GetOrderPayoutRequest(request.TemplateEntityId)));
                //    break;
                case AppPushNotificationTemplateSystemNames.OrderCancelledCustomerNotification:
                    _eventPublisher.Publish(new OrderCancelledEvent(GetOrder(request.TemplateEntityId)));
                    break;
                default:
                    return false;
            }
            return true;
        }

        [NonAction]
        private Order GetOrder(int id) => _orderService.GetOrderById(id);
        [NonAction]
        private Customer GetCustomer(int id) => _customerService.GetCustomerById(id);
        [NonAction]
        private GroupReturnRequest GetGroupReturnRequest(int id) => _returnRequestService.GetGroupReturnRequestById(id);
        [NonAction]
        private Dispute GetDispute(int id) => _returnRequestService.GetDisputeById(id);
        [NonAction]
        private Shipment GetShipment(int id) => _shipmentService.GetShipmentById(id);
        [NonAction]
        private OrderPayoutRequest GetOrderPayoutRequest(int id) => _orderPayoutService.GetOrderPayoutRequestByOrderId(id);

        public class NotificationRequest
        {
            public int CustomerId { get; set; }
            public string SubscriptionId { get; set; }
            public string Template { get; set; }
        }

        public class TriggerNotificationRequest
        {
            public string Template { get; set; }
            public int TemplateEntityId { get; set; }
        }
    }
}