using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Subscription;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/11/07 15:52:00", "M211107_1552_Alter_ServiceSubscription_Add_StopDate")]
    public class M211107_1552_Alter_ServiceSubscription_Add_StopDate : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211107_1552_Alter_ServiceSubscription_Add_StopDate(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(ServiceSubscription))
               .Column(nameof(ServiceSubscription.StopDate))
               .Exists())
            {
                Create.Column(nameof(ServiceSubscription.StopDate))
                    .OnTable(nameof(ServiceSubscription))
                    .AsDateTime()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
