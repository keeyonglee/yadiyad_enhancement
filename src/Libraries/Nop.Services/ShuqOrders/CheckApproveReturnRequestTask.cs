using System;
using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Tasks;

namespace Nop.Services.ShuqOrders
{
    public class CheckApproveReturnRequestTask: IScheduleTask
    {
        private readonly IReturnRequestService _returnRequestService;
        private readonly OrderSettings _orderSettings;
        private readonly IShuqOrderProcessingService _orderProcessingService;
        private readonly ILogger _logger;

        public CheckApproveReturnRequestTask(IReturnRequestService returnRequestService,
            OrderSettings orderSettings,
            IShuqOrderProcessingService orderProcessingService,
            ILogger logger)
        {
            _returnRequestService = returnRequestService;
            _orderSettings = orderSettings;
            _orderProcessingService = orderProcessingService;
            _logger = logger;
        }
        
        private readonly Type type = typeof(CheckApproveReturnRequestTask);
        private string typeName => $"{type.FullName}, {type.Assembly.GetName().Name}";
        public void Execute()
        {
            _logger.Information($"{typeName}: START");
            var olderReturnRequestTime =
                DateTime.UtcNow.AddDays(_orderSettings.DaysForSellerToRespondReturnRequest * -1);
            
            var groupReturnRequests = _returnRequestService
                .GetNotProcessedReturnRequests(olderReturnRequestTime);
            
            foreach (var groupReturnRequest in groupReturnRequests)
            {
                _logger.Information($"{typeName}: Approving Group Return Request {groupReturnRequest.Id}");
                
                groupReturnRequest.NeedReturnShipping = false;
                _orderProcessingService.ApproveReturnRequest(groupReturnRequest);
            }
            _logger.Information($"{typeName}: END");
        }
    }
}