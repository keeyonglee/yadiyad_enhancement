using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.NopStation.WebApi.Filters
{
    public class CustomHeaderActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var webApiSettings = EngineContext.Current.Resolve<WebApiSettings>();

            if (context.HttpContext.Request.Headers.ContainsKey(WebApiCustomerDefaults.DeviceId) && webApiSettings.EnableCORS)
                context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
