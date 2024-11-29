using System;
using Nop.Plugin.NopStation.AppPushNotification.Services;
using Nop.Services.Logging;
using Nop.Services.Tasks;

namespace Nop.Plugin.NopStation.AppPushNotification
{
    public class QueuedAppPushNotificationSendTask : IScheduleTask
    {
        private readonly ILogger _logger;
        private readonly IPushNotificationSender _pushNotificationSender;
        private readonly IQueuedPushNotificationService _queuedPushNotificationService;

        public QueuedAppPushNotificationSendTask(ILogger logger,
            IPushNotificationSender pushNotificationSender,
            IQueuedPushNotificationService queuedPushNotificationService)
        {
            _logger = logger;
            _pushNotificationSender = pushNotificationSender;
            _queuedPushNotificationService = queuedPushNotificationService;
        }

        public void Execute()
        {
            var queuedPushNotifications = _queuedPushNotificationService.GetAllQueuedPushNotifications(false, true);

            for (int i = 0; i < queuedPushNotifications.Count; i++)
            {
                var queuedPushNotification = queuedPushNotifications[i];
                try
                {
                    _pushNotificationSender.SendNotification(queuedPushNotification);
                    queuedPushNotification.SentOnUtc = DateTime.UtcNow;
                    queuedPushNotification.ErrorLog = "";
                    _queuedPushNotificationService.UpdateQueuedPushNotification(queuedPushNotification);
                }
                catch (Exception ex)
                {
                    queuedPushNotification.ErrorLog = $"{queuedPushNotification.ErrorLog}{queuedPushNotification.SentTries + 1}. {ex.Message}{Environment.NewLine}";
                    queuedPushNotification.SentTries = queuedPushNotification.SentTries + 1;
                    _queuedPushNotificationService.UpdateQueuedPushNotification(queuedPushNotification);

                    _logger.Error($"Failed to send app push notification (Id = {queuedPushNotification.Id}): {ex.Message}", ex);

                    continue;
                }
            }
        }
    }
}
