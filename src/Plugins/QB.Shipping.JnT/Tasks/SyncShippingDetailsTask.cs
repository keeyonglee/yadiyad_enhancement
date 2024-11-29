using System;
using Nop.Core.Domain.Shipping;
using Nop.Services.Logging;
using Nop.Services.ShippingShuq;
using Nop.Services.ShuqOrders;
using Nop.Services.Tasks;

namespace QB.Shipping.JnT.Tasks
{
    public class SyncShippingDetailsTask : IScheduleTask
    {
        private readonly IShuqOrderService _orderService;
        private readonly IShuqOrderProcessingService _orderProcessingService;
        private readonly ShipmentCarrierResolver _shipmentCarrierResolver;
        private readonly ILogger _logger;

        public SyncShippingDetailsTask(IShuqOrderService orderService,
            IShuqOrderProcessingService orderProcessingService,
            ShipmentCarrierResolver shipmentCarrierResolver,
            ILogger logger)
        {
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _shipmentCarrierResolver = shipmentCarrierResolver;
            _logger = logger;
        }
        
        private readonly Type type = typeof(SyncShippingDetailsTask);
        private string typeName => $"{type.FullName}, {type.Assembly.GetName().Name}";
        
        public void Execute()
        {

            
            _logger.Information($"{typeName}: START");
            var shipments = _orderService.GetPendingShipmentsForDeliveryMethod(JntShipmentCarrier.CarrierName);
            foreach (var shipment in shipments)
            {
                _logger.Information($"{typeName}: Processing for Shipment {shipment.Id}");
                ProcessShipmentStatus(shipment);
            }
            _logger.Information($"{typeName}: END");
        }
        
        private void ProcessShipmentStatus(Shipment shipment)
        {
            if(shipment?.ShippingMethodId == null)
                return;
                
            if(string.IsNullOrWhiteSpace(shipment?.TrackingNumber))
                return;

            var shippingCarrier = _shipmentCarrierResolver.ResolveByCarrierName(JntShipmentCarrier.CarrierName);

            if (shippingCarrier == null)
                return;

            //var response = shippingCarrier.GetShipmentDetail(shipment.TrackingNumber, "test");
            var response = shippingCarrier.GetShipmentDetail(shipment.TrackingNumber);
            
            _logger.Information($"{typeName}: Processing for Shipment {shipment.Id} - Response: {response?.CurrentStatus}");
                
            if(response == default)
                return;

            _orderProcessingService.ProcessShipment(shipment, response);
        }
    }
}