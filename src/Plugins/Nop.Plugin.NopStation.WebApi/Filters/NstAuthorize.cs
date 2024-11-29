using System.Collections.Generic;
using System.Linq;
using Nop.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Stores;
using Nop.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Services.Localization;
using System;
using Nop.Plugin.NopStation.WebApi.Models.Common;

namespace Nop.Plugin.NopStation.WebApi.Filters
{
    public class NstAuthorizeAttribute : TypeFilterAttribute
    {
        #region Ctor

        public NstAuthorizeAttribute() : base(typeof(NstAuthorize))
        {

        }

        #endregion

        #region Nested filter

        public class NstAuthorize : IActionFilter
        {
            public void OnActionExecuting(ActionExecutingContext filterContext)
            {
                var webApiSettings = EngineContext.Current.Resolve<WebApiSettings>();
                if (!webApiSettings.EnableJwtSecurity)
                    return;
                var identity = ParseNstAuthorizationHeader(filterContext);
                if (identity == false)
                {
                    CreateNstAccessResponceMessage(filterContext);
                    return;
                }
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
                //do nothing
            }

            protected virtual bool ParseNstAuthorizationHeader(ActionExecutingContext actionContext)
            {
                var _storeContext = EngineContext.Current.Resolve<IStoreContext>();
                var httpContext = EngineContext.Current.Resolve<IHttpContextAccessor>().HttpContext;

                httpContext.Request.Headers.TryGetValue(WebApiCustomerDefaults.NST, out StringValues keyFound);
                var requestkey = keyFound.FirstOrDefault();
                try
                {
                    var settingService = EngineContext.Current.Resolve<ISettingService>();
                    var storeService = EngineContext.Current.Resolve<IStoreService>();
                    var workContext = EngineContext.Current.Resolve<IWorkContext>();
                    var storeScope = 0;
                    if (storeService.GetAllStores().Count < 2)
                        storeScope = 0;
                    else
                    {
                        var storeId = _storeContext.CurrentStore.Id;
                        var store = storeService.GetStoreById(storeId);
                        storeScope = store?.Id ?? 0;
                    }

                    var securitySettings = settingService.LoadSetting<WebApiSettings>(storeScope);

                    var tokens = JwtHelper.JwtDecoder.DecodeToObject(requestkey, securitySettings.TokenSecret, true) as IDictionary<string, object>;
                    if (tokens != null)
                    {
                        if (tokens[WebApiCustomerDefaults.NSTKey].ToString() != securitySettings.TokenKey)
                            return false;

                        if (!securitySettings.CheckIat)
                            return true;

                        if (long.TryParse(tokens[WebApiCustomerDefaults.IAT].ToString(), out long createTimeStamp))
                        {
                            var currentTimeStamp = ConvertToTimestamp(DateTime.UtcNow);
                            var leastTimeStamp = ConvertToTimestamp(DateTime.UtcNow.AddSeconds(-securitySettings.TokenSecondsValid));

                            return createTimeStamp <= currentTimeStamp && createTimeStamp >= leastTimeStamp;
                        }
                        return false;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            }

            private long ConvertToTimestamp(DateTime value)
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var elapsedTime = value - epoch;
                return (long)elapsedTime.TotalSeconds;
            }

            private void CreateNstAccessResponceMessage(ActionExecutingContext actionContext)
            {
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                var response = new BaseResponseModel
                {
                    ErrorList = new List<string>
                    {
                        localizationService.GetResource("NopStation.WebApi.Response.InvalidJwtToken")
                    }
                };

                actionContext.Result = new BadRequestObjectResult(response);
                return;
            }
        }

        #endregion
    }
}
