using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Models
{
    public class AppLandingConfigurationModel : BaseNopModel, ISettingsModel
    {
        [NopResourceDisplayName("Admin.NopStation.WebApi.AppLandingConfigurationModel.Fields.EnableFeatureProducts")]
        public bool EnableFeatureProducts { get; set; }
        [NopResourceDisplayName("Admin.NopStation.WebApi.AppLandingConfigurationModel.Fields.EnableBestSellingProducts")]
        public bool EnableBestSellingProducts { get; set; }
        [NopResourceDisplayName("Admin.NopStation.WebApi.AppLandingConfigurationModel.Fields.EnableHomeCategoriesProducts")]
        public bool EnableHomeCategoriesProducts { get; set; }
        [NopResourceDisplayName("Admin.NopStation.WebApi.AppLandingConfigurationModel.Fields.EnableSubCategoriesProducts")]
        public bool EnableSubCategoriesProducts { get; set; }
        [NopResourceDisplayName("Admin.NopStation.WebApi.AppLandingConfigurationModel.Fields.NumberOfHomeCategoriesProducts")]
        public int NumberOfHomeCategoriesProducts { get; set; }
        [NopResourceDisplayName("Admin.NopStation.WebApi.AppLandingConfigurationModel.Fields.NumberOfManufaturer")]
        public int NumberOfManufaturer { get; set; }
        public int ActiveStoreScopeConfiguration { get; set; }
    }
}
