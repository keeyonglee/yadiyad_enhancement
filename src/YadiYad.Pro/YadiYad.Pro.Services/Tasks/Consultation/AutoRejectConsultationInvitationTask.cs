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
using YadiYad.Pro.Services.Consultation;
using YadiYad.Pro.Services.Services.Messages;

namespace YadiYad.Pro.Services.Tasks.Consultation
{
    public class AutoRejectConsultationInvitationTask : IScheduleTask
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IWorkContext _workContext;
        private readonly ConsultationInvitationService _consultationInvitationService;
        private readonly ProWorkflowMessageService _proWorkflowMessageService;

        #endregion

        #region Ctor

        public AutoRejectConsultationInvitationTask(
            ILogger logger,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext,
            IScheduleTaskService scheduleTaskService,
            ProWorkflowMessageService proWorkflowMessageService,
            ConsultationInvitationService consultationInvitationService)
        {
            _consultationInvitationService = consultationInvitationService;
            _proWorkflowMessageService = proWorkflowMessageService;
            _workContext = workContext;
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

            var type = typeof(AutoRejectConsultationInvitationTask);
            var typeName = $"{type.FullName}, {type.Assembly.GetName().Name}";

            var task = _scheduleTaskService.GetTaskByType(typeName);

            if (task != null)
            {
                var lastEndTime = task.LastEndUtc?.AddHours(hoursDiff);

                //if (lastEndTime == null
                //    || scheduledExecuteTime > lastEndTime)
                //{
                    var langaugeId = _workContext.WorkingLanguage.Id;
                    var actorId = _workContext.CurrentCustomer?.Id;
                    var rejectedConsultationJobDTOs = _consultationInvitationService.AutoRejectConsultationInvitation(actorId??1);

                    rejectedConsultationJobDTOs.ForEach(x =>
                    {
                        try
                        {
                            _proWorkflowMessageService.SendConsultationConsultantAutoDeclined(langaugeId, x.Id);
                            _proWorkflowMessageService.SendConsultationOrganizationAutoDeclined(langaugeId, x.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"Exception occur on send email for auto approve payout request. Exception:{ex.ToString()}");
                        }
                    });

                //}
            }
        }

        #endregion
    }
}
