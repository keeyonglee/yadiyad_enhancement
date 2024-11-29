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
    [NopMigration("2021/08/11 00:05:00", "Alter JobApplication Add CancellationDateTime")]
    public class M210811_0005_Alter_JobApplication_Add_CancellationDateTime : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210811_0005_Alter_JobApplication_Add_CancellationDateTime(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(JobApplication))
               .Column(nameof(JobApplication.CancellationDateTime))
               .Exists())
            {
                Create.Column(nameof(JobApplication.CancellationDateTime))
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
