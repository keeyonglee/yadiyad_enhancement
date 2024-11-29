using System;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using Nop.Services.Messages;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Core.Domain.Payout;

namespace Nop.Plugin.NopStation.AppPushNotification.Services
{
    public interface IWorkflowNotificationService
    {
        #region Campaigns

        IList<int> SendCampaignNotification(AppPushNotificationCampaign campaign);

        #endregion

        #region Customer workflow

        IList<int> SendCustomerRegisteredNotification(Customer customer, int languageId);
        
        IList<int> SendCustomerCustomerRegisteredWelcomeNotification(Customer customer, int languageId);
        
        IList<int> SendCustomerEmailValidationNotification(Customer customer, int languageId);

        IList<int> SendCustomerCustomerWelcomeNotification(Customer customer, int languageId);

        #endregion

        #region Order workflow

        IList<int> SendOrderPaidCustomerNotification(Order order, int languageId);
        
        IList<int> SendOrderPlacedCustomerNotification(Order order, int languageId);
        
        IList<int> SendShipmentSentCustomerNotification(Shipment shipment, int languageId);
        
        IList<int> SendShipmentDeliveredCustomerNotification(Shipment shipment, int languageId);
        
        IList<int> SendOrderCompletedCustomerNotification(Order order, int languageId);
        
        IList<int> SendOrderCancelledCustomerNotification(Order order, int languageId);
        
        IList<int> SendOrderRefundedCustomerNotification(Order order, decimal refundedAmount, int languageId);
        
        IList<int> SendOrderStartedPreparingNotification(Order order, int languageId);
        IList<int> SendOrderReturnCustomerNotification(GroupReturnRequest groupReturnRequest, int languageId);
        IList<int> SendOrderRefundCustomerNotification(GroupReturnRequest groupReturnRequest, int languageId);
        IList<int> SendOrderDisputeRaisedCustomerNotification(Dispute dispute, int languageId);

        IList<int> SendOrderPlacedVendorNotification(Order order, int languageId);
        IList<int> SendOrderPlacedDeliveryDateTimeVendorNotification(Order order, int languageId);
        IList<int> SendOrderPlacedDeliveryDateTimeReminderVendorNotification(Order order, int languageId);
        IList<int> SendOrderNewReturnRequestVendorNotification(Order order, int languageId);
        IList<int> SendUnableToLocateDriverVendorNotification(Order order, int languageId);
        IList<int> SendOrderPayoutRequestVendorNotification(OrderPayoutRequest orderPayoutRequest, 
            PayoutAndGroupShuqDTO payoutAndGroupShuqDTO,
            int languageId);

        IList<int> SendOrderCompletedVendorNotification(Order order, int languageId);
        IList<int> SendOrderCancelledVendorNotification(Order order, int languageId);
        IList<int> SendQuantityBelowVendorNotification(Product product, Vendor vendor, int languageId);
        IList<int> SendQuantityBelowVendorNotification(ProductAttributeCombination combination, Vendor vendor, int languageId);

        #endregion

        #region Misc

        IList<int> SendNotification(Customer customer, AppPushNotificationTemplate pushNotificationTemplate,
            int languageId, IEnumerable<Token> tokens, int storeId, DateTime? sendTime = null);

        #endregion

        IList<int> SendOrderDisputeSettlementCustomerNotification(Dispute dispute, int languageId);
        IList<int> SendOrderDisputeOutcomeFullRefundAndReturnAppCustomerNotification(Dispute dispute, int languageId);
        IList<int> SendOrderDisputeOutcomePartialRefundAndReturnAppCustomerNotification(Dispute dispute, int languageId);
        IList<int> SendOrderDisputeOutcomeRejectedAppCustomerNotification(Dispute dispute, int languageId);
        IList<int> SendOrderDisputeOutcomeFullRefundAppCustomerNotification(Dispute dispute, int languageId);
        IList<int> SendOrderDisputeOutcomePartialRefundAppCustomerNotification(Dispute dispute, int languageId);

        IList<int> SendOrderDisputeOutcomeFullRefundAndReturnVendorNotification(Dispute dispute, int languageId);
        IList<int> SendOrderDisputeOutcomePartialRefundAndReturnVendorNotification(Dispute dispute, int languageId);
        IList<int> SendOrderDisputeOutcomeRejectedVendorNotification(Dispute dispute, int languageId);
        IList<int> SendOrderDisputeOutcomeFullRefundVendorNotification(Dispute dispute, int languageId);
        IList<int> SendOrderDisputeOutcomePartialRefundVendorNotification(Dispute dispute, int languageId);
    }
}