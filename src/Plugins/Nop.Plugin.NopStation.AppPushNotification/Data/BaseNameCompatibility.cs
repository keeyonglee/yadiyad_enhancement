using System;
using System.Collections.Generic;
using System.Text;
using Nop.Data.Mapping;
using Nop.Plugin.NopStation.AppPushNotification.Domains;

namespace Nop.Plugin.NopStation.AppPushNotification.Data
{
    public class BaseNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(AppPushNotificationCampaign), "NS_AppPushNotificationCampaign" },
            { typeof(AppPushNotificationTemplate), "NS_AppPushNotificationTemplate" },
            { typeof(AppQueuedPushNotification), "NS_AppQueuedPushNotification" },
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>
        {
        };
    }
}
