namespace Nop.Plugin.NopStation.AppPushNotification.Domains
{
    public class AppPushNotificationTemplateSystemNames
    {
        #region Customer

        /// <summary>
        /// Represents system name of notification about new registration
        /// </summary>
        public const string CustomerRegisteredNotification = "NewCustomer.Notification";

        /// <summary>
        /// Represents system name of customer welcome message
        /// </summary>
        public const string CustomerWelcomeNotification = "Customer.WelcomeNotification";

        /// <summary>
        /// Represents system name of customer welcome message
        /// </summary>
        public const string CustomerRegisteredWelcomeNotification = "Customer.RegisteredWelcomeNotification";

        /// <summary>
        /// Represents system name of email validation message
        /// </summary>
        public const string CustomerEmailValidationNotification = "Customer.EmailValidationNotification";
        
        #endregion

        #region Order

        /// <summary>
        /// Represents system name of notification customer about paid order
        /// </summary>
        public const string OrderPaidCustomerNotification = "OrderPaid.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about placed order
        /// </summary>
        public const string OrderPlacedCustomerNotification = "OrderPlaced.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about sent shipment
        /// </summary>
        public const string ShipmentSentCustomerNotification = "ShipmentSent.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about delivered shipment
        /// </summary>
        public const string ShipmentDeliveredCustomerNotification = "ShipmentDelivered.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about completed order
        /// </summary>
        public const string OrderCompletedCustomerNotification = "OrderCompleted.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about cancelled order
        /// </summary>
        public const string OrderCancelledCustomerNotification = "OrderCancelled.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about refunded order
        /// </summary>
        public const string OrderRefundedCustomerNotification = "OrderRefunded.CustomerNotification";
        
        /// <summary>
        /// Represents system name of notification customer about order started preparation
        /// </summary>
        public const string OrderPreparingCustomerNotification = "OrderPreparing.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about order return approve
        /// </summary>
        public const string OrderReturnApproveCustomerNotification = "OrderReturnApprove.CustomerNotification";
        
        /// <summary>
        /// Represents system name of notification customer about order refund approve
        /// </summary>
        public const string OrderRefundApproveCustomerNotification = "OrderRefundApprove.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about seller raising dispute about order return
        /// </summary>
        public const string OrderDisputeRaisedCustomerNotification = "OrderDisputeRaised.CustomerNotification";
        
        /// <summary>
        /// Represents system name of notification customer about dispute outcome by admin for order
        /// </summary>
        public const string OrderDisputeOutcomeCustomerNotification = "OrderDisputeOutcome.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about order refund is rejected after dispute
        /// </summary>
        public const string OrderDisputeOutcomeNoRefundAppCustomerNotification = "OrderDisputeOutcomeNoRefund.AppCustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about order refund is approved full refund after dispute
        /// </summary>
        public const string OrderDisputeOutcomeFullRefundAppCustomerNotification = "OrderDisputeOutcomeFullRefund.AppCustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about order refund is approved full refund with return after dispute
        /// </summary>
        public const string OrderDisputeOutcomeFullRefundAndReturnAppCustomerNotification = "OrderDisputeOutcomeFullRefundAndReturn.AppCustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about order refund is approved partial refund with return after dispute
        /// </summary>
        public const string OrderDisputeOutcomePartialRefundAndReturnAppCustomerNotification = "OrderDisputeOutcomePartialRefundAndReturn.AppCustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about order refund is approved partial refund after dispute
        /// </summary>
        public const string OrderDisputeOutcomePartialRefundAppCustomerNotification = "OrderDisputeOutcomePartialRefund.AppCustomerNotification";


        public const string OrderDisputeOutcomeNoRefundVendorNotification = "OrderDisputeOutcomeNoRefund.VendorNotification";

        public const string OrderDisputeOutcomeFullRefundVendorNotification = "OrderDisputeOutcomeFullRefund.VendorNotification";

        public const string OrderDisputeOutcomeFullRefundAndReturnVendorNotification = "OrderDisputeOutcomeFullRefundAndReturn.VendorNotification";

        public const string OrderDisputeOutcomePartialRefundAndReturnVendorNotification = "OrderDisputeOutcomePartialRefundAndReturn.VendorNotification";

        public const string OrderDisputeOutcomePartialRefundVendorNotification = "OrderDisputeOutcomePartialRefund.VendorNotification";

        #endregion

        #region Seller/Vendor


        public const string OrderPlacedVendorNotification = "OrderPlaced.VendorNotification";

        public const string ReturnRequestVendorNotification = "ReturnRequest.VendorNotification";

        public const string ReminderArrangeDriverRequestInAdvanceVendorNotification = "ReminderArrangeDriverRequestInAdvance.VendorNotification";

        public const string NewPayoutVendorNotification = "NewPayout.VendorNotification";

        public const string ReminderArrangeDriverRequestNowVendorNotification = "ReminderArrangeDriverRequestNow.VendorNotification";


        public const string OrderCancelledVendorNotification = "OrderCancelled.VendorNotification";

        public const string OrderCompletedVendorNotification = "OrderCompleted.VendorNotification";
        public const string UnableToLocateDriverVendorNotification = "UnableToLocateDriver.VendorNotification";
        
        public const string QuantityBelowVendorNotification = "QuantityBelow.VendorNotification";
        public const string QuantityBelowAttributeCombinationVendorNotification = "QuantityBelow.AttributeCombination.VendorNotification";

        #endregion
    }
}
