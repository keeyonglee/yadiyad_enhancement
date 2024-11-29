using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models
{
    public class ConfigurationModel : BaseNopModel, ISettingsModel
    {
        public ConfigurationModel()
        {
            AvailableApplicationTypes = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.Configuration.Fields.GoogleConsoleApiAccessKey")]
        public string GoogleConsoleApiAccessKey { get; set; }
        public bool GoogleConsoleApiAccessKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.Configuration.Fields.ApplicationTypeId")]
        public int ApplicationTypeId { get; set; }
        public bool ApplicationTypeId_OverrideForStore { get; set; }

        public IList<SelectListItem> AvailableApplicationTypes { get; set; }

        public int ActiveStoreScopeConfiguration { get; set; }
    }
}
