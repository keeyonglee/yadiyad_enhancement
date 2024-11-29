using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2022/02/08 01:33:00", "M220208_0133_Alter_JobApplication_Add_IsOrganizationRead")]
    public class M220208_0133_Alter_JobApplication_Add_IsOrganizationRead : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220208_0133_Alter_JobApplication_Add_IsOrganizationRead(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(JobApplication))
               .Column(nameof(JobApplication.IsOrganizationRead))
               .Exists())
            {
                Create.Column(nameof(JobApplication.IsOrganizationRead))
                    .OnTable(nameof(JobApplication))
                    .AsBoolean()
                    .WithDefaultValue(false)
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
