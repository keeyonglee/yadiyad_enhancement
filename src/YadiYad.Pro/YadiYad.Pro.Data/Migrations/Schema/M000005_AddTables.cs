using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema.Service
{
    [NopMigration("2021/02/09 00:36:00", "create tables ("
        + "ServiceProfile"
        + ", ServiceExpertise"
        + ", ServiceLocation")]
    public class M000005_AddTables : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M000005_AddTables(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<ServiceProfile>(Create);
            _migrationManager.BuildTable<ServiceExpertise>(Create);
        }
    }
}
