using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.StorageMedia.AmazonS3.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.StorageMedia.AmazonS3.Controllers
{
    public class AmazonS3Controller : BasePluginController
    {
        private const string _VIEW_PATH = "~/Plugins/StorageMedia.AmazonS3/Views";

        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly AmazonS3Plugin _amazonS3Plugin;

        public AmazonS3Controller(
            IPermissionService permissionService,
            IStoreContext storeContext,
            INotificationService notificationService,
            ILocalizationService localizationService,
            AmazonS3Plugin amazonS3Plugin,
            ISettingService settingService)
        {
            _settingService = settingService;
            _permissionService = permissionService;
            _storeContext = storeContext;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _amazonS3Plugin = amazonS3Plugin;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            _amazonS3Plugin.EnsureSettingAdded();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var awsS3Config = _settingService.LoadSetting<AmazonS3Settings>(storeScope);

            var model = new ConfigurationModel
            {
                BucketName = awsS3Config.BucketName,
                PrivateBucketName = awsS3Config.PrivateBucketName,
                AccessKey = awsS3Config.AccessKey,
                AccessSecret = awsS3Config.AccessSecret,
                S3BaseUrl = awsS3Config.S3BaseUrl,
                PreSignedMinutes = awsS3Config.PreSignedMinutes,

                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.BucketName_OverrideForStore = _settingService.SettingExists(awsS3Config, x => x.BucketName, storeScope);
                model.PrivateBucketName_OverrideForStore = _settingService.SettingExists(awsS3Config, x => x.PrivateBucketName, storeScope);
                model.AccessKey_OverrideForStore = _settingService.SettingExists(awsS3Config, x => x.AccessKey, storeScope);
                model.AccessSecret_OverrideForStore = _settingService.SettingExists(awsS3Config, x => x.AccessSecret, storeScope);
                model.S3BaseUrl_OverrideForStore = _settingService.SettingExists(awsS3Config, x => x.S3BaseUrl, storeScope);
                model.PreSignedMinutes_OverrideForStore = _settingService.SettingExists(awsS3Config, x => x.PreSignedMinutes, storeScope);
            }

            return View($"{_VIEW_PATH}/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var awsS3Config = _settingService.LoadSetting<AmazonS3Settings>(storeScope);

            //save settings
            awsS3Config.BucketName = model.BucketName;
            awsS3Config.PrivateBucketName = model.PrivateBucketName;
            awsS3Config.AccessKey = model.AccessKey;
            awsS3Config.AccessSecret = model.AccessSecret;
            awsS3Config.S3BaseUrl = model.S3BaseUrl;
            awsS3Config.PreSignedMinutes = model.PreSignedMinutes;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(awsS3Config, x => x.BucketName, model.BucketName_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(awsS3Config, x => x.PrivateBucketName, model.PrivateBucketName_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(awsS3Config, x => x.AccessKey, model.AccessKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(awsS3Config, x => x.AccessSecret, model.AccessSecret_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(awsS3Config, x => x.S3BaseUrl, model.S3BaseUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(awsS3Config, x => x.PreSignedMinutes, model.PreSignedMinutes_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }
    }
}
