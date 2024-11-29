using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.JobSeeker;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/07/14 20:13:00", "Alter JobSeekerProfile Add AvailableHoursDays")]
    public class M210714_2013_Alter_JobSeekerProfile_Add_AvailableHoursDays : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210714_2013_Alter_JobSeekerProfile_Add_AvailableHoursDays(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (!Schema.Table(nameof(JobSeekerProfile))
               .Column(nameof(JobSeekerProfile.AvailableDays))
               .Exists())
            {
                Create.Column(nameof(JobSeekerProfile.AvailableDays))
                    .OnTable(nameof(JobSeekerProfile))
                    .AsInt32()
                    .NotNullable();
            }

            if (!Schema.Table(nameof(JobSeekerProfile))
               .Column(nameof(JobSeekerProfile.AvailableHours))
               .Exists())
            {
                Create.Column(nameof(JobSeekerProfile.AvailableHours))
                    .OnTable(nameof(JobSeekerProfile))
                    .AsInt32()
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
