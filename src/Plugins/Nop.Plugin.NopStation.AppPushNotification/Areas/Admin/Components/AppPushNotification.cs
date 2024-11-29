using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Components
{
    public class AppPushNotificationViewComponent : NopViewComponent
    {
        private readonly INopStationLicenseService _licenseService;

        public AppPushNotificationViewComponent(INopStationLicenseService licenseService)
        {
            _licenseService = licenseService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (!_licenseService.IsLicensed())
                return Content("");
            
            if(additionalData.GetType() != typeof(DeviceModel))
                return Content("");

            var model = (DeviceModel)additionalData;
            if(model.Id == 0)
                return Content("");

            return View(model);
        }
    }
}
