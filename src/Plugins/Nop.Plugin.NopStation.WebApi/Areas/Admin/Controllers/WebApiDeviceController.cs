using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Factories;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Models;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Controllers
{
    [NopStationLicense]
    public class WebApiDeviceController : BaseAdminController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IDeviceModelFactory _deviceModelFactory;
        private readonly IApiDeviceService _deviceService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public WebApiDeviceController(ILocalizationService localizationService,
            INotificationService notificationService,
            IDeviceModelFactory deviceModelFactory,
            IApiDeviceService deviceService,
            IPermissionService permissionService)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _deviceModelFactory = deviceModelFactory;
            _deviceService = deviceService;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods        

        public virtual IActionResult Index()
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageDevice))
                return AccessDeniedView();

            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageDevice))
                return AccessDeniedView();

            var searchModel = _deviceModelFactory.PrepareDeviceSearchModel(new DeviceSearchModel());
            return View(searchModel);
        }

        public virtual IActionResult GetList(DeviceSearchModel searchModel)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageDevice))
                return AccessDeniedView();

            var model = _deviceModelFactory.PrepareDeviceListModel(searchModel);
            return Json(model);
        }

        public virtual IActionResult View(int id)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageDevice))
                return AccessDeniedView();

            var device = _deviceService.GetApiDeviceById(id);
            if (device == null)
                return RedirectToAction("List");

            var model = _deviceModelFactory.PrepareDeviceModel(null, device);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageDevice))
                return AccessDeniedView();

            var device = _deviceService.GetApiDeviceById(id);
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            _deviceService.DeleteApiDevice(device);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.WebApi.Devices.Deleted"));

            return RedirectToAction("List");
        }

        public IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageDevice))
                return AccessDeniedView();

            if (selectedIds != null)
                _deviceService.DeleteApiDevices(_deviceService.GetApiDeviceByIds(selectedIds.ToArray()).ToList());

            return Json(new { Result = true });
        }

        #endregion
    }
}
