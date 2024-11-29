using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Factories;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<AppPushNotificationModelFactory>().As<IAppPushNotificationModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<AppPushNotificationTemplateModelFactory>().As<IPushNotificationTemplateModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<QueuedPushNotificationModelFactory>().As<IQueuedPushNotificationModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<AppPushNotificationCampaignModelFactory>().As<IPushNotificationCampaignModelFactory>().InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}
