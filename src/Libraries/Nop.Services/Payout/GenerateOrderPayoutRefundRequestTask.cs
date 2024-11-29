using Nop.Services.Helpers;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Payout
{
    public partial class GenerateOrderPayoutRefundRequestTask : IScheduleTask
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly OrderPayoutService _orderPayoutService;

        #endregion

        #region Ctor

        public GenerateOrderPayoutRefundRequestTask(
            ILogger logger,
            IDateTimeHelper dateTimeHelper,
            IScheduleTaskService scheduleTaskService,
            OrderPayoutService orderPayoutService)
        {
            _logger = logger;
            _dateTimeHelper = dateTimeHelper;
            _scheduleTaskService = scheduleTaskService;
            _orderPayoutService = orderPayoutService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            var type = typeof(GenerateOrderPayoutRefundRequestTask);
            var typeName = $"{type.FullName}, {type.Assembly.GetName().Name}";

            var task = _scheduleTaskService.GetTaskByType(typeName);

            if (task != null)
            {
                _logger?.Information($"{typeName}: START");
                _orderPayoutService.GenerateOrderPayoutRequest(1, DateTime.UtcNow);
                _logger?.Information($"{typeName}: END");
            }
        }

        #endregion
    }
}
