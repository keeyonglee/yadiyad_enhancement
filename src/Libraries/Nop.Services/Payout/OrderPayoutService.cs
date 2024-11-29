using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Documents;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Payout;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.Payout
{
    public class OrderPayoutService
    {
        #region private variable

        private readonly OrderSettings _orderSettings;

        private readonly DocumentNumberService _documentNumberService;

        private readonly IRepository<OrderPayoutRequest> _orderPayoutRequestRepo;
        private readonly IRepository<OrderRefundRequest> _orderRefundRequestRepo;

        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Vendor> _vendorRepo;
        private readonly IRepository<Customer> _customerRepo;

        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<OrderItem> _orderItemtRepo;

        private readonly IRepository<Shipment> _shipmentRepo;
        private readonly IRepository<ShipmentItem> _shipmentItemRepo;

        private readonly IRepository<ReturnOrder> _returnOrderRepo;
        private readonly IRepository<GroupReturnRequest> _groupReturnRequestRepo;
        private readonly IRepository<Dispute> _disputeRepo;

        private readonly IRepository<ReturnRequest> _returnRequestRepo;

        private readonly IRepository<Invoice> _invoiceRepo;

        #endregion

        #region contructor

        public OrderPayoutService(
            OrderSettings orderSettings,
            DocumentNumberService documentNumberService,
            IRepository<Product> productRepo,
            IRepository<Vendor> vendorRepo,
            IRepository<Customer> customerRepo,
            IRepository<OrderRefundRequest> orderRefundRequestRepo,
            IRepository<OrderPayoutRequest> orderPayoutRequestRepo,
            IRepository<Order> orderRepo,
            IRepository<OrderItem> orderItemtRepo,
            IRepository<Shipment> shipmentRepo,
            IRepository<ShipmentItem> shipmentItemRepo,
            IRepository<ReturnOrder> returnOrderRepo,
            IRepository<GroupReturnRequest> groupReturnRequestRepo,
            IRepository<Dispute> disputeRepo,
            IRepository<ReturnRequest> returnRequestRepo,
            IRepository<Invoice> invoiceRepo)
        {
            _orderSettings = orderSettings;
            _documentNumberService = documentNumberService;

            _productRepo = productRepo;
            _vendorRepo = vendorRepo;
            _orderPayoutRequestRepo = orderPayoutRequestRepo;

            _orderRepo = orderRepo;
            _orderItemtRepo = orderItemtRepo;

            _shipmentRepo = shipmentRepo;
            _shipmentItemRepo = shipmentItemRepo;

            _returnOrderRepo = returnOrderRepo;
            _groupReturnRequestRepo = groupReturnRequestRepo;
            _disputeRepo = disputeRepo;

            _returnRequestRepo = returnRequestRepo;

            _orderRefundRequestRepo = orderRefundRequestRepo;
            _customerRepo = customerRepo;

            _invoiceRepo = invoiceRepo;
        }

        #endregion

        #region method

        public void GenerateOrderPayoutRequest(int actorId, DateTime payoutUTCDate, int orderId = 0)
        {
            var orderPayoutRequests = GetOrderReadyForPayout(payoutUTCDate, orderId);

            var refundRequests = GetOrderRefundRequest(orderPayoutRequests);

            orderPayoutRequests = InsertOrderPayoutRequests(actorId, orderPayoutRequests);

            refundRequests = InsertOrderRefundRequests(actorId, refundRequests);
        }

        public IList<OrderPayoutRequest> GetOrderReadyForPayout(DateTime payoutUTCDate, int orderId = 0)
        {
            //payout/refund is ready when
            // - order cancelled
            // - order received
            // - order delivered and passed allowed return days
            // - group return request is approved, return is not required 
            // - group return request is approved, return is required, no dispute, return condition is mint
            // - group return request is approved, dispute with full refund, return is required, return condition is updated
            // - group return request is approved, dispute with full refund, return is not required
            // - group return request is approved, dispute with partial refund, return is required, return condition is updated
            // - group return request is approved, dispute with partial refund, return is not required
            // - group return request is not approved, dispute with no refund
            var orderPayoutRequests = new List<OrderPayoutRequest>();

            var deliveryCutOffDateUtc = payoutUTCDate.AddDays(_orderSettings.NumberOfDaysReturnRequestAvailable * -1);

            #region get order

            //get order which
            // - CancellationDateUtc < PayoutUTCDate
            // - ReceivedDateUtc < PayoutUTCDate
            // - DeliveryDateUtc < (PayoutUTCDate - Return Request Available days)
            // - return request > 0 (to be filter further)
            //output: one order one record
            var queryOrders =
                (from o in
                    (from o in _orderRepo.Table
                    .Where(o => o.Deleted == false
                    && (orderId == 0 || o.Id == orderId)
                    && o.PaymentStatusId == (int)PaymentStatus.Paid
                    && (o.OrderStatusId == (int)OrderStatus.Complete
                        || o.OrderStatusId == (int)OrderStatus.Cancelled
                        || o.OrderStatusId == (int)OrderStatus.ReturnRefundProcessing))

                     from opr in _orderPayoutRequestRepo.Table
                     .Where(opr => opr.Deleted == false
                     && opr.OrderId == o.Id)
                     .DefaultIfEmpty()

                     from oi in _orderItemtRepo.Table
                     .Where(oi => oi.OrderId == o.Id)

                     from p in _productRepo.Table
                     .Where(p => p.Id == oi.ProductId)

                     from v in _vendorRepo.Table
                     .Where(v => v.Id == p.VendorId)

                     from vc in _customerRepo.Table
                     .Where(vc => vc.VendorId == v.Id)

                     from sm in _shipmentRepo.Table
                    .Where(sm => sm.OrderId == o.Id
                    && sm.Type == (int)ShipmentType.Fulfillment
                    && sm.DeliveryDateUtc != null)
                    .DefaultIfEmpty()

                     from smi in _shipmentItemRepo.Table
                     .Where(smi => smi.OrderItemId == oi.Id
                     && smi.ShipmentId == sm.Id)
                     .DefaultIfEmpty()

                     from rr in _returnRequestRepo.Table
                     .Where(rr => rr.OrderItemId == oi.Id)
                     .DefaultIfEmpty()
                     select new
                     {
                         rr,
                         oi,
                         sm,
                         o,
                         opr,
                         v,
                         vc
                     })
                    .Where(x => x.opr == null)
                 group new
                 {
                     o.rr,
                     o.oi,
                     o.sm,
                     o.vc
                 }
                 by new
                 {
                     OrderId = o.o.Id,
                     CustomerId = o.o.CustomerId,
                     OrderStatusId = o.o.OrderStatusId,
                     VendorCustomerId = o.vc.Id,
                     o.o.OrderTotal,
                     o.o.CancellationDateUtc,
                     o.o.OrderSubtotalExclTax,
                     o.o.OrderSubtotalInclTax,
                     o.o.ActualOrderShippingExclTax,
                     o.o.ActualOrderShippingInclTax,
                     o.o.OrderShippingExclTax,
                     o.o.OrderShippingInclTax,
                     o.o.PlatformSubTotalDiscount,
                     o.o.PlatformShippingDiscount,
                     o.o.ReceivedDateUtc
                 }
                into go
                 select new
                 {
                     go.Key.OrderId,
                     go.Key.CustomerId,
                     go.Key.OrderStatusId,
                     go.Key.VendorCustomerId,
                     go.Key.OrderTotal,
                     go.Key.CancellationDateUtc,

                     go.Key.OrderSubtotalExclTax,
                     go.Key.OrderSubtotalInclTax,

                     go.Key.ActualOrderShippingExclTax,
                     go.Key.ActualOrderShippingInclTax,

                     go.Key.OrderShippingExclTax,
                     go.Key.OrderShippingInclTax,

                     go.Key.PlatformSubTotalDiscount,
                     go.Key.PlatformShippingDiscount,

                     go.Key.ReceivedDateUtc,
                     TotalInsurance = go.Sum(x => x.sm.Insurance),
                     NoReturnRequest = go.Sum(x => x.rr != null ? 1 : 0),
                     MaxDeliveryDateUtc = go.Max(x => x.sm.DeliveryDateUtc),
                 })
                .Where(x =>
                    (x.OrderStatusId == (int)OrderStatus.Cancelled && x.CancellationDateUtc <= payoutUTCDate)
                    || (x.ReceivedDateUtc != null && x.ReceivedDateUtc <= payoutUTCDate)
                    || x.NoReturnRequest > 0
                    || (x.MaxDeliveryDateUtc != null && x.MaxDeliveryDateUtc <= deliveryCutOffDateUtc)
                );

            var orderReadyForPayouts = queryOrders.ToList();

            #endregion

            #region get return order's shipment
            var orderIdsWihtReturn = orderReadyForPayouts
                .Where(x => x.NoReturnRequest > 0)
                .Select(x => x.OrderId)
                .ToList();

            //assumption: one order one group order request
            //refund is finalize when
            //group return request is approved, return is not required 
            //group return request is approved, return is required, no dispute, return condition is mint
            //group return request is approved, dispute with full refund, return is required, return condition is updated
            //group return request is approved, dispute with full refund, return is not required
            //group return request is approved, dispute with partial refund, return is required, return condition is updated
            //group return request is approved, dispute with partial refund, return is not required
            //group return request is not approved, dispute with no refund
            //output: one order one record
            var queryReturnOrders =
                from ro in
                    (from oi in _orderItemtRepo.Table
                    .Where(oi => orderIdsWihtReturn.Contains(oi.OrderId))
                     from rr in _returnRequestRepo.Table
                     .Where(rr => rr.OrderItemId == oi.Id)
                     from grr in _groupReturnRequestRepo.Table
                     .Where(grr => grr.Id == rr.GroupReturnRequestId
                     && grr.ApproveStatusId != (int)ApproveStatusEnum.Pending)

                         //dispute only active when group return request is approved
                     from dps in _disputeRepo.Table
                     .Where(dps => dps.GroupReturnRequestId == grr.Id)
                     .DefaultIfEmpty()

                     from ro in _returnOrderRepo.Table
                     .Where(ro => ro.GroupReturnRequestId == grr.Id
                     )
                     .DefaultIfEmpty()

                     from s in _shipmentRepo.Table
                     .Where(s => s.ReturnOrderId == ro.Id
                     && s.Type == (int)ShipmentType.Return)
                     .DefaultIfEmpty()

                     group new
                     {
                         oi.OrderId,
                         GroupReturnRequestId = grr.Id,
                         ReturnOrderId = ro != null ? (int?)ro.Id : null,
                         ReturnOrderShippingExclTax = ro != null ? (decimal?)ro.EstimatedShippingExclTax : null,
                         ActualReturnOrderShippingExclTax = ro != null ? (decimal?)ro.ActualShippingExclTax : null,

                         ReturnOrderShippingInclTax = ro != null ? (decimal?)ro.EstimatedShippingInclTax : null,
                         ActualReturnOrderShippingInclTax = ro != null ? (decimal?)ro.ActualShippingInclTax : null,

                         DeliveryDateUtc = s != null ? s.DeliveryDateUtc : null,
                         grr.ApprovalDateUtc,
                         grr.ApproveStatusId,
                         grr.ReturnConditionId,
                         NeedReturnShipping = grr.NeedReturnShipping || ro != null,


                         DisputeActionId = dps != null
                            ? (int?)dps.DisputeAction
                            : null,
                         PartialRefundAmount = dps != null ? dps.PartialAmount : null
                     }
                     by new
                     {
                         oi.OrderId,
                         GroupReturnRequestId = grr.Id,
                         grr.ApprovalDateUtc,
                         grr.ApproveStatusId,
                         grr.ReturnConditionId,
                         NeedReturnShipping = grr.NeedReturnShipping || ro != null,

                         ReturnOrderId = ro.Id,
                         ReturnOrderShippingExclTax = ro.EstimatedShippingExclTax,
                         ActualReturnOrderShippingExclTax = ro.ActualShippingExclTax,

                         ReturnOrderShippingInclTax = ro.EstimatedShippingInclTax,
                         ActualReturnOrderShippingInclTax = ro.ActualShippingInclTax,

                         DisputeActionId = dps != null
                            ? (int?)dps.DisputeAction
                            : null,
                         PartialRefundAmount = dps != null ? dps.PartialAmount : null
                     }
                    into ro
                     select new
                     {
                         ro.Key.OrderId,
                         ro.Key.GroupReturnRequestId,
                         ro.Key.ApprovalDateUtc,
                         ro.Key.ApproveStatusId,
                         DeliveryDateUtc = ro.Max(x => x.DeliveryDateUtc),
                         ro.Key.ReturnOrderId,
                         ro.Key.ReturnOrderShippingExclTax,
                         ro.Key.ActualReturnOrderShippingExclTax,
                         ro.Key.ReturnOrderShippingInclTax,
                         ro.Key.ActualReturnOrderShippingInclTax,
                         ro.Key.DisputeActionId,
                         ro.Key.PartialRefundAmount,
                         ro.Key.ReturnConditionId,
                         ro.Key.NeedReturnShipping
                     })
                group new
                {
                    ro.ReturnOrderShippingExclTax,
                    ro.ActualReturnOrderShippingExclTax,
                    ro.ReturnOrderShippingInclTax,
                    ro.ActualReturnOrderShippingInclTax
                }
                by new
                {
                    ro.OrderId,
                    ro.GroupReturnRequestId,
                    ro.DisputeActionId,
                    ro.PartialRefundAmount,
                    ro.DeliveryDateUtc,
                    ro.ApproveStatusId,
                    ro.ApprovalDateUtc,
                    ro.NeedReturnShipping,
                    ro.ReturnConditionId
                }
                into gro
                select new
                {
                    gro.Key.OrderId,
                    gro.Key.GroupReturnRequestId,
                    gro.Key.DisputeActionId,
                    gro.Key.PartialRefundAmount,
                    gro.Key.DeliveryDateUtc,
                    gro.Key.ApproveStatusId,
                    gro.Key.ApprovalDateUtc,
                    gro.Key.ReturnConditionId,
                    gro.Key.NeedReturnShipping,
                    ReturnOrderShippingExclTax = gro.Sum(x => x.ReturnOrderShippingExclTax),
                    ActualReturnOrderShippingExclTax = gro.Sum(x => x.ActualReturnOrderShippingExclTax),
                    ReturnOrderShippingInclTax = gro.Sum(x => x.ReturnOrderShippingInclTax),
                    ActualReturnOrderShippingInclTax = gro.Sum(x => x.ActualReturnOrderShippingInclTax)
                };

            var returnOrders = queryReturnOrders.ToList();

            #endregion

            #region get order ready for payout
            //if return request not approved, proceed payout
            var orderPayoutRequestQuery =
                from fo in
                    (from o in orderReadyForPayouts
                     from ro in returnOrders
                     .Where(ro => ro.OrderId == o.OrderId)
                     .DefaultIfEmpty()
                     select new
                     {
                         OrderId​ = o.OrderId​,
                         GroupReturnRequestId = ro != null ? (int?)ro.GroupReturnRequestId : null,
                         CustomerId = o.CustomerId,
                         OrderStatusId = o.OrderStatusId,


                         OrderTotal​ = o.OrderTotal​,


                         ProductPriceInclTax​ = o.OrderSubtotalInclTax,
                         ProductPriceExclTax​ = o.OrderSubtotalExclTax,

                         //select the highest cost as fulfillment shipment cost between actual shipping cost and order shipping cost (quotation)
                         FulfillmentShipmentInclTax​ = o.ActualOrderShippingInclTax > o.OrderShippingInclTax? o.ActualOrderShippingInclTax: o.OrderShippingInclTax,
                         FulfillmentShipmentExclTax​ = o.ActualOrderShippingExclTax > o.OrderShippingExclTax? o.ActualOrderShippingExclTax: o.OrderShippingExclTax,

                         o.PlatformSubTotalDiscount,
                         o.PlatformShippingDiscount,

                         ReturnShipmentInclTax​ = ro?.ActualReturnOrderShippingInclTax ?? 0,
                         ReturnShipmentExclTax​ = ro?.ActualReturnOrderShippingExclTax ?? 0,


                         DisputeActionId = ro?.DisputeActionId,
                         PartialRefundAmount = ro?.PartialRefundAmount,
                         ReturnOrderDeliveryDateUtc = ro?.DeliveryDateUtc,
                         ApproveStatusId = ro?.ApproveStatusId,
                         ApprovalDateUtc = ro?.ApprovalDateUtc,
                         ReturnConditionId = ro?.ReturnConditionId,
                         NeedReturnShipping = ro?.NeedReturnShipping,


                         DeliveryDateUtc = o.MaxDeliveryDateUtc,
                         ReceivedDateUtc = o.ReceivedDateUtc,
                         CancellationDateUtc = o.CancellationDateUtc,
                         HasReturnRequest = o.NoReturnRequest > 0,
                         VendorCustomerId = o.VendorCustomerId,
                         Insurance = o.TotalInsurance
                     })
                .Where(x =>
                    // - order cancelled
                    x.OrderStatusId == (int)OrderStatus.Cancelled

                    // - order delivered and passed allowed return days
                    || x.HasReturnRequest == false

                    // - group return request is approved, return is not required
                    || (x.HasReturnRequest == true
                        && x.ApproveStatusId == (int)ApproveStatusEnum.Approved
                        && x.NeedReturnShipping == false)

                    // - group return request is approved, return is required, no dispute, return condition is mint
                    || (x.HasReturnRequest == true
                        && x.ApproveStatusId == (int)ApproveStatusEnum.Approved
                        && x.NeedReturnShipping == true
                        && x.DisputeActionId == null
                        && x.ReturnConditionId == (int)ReturnConditionEnum.Mint)

                    // - group return request is approved, dispute with full refund, return is required, return condition is updated
                    || (x.HasReturnRequest == true
                        && x.ApproveStatusId == (int)ApproveStatusEnum.Approved
                        && x.NeedReturnShipping == true
                        && x.DisputeActionId == (int)DisputeActionEnum.FullRefundFromBuyer
                        && x.ReturnConditionId != (int)ReturnConditionEnum.Pending)

                    // - group return request is approved, dispute with full refund, return is not required
                    || (x.HasReturnRequest == true
                        && x.ApproveStatusId == (int)ApproveStatusEnum.Approved
                        && x.NeedReturnShipping == false
                        && x.DisputeActionId == (int)DisputeActionEnum.FullRefundFromBuyer)

                    // - group return request is approved, dispute with partial refund, return is required, return condition is updated
                    || (x.HasReturnRequest == true
                        && x.ApproveStatusId == (int)ApproveStatusEnum.Approved
                        && x.NeedReturnShipping == true
                        && x.DisputeActionId == (int)DisputeActionEnum.PartialRefund
                        && x.ReturnConditionId != (int)ReturnConditionEnum.Pending)

                    // - group return request is approved, dispute with partial refund, return is not required
                    || (x.HasReturnRequest == true
                        && x.ApproveStatusId == (int)ApproveStatusEnum.Approved
                        && x.NeedReturnShipping == false
                        && x.DisputeActionId == (int)DisputeActionEnum.PartialRefund)

                    // - group return request is not approved, dispute with no refund
                    || (x.HasReturnRequest == true
                        && x.ApproveStatusId == (int)ApproveStatusEnum.NotApproved)
                )
                select new
                {
                    fo.OrderId,
                    fo.Insurance,
                    fo.VendorCustomerId​,
                    fo.GroupReturnRequestId,
                    fo.CustomerId,


                    fo.OrderTotal​​,


                    fo.ProductPriceInclTax​,
                    fo.ProductPriceExclTax​,


                    fo.FulfillmentShipmentInclTax​,
                    fo.FulfillmentShipmentExclTax​,

                    fo.PlatformSubTotalDiscount,
                    fo.PlatformShippingDiscount,

                    fo.ReturnShipmentInclTax,
                    fo.ReturnShipmentExclTax​,


                    RefundAmount = 
                        fo.OrderStatusId == (int)OrderStatus.Cancelled
                        ?fo.OrderTotal
                        :fo.HasReturnRequest == true
                        && fo.ApproveStatusId == (int)ApproveStatusEnum.Approved
                        && fo.DisputeActionId != (int)DisputeActionEnum.NoRefund
                        ? ((fo.DisputeActionId == (int)DisputeActionEnum.FullRefundFromBuyer ||fo.DisputeActionId == null)
                            ? fo.OrderTotal
                            : fo.DisputeActionId == (int)DisputeActionEnum.PartialRefund
                                ? fo.PartialRefundAmount
                                : 0)
                        : 0,
                    fo.DeliveryDateUtc,
                    fo.ReceivedDateUtc,
                    fo.CancellationDateUtc,
                    fo.HasReturnRequest,
                    fo.ReturnOrderDeliveryDateUtc,
                    fo.ApproveStatusId,
                    fo.ApprovalDateUtc
                };

            orderPayoutRequests = orderPayoutRequestQuery.ToList()
                .Select(fo => new OrderPayoutRequest
                {
                    OrderId​ = fo.OrderId​,
                    CustomerId = fo.CustomerId,
                    VendorCustomerId = fo.VendorCustomerId​,
                    Insurance = fo.Insurance,


                    OrderTotal​ = fo.OrderTotal​,


                    ProductPriceInclTax​ = fo.ProductPriceInclTax​,
                    ProductPriceExclTax​ = fo.ProductPriceExclTax​,


                    FulfillmentShipmentInclTax​ = fo.FulfillmentShipmentInclTax​,
                    FulfillmentShipmentExclTax​ = fo.FulfillmentShipmentExclTax​,


                    ReturnShipmentInclTax​ = fo.ReturnShipmentInclTax​,
                    ReturnShipmentExclTax​ = fo.ReturnShipmentExclTax​,

                    PlatformSubTotalDiscount = fo.PlatformSubTotalDiscount,
                    PlatformShippingDiscount = fo.PlatformShippingDiscount,

                    RefundAmount = fo.RefundAmount ?? 0,
                    TransactionDate​ =
                        fo.CancellationDateUtc
                        ?? fo.ReturnOrderDeliveryDateUtc
                        ?? fo.ApprovalDateUtc
                        ?? fo.ReceivedDateUtc
                        ?? (fo.DeliveryDateUtc.Value.AddDays(_orderSettings.NumberOfDaysReturnRequestAvailable)),
                    OrderPayoutStatusId = (int)OrderPayoutStatus.Pending
                })
                .ToList();

            #endregion

            return orderPayoutRequests;
        }

        public OrderRefundRequest GetOrderRefundRequestByOrderId(int orderId)
        {
            if (orderId == 0)
                return null;

            return _orderRefundRequestRepo.Table.FirstOrDefault(o => o.OrderId == orderId);
        }

        public OrderPayoutRequest GetOrderPayoutRequestByOrderId(int orderId)
        {
            if (orderId == 0)
                return null;

            return _orderPayoutRequestRepo.Table.FirstOrDefault(o => o.OrderId == orderId);
        }

        public Invoice GetOrderPayoutRequestInvoiceByOrderId(int orderId)
        {
            var query = from opr in _orderPayoutRequestRepo.Table
                            join i in _invoiceRepo.Table on opr.InvoiceId equals i.Id
                            select i;

            return query.FirstOrDefault();
        }

        public IList<OrderRefundRequest> GetOrderRefundRequest(
            IList<OrderPayoutRequest> orderPayoutRequests)
        {
            var orderRefundRequests = 
                orderPayoutRequests
                .Where(x=>x.RefundAmount > 0)
                .Select(x => new OrderRefundRequest
                {
                    CustomerId = x.CustomerId,
                    Amount = x.RefundAmount,
                    DocumentNumber = null,
                    OrderId = x.OrderId,
                    RefundStatus = RefundStatus.New,
                    TransactionDate = x.TransactionDate
                })
                .ToList();

            return orderRefundRequests;
        }

        public IList<OrderPayoutRequest> InsertOrderPayoutRequests(
            int actorId,
            IList<OrderPayoutRequest> orderPayoutRequests)
        {
            foreach (var orderPayoutRequest in orderPayoutRequests)
            {
                orderPayoutRequest.CreateAudit(actorId);
            }

            _orderPayoutRequestRepo.Insert(orderPayoutRequests);

            return orderPayoutRequests;
        }

        public IList<OrderRefundRequest> InsertOrderRefundRequests(
            int actorId,
            IList<OrderRefundRequest> orderRefundRequests)
        {
            foreach (var orderRefundRequest in orderRefundRequests)
            {
                orderRefundRequest.CreateAudit(actorId);
            }

            var newDocumentNumbers = _documentNumberService.GetDocumentNumbers(
                RunningNumberType.Refund,
                orderRefundRequests.Count);

            for (int i = 0; i < newDocumentNumbers.Count; i++)
            {
                orderRefundRequests[i].DocumentNumber = newDocumentNumbers[i];
            }

            _orderRefundRequestRepo.Insert(orderRefundRequests);

            return orderRefundRequests;
        }


        /// <summary>
        /// update payout request to paid and perform necessary update
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="payoutRequestId"></param>
        /// <returns></returns>
        public OrderPayoutRequest UpdatePayoutRequestPaidStatus(int actorId, int payoutRequestId)
        {
            var model = _orderPayoutRequestRepo.Table
                .Where(x => x.Deleted == false
                && x.Id == payoutRequestId)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException();
            }

            model.OrderPayoutStatusId = (int)OrderPayoutStatus.Paid;
            model.UpdateAudit(actorId);
            _orderPayoutRequestRepo.Update(model);

            return model;
        }

        /// <summary>
        /// update refund request to paid and perform necessary update
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="payoutRequestId"></param>
        /// <returns></returns>
        public OrderRefundRequest UpdateRefundRequestPaidStatus(int actorId, int refundRequestId)
        {
            var model = _orderRefundRequestRepo.Table
                .Where(x => x.Deleted == false
                && x.Id == refundRequestId)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException();
            }

            model.RefundStatusId = (int)RefundStatus.Paid;
            model.UpdateAudit(actorId);
            _orderRefundRequestRepo.Update(model);

            return model;
        }

        #endregion
    }
}
