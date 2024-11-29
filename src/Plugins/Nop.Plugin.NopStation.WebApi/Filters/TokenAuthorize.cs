using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Nop.Core.Infrastructure;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Plugin.NopStation.WebApi.Models;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Services.Localization;

namespace Nop.Plugin.NopStation.WebApi.Filters
{
    public class TokenAuthorizeAttribute : TypeFilterAttribute
    {
        #region Ctor

        public TokenAuthorizeAttribute() : base(typeof(TokenAuthorizeAttributeFilter))
        {
        }

        #endregion

        #region Nested class

        public class TokenAuthorizeAttributeFilter : IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationFilterContext actionContext)
            {
                var identity = ParseAuthorizationHeader(actionContext);
                if (identity == false)
                {
                    Challenge(actionContext);
                    return;
                }
            }

            protected virtual bool ParseAuthorizationHeader(AuthorizationFilterContext actionContext)
            {
                bool check = true;

                if (actionContext.HttpContext.Request.Headers.TryGetValue(WebApiCustomerDefaults.Token, out StringValues checkToken))
                {
                    var token = checkToken.FirstOrDefault();
                    var secretKey = WebApiCustomerDefaults.SecretKey;
                    try
                    {
                        var payload = JwtHelper.JwtDecoder.DecodeToObject(token, secretKey, true) as IDictionary<string, object>;
                        check = true;
                    }
                    catch
                    {
                        check = false;
                    }
                }

                return check;
            }

            private void Challenge(AuthorizationFilterContext actionContext)
            {
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                var response = new BaseResponseModel
                {
                    ErrorList = new List<string>
                    {
                        localizationService.GetResource("NopStation.WebApi.Response.InvalidToken")
                    }
                };

                actionContext.Result = new BadRequestObjectResult(response);
                return;
            }
        }

        #endregion
    }
}