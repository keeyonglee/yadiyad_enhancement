using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Factories;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<WebApiModelFactory>().As<IWebApiModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SliderModelFactory>().As<ISliderModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<DeviceModelFactory>().As<IDeviceModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryIconModelFactory>().As<ICategoryIconModelFactory>().InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}
