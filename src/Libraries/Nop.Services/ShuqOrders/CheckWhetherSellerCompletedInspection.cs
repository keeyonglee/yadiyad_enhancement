using System;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Tasks;

namespace Nop.Services.ShuqOrders
{
    public class CheckWhetherSellerCompletedInspection : IScheduleTask
    {
        private readonly IReturnRequestService _returnRequestService;
        private readonly OrderSettings _orderSettings;
        private readonly IShuqOrderProcessingService _orderProcessingService;
        private readonly ILogger _logger;

        public CheckWhetherSellerCompletedInspection(IReturnRequestService returnRequestService,
            OrderSettings orderSettings,
            IShuqOrderProcessingService orderProcessingService,
            ILogger logger)
        {
            _returnRequestService = returnRequestService;
            _orderSettings = orderSettings;
            _orderProcessingService = orderProcessingService;
            _logger = logger;
        }
        private readonly Type type = typeof(CheckWhetherSellerCompletedInspection);
        private string typeName => $"{type.FullName}, {type.Assembly.GetName().Name}";
        
        public void Execute()
        {
            _logger.Information($"{typeName}: START");
            
            var cutOffTime = DateTime.UtcNow.AddDays(-1 * _orderSettings.DaysForSellerToCompleteInspection);
            var pendingRequests = _returnRequestService.GetPendingInspectionReturns(cutOffTime);

            foreach (var groupReturnRequest in pendingRequests)
            {
                _logger.Information($"{typeName}: Completing Group Return For Non Action {groupReturnRequest.Id}");
                _orderProcessingService.CompleteReturn(groupReturnRequest);
            }
            _logger.Information($"{typeName}: END");
        }
    }
}