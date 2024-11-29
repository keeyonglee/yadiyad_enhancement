using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.ShippingShuq;
using Nop.Services.Vendors;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using StackExchange.Profiling.Internal;

namespace Nop.Services.ShuqOrders
{
    public class ShuqOrderService : OrderService, IShuqOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IVendorService _vendorService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IRepository<ProductReview> _productReviewRepository;
        private readonly IRepository<Shipment> _shipmentRepository;
        private readonly ShippingMethodService _shippingMethodService;
        private readonly IRepository<Vendor> _vendorRepository;

        public ShuqOrderService(CachingSettings cachingSettings, IEventPublisher eventPublisher,
            IProductService productService, IRepository<Address> addressRepository,
            IRepository<Customer> customerRepository, IRepository<Order> orderRepository,
            IRepository<MasterOrder> masterOrderRepository, IRepository<OrderItem> orderItemRepository,
            IRepository<OrderNote> orderNoteRepository, IRepository<Product> productRepository,
            IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
            IRepository<RecurringPayment> recurringPaymentRepository,
            IRepository<RecurringPaymentHistory> recurringPaymentHistoryRepository,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService, 
            IShipmentService shipmentService,
            ICustomNumberFormatter customNumberFormatter,
            IVendorService vendorService,
            IRepository<ProductReview> productReviewRepository,
            IRepository<Shipment> shipmentRepository,
            ShippingMethodService shippingMethodService,
            IRepository<Vendor> vendorRepository) :
            base(cachingSettings, eventPublisher, productService, addressRepository, customerRepository,
                orderRepository, masterOrderRepository, orderItemRepository, orderNoteRepository, productRepository,
                productWarehouseInventoryRepository, recurringPaymentRepository, recurringPaymentHistoryRepository,
                shipmentService, customNumberFormatter, vendorRepository, shipmentRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _vendorService = vendorService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _checkoutAttributeService = checkoutAttributeService;
            _productReviewRepository = productReviewRepository;
            _shipmentRepository = shipmentRepository;
            _shippingMethodService = shippingMethodService;
            _vendorRepository = vendorRepository;
        }

        public Vendor GetVendorForOrder(Order order)
        {
            if (order == null)
                return null;
            
            // all items in a order should belong to same vendor
            var items = GetOrderItems(order.Id);
            
            return GetVendorForOrderItem(items.FirstOrDefault());
        }

        public Vendor GetVendorForOrderItem(OrderItem orderItem)
        {
            if (orderItem == null)
                return null;
            
            var product = _productRepository.Table.First(x => x.Id == orderItem.ProductId);

            if (product == null)
                return null;
            
            return _vendorService.GetVendorById(product.VendorId);
        }

        public IList<Shipment> GetNonDeliveredShipments()
        {
            var nonDeliveredShipments = from o in _orderRepository.Table
                join s in _shipmentRepository.Table on o.Id equals s.OrderId
                where (o.ShippingStatusId == (int)ShippingStatus.Shipped || o.ShippingStatusId == (int)ShippingStatus.NotYetShipped)
                      && s.TrackingNumber != null
                select s;
            return nonDeliveredShipments.ToList();
        }

        public IList<Shipment> GetPendingShipmentsForDeliveryMethod(string carrierName)
        {
            var shippingMethodId = _shippingMethodService.GetShippingMethodByName(carrierName).Id;
            var nonDeliveredShipments = from o in _orderRepository.Table
                join s in _shipmentRepository.Table on o.Id equals s.OrderId
                where (o.ShippingStatusId == (int)ShippingStatus.Shipped || o.ShippingStatusId == (int)ShippingStatus.NotYetShipped) 
                      && s.TrackingNumber != null
                      && s.ShippingMethodId == shippingMethodId
                select s;
            return nonDeliveredShipments.ToList();
        }

        public List<ItemToBeDelivered> GetItemsToBeDeliveredByProductIds(int[] productIds)
        {
            var query = (
                    from oi in _orderItemRepository.Table
                    join o in _orderRepository.Table on oi.OrderId equals o.Id
                    where !o.Deleted
                    && productIds.Contains(oi.ProductId)
                    && !string.IsNullOrEmpty(o.CheckoutAttributesXml)
                    && o.OrderStatusId > (int)OrderStatus.Pending && o.OrderStatusId < (int)OrderStatus.Complete
                    && o.ShippingStatusId > (int)ShippingStatus.ShippingNotRequired && o.ShippingStatusId <= (int)ShippingStatus.NotYetShipped
                    select new { oi.ProductId, oi.Quantity, o.CheckoutAttributesXml }
                    )
                    .ToList();
            return query
                    .Select(x =>
                    {
                        var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes();
                        var deliveryDateCheckoutAttribute = checkoutAttributes.Where(x => x.Name == NopOrderDefaults.DeliveryDate).FirstOrDefault();
                        var selectedDeliveryDate = _checkoutAttributeParser.ParseValues(x.CheckoutAttributesXml, deliveryDateCheckoutAttribute.Id);
                        //var selectedDeliveryDate = checkoutAttributeValues.Where(x => x.attribute.Id == deliveryDateCheckoutAttribute.Id).FirstOrDefault();

                        return new ItemToBeDelivered
                        {
                            ProductId = x.ProductId,
                            TotalQuantity = x.Quantity,
                            DeliveryDate = DateTime.Parse(selectedDeliveryDate.First())
                        };
                    })
                    .GroupBy(x => new { x.ProductId, x.DeliveryDate })
                    .Select(x => new ItemToBeDelivered
                    {
                        ProductId = x.Key.ProductId,
                        DeliveryDate = x.Key.DeliveryDate,
                        TotalQuantity = x.Sum(x => x.TotalQuantity)
                    })
                    .ToList();
        }

        public bool CheckBuyerHasReviewProduct(int orderId, int productId)
        {
            if (orderId is 0)
                throw new ArgumentNullException(nameof(orderId));
            if (productId is 0)
                throw new ArgumentNullException(nameof(productId));

            return _productReviewRepository.Table.FirstOrDefault(x => x.OrderId == orderId && x.ProductId == productId) != null;
        }

        public string GetCheckoutDeliveryDate(string attributesXml)
        {
            var attDeliveryDateId = _checkoutAttributeService.GetCheckoutAttributeByName(NopOrderDefaults.DeliveryDate).Id;
            return _checkoutAttributeParser.ParseValues(attributesXml, attDeliveryDateId)[0];
        }

        public string GetCheckoutDeliveryTimeslot(string attributesXml)
        {
            var attDeliveryTimeSlotId = _checkoutAttributeService.GetCheckoutAttributeByName(NopOrderDefaults.DeliveryTimeSlot).Id;
            var deliveryTimeslotValueName = _checkoutAttributeParser.ParseValues(attributesXml, attDeliveryTimeSlotId)[0];
            int deliveryTimeslotValueId;

            bool success = int.TryParse(deliveryTimeslotValueName, out deliveryTimeslotValueId);
            return success ? _checkoutAttributeService.GetCheckoutAttributeValueById(deliveryTimeslotValueId).Name : "";
        }

        public DateTime? GetCheckoutDateTimeSlot(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var deliveryDateText = GetCheckoutDeliveryDate(order.CheckoutAttributesXml);
            var deliveryTimeText = GetCheckoutDeliveryTimeslot(order.CheckoutAttributesXml);

            string[] time = deliveryTimeText.Split(' ');

            var dateString = $"{deliveryDateText} {time[0]}";
            var checking = DateTime.TryParse(dateString, out var correctDateString);
            if (!checking)
                throw new ArgumentException($"{nameof(dateString)} not a proper date");

            return correctDateString.ToUniversalTime();
        }

        public bool CheckEatsDeliveryDateTimeSlotNeedReminder(Order order, int hoursAdvance)
        {
            var scheduleAtDate = GetCheckoutDateTimeSlot(order);
            if ((scheduleAtDate - DateTime.UtcNow).Value.TotalHours > hoursAdvance)
            {
                return true;
            }
            return false;
        }
    }
}