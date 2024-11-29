using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/07/19 12:00:00", "Alter JobProfile Add JobSchedule")]

    public class M210719_1200_Alter_JobProfile_Add_JobSchedule : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210719_1200_Alter_JobProfile_Add_JobSchedule(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (!Schema.Table(nameof(JobProfile))
               .Column(nameof(JobProfile.JobSchedule))
               .Exists())
            {
                Create.Column(nameof(JobProfile.JobSchedule))
                    .OnTable(nameof(JobProfile))
                    .AsInt16()
                    .WithDefaultValue(1)
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
