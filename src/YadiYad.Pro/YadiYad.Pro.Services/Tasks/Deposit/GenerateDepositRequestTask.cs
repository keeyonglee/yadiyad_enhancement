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

namespace YadiYad.Pro.Services.Tasks.Deposit
{
    public partial class GenerateDepositRequestTask : IScheduleTask
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly DepositRequestService _depositRequestService;

        #endregion

        #region Ctor

        public GenerateDepositRequestTask(
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

            var type = typeof(GenerateDepositRequestTask);
            var typeName = $"{type.FullName}, {type.Assembly.GetName().Name}";

            var task = _scheduleTaskService.GetTaskByType(typeName);

            if (task != null)
            {
                var lastSuccessUtc = task.LastSuccessUtc?.AddHours(hoursDiff);

                if (lastSuccessUtc == null
                    || scheduledExecuteTime > lastSuccessUtc)
                {
                    _depositRequestService.CreateDepositRequest(1);
                }
            }
        }

        private string GetIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ip = host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).Last();

            if (ip != null)
            {
                return ip.ToString();
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        } 

        #endregion
    }
}
