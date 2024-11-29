using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/08/10 15:27:00", "Alter Job Application")]
    public class M210810_1527_Alter_JobApplication : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210810_1527_Alter_JobApplication(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(JobApplication))
               .Column(nameof(JobApplication.EndMilestoneId))
               .Exists())
            {
                Create.Column(nameof(JobApplication.EndMilestoneId))
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
