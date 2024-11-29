using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Services.ShippingShuq;

namespace QB.Shipping.Borzo.Infrastructure
{
    public class DependencyRegistrar: IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<BorzoService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<BorzoShipmentCarrier>().As<IShippingCarrier>().InstancePerLifetimeScope();

        }

        public int Order => 1;
    }
}