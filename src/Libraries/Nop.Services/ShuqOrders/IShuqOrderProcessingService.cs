using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.ShippingShuq.DTO;
using Nop.Services.Orders;

namespace Nop.Services.ShuqOrders
{
    public interface IShuqOrderProcessingService : IOrderProcessingService
    {
        void SaveActualShippingCost(Order order, decimal shippingCost);
        void SaveActualShippingCost(ReturnOrder order, decimal shippingCost);
        void ApproveReturnRequest(GroupReturnRequest groupReturnRequest, bool afterDispute = false);
        void ProcessShipment(Shipment shipment, ShipmentDetailDTO response);
        void CancelReturn(Shipment shipment);
        void CompleteReturn(GroupReturnRequest groupReturnRequest);
        void RetryShipping(Order order);
        bool CanReturnProductShip(GroupReturnRequest groupReturnRequest);
        bool CanRaiseDispute(GroupReturnRequest groupReturnRequest);
        void StartPreparing(Order order, string deliveryScheduleAt);
        void ArrangeShipment(Order order, bool requireInsurance);
        Order GetOrder(GroupReturnRequest groupReturnRequest);
    }
}