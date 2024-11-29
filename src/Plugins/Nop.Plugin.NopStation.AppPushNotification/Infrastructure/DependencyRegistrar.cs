using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.NopStation.AppPushNotification.Services;
using Nop.Services.Orders;

namespace Nop.Plugin.NopStation.AppPushNotification.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<AppPushNotificationTemplateService>().As<IPushNotificationTemplateService>().InstancePerLifetimeScope();
            builder.RegisterType<QueuedPushNotificationService>().As<IQueuedPushNotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<WorkflowNotificationService>().As<IWorkflowNotificationService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AppPushNotificationTokenProvider>().As<IPushNotificationTokenProvider>().InstancePerLifetimeScope();
            builder.RegisterType<AppPushNotificationSender>().As<IPushNotificationSender>().InstancePerLifetimeScope();
            //builder.RegisterType<AppPushNotificationOrderProcessingService>().As<IOrderProcessingService>().InstancePerLifetimeScope();

            builder.RegisterType<AppPushNotificationCampaignService>().As<IPushNotificationCampaignService>().InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}
