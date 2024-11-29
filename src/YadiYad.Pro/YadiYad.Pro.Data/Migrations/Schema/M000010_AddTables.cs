using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema.Service
{
    [NopMigration("2021/02/28 13:00:00", "create tables (ServiceApplication)")]

    public class M000010_AddTables : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M000010_AddTables(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<ServiceApplication>(Create);

        }
    }
}
