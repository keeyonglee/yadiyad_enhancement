using Nop.Core.Domain.Tasks;
using Nop.Data;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Services.Deposit;

namespace YadiYad.Pro.Services.Tasks.Deposit
{
    public partial class GenerateDepositRequestReminderTask : IScheduleTask
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly DepositRequestService _depositRequestService;

        #endregion

        #region Ctor

        public GenerateDepositRequestReminderTask(
            ILogger logger,
            IDateTimeHelper dateTimeHelper,
            IScheduleTaskService scheduleTaskService,
            DepositRequestService depositRequestService)
        {
            _depositRequestService = depositRequestService;
            _logger = logger;
            _dateTimeHelper = dateTimeHelper;
            _scheduleTaskService = scheduleTaskService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            var timezone = _dateTimeHelper.DefaultStoreTimeZone;
            var hoursDiff = timezone.BaseUtcOffset.TotalHours;
            var localDateTime = DateTime.UtcNow.AddHours(hoursDiff);
            var scheduledExecuteTime = localDateTime.Date;

            var type = typeof(GenerateDepositRequestReminderTask);
            var typeName = $"{type.FullName}, {type.Assembly.GetName().Name}";

            var task = _scheduleTaskService.GetTaskByType(typeName);

            if (task != null)
            {
                var lastEndTime = task.LastEndUtc?.AddHours(hoursDiff);

                if (lastEndTime == null
                    || scheduledExecuteTime > lastEndTime)
                {
                    _depositRequestService.SendDepositRequestReminder(1);
                }
            }
        }

        #endregion
    }
}
