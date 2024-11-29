using System;
using Nop.Core.Domain.Orders;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Tasks;

namespace Nop.Services.ShuqOrders
{
    public class CheckWhetherCustomerShippedReturn : IScheduleTask
    {
        private readonly IShuqOrderProcessingService _orderProcessingService;
        private readonly OrderSettings _orderSettings;
        private readonly IShipmentService _shipmentService;
        private readonly ILogger _logger;

        public CheckWhetherCustomerShippedReturn(IShuqOrderProcessingService orderProcessingService,
        OrderSettings orderSettings,
        IShipmentService shipmentService,
        ILogger logger)
        {
            _orderProcessingService = orderProcessingService;
            _orderSettings = orderSettings;
            _shipmentService = shipmentService;
            _logger = logger;
        }
        
        private readonly Type type = typeof(CheckWhetherCustomerShippedReturn);
        private string typeName => $"{type.FullName}, {type.Assembly.GetName().Name}";
        
        public void Execute()
        {
            _logger.Information($"{typeName}: START");
            
            var cutOffDate = DateTime.UtcNow.AddDays(-1 * _orderSettings.DaysForBuyerToShipOrder);
            var returnShipments = _shipmentService.GetPendingReturnShipments(cutOffDate);
            
            // Auto Complete the Shipments
            foreach (var shipment in returnShipments)
            {
                _logger.Information($"{typeName}: Cancelling Return Shipment {shipment.Id}");
                _orderProcessingService.CancelReturn(shipment);
            }
            
            _logger.Information($"{typeName}: END");
        }
    }
}