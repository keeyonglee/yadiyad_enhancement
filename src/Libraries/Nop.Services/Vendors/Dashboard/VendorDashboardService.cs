using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LinqToDB.Tools;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Orders;
using StackExchange.Profiling.Internal;

namespace Nop.Services.Vendors.Dashboard
{
    public class VendorDashboardService : IVendorDashboardService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Shipment> _shipmentRepository;
        private readonly OrderSettings _orderSettings;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IRepository<ReturnRequest> _returnRequestRepository;
        private readonly IRepository<GroupReturnRequest> _groupReturnRequestRepository;
        private readonly IRepository<Dispute> _disputeRepository;

        public VendorDashboardService(IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<Product> productRepository,
            IRepository<Shipment> shipmentRepository,
            OrderSettings orderSettings,
            ICheckoutAttributeParser checkoutAttributeParser,
            IRepository<ReturnRequest> returnRequestRepository,
            IRepository<GroupReturnRequest> groupReturnRequestRepository,
            IRepository<Dispute> disputeRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _shipmentRepository = shipmentRepository;
            _orderSettings = orderSettings;
            _checkoutAttributeParser = checkoutAttributeParser;
            _returnRequestRepository = returnRequestRepository;
            _groupReturnRequestRepository = groupReturnRequestRepository;
            _disputeRepository = disputeRepository;
        }
        public virtual int OpenOrderCount(int vendorId)
        {
            return GetOpenOrders(vendorId)
                .Count();
        }

        public virtual int CompletedOrderCount(int vendorId)
        {
            var orders = from o in _orderRepository.Table
                join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                join p in _productRepository.Table on oi.ProductId equals p.Id
                where o.OrderStatusId.Equals((int) OrderStatus.Complete)
                && p.VendorId.Equals(vendorId)
                select o.Id;

            return orders.Distinct().Count();
        }

        public virtual IList<VendorReturnRequestAction> OrdersNeedReturnAction(int vendorId)
        {
            var returns = from g in _groupReturnRequestRepository.Table
                join r in _returnRequestRepository.Table on g.Id equals r.GroupReturnRequestId
                join oi in _orderItemRepository.Table on r.OrderItemId equals oi.Id
                join o in _orderRepository.Table on oi.OrderId equals o.Id
                join p in _productRepository.Table on oi.ProductId equals p.Id
                join dr in _disputeRepository.Table on oi.OrderId equals dr.OrderId into dt
                from d in dt.DefaultIfEmpty()
                where g.ApproveStatusId.Equals((int) ApproveStatusEnum.Pending)
                      && d.Id == null
                      && p.VendorId.Equals(vendorId)
                select new VendorReturnRequestAction
                {
                    OrderId = oi.OrderId,
                    GroupReturnRequestId = g.Id,
                    OrderDate = o.CreatedOnUtc,
                    ReturnRequestDate = g.CreatedOnUtc,
                    ActBefore = g.CreatedOnUtc.AddDays(_orderSettings.DaysForSellerToRespondReturnRequest)
                };

            return returns.Distinct()
                .OrderBy(q => q.ActBefore)
                .ToList();
        }

        public class VendorReturnRequestAction
        {
            public int OrderId { get; set; }
            public int GroupReturnRequestId { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime ReturnRequestDate { get; set; }
            public DateTime ActBefore { get; set; }
        }

        public virtual (int, int) MonthlyOrderSummary(int vendorId)
        {
            var (currentLow, currentHigh) = GetMonthLowHighUTC(DateTime.Now);
            var (previousLow, previousHigh) = GetMonthLowHighUTC(DateTime.Now.AddMonths(-1));
            
            var orders = from o in _orderRepository.Table
                join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                join p in _productRepository.Table on oi.ProductId equals p.Id
                where o.OrderStatusId.NotIn((int) OrderStatus.Cancelled, (int) OrderStatus.Pending)
                      && p.VendorId.Equals(vendorId)
                select o;
            
            var distOrders = orders.Distinct();

            var currentCount = distOrders.Count(q => q.CreatedOnUtc >= currentLow && q.CreatedOnUtc < currentHigh);
            var prevCount = distOrders.Count(q => q.CreatedOnUtc >= previousLow && q.CreatedOnUtc < previousHigh);

            return (prevCount, currentCount);
        }

        private (DateTime, DateTime) GetMonthLowHighUTC(DateTime dateTime)
        {
            var low = new DateTime(new DateTime(dateTime.Year, dateTime.Month, 1).Ticks, DateTimeKind.Local);
            var highDate = dateTime.AddMonths(1);
            var high = new DateTime(new DateTime(highDate.Year, highDate.Month, 1).Ticks, DateTimeKind.Local);
            return (low.ToUniversalTime(), high.ToUniversalTime());
        }

        public virtual IList<ProductSummary> TopSoldProducts(int vendorId, int topCount = 5, int? days = null)
        {
            if (topCount > 1000)
                topCount = 1000;
            
            var products = from o in _orderRepository.Table
                join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                join p in _productRepository.Table on oi.ProductId equals p.Id
                where o.OrderStatusId.NotIn((int) OrderStatus.Cancelled, (int) OrderStatus.Pending)
                      && p.VendorId.Equals(vendorId)
                      && (!days.HasValue || o.CreatedOnUtc > DateTime.UtcNow.AddDays(days.Value))
                group new {o.CreatedOnUtc} by new {ProductId = p.Id, ProductName = p.Name} into grouped
                select new ProductSummary
                {
                    ProductId = grouped.Key.ProductId,
                    ProductName = grouped.Key.ProductName,
                    Count = grouped.Count(),
                    LastCreatedTime = grouped.Max(q => q.CreatedOnUtc)
                };
            
            
            return products
                .OrderByDescending(c => c.Count)
                .Take(topCount)
                .ToList();
        }

        public virtual IList<ProductSummary> TopReturnedProducts(int vendorId, int topCount = 5, int? days = null)
        {
            if (topCount > 1000)
                topCount = 1000;
            
            var products = from r in _returnRequestRepository.Table
                join oi in _orderItemRepository.Table on r.OrderItemId equals oi.Id
                join p in _productRepository.Table on oi.ProductId equals p.Id
                where p.VendorId.Equals(vendorId)
                      && (!days.HasValue || r.CreatedOnUtc > DateTime.UtcNow.AddDays(days.Value))
                group new {r.CreatedOnUtc} by new {ProductId = p.Id, ProductName = p.Name} into grouped
                select new ProductSummary
                {
                    ProductId = grouped.Key.ProductId,
                    ProductName = grouped.Key.ProductName,
                    Count = grouped.Count(),
                    LastCreatedTime = grouped.Max(q => q.CreatedOnUtc)
                };

            return products
                .OrderByDescending(c => c.Count)
                .Take(topCount)
                .ToList();
        }

        public virtual IList<VendorOrderShipment> OrdersPendingShipment(int vendorId)
        {
            var query = from o in GetOpenOrders(vendorId, true)
                join s in _shipmentRepository.Table on o.Id equals s.OrderId into os
                from b in os.DefaultIfEmpty()
                where b.Id == null
                      && o.OrderStatusId != (int) OrderStatus.ReturnRefundProcessing
                      && o.ShippingStatusId == (int) ShippingStatus.NotYetShipped
                      && o.PaymentStatusId == (int) PaymentStatus.Paid
                select new VendorOrderShipment
                {
                    OrderId = o.Id,
                    OrderStatus = (OrderStatus)o.OrderStatusId,
                    OrderTime = o.CreatedOnUtc,
                    ShipmentId = b.Id,
                    ShippingStatus = (ShippingStatus)o.ShippingStatusId,
                    CheckoutAttributeXML = o.CheckoutAttributesXml
                };

            var orders = query.ToList();
            foreach (var order in orders)
            {
                SetDeliveryDue(order);
            }
            
            return orders
                .OrderBy(q => q.DueDate)
                .ToList();
        }

        private IQueryable<Order> GetOpenOrders(int vendorId, bool? needShipping = null)
        {
            var orders = from o in _orderRepository.Table
                join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                join p in _productRepository.Table on oi.ProductId equals p.Id
                where o.OrderStatusId.NotIn((int) OrderStatus.Complete, (int) OrderStatus.Cancelled,
                          (int) OrderStatus.Pending)
                      && p.VendorId.Equals(vendorId)
                      && (!needShipping.HasValue || p.IsShipEnabled == needShipping)
                select o;

            return orders.Distinct();
        }

        private void SetDeliveryDue(VendorOrderShipment order)
        {
            var deliveryDate = _checkoutAttributeParser.GetCheckoutAttributeValues(order.CheckoutAttributeXML, NopOrderDefaults.DeliveryDate).FirstOrDefault();
            var deliveryTime = _checkoutAttributeParser.ParseCheckoutAttributeValues(order.CheckoutAttributeXML)
                .FirstOrDefault(q => q.attribute.Name.Equals(NopOrderDefaults.DeliveryTimeSlot))
                .values
                ?.FirstOrDefault()
                ?.Name
                ?.Split('-')
                ?.FirstOrDefault()
                ?.Trim() ?? "00:00";

            var duration = _orderSettings.DaysForSellerToShipOrder == 0 ? 30 : _orderSettings.DaysForSellerToShipOrder;
            
            var delDate = new DateTime(order.OrderTime.AddDays(duration).Ticks, DateTimeKind.Utc);

            if (!deliveryDate.IsNullOrWhiteSpace())
            {
                if (DateTime.TryParseExact($"{deliveryDate} {deliveryTime}", "MM/dd/yyyy HH:mm",
                    new CultureInfo("en-MY"), DateTimeStyles.None, out delDate))
                {
                    delDate = new DateTime(delDate.Ticks, DateTimeKind.Local);
                }
            }
            order.DueDate = delDate.ToUniversalTime();
        }

        public class VendorOrderShipment
        {
            public int OrderId { get; set; }
            public int ShipmentId { get; set; }
            public OrderStatus OrderStatus { get; set; }
            public ShippingStatus ShippingStatus { get; set; }
            public DateTime OrderTime { get; set; }
            public string CheckoutAttributeXML { get; set; }
            public DateTime DueDate { get; set; }
        }

        public class ProductSummary
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int Count { get; set; }
            public DateTime LastCreatedTime { get; set; }
        }
    }
}