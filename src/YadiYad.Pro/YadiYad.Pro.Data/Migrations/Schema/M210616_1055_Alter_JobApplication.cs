using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/16 10:55:00", "Alter JobApplication")]
    public class M210616_1055_Alter_JobApplication : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210616_1055_Alter_JobApplication(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(JobApplication))
               .Column(nameof(JobApplication.CancellationReasonId))
               .Exists() == false)
            {
                Create.Column(nameof(JobApplication.CancellationReasonId))
                .OnTable(nameof(JobApplication))
                .AsInt32()
                .ForeignKey(nameof(Reason), nameof(Reason.Id))
                .Nullable();
            }

            if (Schema.Table(nameof(JobApplication))
               .Column(nameof(JobApplication.CancellationRemarks))
               .Exists() == false)
            {
                Create.Column(nameof(JobApplication.CancellationRemarks))
                .OnTable(nameof(JobApplication))
                .AsString(int.MaxValue)
                .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
