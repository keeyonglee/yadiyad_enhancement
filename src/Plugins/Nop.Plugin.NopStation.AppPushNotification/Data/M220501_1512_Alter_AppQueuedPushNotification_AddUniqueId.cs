using System;
using FluentMigrator;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.NopStation.AppPushNotification.Domains;

namespace Nop.Plugin.NopStation.AppPushNotification.Data
{
    [NopMigration("2022/01/05 15:12:55:1687541", "NopStation.AppPushNotification Add Notification Instance Tracking")]
    public class M220501_1512_Alter_AppQueuedPushNotification_AddUniqueId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220501_1512_Alter_AppQueuedPushNotification_AddUniqueId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(AppQueuedPushNotification)))
                    .Column(NameCompatibilityManager.GetColumnName(typeof(AppQueuedPushNotification), nameof(AppQueuedPushNotification.UniqueNotificationId)))
                    .Exists())
            {
                Create.Column(NameCompatibilityManager.GetColumnName(typeof(AppQueuedPushNotification), nameof(AppQueuedPushNotification.UniqueNotificationId)))
                    .OnTable(NameCompatibilityManager.GetTableName(typeof(AppQueuedPushNotification)))
                    .AsGuid()
                    .NotNullable()
                    .WithDefaultValue(Guid.NewGuid());
            }
        }

        public override void Down()
        {
        }
    }
}