namespace Nop.Core.Domain.Messages
{
    /// <summary>
    /// Represents message template system names
    /// </summary>
    public static partial class MessageTemplateSystemNames
    {
        #region Customer

        /// <summary>
        /// Represents system name of notification about new registration
        /// </summary>
        public const string CustomerRegisteredNotification = "NewCustomer.Notification";

        /// <summary>
        /// Represents system name of customer welcome message
        /// </summary>
        public const string CustomerWelcomeMessage = "Customer.WelcomeMessage";

        /// <summary>
        /// Represents system name of email validation message
        /// </summary>
        public const string CustomerEmailValidationMessage = "Customer.EmailValidationMessage";

        /// <summary>
        /// Represents system name of email revalidation message
        /// </summary>
        public const string CustomerEmailRevalidationMessage = "Customer.EmailRevalidationMessage";

        /// <summary>
        /// Represents system name of password recovery message
        /// </summary>
        public const string CustomerPasswordRecoveryMessage = "Customer.PasswordRecovery";

        #endregion

        #region Order

        /// <summary>
        /// Represents system name of notification vendor about placed order
        /// </summary>
        public const string OrderPlacedVendorNotification = "OrderPlaced.VendorNotification";

        /// <summary>
        /// Represents system name of notification store owner about placed order
        /// </summary>
        public const string OrderPlacedStoreOwnerNotification = "OrderPlaced.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification affiliate about placed order
        /// </summary>
        public const string OrderPlacedAffiliateNotification = "OrderPlaced.AffiliateNotification";

        /// <summary>
        /// Represents system name of notification store owner about paid order
        /// </summary>
        public const string OrderPaidStoreOwnerNotification = "OrderPaid.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification customer about paid order
        /// </summary>
        public const string OrderPaidCustomerNotification = "OrderPaid.CustomerNotification";

        /// <summary>
        /// Represents system name of notification vendor about paid order
        /// </summary>
        public const string OrderPaidVendorNotification = "OrderPaid.VendorNotification";

        /// <summary>
        /// Represents system name of notification affiliate about paid order
        /// </summary>
        public const string OrderPaidAffiliateNotification = "OrderPaid.AffiliateNotification";

        /// <summary>
        /// Represents system name of notification customer about placed order
        /// </summary>
        public const string OrderPlacedCustomerNotification = "OrderPlaced.CustomerNotification";
        
        /// <summary>
        /// Represents system name of notification customer about placed order
        /// </summary>
        public const string OrderPreparingCustomerNotification = "OrderPreparing.CustomerNotification";

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
        /// Represents system name of notification store owner about refunded order
        /// </summary>
        public const string OrderRefundedStoreOwnerNotification = "OrderRefunded.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification customer about refunded order
        /// </summary>
        public const string OrderRefundedCustomerNotification = "OrderRefunded.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about new order note
        /// </summary>
        public const string NewOrderNoteAddedCustomerNotification = "Customer.NewOrderNote";

        /// <summary>
        /// Represents system name of notification store owner about cancelled recurring order
        /// </summary>
        public const string RecurringPaymentCancelledStoreOwnerNotification = "RecurringPaymentCancelled.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification customer about cancelled recurring order
        /// </summary>
        public const string RecurringPaymentCancelledCustomerNotification = "RecurringPaymentCancelled.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about failed payment for the recurring payments
        /// </summary>
        public const string RecurringPaymentFailedCustomerNotification = "RecurringPaymentFailed.CustomerNotification";

        public const string OrderCancelledVendorNotification = "OrderCancelled.VendorNotification";
        public const string OrderCompletedVendorNotification = "OrderCompleted.VendorNotification";
        public const string OrderReturnRefundVendorNotification = "OrderReturnRefund.VendorNotification";
        public const string OrderDisputeOutcomeNoRefundVendorNotification = "OrderDisputeOutcomeNoRefund.VendorNotification";
        public const string OrderDisputeOutcomeFullRefundVendorNotification = "OrderDisputeOutcomeFullRefund.VendorNotification";
        public const string OrderDisputeOutcomeFullRefundAndReturnVendorNotification = "OrderDisputeOutcomeFullRefundAndReturn.VendorNotification";
        public const string OrderDisputeOutcomePartialRefundAndReturnVendorNotification = "OrderDisputeOutcomePartialRefundAndReturn.VendorNotification";
        public const string OrderDisputeOutcomePartialRefundVendorNotification = "OrderDisputeOutcomePartialRefund.VendorNotification";

        #endregion

        #region Newsletter

        /// <summary>
        /// Represents system name of subscription activation message
        /// </summary>
        public const string NewsletterSubscriptionActivationMessage = "NewsLetterSubscription.ActivationMessage";

        /// <summary>
        /// Represents system name of subscription deactivation message
        /// </summary>
        public const string NewsletterSubscriptionDeactivationMessage = "NewsLetterSubscription.DeactivationMessage";

        #endregion

        #region To friend

        /// <summary>
        /// Represents system name of 'Email a friend' message
        /// </summary>
        public const string EmailAFriendMessage = "Service.EmailAFriend";

        /// <summary>
        /// Represents system name of 'Email a friend' message with wishlist
        /// </summary>
        public const string WishlistToFriendMessage = "Wishlist.EmailAFriend";

        #endregion

        #region Return requests

        /// <summary>
        /// Represents system name of notification store owner about new return request
        /// </summary>
        public const string NewReturnRequestStoreOwnerNotification = "NewReturnRequest.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification customer about new return request
        /// </summary>
        public const string NewReturnRequestCustomerNotification = "NewReturnRequest.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about changing return request status
        /// </summary>
        public const string ReturnRequestStatusChangedCustomerNotification = "ReturnRequestStatusChanged.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about dispute raised by vendor
        /// </summary>
        public const string OrderDisputeRaisedCustomerNotification = "OrderDisputeRaised.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about order return is approved and product need to be returned
        /// </summary>
        public const string OrderReturnApproveCustomerNotification = "OrderReturnApprove.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about order refund is approved
        /// </summary>
        public const string OrderRefundApproveCustomerNotification = "OrderRefundApprove.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about order refund is rejected after dispute
        /// </summary>
        public const string OrderDisputeOutcomeNoRefundCustomerNotification = "OrderDisputeOutcomeNoRefund.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about order refund is approved full refund after dispute
        /// </summary>
        public const string OrderDisputeOutcomeFullRefundCustomerNotification = "OrderDisputeOutcomeFullRefund.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about order refund is approved full refund with return after dispute
        /// </summary>
        public const string OrderDisputeOutcomeFullRefundAndReturnCustomerNotification = "OrderDisputeOutcomeFullRefundAndReturn.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about order refund is approved partial refund with return after dispute
        /// </summary>
        public const string OrderDisputeOutcomePartialRefundAndReturnCustomerNotification = "OrderDisputeOutcomePartialRefundAndReturn.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about order refund is approved partial refund after dispute
        /// </summary>
        public const string OrderDisputeOutcomePartialRefundCustomerNotification = "OrderDisputeOutcomePartialRefund.CustomerNotification";

        #endregion

        #region Forum

        /// <summary>
        /// Represents system name of notification about new forum topic
        /// </summary>
        public const string NewForumTopicMessage = "Forums.NewForumTopic";

        /// <summary>
        /// Represents system name of notification about new forum post
        /// </summary>
        public const string NewForumPostMessage = "Forums.NewForumPost";

        /// <summary>
        /// Represents system name of notification about new private message
        /// </summary>
        public const string PrivateMessageNotification = "Customer.NewPM";

        #endregion

        #region Misc

        /// <summary>
        /// Represents system name of notification store owner about applying new vendor account
        /// </summary>
        public const string NewVendorAccountApplyStoreOwnerNotification = "VendorAccountApply.StoreOwnerNotification";
        public const string NewVendorAccountApproveStoreOwnerNotification = "VendorAccountApprove.StoreOwnerNotification";
        public const string NewVendorAccountRejectStoreOwnerNotification = "VendorAccountReject.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification vendor about changing information
        /// </summary>
        public const string VendorInformationChangeNotification = "VendorInformationChange.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification about gift card
        /// </summary>
        public const string GiftCardNotification = "GiftCard.Notification";

        /// <summary>
        /// Represents system name of notification store owner about new product review
        /// </summary>
        public const string ProductReviewStoreOwnerNotification = "Product.ProductReview";

        /// <summary>
        /// Represents system name of notification customer about product review reply
        /// </summary>
        public const string ProductReviewReplyCustomerNotification = "ProductReview.Reply.CustomerNotification";

        /// <summary>
        /// Represents system name of notification store owner about below quantity of product
        /// </summary>
        public const string QuantityBelowStoreOwnerNotification = "QuantityBelow.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification store owner about below quantity of product attribute combination
        /// </summary>
        public const string QuantityBelowAttributeCombinationStoreOwnerNotification = "QuantityBelow.AttributeCombination.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification store owner about submitting new VAT
        /// </summary>
        public const string NewVatSubmittedStoreOwnerNotification = "NewVATSubmitted.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification store owner about new blog comment
        /// </summary>
        public const string BlogCommentNotification = "Blog.BlogComment";

        /// <summary>
        /// Represents system name of notification store owner about new news comment
        /// </summary>
        public const string NewsCommentNotification = "News.NewsComment";

        /// <summary>
        /// Represents system name of notification customer about product receipt
        /// </summary>
        public const string BackInStockNotification = "Customer.BackInStock";

        /// <summary>
        /// Represents system name of 'Contact us' message
        /// </summary>
        public const string ContactUsMessage = "Service.ContactUs";

        /// <summary>
        /// Represents system name of 'Contact vendor' message
        /// </summary>
        public const string ContactVendorMessage = "Service.ContactVendor";

        /// <summary>
        /// Represents system name of 'product publish approval request' message
        /// </summary>
        public const string ProductPublishApprovalRequestOperatorNotification = "ProductPublish.ApprovalRequest";

        /// <summary>
        /// Represents system name of 'product publish approved' message
        /// </summary>
        public const string ProductPublishApprovedStoreOwnerNotification = "ProductPublish.Approved";

        /// <summary>
        /// Represents system name of 'product publish rejected' message
        /// </summary>
        public const string ProductPublishRejectedStoreOwnerNotification = "ProductPublish.Rejected";
        /// <summary>
        /// Represents system name of 'low product stock' message
        /// </summary>
        public const string QuantityBelowVendorNotification = "QuantityBelow.VendorNotification";
        /// <summary>
        /// Represents system name of 'low product attribute stock' message
        /// </summary>
        public const string QuantityBelowAttributeCombinationVendorNotification = "QuantityBelow.AttributeCombination.VendorNotification";

        #endregion

        #region Bank

        /// <summary>
        /// Represents system name of notification customer about approval for customer bank application
        /// </summary>
        public const string CustomerBankAccountApplicationApprovedMessage = "Customer.BankAccount.Approved";

        /// <summary>
        /// Represents system name of notification customer about refusal for customer bank application 
        /// </summary>
        public const string CustomerBankAccountApplicationRejectedMessage = "Customer.BankAccount.Rejected";


        #endregion

        #region Payout

        /// <summary>
        /// Represents system name of notification vendor about new payout
        /// </summary>
        public const string NewPayoutVendorNotification = "NewPayout.VendorNotification";

        #endregion

        #region Shipment

        /// <summary>
        /// Represents system name of notification vendor about reminder arrange driver
        /// </summary>
        public const string ReminderArrangeDriverRequestNowVendorNotification = "ReminderArrangeDriverRequestNow.VendorNotification";

        /// <summary>
        /// Represents system name of notification vendor about reminder arrange driver
        /// </summary>
        public const string UnableToLocateDriverVendorNotification = "UnableToLocateDriver.VendorNotification";

        #endregion
        
    }
}