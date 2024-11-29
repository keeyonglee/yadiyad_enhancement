using Nop.Core;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using System;
using Nop.Plugin.NopStation.AppPushNotification.Services.Models;

namespace Nop.Plugin.NopStation.AppPushNotification.Services
{
    public interface IQueuedPushNotificationService
    {
        void DeleteQueuedPushNotification(AppQueuedPushNotification queuedPushNotification);
        void DeleteSentQueuedPushNotification();

        void InsertQueuedPushNotification(AppQueuedPushNotification queuedPushNotification);

        void UpdateQueuedPushNotification(AppQueuedPushNotification queuedPushNotification);

        AppQueuedPushNotification GetQueuedPushNotificationById(int queuedPushNotificationId);

        IPagedList<AppQueuedPushNotification> GetAllQueuedPushNotifications(bool? sentStatus = null,
            bool enableDateConsideration = false, DateTime? sentFromUtc = null, DateTime? sentToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

        IPagedList<AppNotificationModel> GetCustomerNotifications(int customerId, int appTypeId, int pageIndex, int pageSize = 1000);
    }
}