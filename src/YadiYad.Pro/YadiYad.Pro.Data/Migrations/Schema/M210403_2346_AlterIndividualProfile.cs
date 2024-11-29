using FluentMigrator;
using Nop.Core.Domain.Directory;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/04/03 23:47:00", "Alter AlterIndividualProfile Alter StateProvinceId")]
    public class M210403_2346_AlterIndividualProfile : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210403_2346_AlterIndividualProfile(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(IndividualProfile)).Constraint("FK_19209ADA09AB798A5ABFB6887D66407821C78A2F").Exists())
            {
                Delete.ForeignKey("FK_19209ADA09AB798A5ABFB6887D66407821C78A2F")
                    .OnTable(nameof(IndividualProfile));
            }

            if (Schema.Table(nameof(IndividualProfile))
                .Constraint("FK_IndividualProfile_StateProvinceId").Exists() == false)
            {
                Create.ForeignKey("FK_IndividualProfile_StateProvinceId")
                    .FromTable(nameof(IndividualProfile))
                    .ForeignColumn(nameof(IndividualProfile.StateProvinceId))
                    .ToTable(nameof(StateProvince))
                    .PrimaryColumn(nameof(StateProvince.Id));
            }
        }

        public override void Down()
        {
        }
    }
}
