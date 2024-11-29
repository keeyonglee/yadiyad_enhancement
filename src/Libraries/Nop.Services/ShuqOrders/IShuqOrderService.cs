using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Orders;
using Nop.Core.Domain.Vendors;
using System;

namespace Nop.Services.ShuqOrders
{
    /// <summary>
    /// Custom Order Service which extends Nop Order Servoce
    /// The order to be used here should be Nop Order for Shuq rather than Pro Order, unless specified otherwise in method signature
    /// </summary>
    public interface IShuqOrderService : IOrderService
    {
        Vendor GetVendorForOrder(Order order);
        Vendor GetVendorForOrderItem(OrderItem orderItem);
        IList<Shipment> GetNonDeliveredShipments();

        List<ItemToBeDelivered> GetItemsToBeDeliveredByProductIds(int[] productIds);

        bool CheckBuyerHasReviewProduct(int orderId, int productId);
        IList<Shipment> GetPendingShipmentsForDeliveryMethod(string carrierName);
        string GetCheckoutDeliveryDate(string attributesXml);
        string GetCheckoutDeliveryTimeslot(string attributesXml);

        DateTime? GetCheckoutDateTimeSlot(Order order);
        bool CheckEatsDeliveryDateTimeSlotNeedReminder(Order order, int hoursAdvance);
    }
}