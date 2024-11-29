using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Services.Customers;

namespace Nop.Plugin.NopStation.WebApi.Controllers
{
    public class StartupApiController : BaseApiController
    {
        private readonly IApiDeviceService _deviceService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;

        public StartupApiController(IApiDeviceService deviceService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ICustomerService customerService)
        {
            _deviceService = deviceService;
            _workContext = workContext;
            _storeContext = storeContext;
            _customerService = customerService;
        }

        [HttpPost]
        [Route("api/appstart")]
        public IActionResult AppStart([FromBody]BaseQueryModel<AppStartModel> queryModel)
        {
            var model = queryModel.Data;
            
            var deviceId = Request.GetAppDeviceId();
            var appTypeId = Request.GetAppTypeId();
            var device = _deviceService.GetApiDeviceByDeviceId(deviceId, _storeContext.CurrentStore.Id);

            if (device != null)
            {
                device.CustomerId = _workContext.CurrentCustomer.Id;
                device.SubscriptionId = model.SubscriptionId;
                device.UpdatedOnUtc = DateTime.UtcNow;
                device.DeviceTypeId = model.DeviceTypeId;
                device.IsRegistered = !_customerService.IsRegistered(_workContext.CurrentCustomer);

                _deviceService.UpdateApiDevice(device);
            }
            else
            {
                var newDevice = new ApiDevice
                {
                    CustomerId = _workContext.CurrentCustomer.Id,
                    DeviceToken = deviceId,
                    SubscriptionId = model.SubscriptionId,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    DeviceTypeId = model.DeviceTypeId,
                    IsRegistered = !_customerService.IsRegistered(_workContext.CurrentCustomer),
                    StoreId = _storeContext.CurrentStore.Id,
                    AppTypeId = appTypeId
                };

                _deviceService.InsertApiDevice(newDevice);
            }

            return Ok();
        }
    }
}
