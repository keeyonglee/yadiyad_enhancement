using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Factories
{
    public interface IAppPushNotificationModelFactory
    {
        ConfigurationModel PrepareConfigurationModel(ConfigurationModel model);
    }
}