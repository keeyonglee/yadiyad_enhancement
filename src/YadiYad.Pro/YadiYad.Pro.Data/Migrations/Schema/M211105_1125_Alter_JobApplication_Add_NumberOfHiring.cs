using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/11/05 11:25:00", "Alter JobApplication Add NumberOfHiring")]
    public class M211105_1125_Alter_JobApplication_Add_NumberOfHiring : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211105_1125_Alter_JobApplication_Add_NumberOfHiring(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(JobApplication))
               .Column(nameof(JobApplication.NumberOfHiring))
               .Exists())
            {
                Create.Column(nameof(JobApplication.NumberOfHiring))
                    .OnTable(nameof(JobApplication))
                    .AsInt32()
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
