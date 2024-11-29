using Nop.Core;
using Nop.Plugin.NopStation.WebApi.Domains;
using System;

namespace Nop.Plugin.NopStation.AppPushNotification.Domains
{
    public class AppQueuedPushNotification : BaseEntity
    {
        public int CustomerId { get; set; }

        public int StoreId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string ImageUrl { get; set; }

        public string ErrorLog { get; set; }

        public int SentTries { get; set; }

        public int AppDeviceId { get; set; }

        public int DeviceTypeId { get; set; }

        public int ActionTypeId { get; set; }

        public string ActionValue { get; set; }

        public string SubscriptionId { get; set; }
        public int AppTypeId { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime? SentOnUtc { get; set; }

        public DateTime? DontSendBeforeDateUtc { get; set; }
        public string UniqueNotificationId { get; set; }

        public NotificationActionType ActionType
        {
            get => (NotificationActionType)ActionTypeId;
            set => ActionTypeId = (int)value;
        }

        public DeviceType DeviceType
        {
            get => (DeviceType)DeviceTypeId;
            set => DeviceTypeId = (int)value;
        }
        public AppType AppType
        {
            get => (AppType)AppTypeId;
            set => AppTypeId = (int)value;
        }
    }
}
