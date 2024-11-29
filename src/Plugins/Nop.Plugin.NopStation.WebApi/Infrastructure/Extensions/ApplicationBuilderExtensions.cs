using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Data;
using Nop.Core.Infrastructure;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Plugin.NopStation.WebApi.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseApiNotFound(this IApplicationBuilder application)
        {
            application.UseStatusCodePages(async context =>
            {
                //handle 404 Not Found
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    if (context.HttpContext.Request.Headers.ContainsKey(WebApiCustomerDefaults.DeviceId))
                    {
                        var res = new BaseResponseModel();
                        res.Message = EngineContext.Current.Resolve<ILocalizationService>().GetResource("NopStation.WebApi.Response.PageNotFound");
                        var json = JsonConvert.SerializeObject(res);
                        context.HttpContext.Response.ContentType = "application/json";
                        await context.HttpContext.Response.WriteAsync(json);
                    }
                }
            });
        }

        public static void UseApiExceptionHandler(this IApplicationBuilder application)
        {
            //log errors
            application.UseExceptionHandler(handler =>
            {
                handler.Run(async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    if (exception == null)
                        return;

                    if (context.Request.Headers.ContainsKey(WebApiCustomerDefaults.DeviceId))
                    {
                        try
                        {
                            //check whether database is installed
                            if (DataSettingsManager.DatabaseIsInstalled)
                            {
                                //get current customer
                                var currentCustomer = EngineContext.Current.Resolve<IWorkContext>().CurrentCustomer;

                                //log error
                                EngineContext.Current.Resolve<ILogger>().Error(exception.Message, exception, currentCustomer);
                            }
                        }
                        finally
                        {
                            //rethrow the exception to show the error page
                            var baseResponse = new BaseResponseModel();
                            baseResponse.ErrorList.Add(exception.Message);
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(baseResponse));
                        }
                    }
                });
            });
        }
    }
}
