using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using System;
using System.Text.Json;

namespace YadiYad.Pro.Web.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CamelCaseResponseFormatter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context == null || context.Result == null)
            {
                return;
            }

            var formatter = new SystemTextJsonOutputFormatter(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var response = (context.Result as Microsoft.AspNetCore.Mvc.OkObjectResult);

            if (response != null)
            {
                response.Formatters.Add(formatter);
            }
        }
    }
}
