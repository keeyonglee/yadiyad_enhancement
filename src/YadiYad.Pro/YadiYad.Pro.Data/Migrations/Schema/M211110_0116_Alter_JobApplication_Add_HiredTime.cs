using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/11/10 01:16:00", "M211110_0116_Alter_JobApplication_Add_HiredTime")]
    public class M211110_0116_Alter_JobApplication_Add_HiredTime : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211110_0116_Alter_JobApplication_Add_HiredTime(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(JobApplication))
               .Column(nameof(JobApplication.HiredTime))
               .Exists())
            {
                Create.Column(nameof(JobApplication.HiredTime))
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
