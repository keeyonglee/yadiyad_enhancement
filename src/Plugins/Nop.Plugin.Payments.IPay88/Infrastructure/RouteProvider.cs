using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.IPay88.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //RedirectToPaymentUrl
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.IPay88.RedirectToPaymentUrl", "Plugins/PaymentIPay88/RedirectToPaymentUrl",
                 new { controller = "PaymentIPay88", action = "RedirectToPaymentUrl" });

            //PaymentResult
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.IPay88.PaymentResult", "Plugins/PaymentIPay88/PaymentResult",
                 new { controller = "PaymentIPay88", action = "PaymentResult" });

            //BackendUrl
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.IPay88.BackendUrl", "Plugins/PaymentIPay88/BackendUrl",
                 new { controller = "PaymentIPay88", action = "BackendUrl" });

            //Cancel
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.IPay88.CancelOrder", "Plugins/PaymentIPay88/CancelOrder",
                 new { controller = "PaymentIPay88", action = "CancelOrder" });

            //PaymentInfo
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.IPay88.PaymentInfo", "Plugins/PaymentIPay88/PaymentInfo",
                 new { controller = "PaymentIPay88", action = "PaymentInfo" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => -1;
    }
}