using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Plugin.NopStation.Core.Models;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.NopStation.Core.Controllers
{
    public class NopStationCoreController : BaseAdminController
    {
        private readonly IStoreContext _storeContext;
        private readonly INopStationLicenseService _licenseService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly INopStationPluginManager _nopStationPluginManager;

        public NopStationCoreController(IStoreContext storeContext,
            INopStationLicenseService licenseService, 
            INotificationService notificationService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IWorkContext workContext,
            IBaseAdminModelFactory baseAdminModelFactory,
            INopStationPluginManager nopStationPluginManager)
        {
            _storeContext = storeContext;
            _licenseService = licenseService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _workContext = workContext;
            _baseAdminModelFactory = baseAdminModelFactory;
            _nopStationPluginManager = nopStationPluginManager;
        }

        [NopStationLicense]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(CorePermissionProvider.ManageConfiguration))
                return AccessDeniedView();

            return RedirectToAction("License");
        }

        [NopStationLicense]
        public IActionResult LocaleResource()
        {
            if (!_permissionService.Authorize(CorePermissionProvider.ManageConfiguration))
                return AccessDeniedView();

            var searchModel = new CoreLocaleResourceSearchModel();
            searchModel.SearchLanguageId = _workContext.WorkingLanguage.Id;
            _baseAdminModelFactory.PrepareLanguages(searchModel.AvailableLanguages, false);

            var plugins = _nopStationPluginManager.LoadNopStationPlugins(storeId: _storeContext.CurrentStore.Id);
            foreach (var item in plugins)
            {
                searchModel.AvailablePlugins.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                {
                    Value = item.PluginDescriptor.SystemName,
                    Text = item.PluginDescriptor.FriendlyName
                });
            }
            searchModel.AvailablePlugins.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
            {
                Value = "",
                Text = _localizationService.GetResource("Admin.NopStation.Core.Resources.List.SearchPluginSystemName.All")
            });
            
            return View(searchModel);
        }

        [HttpPost, NopStationLicense]
        public IActionResult LocaleResource(CoreLocaleResourceSearchModel searchModel)
        {
            if (!_permissionService.Authorize(CorePermissionProvider.ManageConfiguration))
                return AccessDeniedDataTablesJson();

            var resources = _nopStationPluginManager.LoadPluginStringResources(searchModel.SearchPluginSystemName, 
                searchModel.SearchResourceName, searchModel.SearchLanguageId, _storeContext.CurrentStore.Id, 
                searchModel.Page - 1, searchModel.PageSize);

            var model = new CoreLocaleResourceListModel().PrepareToGrid(searchModel, resources, () =>
            {
                return resources.Select(resource =>
                {
                    return new CoreLocaleResourceModel()
                    {
                        ResourceName = resource.Key,
                        ResourceValue = resource.Value,
                        ResourceNameLanguageId = $"{resource.Key}___{searchModel.SearchLanguageId}"
                    };
                });
            });

            return Json(model);
        }

        [HttpPost, NopStationLicense]
        public IActionResult ResourceUpdate(CoreLocaleResourceModel model)
        {
            if (!_permissionService.Authorize(CorePermissionProvider.ManageConfiguration))
                return AccessDeniedDataTablesJson();

            if(string.IsNullOrWhiteSpace(model.ResourceNameLanguageId))
                return ErrorJson(_localizationService.GetResource("Admin.NopStation.Core.Resources.FailedToSave"));

            var token = model.ResourceNameLanguageId.Split(new[] { "___" }, StringSplitOptions.None);
            model.ResourceName = token[0];
            model.LanguageId = int.Parse(token[1]);

            if (model.ResourceValue != null)
                model.ResourceValue = model.ResourceValue.Trim();

            var resource = _localizationService.GetLocaleStringResourceByName(model.ResourceName, model.LanguageId);

            if (resource != null)
            {
                resource.ResourceValue = model.ResourceValue;
                _localizationService.UpdateLocaleStringResource(resource);
            }
            else
            {
                var rs = model.ToEntity<LocaleStringResource>();
                rs.LanguageId = model.LanguageId;
                _localizationService.InsertLocaleStringResource(rs);
            }

            return new NullJsonResult();
        }

        public IActionResult PluginInfo()
        {
            if (!_permissionService.Authorize(CorePermissionProvider.ManageLicense))
                return AccessDeniedView();

            var plugins = _nopStationPluginManager.LoadNopStationPlugins(storeId: _storeContext.CurrentStore.Id);
            var model = plugins.Select(x => new PluginInfoModel()
            {
                FileName = x.PluginDescriptor.AssemblyFileName,
                Version = x.PluginDescriptor.Version,
                Name = x.PluginDescriptor.FriendlyName
            });

            return View(model);
        }

        public IActionResult License()
        {
            if (!_permissionService.Authorize(CorePermissionProvider.ManageLicense))
                return AccessDeniedView();

            var storeId = _storeContext.ActiveStoreScopeConfiguration;

            var model = new LicenseModel();
            model.ActiveStoreScopeConfiguration = storeId;

            return View(model);
        }
        
        [HttpPost]
        public IActionResult License(LicenseModel model)
        {
            if (!_permissionService.Authorize(CorePermissionProvider.ManageLicense))
                return AccessDeniedView();

            var result = _licenseService.VerifyProductKey(model.LicenseString);

            switch (result)
            {
                case KeyVerificationResult.InvalidProductKey:
                    _notificationService.ErrorNotification(_localizationService.GetResource("Admin.NopStation.Core.License.InvalidProductKey"));
                    return View(model);
                case KeyVerificationResult.InvalidForDomain:
                    _notificationService.ErrorNotification(_localizationService.GetResource("Admin.NopStation.Core.License.InvalidForDomain"));
                    return View(model);
                case KeyVerificationResult.InvalidForNOPVersion:
                    _notificationService.ErrorNotification(_localizationService.GetResource("Admin.NopStation.Core.License.InvalidForNOPVersion"));
                    return View(model);
                case KeyVerificationResult.Valid:
                    var storeId = _storeContext.ActiveStoreScopeConfiguration;
                    var settings = _settingService.LoadSetting<NopStationCoreSettings>(storeId);

                    settings.LicenseStrings.Add(model.LicenseString);
                    _settingService.SaveSetting(settings);

                    _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.Core.License.Saved"));

                    return RedirectToAction("License");
                default:
                    return RedirectToAction("License");
            }
        }
    }
}
