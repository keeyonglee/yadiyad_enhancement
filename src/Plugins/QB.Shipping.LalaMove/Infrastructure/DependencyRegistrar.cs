using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Text;
using Nop.Services.ShippingShuq;

namespace QB.Shipping.LalaMove.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<LalamoveShipmentCarrier>().AsSelf().As<IShippingCarrier>().InstancePerLifetimeScope();
            builder.RegisterType<LalaMoveService>().AsSelf().InstancePerLifetimeScope();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 1;
    }
}
