using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Individual;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2022/01/14 00:00:00", "M220114_0000_Alter_IndividualProfile_Add_IsTourCompleted")]
    public class M220114_0000_Alter_IndividualProfile_Add_IsTourCompleted : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220114_0000_Alter_IndividualProfile_Add_IsTourCompleted(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(IndividualProfile))
               .Column(nameof(IndividualProfile.IsTourCompleted))
               .Exists())
            {
                Create.Column(nameof(IndividualProfile.IsTourCompleted))
                    .OnTable(nameof(IndividualProfile))
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
