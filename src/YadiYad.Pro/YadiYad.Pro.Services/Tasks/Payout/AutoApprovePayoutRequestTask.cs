using Nop.Core;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using YadiYad.Pro.Services.Deposit;
using YadiYad.Pro.Services.Payout;
using YadiYad.Pro.Services.Services.Messages;

namespace YadiYad.Pro.Services.Tasks.Payout
{
    public partial class AutoApprovePayoutRequestTask : IScheduleTask
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly PayoutRequestService _payoutRequestService;
        private readonly ProWorkflowMessageService _proWorkflowMessageService;

        #endregion

        #region Ctor

        public AutoApprovePayoutRequestTask(
            ILogger logger,
            IWorkContext workContext,
            IDateTimeHelper dateTimeHelper,
            IScheduleTaskService scheduleTaskService,
            ProWorkflowMessageService proWorkflowMessageService,
            PayoutRequestService payoutRequestService)
        {
            _logger = logger;
            _workContext = workContext;
            _proWorkflowMessageService = proWorkflowMessageService;
            _payoutRequestService = payoutRequestService;
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

            var type = typeof(AutoApprovePayoutRequestTask);
            var typeName = $"{type.FullName}, {type.Assembly.GetName().Name}";

            var task = _scheduleTaskService.GetTaskByType(typeName);

            if (task != null)
            {
                var lastEndTime = task.LastEndUtc?.AddHours(hoursDiff);

                if (lastEndTime == null
                    || scheduledExecuteTime > lastEndTime)
                {
                    var langaugeId = _workContext.WorkingLanguage.Id;
                    var payoutRequestDTOs = _payoutRequestService.AutoApprovedPayoutRequest();

                    payoutRequestDTOs.ForEach(x =>
                    {
                        try
                        {
                            _proWorkflowMessageService.SendApprovedPayoutRequestMessage(langaugeId, x);
                            _proWorkflowMessageService.SendAutoApprovedPayoutRequestMessage(langaugeId, x);
                        }
                        catch(Exception ex)
                        {
                            _logger.Error($"Exception occur on send email for auto approve payout request. Exception:{ex.ToString()}");
                        }
                    });
                }
            }
        }

        #endregion
    }
}
