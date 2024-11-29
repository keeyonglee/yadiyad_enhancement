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
    [NopMigration("2021/07/28 00:05:00", "Alter JobSeekerProfile Alter AvailableHoursDays")]
    public class M210728_0005_Alter_JobSeekerProfile_Alter_AvailableHoursDays : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210728_0005_Alter_JobSeekerProfile_Alter_AvailableHoursDays(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            Alter.Column(nameof(JobSeekerProfile.AvailableDays))
                .OnTable(nameof(JobSeekerProfile))
                .AsInt32()
                .Nullable();

            Alter.Column(nameof(JobSeekerProfile.AvailableHours))
                .OnTable(nameof(JobSeekerProfile))
                .AsInt32()
                .Nullable();
        }

        public override void Down()
        {
        }
    }
}
