using Autofac;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Web.Framework.Infrastructure.Extensions;
using YadiYad.Pro.Web.Infrastructure;
using Wkhtmltopdf.NetCore;

namespace Nop.Web
{
    /// <summary>
    /// Represents startup class of application
    /// </summary>
    public class Startup
    {
        #region Fields

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private IEngine _engine;
        private NopConfig _nopConfig;

        #endregion

        #region Ctor

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #endregion

        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public void ConfigureServices(IServiceCollection services)
        {
            _ = DataSettingsManager.TryLoadEnvDataSettings(_configuration);
            (_engine, _nopConfig) = services.ConfigureApplicationServices(_configuration, _webHostEnvironment);
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddAutoMapper(c => c.AddProfile<AutoMappingProfile>(), typeof(Startup));
            services.AddWkhtmltopdf();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            _engine.RegisterDependencies(builder, _nopConfig);
        }

        /// <summary>
        /// Configure the application HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            application.ConfigureRequestPipeline();

            application.StartEngine();
        }
    }
}