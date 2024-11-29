using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.JobSeeker;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2022/01/03 23:00:00", "M220103_2300_Alter_JobSeekerLanguageProficiency_Add_ProficiencyWrittenLevel")]
    public class M220103_2300_Alter_JobSeekerLanguageProficiency_Add_ProficiencyWrittenLevel : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220103_2300_Alter_JobSeekerLanguageProficiency_Add_ProficiencyWrittenLevel(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(JobSeekerLanguageProficiency))
               .Column(nameof(JobSeekerLanguageProficiency.ProficiencyWrittenLevel))
               .Exists())
            {
                Create.Column(nameof(JobSeekerLanguageProficiency.ProficiencyWrittenLevel))
                    .OnTable(nameof(JobSeekerLanguageProficiency))
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
