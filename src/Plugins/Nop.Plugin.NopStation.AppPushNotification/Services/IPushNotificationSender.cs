using Nop.Plugin.NopStation.AppPushNotification.Domains;
using Nop.Plugin.NopStation.WebApi.Domains;

namespace Nop.Plugin.NopStation.AppPushNotification.Services
{
    public interface IPushNotificationSender
    {
        void SendNotification(AppQueuedPushNotification queuedPushNotification);

        void SendNotification(DeviceType deviceType, string title, string body, string subscriptionId, int appTypeId,
            int actionTypeId = 0, string actionValue = "", string imageUrl = "");
    }
}