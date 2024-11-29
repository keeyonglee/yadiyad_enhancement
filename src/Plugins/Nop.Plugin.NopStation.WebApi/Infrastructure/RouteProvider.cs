using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Mvc.Routing;


namespace Nop.Plugin.NopStation.WebApi.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var pattern = string.Empty;
            if (DataSettingsManager.DatabaseIsInstalled)
            {
                var localizationSettings = endpointRouteBuilder.ServiceProvider.GetRequiredService<LocalizationSettings>();
                if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    var langservice = endpointRouteBuilder.ServiceProvider.GetRequiredService<ILanguageService>();
                    var languages = langservice.GetAllLanguages().ToList();
                    pattern = "{language:lang=" + languages.FirstOrDefault().UniqueSeoCode + "}/";
                }
            }

            endpointRouteBuilder.MapControllerRoute("NopStationCheckoutPaymentInfo", $"{pattern}nopstationcheckout/paymentinfo",
                new { controller = "NopStationCheckout", action = "PaymentInfo" });
            endpointRouteBuilder.MapControllerRoute("NopStationCheckoutRedirect", $"{pattern}nopstationcheckout/redirect",
                new { controller = "NopStationCheckout", action = "Redirect" });
            endpointRouteBuilder.MapControllerRoute("NopStationCheckoutStep", pattern + "nopstationcheckout/step/{nextStep}",
                new { controller = "NopStationCheckout", action = "Step" });
        }

        public int Priority => 0;
    }
}
