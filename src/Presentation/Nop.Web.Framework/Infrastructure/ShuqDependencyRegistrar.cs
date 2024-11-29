using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Services.Documents;
using Nop.Services.Payout;
using Nop.Services.Events;
using Nop.Services.ShippingShuq;
using Nop.Services.ShuqOrders;

namespace Nop.Web.Framework.Infrastructure
{
    public class ShuqDependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<ShuqOrderService>()
                .As<IShuqOrderService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ShuqOrderProcessingService>()
                .As<IShuqOrderProcessingService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DocumentNumberService>()
                .As<DocumentNumberService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<OrderPayoutService>()
                .As<OrderPayoutService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ShuqOrderEventConsumer>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<ShipmentProcessor>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }

        public int Order => 2;
    }
}