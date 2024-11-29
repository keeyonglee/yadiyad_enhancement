using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.JobSeeker;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2022/07/21 10:54:00", "Add JobSeekerCV")]
    public class M220721_1050_Add_JobSeekerCV : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220721_1050_Add_JobSeekerCV(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        
        public override void Up()
        {
            _migrationManager.BuildTable<JobSeekerCV>(Create);
        }

        public override void Down()
        {
        }
    }
}