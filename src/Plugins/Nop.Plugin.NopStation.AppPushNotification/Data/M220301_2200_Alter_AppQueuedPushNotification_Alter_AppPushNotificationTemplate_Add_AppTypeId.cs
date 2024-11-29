using FluentMigrator;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.NopStation.AppPushNotification.Data
{
    [NopMigration("2022/03/01 22:00:00", "Alter_AppQueuedPushNotification_Alter_AppPushNotificationTemplate_Add_AppTypeId")]
    public class M220301_2200_Alter_AppQueuedPushNotification_Alter_AppPushNotificationTemplate_Add_AppTypeId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220301_2200_Alter_AppQueuedPushNotification_Alter_AppPushNotificationTemplate_Add_AppTypeId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(AppPushNotificationTemplate)))
                    .Column(NameCompatibilityManager.GetColumnName(typeof(AppPushNotificationTemplate), nameof(AppPushNotificationTemplate.AppTypeId)))
                    .Exists())
            {
                Create.Column(NameCompatibilityManager.GetColumnName(typeof(AppPushNotificationTemplate), nameof(AppPushNotificationTemplate.AppTypeId)))
                    .OnTable(NameCompatibilityManager.GetTableName(typeof(AppPushNotificationTemplate)))
                    .AsInt32()
                    .NotNullable()
                    .WithDefaultValue(10);
            }

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(AppQueuedPushNotification)))
                  .Column(NameCompatibilityManager.GetColumnName(typeof(AppQueuedPushNotification), nameof(AppQueuedPushNotification.AppTypeId)))
                  .Exists())
            {
                Create.Column(NameCompatibilityManager.GetColumnName(typeof(AppQueuedPushNotification), nameof(AppQueuedPushNotification.AppTypeId)))
                    .OnTable(NameCompatibilityManager.GetTableName(typeof(AppQueuedPushNotification)))
                    .AsInt32()
                    .NotNullable()
                    .WithDefaultValue(10);
            }

        }

        public override void Down()
        {
        }
    }
}
