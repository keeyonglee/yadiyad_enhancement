using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema.Service
{
    [NopMigration("2021/02/15 12:17:00", "create tables (JobProfile, JobProfileExpertise)")]

    public class M000007_AddTables : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M000007_AddTables(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<JobProfile>(Create);
            _migrationManager.BuildTable<JobProfileExpertise>(Create);

        }
    }
}
