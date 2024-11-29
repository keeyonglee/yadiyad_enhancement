using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Factories;
using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models;
using Nop.Plugin.NopStation.AppPushNotification.Services;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Controllers
{
    [NopStationLicense]
    public class AppPushNotificationController : BaseAdminController
    {
        #region Fields
        
        private readonly INotificationService _notificationService;
        private readonly IAppPushNotificationModelFactory _appModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IPermissionService _permissionService;
        private readonly IApiDeviceService _apiDeviceService;
        private readonly IPushNotificationSender _pushNotificationSender;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public AppPushNotificationController(INotificationService notificationService,
            IAppPushNotificationModelFactory appModelFactory,
            ILocalizationService localizationService,
            ISettingService settingService,
            IStoreContext storeContext,
            IPermissionService permissionService,
            IApiDeviceService apiDeviceService,
            IPushNotificationSender pushNotificationSender,
            ICustomerService customerService)
        {
            _permissionService = permissionService;
            _notificationService = notificationService;
            _appModelFactory = appModelFactory;
            _localizationService = localizationService;
            _settingService = settingService;
            _storeContext = storeContext;
            _apiDeviceService = apiDeviceService;
            _pushNotificationSender = pushNotificationSender;
            _customerService = customerService;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageConfiguration))
                return AccessDeniedView();

            var model = _appModelFactory.PrepareConfigurationModel(new ConfigurationModel());
            return View(model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageConfiguration))
                return AccessDeniedView();

            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var appSettings = _settingService.LoadSetting<AppPushNotificationSettings>(storeScope);
            appSettings = model.ToSettings(appSettings);

            _settingService.SaveSettingOverridablePerStore(appSettings, x => x.GoogleConsoleApiAccessKey, model.GoogleConsoleApiAccessKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(appSettings, x => x.ApplicationTypeId, model.ApplicationTypeId_OverrideForStore, storeScope, false);

            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.AppPushNotification.Configuration.Updated"));

            return RedirectToAction("Configure");
        }

        [HttpPost]
        public IActionResult SendTestNotification(int id)
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageConfiguration)||
                !_permissionService.Authorize(WebApiPermissionProvider.ManageDevice))
                return AccessDeniedView();

            var device = _apiDeviceService.GetApiDeviceById(id);
            if (device == null)
                return Json(new { Result = false });

            var title = string.Format(_localizationService.GetResource("Admin.NopStation.AppPushNotification.TestNotification.Title"),
                _localizationService.GetResource("Admin.NopStation.AppPushNotification.TestNotification.Guest"));
            var customer = _customerService.GetCustomerById(device.CustomerId);
            if(customer != null && _customerService.IsRegistered(customer))
                title = string.Format(_localizationService.GetResource("Admin.NopStation.AppPushNotification.TestNotification.Title"),
                    _customerService.GetCustomerFullName(customer));

            var body = string.Format(_localizationService.GetResource("Admin.NopStation.AppPushNotification.TestNotification.Body"),
                _localizationService.GetLocalized(_storeContext.CurrentStore, x => x.Name));

            _pushNotificationSender.SendNotification(device.DeviceType, title, body, device.SubscriptionId, device.AppTypeId);

            return Json(new { 
                Result = true, 
                Message = _localizationService.GetResource("Admin.NopStation.AppPushNotification.TestNotification.SentSuccessFully") 
            });
        }

        #endregion
    }
}
