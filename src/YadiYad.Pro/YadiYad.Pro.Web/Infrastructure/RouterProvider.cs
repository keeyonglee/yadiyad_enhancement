using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Mvc.Routing;

namespace YadiYad.Pro.Web.Infrastructure
{
    /// <summary>
    /// Represents provider that provided basic routes
    /// </summary>
    public partial class RouteProvider : IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var pattern = "";
            //home page
            endpointRouteBuilder.MapControllerRoute("ProHomepage", "Pro/Home/Info",
                new { areas = "Pro", controller = "Home", action = "Info" });
            //home page
            endpointRouteBuilder.MapControllerRoute("ProLogin", "Pro/Account/Login",
                new { areas = "Pro", controller = "Account", action = "Login" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;

        #endregion
    }
}
