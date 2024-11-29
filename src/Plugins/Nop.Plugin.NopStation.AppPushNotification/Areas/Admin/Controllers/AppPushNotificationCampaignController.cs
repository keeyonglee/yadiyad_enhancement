using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Factories;
using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using Nop.Plugin.NopStation.AppPushNotification.Services;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Controllers
{
    [NopStationLicense]
    public class AppPushNotificationCampaignController : BaseAdminController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPushNotificationCampaignModelFactory _pushNotificationCampaignModelFactory;
        private readonly IPushNotificationCampaignService _campaignService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILanguageService _languageService;

        #endregion

        #region Ctor

        public AppPushNotificationCampaignController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPushNotificationCampaignModelFactory pushNotificationCampaignModelFactory,
            IPushNotificationCampaignService campaignService,
            IDateTimeHelper dateTimeHelper,
            IPermissionService permissionService,
            ILocalizedEntityService localizedEntityService,
            ILanguageService languageService)
        {
            _permissionService = permissionService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _pushNotificationCampaignModelFactory = pushNotificationCampaignModelFactory;
            _campaignService = campaignService;
            _dateTimeHelper = dateTimeHelper;
            _localizedEntityService = localizedEntityService;
            _languageService = languageService;
        }

        #endregion

        #region Utilities

        protected virtual void CopyLocalizationData(AppPushNotificationCampaign campaign, AppPushNotificationCampaign campaignCopy)
        {
            var languages = _languageService.GetAllLanguages(true);

            //localization
            foreach (var lang in languages)
            {
                var name = _localizationService.GetLocalized(campaign, x => x.Title, lang.Id, false, false);
                if (!string.IsNullOrEmpty(name))
                    _localizedEntityService.SaveLocalizedValue(campaignCopy, x => x.Title, name, lang.Id);

                var shortDescription = _localizationService.GetLocalized(campaign, x => x.Body, lang.Id, false, false);
                if (!string.IsNullOrEmpty(shortDescription))
                    _localizedEntityService.SaveLocalizedValue(campaignCopy, x => x.Body, shortDescription, lang.Id);
            }
        }

        protected int[] GetVibration(string vibration)
        {
            if (string.IsNullOrWhiteSpace(vibration))
                return new int[] { };

            var lst = new List<int>();
            var tokens = vibration.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in tokens)
            {
                if (int.TryParse(item, out var x))
                    lst.Add(x);
            }

            return lst.ToArray();
        }

        protected virtual void UpdateLocales(AppPushNotificationCampaign category, AppPushNotificationCampaignModel model)
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

        #endregion

        #region Method

        public IActionResult Index()
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var searchModel = _pushNotificationCampaignModelFactory.PreparePushNotificationCampaignSearchModel(new AppPushNotificationCampaignSearchModel());
            return View(searchModel);
        }

        public IActionResult GetList(AppPushNotificationCampaignSearchModel searchModel)
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageCampaigns))
                return AccessDeniedDataTablesJson();

            var model = _pushNotificationCampaignModelFactory.PreparePushNotificationCampaignListModel(searchModel);
            return Json(model);
        }

        public IActionResult Create()
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var model = _pushNotificationCampaignModelFactory.PreparePushNotificationCampaignModel(new AppPushNotificationCampaignModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public IActionResult Create(AppPushNotificationCampaignModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var campaign = model.ToEntity<AppPushNotificationCampaign>();
                if (model.CustomerRoles.Any())
                    campaign.CustomerRoles = string.Join(",", model.CustomerRoles);

                if (model.DeviceTypes.Any())
                    campaign.DeviceTypes = string.Join(",", model.DeviceTypes);

                campaign.SendingWillStartOnUtc = _dateTimeHelper.ConvertToUtcTime(model.SendingWillStartOn, _dateTimeHelper.CurrentTimeZone);
                campaign.CreatedOnUtc = DateTime.UtcNow;

                _campaignService.InsertPushNotificationCampaign(campaign);

                UpdateLocales(campaign, model);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Created"));
                
                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = campaign.Id });
            }

            model = _pushNotificationCampaignModelFactory.PreparePushNotificationCampaignModel(model, null);
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaign = _campaignService.GetPushNotificationCampaignById(id);

            if (campaign == null || campaign.Deleted)
                return RedirectToAction("List");

            var model = _pushNotificationCampaignModelFactory.PreparePushNotificationCampaignModel(null, campaign);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public IActionResult Edit(AppPushNotificationCampaignModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaign = _campaignService.GetPushNotificationCampaignById(model.Id);

            if (campaign == null || campaign.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                campaign = model.ToEntity(campaign);
                if (model.CustomerRoles.Any())
                    campaign.CustomerRoles = string.Join(",", model.CustomerRoles);
                else
                    campaign.CustomerRoles = null;

                if (model.DeviceTypes.Any())
                    campaign.DeviceTypes = string.Join(",", model.DeviceTypes);
                else
                    campaign.DeviceTypes = null;

                campaign.SendingWillStartOnUtc = _dateTimeHelper.ConvertToUtcTime(model.SendingWillStartOn, _dateTimeHelper.CurrentTimeZone);

                _campaignService.UpdatePushNotificationCampaign(campaign);

                UpdateLocales(campaign, model);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = campaign.Id });
            }

            model = _pushNotificationCampaignModelFactory.PreparePushNotificationCampaignModel(model, campaign);
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaign = _campaignService.GetPushNotificationCampaignById(id);
            if (campaign == null || campaign.Deleted)
                return RedirectToAction("List");

            _campaignService.DeletePushNotificationCampaign(campaign);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult CopyCampaign(AppPushNotificationCampaignModel model)
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var copyModel = model.CopyPushNotificationCampaignModel;
            try
            {
                var originalCampaign = _campaignService.GetPushNotificationCampaignById(copyModel.Id);

                var newCampaign = originalCampaign.Clone();
                newCampaign.Name = copyModel.Name;
                newCampaign.CreatedOnUtc = DateTime.UtcNow;
                newCampaign.SendingWillStartOnUtc = _dateTimeHelper.ConvertToUtcTime(copyModel.SendingWillStartOnUtc, _dateTimeHelper.CurrentTimeZone);

                _campaignService.InsertPushNotificationCampaign(newCampaign);

                CopyLocalizationData(originalCampaign, newCampaign);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Copied"));

                return RedirectToAction("Edit", new { id = newCampaign.Id });
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = copyModel.Id });
            }
        }

        #endregion
    }
}
