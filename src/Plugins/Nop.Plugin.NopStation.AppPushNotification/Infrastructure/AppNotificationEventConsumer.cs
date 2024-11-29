using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.NopStation.AppPushNotification.Services;
using Nop.Services.Events;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.ShuqOrders;

namespace Nop.Plugin.NopStation.AppPushNotification.Infrastructure
{
    public partial class AppNotificationEventConsumer : IConsumer<CustomerRegisteredEvent>,
        IConsumer<OrderPlacedEvent>,
        IConsumer<OrderPaidEvent>,
        IConsumer<OrderCancelledEvent>,
        IConsumer<ShipmentSentEvent>,
        IConsumer<ShipmentDeliveredEvent>,
        IConsumer<OrderRefundedEvent>
    {
        private readonly IWorkContext _workContext;
        private readonly IWorkflowNotificationService _workflowNotificationService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly IOrderService _orderService;
        private readonly VendorNotificationSettings _vendorNotificationSettings;
        private readonly IShuqOrderService _shuqOrderService;

        public AppNotificationEventConsumer(IWorkContext workContext,
            IWorkflowNotificationService workflowNotificationService,
            LocalizationSettings localizationSettings,
            CustomerSettings customerSettings,
            IOrderService orderService,
            VendorNotificationSettings vendorNotificationSettings,
            IShuqOrderService shuqOrderService)
        {
            _workContext = workContext;
            _workflowNotificationService = workflowNotificationService;
            _localizationSettings = localizationSettings;
            _customerSettings = customerSettings;
            _orderService = orderService;
            _vendorNotificationSettings = vendorNotificationSettings;
            _shuqOrderService = shuqOrderService;
        }

        public void HandleEvent(CustomerRegisteredEvent eventMessage)
        {
            //_workflowNotificationService.SendCustomerRegisteredNotification(eventMessage.Customer,
            //    _localizationSettings.DefaultAdminLanguageId);

            switch (_customerSettings.UserRegistrationType)
            {
                case UserRegistrationType.EmailValidation:
                    _workflowNotificationService.SendCustomerEmailValidationNotification(eventMessage.Customer, 
                        _workContext.WorkingLanguage.Id);
                    break;
                case UserRegistrationType.Standard:
                    _workflowNotificationService.SendCustomerCustomerRegisteredWelcomeNotification(eventMessage.Customer, 
                        _workContext.WorkingLanguage.Id);
                    break;
                default:
                    break;
            }
        }

        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            _workflowNotificationService.SendOrderPlacedCustomerNotification(eventMessage.Order,
                     _localizationSettings.DefaultAdminLanguageId);

            _workflowNotificationService.SendOrderPlacedVendorNotification(eventMessage.Order,
                     _localizationSettings.DefaultAdminLanguageId);

            if (!string.IsNullOrEmpty(eventMessage.Order.CheckoutAttributesXml))
            {
                _workflowNotificationService.SendOrderPlacedDeliveryDateTimeVendorNotification(eventMessage.Order,
                     _localizationSettings.DefaultAdminLanguageId);

                if (_shuqOrderService.CheckEatsDeliveryDateTimeSlotNeedReminder(eventMessage.Order, _vendorNotificationSettings.ArrangeDriverRequestAdvanceHours))
                {
                    _workflowNotificationService.SendOrderPlacedDeliveryDateTimeReminderVendorNotification(eventMessage.Order,
                             _localizationSettings.DefaultAdminLanguageId);
                }
            }
        }

        public void HandleEvent(OrderPaidEvent eventMessage)
        {
            _workflowNotificationService.SendOrderPaidCustomerNotification(eventMessage.Order,
                     _localizationSettings.DefaultAdminLanguageId);
        }

        public void HandleEvent(OrderCancelledEvent eventMessage)
        {
            _workflowNotificationService.SendOrderCancelledCustomerNotification(eventMessage.Order,
                     _localizationSettings.DefaultAdminLanguageId);
            _workflowNotificationService.SendOrderCancelledVendorNotification(eventMessage.Order,
                     _localizationSettings.DefaultAdminLanguageId);
        }

        public void HandleEvent(ShipmentSentEvent eventMessage)
        {
            var order = _orderService.GetOrderById(eventMessage.Shipment.OrderId);
            _workflowNotificationService.SendShipmentSentCustomerNotification(eventMessage.Shipment, 
                order.CustomerLanguageId);
        }

        public void HandleEvent(ShipmentDeliveredEvent eventMessage)
        {
            var order = _orderService.GetOrderById(eventMessage.Shipment.OrderId);
            _workflowNotificationService.SendShipmentDeliveredCustomerNotification(eventMessage.Shipment,
                order.CustomerLanguageId);
        }

        public void HandleEvent(OrderRefundedEvent eventMessage)
        {
            _workflowNotificationService.SendOrderRefundedCustomerNotification(eventMessage.Order,
                eventMessage.Amount, eventMessage.Order.CustomerLanguageId);
        }
    }
}
