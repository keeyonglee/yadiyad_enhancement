using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using Nop.Plugin.NopStation.AppPushNotification.Extensions;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.ShuqOrders;
using Nop.Services.Stores;
using Nop.Services.Vendors;

namespace Nop.Plugin.NopStation.AppPushNotification.Services
{
    public partial class WorkflowNotificationService : IWorkflowNotificationService
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IPushNotificationTemplateService _pushNotificationTemplateService;
        private readonly IPushNotificationTokenProvider _pushNotificationTokenProvider;
        private readonly IQueuedPushNotificationService _queuedPushNotificationService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ITokenizer _tokenizer;
        private readonly IPictureService _pictureService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IApiDeviceService _apiDeviceService;
        private readonly IPushNotificationCampaignService _pushNotificationCampaignService;
        private readonly IOrderService _orderService;
        private readonly IShuqOrderProcessingService _orderProcessingService;
        private readonly IVendorService _vendorService;
        private readonly IShuqOrderService _shuqOrderService;
        private readonly VendorNotificationSettings _vendorNotificationSettings;
        private readonly IProductService _productService;

        #endregion

        #region Ctor

        public WorkflowNotificationService(ICustomerService customerService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IPushNotificationTemplateService pushNotificationTemplateService,
            IPushNotificationTokenProvider pushNotificationTokenProvider,
            IQueuedPushNotificationService queuedPushNotificationService,
            IStoreContext storeContext,
            IStoreService storeService,
            ITokenizer tokenizer,
            IPictureService pictureService,
            IGenericAttributeService genericAttributeService,
            IApiDeviceService apiDeviceService,
            IPushNotificationCampaignService pushNotificationCampaignService,
            IOrderService orderService,
            IShuqOrderProcessingService orderProcessingService,
            IVendorService vendorService,
            IShuqOrderService shuqOrderService,
            VendorNotificationSettings vendorNotificationSettings,
            IProductService productService)
        {
            _customerService = customerService;
            _languageService = languageService;
            _localizationService = localizationService;
            _pushNotificationTemplateService = pushNotificationTemplateService;
            _pushNotificationTokenProvider = pushNotificationTokenProvider;
            _queuedPushNotificationService = queuedPushNotificationService;
            _storeContext = storeContext;
            _storeService = storeService;
            _tokenizer = tokenizer;
            _pictureService = pictureService;
            _genericAttributeService = genericAttributeService;
            _apiDeviceService = apiDeviceService;
            _pushNotificationCampaignService = pushNotificationCampaignService;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _vendorService = vendorService;
            _shuqOrderService = shuqOrderService;
            _vendorNotificationSettings = vendorNotificationSettings;
            _productService = productService;
        }

        #endregion

        #region Utilities

        protected virtual IList<AppPushNotificationTemplate> GetActivePushNotificationTemplates(string notificationTemplateName, int storeId)
        {
            //get message templates by the name
            var pushNotificationTemplates = _pushNotificationTemplateService.GetPushNotificationTemplatesByName(notificationTemplateName, storeId);

            //no template found
            if (!pushNotificationTemplates?.Any() ?? true)
                return new List<AppPushNotificationTemplate>();

            //filter active templates
            return pushNotificationTemplates.Where(notificationTemplate => notificationTemplate.Active).ToList();
        }

        protected virtual int EnsureLanguageIsActive(int languageId, int storeId)
        {
            //load language by specified ID
            var language = _languageService.GetLanguageById(languageId);

            if (language == null || !language.Published)
            {
                //load any language from the specified store
                language = _languageService.GetAllLanguages(storeId: storeId).FirstOrDefault();
            }

            if (language == null || !language.Published)
            {
                //load any language
                language = _languageService.GetAllLanguages().FirstOrDefault();
            }

            if (language == null)
                throw new Exception("No active language could be loaded");

            return language.Id;
        }

        #endregion

        #region Methods

        #region Campaigns

        public virtual IList<int> SendCampaignNotification(AppPushNotificationCampaign campaign)
        {
            if (campaign == null)
                throw new ArgumentNullException(nameof(campaign));

            var i = 0;
            var ids = new List<int>();

            var store = _storeService.GetStoreById(campaign.LimitedToStoreId);
            if (store == null)
                store = _storeContext.CurrentStore;

            while (true)
            {
                var devices = _pushNotificationCampaignService.GetCampaignDevices(campaign, i);
                if (!devices.Any())
                    break;

                foreach (var device in devices)
                {
                    var customer = _customerService.GetCustomerById(device.CustomerId);
                    var languageId = customer == null ? 0 : _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.LanguageIdAttribute, store.Id);
                    languageId = EnsureLanguageIsActive(languageId, store.Id);

                    //tokens
                    var commonTokens = new List<Token>();
                    if (customer != null)
                        _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, customer);

                    var tokens = new List<Token>(commonTokens);
                    _pushNotificationTokenProvider.AddStoreTokens(tokens, store);

                    var ds = new List<ApiDevice>() { device };
                    ids.AddRange(SendNotification(ds, campaign, languageId, tokens, store.Id));
                }
                i++;
            }
            return ids;
        }

        #endregion

        #region Customer workflow

        public virtual IList<int> SendCustomerRegisteredNotification(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.CustomerRegisteredNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, customer);

            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(customer, notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        public virtual IList<int> SendCustomerCustomerRegisteredWelcomeNotification(Customer customer, int languageId) //can be expanded using different db table
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.CustomerRegisteredWelcomeNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, customer);

            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(customer, notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        public virtual IList<int> SendCustomerEmailValidationNotification(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.CustomerEmailValidationNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, customer);

            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(customer, notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        public virtual IList<int> SendCustomerCustomerWelcomeNotification(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.CustomerWelcomeNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddCustomerTokens(commonTokens, customer);

            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(customer, notificationTemplate, languageId, tokens, store.Id));
            }

            return ids;
        }

        #endregion

        #region Order workflow

        public virtual IList<int> SendOrderPaidCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderPaidCustomerNotification, store.Id);
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

        public virtual IList<int> SendOrderPlacedCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderPlacedCustomerNotification, store.Id);
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

        public virtual IList<int> SendShipmentSentCustomerNotification(Shipment shipment, int languageId)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            var order = _orderService.GetOrderById(shipment.OrderId);
            if (order == null)
                throw new Exception("Order cannot be loaded");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.ShipmentSentCustomerNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddShipmentTokens(commonTokens, shipment, languageId);
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

        public virtual IList<int> SendShipmentDeliveredCustomerNotification(Shipment shipment, int languageId)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            var order = _orderService.GetOrderById(shipment.OrderId);
            if (order == null)
                throw new Exception("Order cannot be loaded");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.ShipmentDeliveredCustomerNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddShipmentTokens(commonTokens, shipment, languageId);
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

        public virtual IList<int> SendOrderCompletedCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderCompletedCustomerNotification, store.Id);
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

        public virtual IList<int> SendOrderCancelledCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderCancelledCustomerNotification, store.Id);
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

        public virtual IList<int> SendOrderRefundedCustomerNotification(Order order, decimal refundedAmount, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderRefundedCustomerNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _pushNotificationTokenProvider.AddOrderRefundedTokens(commonTokens, order, refundedAmount);
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

        public IList<int> SendOrderPlacedVendorNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var vendor = _vendorService.GetVendorByOrderId(order.Id);
            var vendorCustomerId = _vendorService.GetVendorCustomerIdByOrderId(order.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderPlacedVendorNotification, store.Id);
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

        public IList<int> SendOrderPlacedDeliveryDateTimeVendorNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var vendor = _vendorService.GetVendorByOrderId(order.Id);
            var vendorCustomerId = _vendorService.GetVendorCustomerIdByOrderId(order.Id);
            DateTime? deliveryDateTime = null;
            if (!string.IsNullOrEmpty(order.CheckoutAttributesXml))
            {
                deliveryDateTime = _shuqOrderService.GetCheckoutDateTimeSlot(order);
                deliveryDateTime = deliveryDateTime.Value.AddMinutes(- _vendorNotificationSettings.ArrangeDriverRequestAdvanceMinutes).AddHours(-8);
            }

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.ReminderArrangeDriverRequestNowVendorNotification, store.Id);
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
                ids.AddRange(SendNotification(_customerService.GetCustomerById(vendorCustomerId), notificationTemplate, languageId, tokens, store.Id, deliveryDateTime));
            }
            return ids;
        }

        public IList<int> SendOrderPlacedDeliveryDateTimeReminderVendorNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var vendor = _vendorService.GetVendorByOrderId(order.Id);
            var vendorCustomerId = _vendorService.GetVendorCustomerIdByOrderId(order.Id);
            DateTime? deliveryDateTime = null;
            if (!string.IsNullOrEmpty(order.CheckoutAttributesXml))
            {
                deliveryDateTime = _shuqOrderService.GetCheckoutDateTimeSlot(order);
                deliveryDateTime = deliveryDateTime.Value.AddHours(- _vendorNotificationSettings.ArrangeDriverRequestAdvanceHours).AddHours(-8);
            }

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.ReminderArrangeDriverRequestInAdvanceVendorNotification, store.Id);
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
                ids.AddRange(SendNotification(_customerService.GetCustomerById(vendorCustomerId), notificationTemplate, languageId, tokens, store.Id, deliveryDateTime));
            }
            return ids;
        }

        public virtual IList<int> SendOrderCompletedVendorNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var vendor = _vendorService.GetVendorByOrderId(order.Id);
            var vendorCustomerId = _vendorService.GetVendorCustomerIdByOrderId(order.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderCompletedVendorNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
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

        public virtual IList<int> SendOrderCancelledVendorNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var vendor = _vendorService.GetVendorByOrderId(order.Id);
            var vendorCustomerId = _vendorService.GetVendorCustomerIdByOrderId(order.Id);

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.OrderCancelledVendorNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _pushNotificationTokenProvider.AddOrderTokens(commonTokens, order, languageId);
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
        
        public virtual IList<int> SendQuantityBelowVendorNotification(Product product, Vendor vendor, int languageId)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));
            
            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var vendorCustomerId = _vendorService.GetCustomerIdByVendor(vendor);
            
            if (vendorCustomerId == 0)
                throw new ArgumentNullException(nameof(vendorCustomerId));

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.QuantityBelowVendorNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            
            _pushNotificationTokenProvider.AddProductTokens(commonTokens, product, languageId);
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
        
        public virtual IList<int> SendQuantityBelowVendorNotification(ProductAttributeCombination combination, Vendor vendor, int languageId)
        {
            if (combination == null)
                throw new ArgumentNullException(nameof(combination));
            
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));
            
            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var vendorCustomerId = _vendorService.GetCustomerIdByVendor(vendor);
            
            if (vendorCustomerId == 0)
                throw new ArgumentNullException(nameof(vendorCustomerId));

            var notificationTemplates = GetActivePushNotificationTemplates(AppPushNotificationTemplateSystemNames.QuantityBelowAttributeCombinationVendorNotification, store.Id);
            if (!notificationTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            var product = _productService.GetProductById(combination.ProductId);
            
            _pushNotificationTokenProvider.AddProductTokens(commonTokens, product, languageId);
            _pushNotificationTokenProvider.AddVendorTokens(commonTokens, vendor);
            _pushNotificationTokenProvider.AddAttributeCombinationTokens(commonTokens, combination, languageId);

            var ids = new List<int>();
            foreach (var notificationTemplate in notificationTemplates)
            {
                var tokens = new List<Token>(commonTokens);
                _pushNotificationTokenProvider.AddStoreTokens(tokens, store);
                ids.AddRange(SendNotification(_customerService.GetCustomerById(vendorCustomerId), notificationTemplate, languageId, tokens, store.Id));
            }
            return ids;
        }

        #endregion

        #region Misc

        public virtual IList<int> SendNotification(Customer customer, 
            AppPushNotificationTemplate template, int languageId, IEnumerable<Token> tokens, int storeId, DateTime? sendTime = null)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));

            var language = _languageService.GetLanguageById(languageId);

            var title = _localizationService.GetLocalized(template, mt => mt.Title, languageId);
            var titleReplaced = _tokenizer.Replace(title, tokens, true);

            var body = _localizationService.GetLocalized(template, mt => mt.Body, languageId);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);

            var valueReplaced = !string.IsNullOrWhiteSpace(template.ActionValue) ? 
                _tokenizer.Replace(template.ActionValue, tokens, true) : null;

            var imageUrl = _pictureService.GetPictureUrl(template.ImageId, showDefaultPicture: false);
            if (string.IsNullOrWhiteSpace(imageUrl))
                imageUrl = null;

            if (sendTime == null)
            {
                sendTime = template.SendImmediately || !template.DelayBeforeSend.HasValue ? null
                    : (DateTime?)(DateTime.UtcNow + TimeSpan.FromMinutes(template.DelayPeriod.ToMinutes(template.DelayBeforeSend.Value)));
            }

            var devices = _apiDeviceService.SearchApiDevices(customer.Id, template.AppTypeId);
            return SendNotification(devices, titleReplaced, bodyReplaced, imageUrl, 
                template.ActionType, valueReplaced, storeId, template.AppTypeId, sendTime);
        }

        public virtual IList<int> SendNotification(IList<ApiDevice> devices, 
            AppPushNotificationCampaign campaign, int languageId, IEnumerable<Token> tokens, int storeId)
        {
            if (campaign == null)
                throw new ArgumentNullException(nameof(campaign));

            var language = _languageService.GetLanguageById(languageId);

            var title = _localizationService.GetLocalized(campaign, mt => mt.Title, languageId);
            var titleReplaced = _tokenizer.Replace(title, tokens, true);

            var body = _localizationService.GetLocalized(campaign, mt => mt.Body, languageId);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);

            var imageUrl = _pictureService.GetPictureUrl(campaign.ImageId, showDefaultPicture: false);
            if (string.IsNullOrWhiteSpace(imageUrl))
                imageUrl = null;

            return SendNotification(devices, titleReplaced, bodyReplaced, imageUrl, campaign.ActionType,
                campaign.ActionValue, storeId, devices[0].AppTypeId);
        }

        public IList<int> SendNotification(IList<ApiDevice> devices, string title, string body, 
            string imageUrl, NotificationActionType actionType, string actionValue, 
            int storeId, int appTypeId, DateTime? sendTime = null)
        {
            var guid = Guid.NewGuid();
            var ids = new List<int>();
            foreach (var device in devices)
            {
                var queuedPushNotification = new AppQueuedPushNotification
                {
                    Body = body,
                    CreatedOnUtc = DateTime.UtcNow,
                    CustomerId = device.CustomerId,
                    StoreId = storeId,
                    Title = title,
                    ImageUrl = imageUrl,
                    DontSendBeforeDateUtc = sendTime,
                    AppDeviceId = device.Id,
                    ActionValue = actionValue,
                    ActionType = actionType,
                    DeviceTypeId = device.DeviceTypeId,
                    SubscriptionId = device.SubscriptionId,
                    UniqueNotificationId = guid.ToString(),
                    AppTypeId = appTypeId
                };

                _queuedPushNotificationService.InsertQueuedPushNotification(queuedPushNotification);
                ids.Add(queuedPushNotification.Id);
            }

            return ids;
        }

        #endregion

        #endregion
    }
}