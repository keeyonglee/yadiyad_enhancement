using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/20 18:09:00", "Alter JobApplication Add JobType JobRequired")]

    public class M210620_1809_AlterJobApplicationAddJobTypeRequired : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210620_1809_AlterJobApplicationAddJobTypeRequired(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(JobApplication))
               .Column(nameof(JobApplication.JobType))
               .Exists() == false)
            {
                Create.Column(nameof(JobApplication.JobType))
                .OnTable(nameof(JobApplication))
                .AsInt32()
                .WithDefaultValue(0)
                .NotNullable();
            }

            Alter.Column(nameof(JobApplication.JobType))
            .OnTable(nameof(JobApplication))
            .AsInt32()
            .NotNullable();

            if (Schema.Table(nameof(JobApplication))
               .Column(nameof(JobApplication.JobRequired))
               .Exists() == false)
            {
                Create.Column(nameof(JobApplication.JobRequired))
                .OnTable(nameof(JobApplication))
                .AsInt32()
                .Nullable();
            }
        }

        public override void Down()
        {
        }


    }
}

