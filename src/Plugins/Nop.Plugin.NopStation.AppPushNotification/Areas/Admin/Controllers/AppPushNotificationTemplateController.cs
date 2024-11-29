using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Factories;
using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using Nop.Plugin.NopStation.AppPushNotification.Services;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Mvc.Filters;
using System.Linq;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Controllers
{
    [NopStationLicense]
    public class AppPushNotificationTemplateController : BaseAdminController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPushNotificationTemplateModelFactory _pushNotificationTemplateModelFactory;
        private readonly IPushNotificationTemplateService _pushNotificationTemplateService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public AppPushNotificationTemplateController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPushNotificationTemplateModelFactory pushNotificationTemplateModelFactory,
            IPushNotificationTemplateService pushNotificationTemplateService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            ILocalizedEntityService localizedEntityService,
            IPermissionService permissionService)
        {
            _permissionService = permissionService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _pushNotificationTemplateModelFactory = pushNotificationTemplateModelFactory;
            _pushNotificationTemplateService = pushNotificationTemplateService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _localizedEntityService = localizedEntityService;
        }

        #endregion

        #region Utilities

        protected virtual void UpdateLocales(AppPushNotificationTemplate category, AppPushNotificationTemplateModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(category,
                    x => x.Title,
                    localized.Title,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                    x => x.Body,
                    localized.Body,
                    localized.LanguageId);
            }
        }

        protected virtual void SaveStoreMappings(AppPushNotificationTemplate pushNotificationTemplate, AppPushNotificationTemplateModel model)
        {
            pushNotificationTemplate.LimitedToStores = model.SelectedStoreIds.Any();

            //manage store mappings
            var existingStoreMappings = _storeMappingService.GetStoreMappings(pushNotificationTemplate);
            foreach (var store in _storeService.GetAllStores())
            {
                var existingStoreMapping = existingStoreMappings.FirstOrDefault(storeMapping => storeMapping.StoreId == store.Id);

                //new store mapping
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    if (existingStoreMapping == null)
                        _storeMappingService.InsertStoreMapping(pushNotificationTemplate, store.Id);
                }
                //or remove existing one
                else if (existingStoreMapping != null)
                    _storeMappingService.DeleteStoreMapping(existingStoreMapping);
            }
        }

        #endregion

        #region Methods        

        public virtual IActionResult Index()
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageTemplates))
                return AccessDeniedView();

            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageTemplates))
                return AccessDeniedView();

            var searchModel = _pushNotificationTemplateModelFactory.PreparePushNotificationTemplateSearchModel(new AppPushNotificationTemplateSearchModel());
            return View(searchModel);
        }

        public virtual IActionResult GetList(AppPushNotificationTemplateSearchModel searchModel)
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageTemplates))
                return AccessDeniedDataTablesJson();

            var model = _pushNotificationTemplateModelFactory.PreparePushNotificationTemplateListModel(searchModel);
            return Json(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageTemplates))
                return AccessDeniedView();

            var pushNotificationTemplate = _pushNotificationTemplateService.GetPushNotificationTemplateById(id);
            if (pushNotificationTemplate == null)
                return RedirectToAction("List");

            var model = _pushNotificationTemplateModelFactory.PreparePushNotificationTemplateModel(null, pushNotificationTemplate);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(AppPushNotificationTemplateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageTemplates))
                return AccessDeniedView();

            var pushNotificationTemplate = _pushNotificationTemplateService.GetPushNotificationTemplateById(model.Id);
            if (pushNotificationTemplate == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                pushNotificationTemplate = model.ToEntity(pushNotificationTemplate);

                _pushNotificationTemplateService.UpdatePushNotificationTemplate(pushNotificationTemplate);

                SaveStoreMappings(pushNotificationTemplate, model);

                UpdateLocales(pushNotificationTemplate, model);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = pushNotificationTemplate.Id });
            }

            model = _pushNotificationTemplateModelFactory.PreparePushNotificationTemplateModel(model, pushNotificationTemplate);
            
            return View(model);
        }

        #endregion
    }
}
