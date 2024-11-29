using Nop.Core.Configuration;

namespace Nop.Plugin.NopStation.AppPushNotification
{
    public class AppPushNotificationSettings : ISettings
    {
        public int DefaultIconId { get; set; }

        public string GoogleConsoleApiAccessKey { get; set; }
        public string VendorAppApiAccessKey { get; set; }

        public int ApplicationTypeId { get; set; }
    }
}
