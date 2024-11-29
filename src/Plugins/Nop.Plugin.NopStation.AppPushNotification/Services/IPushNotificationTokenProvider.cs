using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payout;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using Nop.Services.Messages;
using System.Collections.Generic;

namespace Nop.Plugin.NopStation.AppPushNotification.Services
{
    public interface IPushNotificationTokenProvider
    {
        IEnumerable<string> GetTokenGroups(AppPushNotificationTemplate AppPushNotificationTemplate);

        IEnumerable<string> GetListOfAllowedTokens(IEnumerable<string> tokenGroups);

        void AddCustomerTokens(IList<Token> commonTokens, Customer customer);

        void AddStoreTokens(IList<Token> tokens, Store store);

        void AddOrderTokens(IList<Token> commonTokens, Order order, int languageId, int vendorId = 0);

        void AddShipmentTokens(IList<Token> commonTokens, Shipment shipment, int languageId);

        void AddOrderRefundedTokens(IList<Token> commonTokens, Order order, decimal refundedAmount);

        void AddOrderNoteTokens(IList<Token> commonTokens, OrderNote orderNote);

        void AddRecurringPaymentTokens(IList<Token> commonTokens, RecurringPayment recurringPayment);

        void AddNewsLetterSubscriptionTokens(IList<Token> commonTokens, NewsLetterSubscription subscription);

        void AddProductTokens(IList<Token> commonTokens, Product product, int languageId);

        void AddReturnRequestTokens(IList<Token> commonTokens, ReturnRequest returnRequest, OrderItem orderItem);

        void AddForumTopicTokens(IList<Token> commonTokens, ForumTopic forumTopic,
            int? friendlyForumTopicPageIndex = null, int? appendedPostIdentifierAnchor = null);

        void AddForumTokens(IList<Token> commonTokens, Forum forum);

        void AddForumPostTokens(IList<Token> commonTokens, ForumPost forumPost);

        void AddPrivateMessageTokens(IList<Token> commonTokens, PrivateMessage privateMessage);

        void AddVendorTokens(IList<Token> commonTokens, Vendor vendor);

        void AddGiftCardTokens(IList<Token> commonTokens, GiftCard giftCard);

        void AddProductReviewTokens(IList<Token> commonTokens, ProductReview productReview);

        void AddAttributeCombinationTokens(IList<Token> commonTokens, ProductAttributeCombination combination, int languageId);
        
        void AddBackInStockTokens(IList<Token> commonTokens, BackInStockSubscription subscription);
        void AddOrderSettingsTokens(IList<Token> tokens);
        void AddDisputeTokens(List<Token> commonTokens, Dispute dispute);
        void AddOrderPayoutRequest(List<Token> commonTokens, OrderPayoutRequest orderPayoutRequest, int languageId);
        void AddPayoutTokens(List<Token> commonTokens, PayoutAndGroupShuqDTO payoutAndGroupDTO);
    }
}