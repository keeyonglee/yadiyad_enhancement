﻿using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.StorageMedia.AmazonS3.Services;
using Nop.Services.Media;
using Nop.Services.Media.RoxyFileman;
using Nop.Services.Messages;

namespace Nop.Plugin.StorageMedia.AmazonS3.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
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
            builder.RegisterType<AmazonS3Service>().As<AmazonS3Service>().InstancePerLifetimeScope();
            builder.RegisterType<AmazonS3PictureService>().As<IPictureService>().InstancePerLifetimeScope();
            builder.RegisterType<AmazonS3DownloadService>().As<IDownloadService>().InstancePerLifetimeScope();
            builder.RegisterType<AmazonS3Plugin>().As<AmazonS3Plugin>().InstancePerLifetimeScope();
            builder.RegisterType<AmazonS3RoxyFileManService>().As<IRoxyFilemanService>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 1;
    }
}
