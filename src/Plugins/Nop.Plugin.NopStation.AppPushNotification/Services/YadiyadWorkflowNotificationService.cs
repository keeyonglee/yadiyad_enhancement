using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payout;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using Nop.Services.Messages;
using Nop.Services.ShuqOrders;

namespace Nop.Plugin.NopStation.AppPushNotification.Services
{
    public partial class WorkflowNotificationService : IWorkflowNotificationService
    {

        public IList<int> SendOrderStartedPreparingNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderPreparingCustomerNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, _customerService.GetCustomerById(order.CustomerId));

            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(order.CustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        public IList<int> SendOrderReturnCustomerNotification(GroupReturnRequest groupReturnRequest, int languageId)
        {
            if (groupReturnRequest == null)
                throw new ArgumentNullException(nameof(groupReturnRequest));

            var order = _orderProcessingService.GetOrder(groupReturnRequest);
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderReturnApproveCustomerNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddOrderSettingsTokens(commonTokens);
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, _customerService.GetCustomerById(order.CustomerId));

            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(order.CustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        public IList<int> SendOrderRefundCustomerNotification(GroupReturnRequest groupReturnRequest, int languageId)
        {
            if (groupReturnRequest == null)
                throw new ArgumentNullException(nameof(groupReturnRequest));

            var order = _orderProcessingService.GetOrder(groupReturnRequest);
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderRefundApproveCustomerNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddOrderSettingsTokens(commonTokens);
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, _customerService.GetCustomerById(order.CustomerId));

            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(order.CustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        public IList<int> SendOrderDisputeRaisedCustomerNotification(Dispute dispute, int languageId)
        {
            if (dispute == null)
                throw new ArgumentNullException(nameof(dispute));

            var order = _orderService.GetOrderById(dispute.OrderId);
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            
            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderDisputeRaisedCustomerNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, _customerService.GetCustomerById(order.CustomerId));

            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(order.CustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }
        
        public IList<int> SendOrderDisputeSettlementCustomerNotification(Dispute dispute, int languageId)
        {
            if (dispute == null)
                throw new ArgumentNullException(nameof(dispute));

            var order = _orderService.GetOrderById(dispute.OrderId);
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            
            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderDisputeOutcomeCustomerNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddDisputeTokens(commonTokens, dispute);
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, _customerService.GetCustomerById(order.CustomerId));

            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(order.CustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        public IList<int> SendOrderDisputeOutcomeFullRefundAndReturnAppCustomerNotification(Dispute dispute, int languageId)
        {
            if (dispute == null)
                throw new ArgumentNullException(nameof(dispute));

            var order = _orderService.GetOrderById(dispute.OrderId);
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderDisputeOutcomeFullRefundAndReturnAppCustomerNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddDisputeTokens(commonTokens, dispute);
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, _customerService.GetCustomerById(order.CustomerId));
            _pushNotificationTokenProvider.AddOrderSettingsTokens(commonTokens);

            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(order.CustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        public IList<int> SendOrderDisputeOutcomePartialRefundAndReturnAppCustomerNotification(Dispute dispute, int languageId)
        {
            if (dispute == null)
                throw new ArgumentNullException(nameof(dispute));

            var order = _orderService.GetOrderById(dispute.OrderId);
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderDisputeOutcomePartialRefundAndReturnAppCustomerNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddDisputeTokens(commonTokens, dispute);
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, _customerService.GetCustomerById(order.CustomerId));
            _pushNotificationTokenProvider.AddOrderSettingsTokens(commonTokens);

            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(order.CustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        public IList<int> SendOrderDisputeOutcomeRejectedAppCustomerNotification(Dispute dispute, int languageId)
        {
            if (dispute == null)
                throw new ArgumentNullException(nameof(dispute));

            var order = _orderService.GetOrderById(dispute.OrderId);
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderDisputeOutcomeNoRefundAppCustomerNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddDisputeTokens(commonTokens, dispute);
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, _customerService.GetCustomerById(order.CustomerId));

            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(order.CustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        public IList<int> SendOrderDisputeOutcomeFullRefundAppCustomerNotification(Dispute dispute, int languageId)
        {
            if (dispute == null)
                throw new ArgumentNullException(nameof(dispute));

            var order = _orderService.GetOrderById(dispute.OrderId);
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderDisputeOutcomeFullRefundAppCustomerNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddDisputeTokens(commonTokens, dispute);
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, _customerService.GetCustomerById(order.CustomerId));

            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(order.CustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        public IList<int> SendOrderDisputeOutcomePartialRefundAppCustomerNotification(Dispute dispute, int languageId)
        {
            if (dispute == null)
                throw new ArgumentNullException(nameof(dispute));

            var order = _orderService.GetOrderById(dispute.OrderId);
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderDisputeOutcomePartialRefundAppCustomerNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddDisputeTokens(commonTokens, dispute);
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, _customerService.GetCustomerById(order.CustomerId));

            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(order.CustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }


        public IList<int> SendOrderNewReturnRequestVendorNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var vendor = _vendorService.GetVendorByOrderId(order.Id);
            var vendorCustomerId = _vendorService.GetVendorCustomerIdByOrderId(order.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.ReturnRequestVendorNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddVendorTokens(commonTokens, vendor);

            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(vendorCustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        public IList<int> SendUnableToLocateDriverVendorNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var vendor = _vendorService.GetVendorByOrderId(order.Id);
            var vendorCustomerId = _vendorService.GetVendorCustomerIdByOrderId(order.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.UnableToLocateDriverVendorNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddVendorTokens(commonTokens, vendor);

            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(vendorCustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        public IList<int> SendOrderPayoutRequestVendorNotification(OrderPayoutRequest orderPayoutRequest, 
            PayoutAndGroupShuqDTO payoutAndGroupShuqDTO,
            int languageId)
        {
            if (orderPayoutRequest == null)
                throw new ArgumentNullException(nameof(orderPayoutRequest));

            var order = _orderService.GetOrderById(orderPayoutRequest.OrderId);
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var vendor = _vendorService.GetVendorByOrderId(order.Id);
            var vendorCustomerId = _vendorService.GetVendorCustomerIdByOrderId(order.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.NewPayoutVendorNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddVendorTokens(commonTokens, vendor);
            _pushNotificationTokenProvider.AddOrderPayoutRequest(commonTokens, orderPayoutRequest, languageId);
            _pushNotificationTokenProvider.AddPayoutTokens(commonTokens, payoutAndGroupShuqDTO);

            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(vendorCustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        public IList<int> SendOrderDisputeOutcomeFullRefundAndReturnVendorNotification(Dispute dispute, int languageId)
        {
            if (dispute == null)
                throw new ArgumentNullException(nameof(dispute));

            var order = _orderService.GetOrderById(dispute.OrderId);
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var vendor = _vendorService.GetVendorByOrderId(order.Id);
            var vendorCustomerId = _vendorService.GetVendorCustomerIdByOrderId(order.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderDisputeOutcomeFullRefundAndReturnVendorNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddDisputeTokens(commonTokens, dispute);
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, _customerService.GetCustomerById(order.CustomerId));
            _pushNotificationTokenProvider.AddOrderSettingsTokens(commonTokens);
            _pushNotificationTokenProvider.AddVendorTokens(commonTokens, vendor);


            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(vendorCustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        public IList<int> SendOrderDisputeOutcomePartialRefundAndReturnVendorNotification(Dispute dispute, int languageId)
        {
            if (dispute == null)
                throw new ArgumentNullException(nameof(dispute));

            var order = _orderService.GetOrderById(dispute.OrderId);
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var vendor = _vendorService.GetVendorByOrderId(order.Id);
            var vendorCustomerId = _vendorService.GetVendorCustomerIdByOrderId(order.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderDisputeOutcomePartialRefundAndReturnVendorNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddDisputeTokens(commonTokens, dispute);
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, _customerService.GetCustomerById(order.CustomerId));
            _pushNotificationTokenProvider.AddOrderSettingsTokens(commonTokens);
            _pushNotificationTokenProvider.AddVendorTokens(commonTokens, vendor);


            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(vendorCustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        public IList<int> SendOrderDisputeOutcomeRejectedVendorNotification(Dispute dispute, int languageId)
        {
            if (dispute == null)
                throw new ArgumentNullException(nameof(dispute));

            var order = _orderService.GetOrderById(dispute.OrderId);
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var vendor = _vendorService.GetVendorByOrderId(order.Id);
            var vendorCustomerId = _vendorService.GetVendorCustomerIdByOrderId(order.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderDisputeOutcomeNoRefundVendorNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddDisputeTokens(commonTokens, dispute);
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, _customerService.GetCustomerById(order.CustomerId));
            _pushNotificationTokenProvider.AddVendorTokens(commonTokens, vendor);


            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(vendorCustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        public IList<int> SendOrderDisputeOutcomeFullRefundVendorNotification(Dispute dispute, int languageId)
        {
            if (dispute == null)
                throw new ArgumentNullException(nameof(dispute));

            var order = _orderService.GetOrderById(dispute.OrderId);
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var vendor = _vendorService.GetVendorByOrderId(order.Id);
            var vendorCustomerId = _vendorService.GetVendorCustomerIdByOrderId(order.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderDisputeOutcomeFullRefundVendorNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddDisputeTokens(commonTokens, dispute);
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, _customerService.GetCustomerById(order.CustomerId));
            _pushNotificationTokenProvider.AddVendorTokens(commonTokens, vendor);


            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(vendorCustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        public IList<int> SendOrderDisputeOutcomePartialRefundVendorNotification(Dispute dispute, int languageId)
        {
            if (dispute == null)
                throw new ArgumentNullException(nameof(dispute));

            var order = _orderService.GetOrderById(dispute.OrderId);
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var vendor = _vendorService.GetVendorByOrderId(order.Id);
            var vendorCustomerId = _vendorService.GetVendorCustomerIdByOrderId(order.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderDisputeOutcomePartialRefundVendorNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddDisputeTokens(commonTokens, dispute);
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, _customerService.GetCustomerById(order.CustomerId));
            _pushNotificationTokenProvider.AddVendorTokens(commonTokens, vendor);


            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(vendorCustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }
    }
}