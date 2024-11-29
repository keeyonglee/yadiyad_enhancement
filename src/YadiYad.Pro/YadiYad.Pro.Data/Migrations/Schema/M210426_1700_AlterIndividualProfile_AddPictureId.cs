using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Individual;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/04/26 17:00:00", "alter IndividualProfile - add Picture id")]

    public class M210426_1700_AlterIndividualProfile_AddPictureId : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210426_1700_AlterIndividualProfile_AddPictureId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(IndividualProfile))
               .Column(nameof(IndividualProfile.PictureId))
               .Exists() == false)
            {
                Create.Column(nameof(IndividualProfile.PictureId))
                .OnTable(nameof(IndividualProfile))
                .AsInt32().ForeignKey(nameof(Picture), nameof(Picture.Id))
                .Nullable();
            }

            if (Schema.Table(nameof(IndividualProfile))
               .Column("ProfileImage")
               .Exists() == true)
            {
                Delete.Column("ProfileImage").FromTable(nameof(IndividualProfile));
            }
        }

        public override void Down()
        {
        }

    }
}
