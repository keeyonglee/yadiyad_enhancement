using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema.Service
{
    [NopMigration("2021/02/20 17:08:00", "create tables (JobApplication, JobInvitation)")]

    public class M000009_AddTables : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M000009_AddTables(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(JobSeekerProfile)).Exists() == false)
            {
                _migrationManager.BuildTable<JobSeekerProfile>(Create);
            }
            _migrationManager.BuildTable<JobInvitation>(Create);

            if (Schema.Table(nameof(Reason)).Exists() == false)
            {
                _migrationManager.BuildTable<Reason>(Create);
            }
            _migrationManager.BuildTable<JobApplication>(Create);

        }
    }
}
