using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Stores;
using Nop.Plugin.NopStation.WebApi.Domains;
using System;

namespace Nop.Plugin.NopStation.AppPushNotification.Domains
{
    public class AppPushNotificationTemplate : BaseEntity, ILocalizedEntity, IStoreMappingSupported
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public int ImageId { get; set; }

        public bool Active { get; set; }

        public bool LimitedToStores { get; set; }

        public bool SendImmediately { get; set; }

        public int? DelayBeforeSend { get; set; }

        public int DelayPeriodId { get; set; }

        public int ActionTypeId { get; set; }

        public string ActionValue { get; set; }
        public int AppTypeId { get; set; }

        public NotificationDelayPeriod DelayPeriod
        {
            get => (NotificationDelayPeriod)DelayPeriodId;
            set => DelayPeriodId = (int)value;
        }

        public NotificationActionType ActionType
        {
            get => (NotificationActionType)ActionTypeId;
            set => ActionTypeId = (int)value;
        }
        public AppType AppType
        {
            get => (AppType)AppTypeId;
            set => AppTypeId = (int)value;
        }
    }
}
