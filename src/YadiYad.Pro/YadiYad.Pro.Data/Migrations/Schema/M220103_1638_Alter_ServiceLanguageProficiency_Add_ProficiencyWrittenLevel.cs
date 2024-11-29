using Nop.Data.Migrations;
using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2022/01/03 16:50:00", "M220103_1638_Alter_ServiceLanguageProficiency_Add_ProficiencyWrittenLevel")]
    public class M220103_1638_Alter_ServiceLanguageProficiency_Add_ProficiencyWrittenLevel : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220103_1638_Alter_ServiceLanguageProficiency_Add_ProficiencyWrittenLevel(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(ServiceLanguageProficiency))
               .Column(nameof(ServiceLanguageProficiency.ProficiencyWrittenLevel))
               .Exists())
            {
                Create.Column(nameof(ServiceLanguageProficiency.ProficiencyWrittenLevel))
                    .OnTable(nameof(ServiceLanguageProficiency))
                    .AsInt16()
                    .NotNullable()
                    .WithDefaultValue((int)LanguageSpokenWrittenProficiency.Basic);
            }
        }

        public override void Down()
        {
        }
    }
}
