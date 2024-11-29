using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Events;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.ShippingShuq;

namespace Nop.Services.ShuqOrders
{
    public class ShuqOrderEventConsumer : IConsumer<ReturnRequestDeclinedEvent>
    {
        private readonly IReturnRequestService _returnRequestService;
        private readonly OrderService _orderService;
        private readonly IShuqOrderProcessingService _orderProcessingService;
        private readonly ShipmentCarrierResolver _shippingResolver;
        private readonly IShipmentService _shipmentService;

        public ShuqOrderEventConsumer(IReturnRequestService returnRequestService,
            OrderService orderService, IShuqOrderProcessingService orderProcessingService,
            ShipmentCarrierResolver shippingResolver, IShipmentService shipmentService)
        {
            _returnRequestService = returnRequestService;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _shippingResolver = shippingResolver;
            _shipmentService = shipmentService;
        }

        public void HandleEvent(ReturnRequestDeclinedEvent eventMessage)
        {
            var groupReturnRequest = eventMessage?.GroupReturnRequest;
            if (groupReturnRequest == null)
                return;
           
            var returnRequests = _returnRequestService.GetReturnRequestByGroupReturnRequestId(groupReturnRequest.Id);
            var order = _orderService.GetOrderByOrderItem(returnRequests.First().OrderItemId);

            order.OrderStatus = OrderStatus.Complete;
            _orderService.UpdateOrder(order);

            _orderProcessingService.CheckOrderStatus(order);
        }
    }
}