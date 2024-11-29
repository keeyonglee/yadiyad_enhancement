using System;
using Nop.Core;
using Nop.Core.Domain.Localization;

namespace Nop.Plugin.NopStation.AppPushNotification.Domains
{
    public class AppPushNotificationCampaign : BaseEntity, ILocalizedEntity
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public int ImageId { get; set; }

        public DateTime SendingWillStartOnUtc { get; set; }

        public DateTime? AddedToQueueOnUtc { get; set; }

        public int LimitedToStoreId { get; set; }

        public bool Deleted { get; set; }

        public string CustomerRoles { get; set; }

        public string DeviceTypes { get; set; }

        public int ActionTypeId { get; set; }

        public string ActionValue { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public NotificationActionType ActionType
        {
            get => (NotificationActionType)ActionTypeId;
            set => ActionTypeId = (int)value;
        }

        public AppPushNotificationCampaign Clone()
        {
            var campaign = new AppPushNotificationCampaign()
            {
                Body = Body,
                ImageId = ImageId,
                LimitedToStoreId = LimitedToStoreId,
                Name = Name,
                SendingWillStartOnUtc = SendingWillStartOnUtc,
                Title = Title,
                CustomerRoles = CustomerRoles,
                DeviceTypes = DeviceTypes,
                ActionValue = ActionValue,
                ActionTypeId = ActionTypeId
            };

            return campaign;
        }
    }
}
