using System.Linq;
using Nop.Core;
using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Factories
{
    public class AppPushNotificationModelFactory : IAppPushNotificationModelFactory
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public AppPushNotificationModelFactory(ISettingService settingService,
            IStoreContext storeContext)
        {
            _settingService = settingService;
            _storeContext = storeContext;
        }

        #endregion
        
        #region Methods

        public ConfigurationModel PrepareConfigurationModel(ConfigurationModel model)
        {
            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<AppPushNotificationSettings>(storeId);

            model = settings.ToSettingsModel<ConfigurationModel>();
            model.ActiveStoreScopeConfiguration = storeId;
            model.AvailableApplicationTypes = ApplicationType.Native.ToSelectList().ToList();

            if (storeId == 0)
                return model;

            model.GoogleConsoleApiAccessKey_OverrideForStore = _settingService.SettingExists(settings, x => x.GoogleConsoleApiAccessKey, storeId);
            model.ApplicationTypeId_OverrideForStore = _settingService.SettingExists(settings, x => x.ApplicationTypeId, storeId);

            return model;
        }

        #endregion
    }
}
