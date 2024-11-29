using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/19 19:07:00", "Alter JobApplication Add StartEndDate")]

    public class M210619_1907_AlterJobApplicationAddStartEndDate : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210619_1907_AlterJobApplicationAddStartEndDate(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(JobApplication))
               .Column(nameof(JobApplication.StartDate))
               .Exists() == false)
            {
                Create.Column(nameof(JobApplication.StartDate))
                .OnTable(nameof(JobApplication))
                .AsDateTime()
                .Nullable();
            }
            if (Schema.Table(nameof(JobApplication))
               .Column(nameof(JobApplication.EndDate))
               .Exists() == false)
            {
                Create.Column(nameof(JobApplication.EndDate))
                .OnTable(nameof(JobApplication))
                .AsDateTime()
                .Nullable();
            }
        }

        public override void Down()
        {
        }

       
    }
}
