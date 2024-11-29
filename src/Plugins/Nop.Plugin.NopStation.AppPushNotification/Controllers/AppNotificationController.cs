using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.NopStation.AppPushNotification.Services;
using Nop.Plugin.NopStation.AppPushNotification.Services.Models;
using Nop.Plugin.NopStation.WebApi.Controllers;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Web.Models.Common;
using Nop.Web.Models.Order;

namespace Nop.Plugin.NopStation.AppPushNotification.Controllers
{
    [Route("api/appnotification")]
    public class AppNotificationController : BaseApiController
    {
        private readonly IQueuedPushNotificationService _pushNotificationService;
        private readonly IWorkContext _workContext;

        public AppNotificationController(IQueuedPushNotificationService pushNotificationService,
            IWorkContext workContext)
        {
            _pushNotificationService = pushNotificationService;
            _workContext = workContext;
        }

        [HttpGet("list")]
        public IActionResult List(int pageNumber, int pageSize = 1000)
        {
            int pageIndex = 0;
            if (pageNumber > 0)
                pageIndex = pageNumber - 1;

            var appTypeId = Request.GetAppTypeId();
            var response = new GenericResponseModel<CustomerNotificationModel>();
            var pagedResult = _pushNotificationService.GetCustomerNotifications(_workContext.CurrentCustomer.Id, appTypeId, pageIndex, pageSize);
            response.Data = new CustomerNotificationModel
            {
                PagerModel = new PagerModel
                {
                    PageSize = pagedResult.PageSize,
                    TotalRecords = pagedResult.TotalCount,
                    PageIndex = pagedResult.PageIndex,
                },
                Notifications = pagedResult
            };
            return Ok(response);
        }
    }
}