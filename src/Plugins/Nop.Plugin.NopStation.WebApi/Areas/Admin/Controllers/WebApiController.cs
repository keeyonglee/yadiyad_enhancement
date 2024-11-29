using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Localization;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Factories;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Models;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Plugin.NopStation.WebApi.Infrastructure.Cache;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Controllers
{
    [NopStationLicense]
    public class WebApiController : BaseAdminController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ILanguageService _languageService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IWebApiModelFactory _webApiModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly IApiStringResourceService _apiStringResourceService;
        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public WebApiController(ILocalizationService localizationService,
            INotificationService notificationService,
            ILanguageService languageService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IWebApiModelFactory webApiModelFactory,
            IPermissionService permissionService,
            IApiStringResourceService apiStringResourceService,
            IStaticCacheManager cacheManager)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _languageService = languageService;
            _settingService = settingService;
            _storeContext = storeContext;
            _workContext = workContext;
            _webApiModelFactory = webApiModelFactory;
            _permissionService = permissionService;
            _apiStringResourceService = apiStringResourceService;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        #endregion

        #region Methods

        #region Configuration

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageConfiguration))
                return AccessDeniedView();

            var model = _webApiModelFactory.PrepareConfigurationModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageConfiguration))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var webApiSettings = _settingService.LoadSetting<WebApiSettings>(storeScope);
            webApiSettings = model.ToSettings(webApiSettings);

            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.CheckIat, model.CheckIat_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.DefaultCategoryIcon, model.DefaultCategoryIcon_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.IOSProductPriceTextSize, model.IOSProductPriceTextSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.IonicProductPriceTextSize, model.IonicProductPriceTextSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.AndroidProductPriceTextSize, model.AndroidProductPriceTextSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.EnableCORS, model.EnableCORS_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.ShowHomepageSlider, model.ShowHomepageSlider_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.SliderAutoPlay, model.SliderAutoPlay_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.SliderAutoPlayTimeout, model.SliderAutoPlayTimeout_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.EnableJwtSecurity, model.EnableJwtSecurity_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.MaximumNumberOfHomePageSliders, model.MaximumNumberOfHomePageSliders_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.NumberOfHomepageCategoryProducts, model.NumberOfHomepageCategoryProducts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.NumberOfManufacturers, model.NumberOfManufacturers_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.ShowBestsellersOnHomepage, model.ShowBestsellersOnHomepage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.NumberOfBestsellersOnHomepage, model.NumberOfBestsellersOnHomepage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.ShowFeaturedProducts, model.ShowFeaturedProducts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.ShowHomepageCategoryProducts, model.ShowHomepageCategoryProducts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.ShowManufacturers, model.ShowManufacturers_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.ShowSubCategoryProducts, model.ShowSubCategoryProducts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.TokenKey, model.TokenKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.TokenSecondsValid, model.TokenSecondsValid_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.TokenSecret, model.TokenSecret_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.AndroidVersion, model.AndroidVersion_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.AndriodForceUpdate, model.AndriodForceUpdate_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.PlayStoreUrl, model.PlayStoreUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.IOSVersion, model.IOSVersion_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.IOSForceUpdate, model.IOSForceUpdate_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.AppStoreUrl, model.AppStoreUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.LogoId, model.LogoId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.LogoSize, model.LogoSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.ShowChangeBaseUrlPanel, model.ShowChangeBaseUrlPanel_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.PrimaryThemeColor, model.PrimaryThemeColor_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.BottomBarActiveColor, model.BottomBarActiveColor_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.BottomBarBackgroundColor, model.BottomBarBackgroundColor_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.BottomBarInactiveColor, model.BottomBarInactiveColor_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.TopBarBackgroundColor, model.TopBarBackgroundColor_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.TopBarTextColor, model.TopBarTextColor_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.GradientStartingColor, model.GradientStartingColor_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.GradientMiddleColor, model.GradientMiddleColor_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.GradientEndingColor, model.GradientEndingColor_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.GradientEnabled, model.GradientEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(webApiSettings, x => x.ProductBarcodeScanKeyId, model.ProductBarcodeScanKeyId_OverrideForStore, storeScope, false);

            _settingService.ClearCache();
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SliderPrefixCacheKey);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.WebApi.Configuration.Updated"));
            return RedirectToAction("Configure");
        }

        #endregion

        #region Resources

        [HttpPost]
        public virtual IActionResult Resources(LocaleResourceSearchModel searchModel)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageConfiguration))
                return AccessDeniedDataTablesJson();

            //try to get a language with the specified id
            var language = _languageService.GetLanguageById(searchModel.LanguageId);
            if (language == null)
                return RedirectToAction("List");

            //prepare model
            var model = _webApiModelFactory.PrepareLocaleResourceListModel(searchModel, language);

            return Json(model);
        }

        //ValidateAttribute is used to force model validation
        [HttpPost]
        public virtual IActionResult ResourceUpdate([Validate] LocaleResourceModel model)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageConfiguration))
                return AccessDeniedDataTablesJson();

            if (model.ResourceName != null)
                model.ResourceName = model.ResourceName.Trim();
            if (model.ResourceValue != null)
                model.ResourceValue = model.ResourceValue.Trim();

            if (!ModelState.IsValid)
            {
                return ErrorJson(ModelState.SerializeErrors());
            }

            var resource = _localizationService.GetLocaleStringResourceById(model.Id);
            // if the resourceName changed, ensure it isn't being used by another resource
            if (!resource.ResourceName.Equals(model.ResourceName, StringComparison.InvariantCultureIgnoreCase))
            {
                var res = _localizationService.GetLocaleStringResourceByName(model.ResourceName, model.LanguageId, false);
                if (res != null && res.Id != resource.Id)
                {
                    var appResource = _apiStringResourceService.GetApiStringResourceByName(res.ResourceName);
                    if (appResource == null)
                        return ErrorJson(string.Format(_localizationService.GetResource("Admin.NopStation.WebApi.Resources.NameAlreadyExists"),
                            model.ResourceName, _localizationService.GetResource("Admin.NopStation.WebApi.Resources.AddFromExistingRecords")));
                    else
                        return ErrorJson(string.Format(_localizationService.GetResource("Admin.Configuration.Languages.Resources.NameAlreadyExists"), res.ResourceName));
                }
            }

            //fill entity from model
            resource = model.ToEntity(resource);

            _localizationService.UpdateLocaleStringResource(resource);

            var ar = _apiStringResourceService.GetApiStringResourceByName(resource.ResourceName);
            if (ar == null)
            {
                var appResource = new ApiStringResource()
                {
                    ResourceName = resource.ResourceName
                };
                _apiStringResourceService.InsertApiStringResource(appResource);
            }

            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.StringResourcePrefixCacheKey);
            return new NullJsonResult();
        }

        //ValidateAttribute is used to force model validation
        [HttpPost]
        public virtual IActionResult ResourceAdd(int languageId, [Validate] LocaleResourceModel model)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageConfiguration))
                return AccessDeniedDataTablesJson();

            if (model.ResourceName != null)
                model.ResourceName = model.ResourceName.Trim();
            if (model.ResourceValue != null)
                model.ResourceValue = model.ResourceValue.Trim();

            if (!ModelState.IsValid)
            {
                return ErrorJson(ModelState.SerializeErrors());
            }

            var res = _localizationService.GetLocaleStringResourceByName(model.ResourceName, model.LanguageId, false);
            if (res == null)
            {
                //fill entity from model
                var resource = model.ToEntity<LocaleStringResource>();

                resource.LanguageId = languageId;

                _localizationService.InsertLocaleStringResource(resource);

                var ar = _apiStringResourceService.GetApiStringResourceByName(resource.ResourceName);
                if (ar == null)
                {
                    var appResource = new ApiStringResource()
                    {
                        ResourceName = resource.ResourceName
                    };
                    _apiStringResourceService.InsertApiStringResource(appResource);
                }
            }
            else
            {
                var ar = _apiStringResourceService.GetApiStringResourceByName(res.ResourceName);
                if (ar == null)
                    return ErrorJson(string.Format(_localizationService.GetResource("Admin.NopStation.WebApi.Resources.NameAlreadyExists"), 
                        model.ResourceName, _localizationService.GetResource("Admin.NopStation.WebApi.Resources.AddFromExistingRecords")));
                else
                    return ErrorJson(string.Format(_localizationService.GetResource("Admin.Configuration.Languages.Resources.NameAlreadyExists"), res.ResourceName));
            }

            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.StringResourcePrefixCacheKey);
            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult ResourceDelete(int id)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageConfiguration))
                return AccessDeniedDataTablesJson();

            //try to get a locale resource with the specified id
            var resource = _localizationService.GetLocaleStringResourceById(id)
                ?? throw new ArgumentException("No resource found with the specified id", nameof(id));

            var appResource = _apiStringResourceService.GetApiStringResourceByName(resource.ResourceName);
            if (appResource != null)
                _apiStringResourceService.DeleteApiStringResource(appResource);

            return new NullJsonResult();
        }

        public IActionResult ExistingResourceAddPopup(int languageId)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageConfiguration))
                return AccessDeniedView();

            var model = new ConfigurationModel();
            model.LocaleResourceSearchModel.LanguageId = languageId;

            return View(model);
        }

        [HttpPost]
        public IActionResult ExistingResourceAddPopup(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageConfiguration))
                return AccessDeniedView();

            if (model.SelectedResourceIds.Any())
            {
                foreach (var item in model.SelectedResourceIds)
                {
                    var resource = _localizationService.GetLocaleStringResourceById(item);
                    if (resource == null)
                        continue;

                    var ar = _apiStringResourceService.GetApiStringResourceByName(resource.ResourceName);
                    if (ar == null)
                    {
                        var appResource = new ApiStringResource()
                        {
                            ResourceName = resource.ResourceName
                        };
                        _apiStringResourceService.InsertApiStringResource(appResource);
                    }
                }
            }

            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.StringResourcePrefixCacheKey);
            ViewBag.RefreshPage = true;

            return View(new ConfigurationModel());
        }

        #endregion

        #endregion
    }
}
