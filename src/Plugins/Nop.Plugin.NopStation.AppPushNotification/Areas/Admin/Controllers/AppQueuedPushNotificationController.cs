using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Factories;
using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models;
using Nop.Plugin.NopStation.AppPushNotification.Services;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Controllers;
using System;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Services.Helpers;
using Nop.Services.Security;
using Nop.Plugin.NopStation.Core.Infrastructure;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Controllers
{
    [NopStationLicense]
    public class AppQueuedPushNotificationController : BaseAdminController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IQueuedPushNotificationModelFactory _queuedPushNotificationModelFactory;
        private readonly IQueuedPushNotificationService _queuedPushNotificationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public AppQueuedPushNotificationController(ILocalizationService localizationService,
            INotificationService notificationService,
            IQueuedPushNotificationModelFactory queuedPushNotificationModelFactory,
            IQueuedPushNotificationService queuedPushNotificationService,
            IDateTimeHelper dateTimeHelper,
            IPermissionService permissionService)
        {
            _permissionService = permissionService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _queuedPushNotificationModelFactory = queuedPushNotificationModelFactory;
            _queuedPushNotificationService = queuedPushNotificationService;
            _dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods        

        public virtual IActionResult Index()
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageQueuedNotifications))
                return AccessDeniedView();

            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageQueuedNotifications))
                return AccessDeniedView();

            var searchModel = _queuedPushNotificationModelFactory.PrepareQueuedPushNotificationSearchModel(new AppQueuedPushNotificationSearchModel());
            return View(searchModel);
        }

        public virtual IActionResult GetList(AppQueuedPushNotificationSearchModel searchModel)
        {
            var model = _queuedPushNotificationModelFactory.PrepareQueuedPushNotificationListModel(searchModel);
            return Json(model);
        }
        
        public virtual IActionResult View(int id)
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageQueuedNotifications))
                return AccessDeniedDataTablesJson();

            var queuedPushNotification = _queuedPushNotificationService.GetQueuedPushNotificationById(id);
            if (queuedPushNotification == null)
                return RedirectToAction("List");

            var model = _queuedPushNotificationModelFactory.PrepareQueuedPushNotificationModel(null, queuedPushNotification);

            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageQueuedNotifications))
                return AccessDeniedView();

            //try to get a queued queuedPushNotification with the specified id
            var queuedPushNotification = _queuedPushNotificationService.GetQueuedPushNotificationById(id);
            if (queuedPushNotification == null)
                return RedirectToAction("List");

            //prepare model
            var model = _queuedPushNotificationModelFactory.PrepareQueuedPushNotificationModel(null, queuedPushNotification);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(AppQueuedPushNotificationModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageQueuedNotifications))
                return AccessDeniedView();

            var queuedPushNotification = _queuedPushNotificationService.GetQueuedPushNotificationById(model.Id);
            if (queuedPushNotification == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                queuedPushNotification = model.ToEntity(queuedPushNotification);
                queuedPushNotification.DontSendBeforeDateUtc = model.SendImmediately || !model.DontSendBeforeDate.HasValue ?
                    null : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value, _dateTimeHelper.CurrentTimeZone);
                _queuedPushNotificationService.UpdateQueuedPushNotification(queuedPushNotification);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.System.QueuedEmails.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = queuedPushNotification.Id }) : RedirectToAction("List");
            }

            model = _queuedPushNotificationModelFactory.PrepareQueuedPushNotificationModel(model, queuedPushNotification, true);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageQueuedNotifications))
                return AccessDeniedView();

            var queuedPushNotification = _queuedPushNotificationService.GetQueuedPushNotificationById(id);
            if (queuedPushNotification == null)
                return RedirectToAction("List");

            _queuedPushNotificationService.DeleteQueuedPushNotification(queuedPushNotification);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Deleted"));

            return RedirectToAction("List");
        }

        //[HttpPost]
        public virtual IActionResult DeleteSentQueuedPushNotification()
        {
            if (!_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageQueuedNotifications))
                return AccessDeniedView();

            _queuedPushNotificationService.DeleteSentQueuedPushNotification();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Deleted"));

            return RedirectToAction("List");
        }

        #endregion
    }
}
