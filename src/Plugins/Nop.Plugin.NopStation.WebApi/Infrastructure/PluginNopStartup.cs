using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.NopStation.WebApi.Filters;
using Nop.Plugin.NopStation.WebApi.Infrastructure.Extensions;

namespace Nop.Plugin.NopStation.WebApi.Infrastructure
{
    public class PluginNopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ViewLocationExpander());
            });

            services.AddMvc(config =>
            {
                config.Filters.Add(new CustomHeaderActionFilter());
            });

            services.AddCors(option =>
            {
                option.AddPolicy("AllowAll", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseApiExceptionHandler();
            app.UseApiNotFound();
            app.UseCors("AllowAll");
        }

        public int Order => 999;
    }
}