using Autofac;
using Autofac.Core;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.NopStation.WebApi.Data;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Plugin.NopStation.WebApi.Factories;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.NopStation.WebApi.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<CategoryIconService>().As<ICategoryIconService>().InstancePerLifetimeScope();
            builder.RegisterType<ApiDeviceService>().As<IApiDeviceService>().InstancePerLifetimeScope();
            builder.RegisterType<ApiSliderService>().As<IApiSliderService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerApiService>().As<ICustomerApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductApiService>().As<IProductApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ApiWorkContext>().As<IWorkContext>().InstancePerLifetimeScope();
            builder.RegisterType<ApiStringResourceService>().As<IApiStringResourceService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductAttributeApiParser>().As<IProductAttributeApiParser>().InstancePerLifetimeScope();

            builder.RegisterType<CommonApiModelFactory>().As<ICommonApiModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SliderModelFactory>().As<ISliderModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CatalogApiModelFactory>().As<ICatalogApiModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CartCheckoutApiModelFactory>().AsSelf().InstancePerLifetimeScope();
        }

        public int Order => int.MaxValue;
    }
}
