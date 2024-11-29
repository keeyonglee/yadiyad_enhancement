using System;
using System.Collections.Generic;
using System.Linq;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.ShippingShuq;
using Nop.Core.Domain.ShippingShuq.DTO;
using Nop.Core.Domain.Tax;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Documents;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.ShippingShuq;
using Nop.Services.Tax;
using Nop.Services.Vendors;

namespace Nop.Services.ShuqOrders
{
    public class ShuqOrderProcessingService : OrderProcessingService, IShuqOrderProcessingService
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IShuqOrderService _orderService;
        private readonly IShipmentService _shipmentService;
        private readonly ShippingSettings _shippingSettings;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IShipmentProcessor _shipmentProcessor;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ICustomerService _customerService;
        private readonly IVendorService _vendorService;
 
        #endregion

        #region Ctor

        public ShuqOrderProcessingService(CurrencySettings currencySettings, IAddressService addressService,
            IAffiliateService affiliateService, ICategoryService categoryService,
            ICheckoutAttributeFormatter checkoutAttributeFormatter, ICountryService countryService,
            ICurrencyService currencyService, ICustomerActivityService customerActivityService,
            ICustomerService customerService, ICustomNumberFormatter customNumberFormatter,
            IDiscountService discountService, IEncryptionService encryptionService, IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService, IGiftCardService giftCardService,
            ILanguageService languageService, ILocalizationService localizationService, ILogger logger,
            IShuqOrderService orderService, IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentPluginManager paymentPluginManager, IPaymentService paymentService, IPdfService pdfService,
            IPriceCalculationService priceCalculationService, IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter, IProductAttributeParser productAttributeParser,
            IProductService productService, IRewardPointService rewardPointService,
            IShipmentService shipmentService, IShippingService shippingService,
            IShoppingCartService shoppingCartService, IStateProvinceService stateProvinceService,
            ITaxService taxService, IVendorService vendorService, IWebHelper webHelper, IWorkContext workContext,
            IWorkflowMessageService workflowMessageService, LocalizationSettings localizationSettings,
            OrderSettings orderSettings, PaymentSettings paymentSettings, RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings, TaxSettings taxSettings, IHttpContextAccessor httpContextAccessor,
            IReturnRequestService returnRequestService,
            DocumentNumberService documentNumberService, IShipmentProcessor shipmentProcessor, IVendorAttributeService vendorAttributeService, ShipmentCarrierResolver shipmentCarrierResolver, IVendorAttributeParser vendorAttributeParser) :
            base(currencySettings, addressService, affiliateService, categoryService, checkoutAttributeFormatter,
                countryService, currencyService, customerActivityService, customerService, customNumberFormatter,
                discountService, encryptionService, eventPublisher, genericAttributeService, giftCardService,
                languageService, localizationService, logger, orderService, orderTotalCalculationService,
                paymentPluginManager, paymentService, pdfService, priceCalculationService, priceFormatter,
                productAttributeFormatter, productAttributeParser, productService, rewardPointService, shipmentService,
                shippingService, shoppingCartService, stateProvinceService, taxService, vendorService, webHelper,
                workContext, workflowMessageService, localizationSettings, orderSettings, paymentSettings,
                rewardPointsSettings, shippingSettings, taxSettings, httpContextAccessor, returnRequestService, documentNumberService, vendorAttributeService, shipmentCarrierResolver, vendorAttributeParser)
        {
            _categoryService = categoryService;
            _eventPublisher = eventPublisher;
            _orderService = orderService;
            _shipmentService = shipmentService;
            _shippingSettings = shippingSettings;
            _returnRequestService = returnRequestService;
            _shipmentProcessor = shipmentProcessor;
            _workflowMessageService = workflowMessageService;
            _customerService = customerService;
            _vendorService = vendorService;
        }

        #endregion

        public virtual void SaveActualShippingCost(Order order, decimal shippingCost)
        {
            if (order == null)
                return;

            if (order.ActualOrderShippingExclTax == shippingCost)
                return;

            order.ActualOrderShippingExclTax = shippingCost;
            order.ActualOrderShippingInclTax = shippingCost;
            _orderService.UpdateOrder(order);
        }

        public void SaveActualShippingCost(ReturnOrder order, decimal shippingCost)
        {
            if (order == null)
                return;

            if (order.ActualShippingExclTax == shippingCost)
                return;

            order.ActualShippingExclTax = shippingCost;
            order.ActualShippingInclTax = shippingCost;
            _returnRequestService.UpdateReturnOrder(order);
        }

        protected virtual Shipment AddReturnShipment(ReturnOrder returnOrder)
        {
            if (returnOrder == null)
                return default;

            var order = GetOrder(returnOrder);
            // should be at least one fulfilment shipment to create return shipment
            var shipment = _shipmentService.GetShipmentsByOrderId(order.Id).First();

            var returnShipment = new Shipment
            {
                ShipmentType = ShipmentType.Return,
                ReturnOrderId = returnOrder.Id,
                OrderId = shipment.OrderId,
                TotalWeight = shipment.TotalWeight,
                ShippingMethodId = shipment.ShippingMethodId,
                CreatedOnUtc = DateTime.UtcNow,
                ShippingTotal = shipment.ShippingTotal,
                RequireInsurance = false, // Insurance not required for returns
                DeliveryMode = shipment.DeliveryMode,
            };
            _shipmentService.InsertShipment(returnShipment);
            return returnShipment;
        }

        public virtual void ApproveReturnRequest(GroupReturnRequest groupReturnRequest, bool afterDispute = false)
        {
            var order = GetOrder(groupReturnRequest);
            // Check Whether Customer Need to Return the items
            if (!groupReturnRequest.NeedReturnShipping
                || !HasShippableItems(groupReturnRequest)
                || !HasReturnableProducts(groupReturnRequest))
            {
                groupReturnRequest.NeedReturnShipping = false;

                // Complete the order
                
                order.OrderStatus = OrderStatus.Complete;
                _orderService.UpdateOrder(order);
                
                CheckOrderStatus(order);
            }
            else
            {
                ProcessReturn(groupReturnRequest);
            }

            var result = _returnRequestService.Approve(groupReturnRequest);

            if (result && !afterDispute)
            {
                _eventPublisher.Publish(new ReturnRequestApprovedEvent(groupReturnRequest));
                if (groupReturnRequest.NeedReturnShipping)
                {
                    _workflowMessageService.SendOrderReturnApproveCustomerNotification(order);
                }
                else
                {
                    _workflowMessageService.SendOrderRefundApproveCustomerNotification(order);
                }
            }
                
        }

        public void ProcessShipment(Shipment shipment, ShipmentDetailDTO response)
        {
            // get the latest from db before processing
            shipment = _shipmentService.GetShipmentById(shipment.Id);
            if (shipment.ShipmentType == ShipmentType.Fulfillment)
            {
                ProcessShipmentForFulfillmentOrder(shipment, response);
            }
            else
            {
                ProcessShipmentForReturnOrder(shipment, response);
            }
        }

        public void CompleteReturn(GroupReturnRequest groupReturnRequest)
        {
            if (groupReturnRequest == null)
                return;

            var order = GetOrder(groupReturnRequest);

            groupReturnRequest.ReturnCondition = ReturnConditionEnum.Mint;
            groupReturnRequest.ApproveStatus = ApproveStatusEnum.Approved;
            groupReturnRequest.ApprovalDateUtc = DateTime.UtcNow;
            _returnRequestService.UpdateGroupReturnRequest(groupReturnRequest);

            var returnRequests = _returnRequestService.GetReturnRequestByGroupReturnRequestId(groupReturnRequest.Id);
            foreach (var returnRequest in returnRequests)
            {
                returnRequest.ReturnRequestStatus = ReturnRequestStatus.Received;
                _returnRequestService.UpdateReturnRequest(returnRequest);
            }

            order.OrderStatus = OrderStatus.Complete;
            _orderService.UpdateOrder(order);
        }

        public void CancelReturn(Shipment shipment)
        {
            if (shipment == null)
                return;

            if (shipment.ShipmentType != ShipmentType.Return)
                return;

            var rOrder = _returnRequestService.GetReturnOrderById(shipment.ReturnOrderId ?? 0);
            var order = _orderService.GetOrderById(shipment.OrderId);

            var returnRequests = _returnRequestService.GetReturnRequestByGroupReturnRequestId(rOrder.GroupReturnRequestId);
            foreach (var returnRequest in returnRequests)
            {
                returnRequest.ReturnRequestStatus = ReturnRequestStatus.Cancelled;
                _returnRequestService.UpdateReturnRequest(returnRequest);
            }

            order.OrderStatus = OrderStatus.Complete;
            _orderService.UpdateOrder(order);
        }
        
        public virtual Order GetOrder(GroupReturnRequest groupReturnRequest)
        {
            var returnRequests = _returnRequestService.GetReturnRequestByGroupReturnRequestId(groupReturnRequest.Id);
            return _orderService.GetOrderByOrderItem(returnRequests.First().OrderItemId);
        }

        protected virtual void ProcessShipmentForFulfillmentOrder(Shipment shipment, ShipmentDetailDTO response)
        {
            var order = _orderService.GetOrderById(shipment.OrderId);
            SaveActualShippingCost(order, response.ShippingCost);

            if (response.CurrentStatus == CarrierShippingStatus.PickedUp)
                Ship(shipment, true);

            if (response.CurrentStatus == CarrierShippingStatus.InProcess)
                return;

            if (response.CurrentStatus == CarrierShippingStatus.Delivered)
            {
                if (order.ShippingStatus == ShippingStatus.NotYetShipped)
                    Ship(shipment, false);

                Deliver(shipment, true);
            }

            if (response.CurrentStatus == CarrierShippingStatus.Aborted)
                RetryShipment(shipment);
        }

        protected virtual void ProcessShipmentForReturnOrder(Shipment shipment, ShipmentDetailDTO response)
        {
            var rOrder = _returnRequestService.GetReturnOrderById(shipment.ReturnOrderId ?? 0);
            SaveActualShippingCost(rOrder, response.ShippingCost);

            if (response.CurrentStatus == CarrierShippingStatus.PickedUp)
            {
                rOrder.IsShipped = true;
                _returnRequestService.UpdateReturnOrder(rOrder);

                shipment.ShippedDateUtc = DateTime.UtcNow;
                _shipmentService.UpdateShipment(shipment);
            }

            if (response.CurrentStatus == CarrierShippingStatus.InProcess)
                return;

            if (response.CurrentStatus == CarrierShippingStatus.Delivered)
                DeliverReturnShipment(shipment, rOrder);

            if (response.CurrentStatus == CarrierShippingStatus.Aborted)
                RetryShipment(shipment);
        }


        protected virtual void DeliverReturnShipment(Shipment shipment, ReturnOrder rOrder)
        {
            // Set Return Order Shipped
            var groupReturnRequest = _returnRequestService.GetGroupReturnRequestById(rOrder.GroupReturnRequestId);

            if (groupReturnRequest.ReturnCondition != ReturnConditionEnum.Pending)
            {
                groupReturnRequest.ReturnCondition = ReturnConditionEnum.Pending;
                _returnRequestService.UpdateGroupReturnRequest(groupReturnRequest);
            }

            shipment.ShippedDateUtc ??= DateTime.UtcNow;
            shipment.DeliveryDateUtc ??= DateTime.UtcNow;

            _shipmentService.UpdateShipment(shipment);
        }

        protected virtual void RetryShipment(Shipment shipment)
        {
            shipment.TrackingNumber = null;
            shipment.RetryCount += 1;
            shipment.ShippedDateUtc = null;

            //remove tracking number and retry
            _shipmentService.UpdateShipment(shipment);

            if (shipment.RetryCount >= _shippingSettings.MaxAutoRetries)
            {
                FailShipment(shipment);
                return;
            }

            _shipmentProcessor.Ship(shipment);
        }

        protected virtual void FailShipment(Shipment shipment)
        {
            if (shipment.ShipmentType == ShipmentType.Fulfillment)
            {
                var order = _orderService.GetOrderById(shipment.OrderId);
                var vendor = _vendorService.GetVendorByOrderId(order.Id);
                if (order.OrderStatus == OrderStatus.Processing)
                {
                    order.ShippingStatus = ShippingStatus.Failed;
                }
                _orderService.UpdateOrder(order);

                //Notifications
                _eventPublisher.Publish(new UnableToLocateDriverEvent(order));
                _workflowMessageService.SendUnableToLocateDriverVendorNotification(order, vendor, order.CustomerLanguageId);
            }
            else
            {
                var rOrder = _returnRequestService.GetReturnOrderById(shipment.ReturnOrderId ?? 0);
                rOrder.ShippingStatus = ShippingStatus.Failed;
                _returnRequestService.UpdateReturnOrder(rOrder);
            }
        }

        protected virtual void ProcessReturn(GroupReturnRequest groupReturnRequest)
        {
            // create return order
            var returnOrder = CreateReturnOrder(groupReturnRequest);

            // create shipment
            var shipment = AddReturnShipment(returnOrder);

            if (shipment == default)
                return;

            _shipmentProcessor.Ship(shipment);
        }

        protected virtual bool HasReturnableProducts(GroupReturnRequest groupReturnRequest)
        {
            var categories = _returnRequestService.GetProductCategories(groupReturnRequest);

            if (categories?.Count == 0)
                return false;

            // return required if not all categories doesnt require return
            return !categories
                    .All(category =>
                        // return not required if atleast one parent category doesnt require return
                        _categoryService.GetCategoryBreadCrumb(category)
                            .Any(c => c.NonReturnable));

        }
        

        protected virtual IList<ReturnOrder> GetReturnOrders(GroupReturnRequest groupReturnRequest)
        {
            return _returnRequestService.GetReturnOrderByGroupReturnRequestId(groupReturnRequest.Id);
        }

        protected virtual IList<ReturnRequest> GetReturnRequests(GroupReturnRequest groupReturnRequest)
        {
            return _returnRequestService.GetReturnRequestByGroupReturnRequestId(groupReturnRequest.Id);
        }

        protected virtual Order GetOrder(ReturnOrder returnOrder)
        {
            var groupReturnRequest = _returnRequestService.GetGroupReturnRequestById(returnOrder.GroupReturnRequestId);
            return GetOrder(groupReturnRequest);
        }

        public virtual bool HasShippableItems(Order order)
        {
            return _orderService
                .GetOrderItems(order.Id, isShipEnabled: true)
                .Any();
        }

        protected virtual bool HasShippableItems(GroupReturnRequest groupReturnRequest)
        {
            var returnRequests = GetReturnRequests(groupReturnRequest);
            var order = GetOrder(groupReturnRequest);
            var orderItems = _orderService
                .GetOrderItems(order.Id, isShipEnabled: true)
                .Select(oi => oi.Id);
            return returnRequests
                .Any(s => orderItems.Contains(s.OrderItemId));
        }

        public virtual bool CanReturnProductShip(GroupReturnRequest groupReturnRequest)
        {
            if (!HasReturnableProducts(groupReturnRequest) || !HasShippableItems(groupReturnRequest))
                return false;
            return true;
        }

        private ReturnOrder CreateReturnOrder(GroupReturnRequest groupReturnRequest)
        {
            // Check whether return order already created
            var returnOrders = GetReturnOrders(groupReturnRequest);
            if (returnOrders?.Count != 0)
                return default;

            var returnRequestList = GetReturnRequests(groupReturnRequest);
            var order = GetOrder(groupReturnRequest);
            var rOrder = new ReturnOrder
            {
                GroupReturnRequestId = groupReturnRequest.Id,
                ShippingStatus = ShippingStatus.NotYetShipped,

                EstimatedShippingExclTax = order.ActualOrderShippingExclTax,
                EstimatedShippingInclTax = order.ActualOrderShippingInclTax,
                // will be updated by scheduler after receiving correct shipment amount
                ActualShippingExclTax = order.ActualOrderShippingExclTax,
                ActualShippingInclTax = order.ActualOrderShippingInclTax
            };
            _returnRequestService.InsertReturnOrder(rOrder);

            var rOrderItems = returnRequestList
                .Select(rr => new ReturnOrderItem
                {
                    ReturnRequestId = rr.Id,
                    ReturnOrderId = rOrder.Id,
                });
            _returnRequestService.InsertReturnOrderItems(rOrderItems);

            return rOrder;
        }

        public void RetryShipping(Order order)
        {
            if (order == null)
                return;

            // check order not completed
            if (order.OrderStatus == OrderStatus.Complete)
                return;

            // check shipping status failed
            if (order.ShippingStatus != ShippingStatus.Failed)
                return;

            var shipment = _shipmentService
                .GetShipmentsByOrderId(order.Id)
                .FirstOrDefault(q => q.DeliveryDateUtc == null);

            if (shipment == null)
                return;

            shipment.RetryCount = 0;
            RetryShipment(shipment);
        }

        public bool CanRaiseDispute (GroupReturnRequest groupReturnRequest)
        {
            var dispute = _returnRequestService.GetDisputeByGroupReturnRequestId(groupReturnRequest.Id).FirstOrDefault();

            if (dispute != null)
                if (dispute.IsReturnDispute == true)
                    return false;

            if (groupReturnRequest.ApproveStatus == ApproveStatusEnum.Approved)
                return false;

            return true;
        }

        public void StartPreparing(Order order, string deliveryScheduleAt)
        {
            if (!(order is { OrderStatus: OrderStatus.Processing }))
                return;

            var customer = _customerService.GetCustomerById(order.CustomerId);

            SetOrderStatus(order, OrderStatus.Preparing, false);

            //create shipment for lalamove
            var shipment = new Shipment
            {
                OrderId = order.Id,
                Type = (int)ShipmentType.Fulfillment,
                CreatedOnUtc = DateTime.UtcNow,
                ScheduleAt = deliveryScheduleAt,
                DeliveryModeId = (int)_orderService.OrderHasDeliveryMode(order.Id, DeliveryMode.Car)
            };

            _shipmentService.InsertShipment(shipment);
            _shipmentProcessor.Ship(shipment);

            _eventPublisher.Publish(new OrderPreparedEvent(order));
            _workflowMessageService.SendOrderPreparingCustomerNotification(order, customer, order.CustomerLanguageId);
        }

        public void ArrangeShipment(Order order, bool requireInsurance)
        {
            //create shipment for jnt
            var shipment = new Shipment
            {
                OrderId = order.Id,
                Type = (int)ShipmentType.Fulfillment,
                CreatedOnUtc = DateTime.UtcNow,
                RequireInsurance = requireInsurance
            };

            _shipmentService.InsertShipment(shipment);
            _shipmentProcessor.Ship(shipment);
        }
    }
}