using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.NopStation.AppPushNotification.Domains;

namespace Nop.Plugin.NopStation.AppPushNotification.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/07/08 08:30:55:1687541", "NopStation.AppPushNotification base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<AppPushNotificationCampaign>(Create);
            _migrationManager.BuildTable<AppPushNotificationTemplate>(Create);
            _migrationManager.BuildTable<AppQueuedPushNotification>(Create);
        }
    }
}
