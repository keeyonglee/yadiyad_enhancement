using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;

namespace YadiYad.Pro.Data.Migrations.Schema.Service
{
    [NopMigration("2021/02/17 14:16:00", "create tables (TimeZone, ConsultationProfile)")]

    public class M000008_AddTables : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M000008_AddTables(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<TimeZone>(Create);
            _migrationManager.BuildTable<ConsultationProfile>(Create);

        }
    }
}
