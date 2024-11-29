using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/04/03 18:27:00", "Alter ServiceLanguageProficiency Alter LanguageId")]
    public class M210403_1818_AlterServiceLanguageProficiency : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210403_1818_AlterServiceLanguageProficiency(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ServiceLanguageProficiency)).Constraint("FK_98D1E8F512237876F4AA206E63973A33CA84F0BD").Exists())
            {
                Delete.ForeignKey("FK_98D1E8F512237876F4AA206E63973A33CA84F0BD")
                    .OnTable(nameof(ServiceLanguageProficiency));
            }

            if (Schema.Table(nameof(ServiceLanguageProficiency))
                .Constraint("FK_ServiceLanguageProficiency_LanguageId").Exists() == false)
            {
                Create.ForeignKey("FK_ServiceLanguageProficiency_LanguageId")
                    .FromTable(nameof(ServiceLanguageProficiency))
                    .ForeignColumn(nameof(ServiceLanguageProficiency.LanguageId))
                    .ToTable(nameof(CommunicateLanguage))
                    .PrimaryColumn(nameof(CommunicateLanguage.Id));
            }
        }

        public override void Down()
        {
        }
    }
}
