using Nop.Core;
using Nop.Plugin.StorageMedia.AmazonS3.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.StorageMedia.AmazonS3
{
    public class AmazonS3Plugin : BasePlugin, IMiscPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public AmazonS3Plugin(
            ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/AmazonS3/Configure";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            EnsureSettingAdded();
            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<AmazonS3Settings>();

            //locales
            _localizationService.DeletePluginLocaleResources("Plugins.StorageMedia.AmazonS3");

            base.Uninstall();
        }

        public void EnsureSettingAdded()
        {
            //settings
            var settings = _settingService.LoadSetting<AmazonS3Settings>();
            settings = settings ?? new AmazonS3Settings();
            _settingService.SaveSetting(settings);

            var localeStrings = new Dictionary<string, string>
            {
                ["Plugins.StorageMedia.AmazonS3.Instructions"] = "Amazon S3 Setting",
                ["Plugins.StorageMedia.AmazonS3.Fields.BucketName"] = "Bucket Name",
                ["Plugins.StorageMedia.AmazonS3.Fields.AccessKey"] = "Access Key",
                ["Plugins.StorageMedia.AmazonS3.Fields.AccessSecret"] = "Access Secret",
                ["Plugins.StorageMedia.AmazonS3.Fields.S3BaseUrl"] = "S3 Base Url",
                ["Plugins.StorageMedia.AmazonS3.Fields.PreSignedMinutes"] = "PreSigned Minutes"
            };

            var localesStringKeys = new List<string>();
            localesStringKeys.AddRange(localeStrings.Keys);

            foreach (var localesStringKey in localesStringKeys)
            {
                var locale = _localizationService.GetLocaleStringResourceByName(localesStringKey);
                if (locale != null)
                {
                    localeStrings[localesStringKey] = locale.ResourceValue;
                }
            }

            //locales
            _localizationService.AddPluginLocaleResource(localeStrings);
        }

        #endregion
    }
}
