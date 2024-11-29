using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payout;
using Nop.Core.Domain.ShippingShuq;
using Nop.Core.Events;
using Nop.Services.Events;

namespace Nop.Plugin.NopStation.AppPushNotification.Infrastructure
{
    public partial class AppNotificationEventConsumer : IConsumer<OrderPreparedEvent>,
        IConsumer<OrderCompletedEvent>,
        IConsumer<ReturnRequestApprovedEvent>,
        IConsumer<EntityInsertedEvent<Dispute>>,
        IConsumer<OrderDisputeSettlementOutcomeEvent>,
        IConsumer<ReturnRequestNewEvent>,
        IConsumer<OrderPayoutRequestEvent>,
        IConsumer<UnableToLocateDriverEvent>,
        IConsumer<LowStockProductEvent>,
        IConsumer<LowStockProductAttributeEvent>

    {
        public void HandleEvent(OrderPreparedEvent eventMessage)
        {
            _workflowNotificationService.SendOrderStartedPreparingNotification(eventMessage.Order,
                _localizationSettings.DefaultAdminLanguageId);
        }

        public void HandleEvent(OrderCompletedEvent eventMessage)
        {
            _workflowNotificationService.SendOrderCompletedCustomerNotification(eventMessage.Order,
                _localizationSettings.DefaultAdminLanguageId);
            _workflowNotificationService.SendOrderCompletedVendorNotification(eventMessage.Order,
                _localizationSettings.DefaultAdminLanguageId);
        }

        public void HandleEvent(ReturnRequestApprovedEvent eventMessage)
        {
            if (eventMessage.GroupReturnRequest.NeedReturnShipping)
                _workflowNotificationService.SendOrderReturnCustomerNotification(eventMessage.GroupReturnRequest,
                    _localizationSettings.DefaultAdminLanguageId);
            else
                _workflowNotificationService.SendOrderRefundCustomerNotification(eventMessage.GroupReturnRequest,
                    _localizationSettings.DefaultAdminLanguageId);
        }

        public void HandleEvent(OrderDisputeSettlementOutcomeEvent eventMessage)
        {
            //_workflowNotificationService.SendOrderDisputeSettlementCustomerNotification(eventMessage.Dispute,
            //    _localizationSettings.DefaultAdminLanguageId);
            if (eventMessage.Dispute.DisputeAction == (int)DisputeActionEnum.FullRefundFromBuyer)
            {
                if (eventMessage.GroupReturnRequest.NeedReturnShipping)
                {
                    _workflowNotificationService.SendOrderDisputeOutcomeFullRefundAndReturnAppCustomerNotification(eventMessage.Dispute,
                        _localizationSettings.DefaultAdminLanguageId);
                    _workflowNotificationService.SendOrderDisputeOutcomeFullRefundAndReturnVendorNotification(eventMessage.Dispute,
                        _localizationSettings.DefaultAdminLanguageId);
                }
                else
                {
                    _workflowNotificationService.SendOrderDisputeOutcomeFullRefundAppCustomerNotification(eventMessage.Dispute,
                        _localizationSettings.DefaultAdminLanguageId);
                    _workflowNotificationService.SendOrderDisputeOutcomeFullRefundVendorNotification(eventMessage.Dispute,
                        _localizationSettings.DefaultAdminLanguageId);
                }
            }
            else if (eventMessage.Dispute.DisputeAction == (int)DisputeActionEnum.PartialRefund)
            {
                if (eventMessage.GroupReturnRequest.NeedReturnShipping)
                {
                    _workflowNotificationService.SendOrderDisputeOutcomePartialRefundAndReturnAppCustomerNotification(eventMessage.Dispute,
                        _localizationSettings.DefaultAdminLanguageId);
                    _workflowNotificationService.SendOrderDisputeOutcomePartialRefundAndReturnVendorNotification(eventMessage.Dispute,
                        _localizationSettings.DefaultAdminLanguageId);
                }
                else
                {
                    _workflowNotificationService.SendOrderDisputeOutcomePartialRefundAppCustomerNotification(eventMessage.Dispute,
                        _localizationSettings.DefaultAdminLanguageId);
                    _workflowNotificationService.SendOrderDisputeOutcomePartialRefundVendorNotification(eventMessage.Dispute,
                        _localizationSettings.DefaultAdminLanguageId);
                }
            }
            else
            {
                _workflowNotificationService.SendOrderDisputeOutcomeRejectedAppCustomerNotification(eventMessage.Dispute,
                        _localizationSettings.DefaultAdminLanguageId);
                _workflowNotificationService.SendOrderDisputeOutcomeRejectedVendorNotification(eventMessage.Dispute,
                        _localizationSettings.DefaultAdminLanguageId);
            }
        }

        public void HandleEvent(EntityInsertedEvent<Dispute> eventMessage)
        {
            _workflowNotificationService.SendOrderDisputeRaisedCustomerNotification(eventMessage.Entity,
                _localizationSettings.DefaultAdminLanguageId);
        }
        
        public void HandleEvent(EntityUpdatedEvent<Dispute> eventMessage)
        {
            _workflowNotificationService.SendOrderDisputeRaisedCustomerNotification(eventMessage.Entity,
                _localizationSettings.DefaultAdminLanguageId);
        }

        public void HandleEvent(ReturnRequestNewEvent eventMessage)
        {
            _workflowNotificationService.SendOrderNewReturnRequestVendorNotification(eventMessage.Order, 
                _localizationSettings.DefaultAdminLanguageId);
        }

        public void HandleEvent(OrderPayoutRequestEvent eventMessage)
        {
            _workflowNotificationService.SendOrderPayoutRequestVendorNotification(eventMessage.OrderPayoutRequest,
                eventMessage.PayoutAndGroupDTO,
                _localizationSettings.DefaultAdminLanguageId);
        }

        public void HandleEvent(UnableToLocateDriverEvent eventMessage)
        {
            _workflowNotificationService.SendUnableToLocateDriverVendorNotification(eventMessage.Order,
                _localizationSettings.DefaultAdminLanguageId);
        }
        
        public void HandleEvent(LowStockProductEvent eventMessage)
        {
            _workflowNotificationService.SendQuantityBelowVendorNotification(eventMessage.Product,
                eventMessage.Vendor,
                _localizationSettings.DefaultAdminLanguageId);
        }
        
        public void HandleEvent(LowStockProductAttributeEvent eventMessage)
        {
            _workflowNotificationService.SendQuantityBelowVendorNotification(eventMessage.Combination,
                eventMessage.Vendor,
                _localizationSettings.DefaultAdminLanguageId);
        }
    }
}