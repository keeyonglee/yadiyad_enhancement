using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Individual;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2022/04/25 12:40:00", "M220425_1240_Alter_IndividualProfileAddCity")]
    public class M220425_1240_Alter_IndividualProfileAddCity : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220425_1240_Alter_IndividualProfileAddCity(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(IndividualProfile))
               .Column(nameof(IndividualProfile.CityId))
               .Exists())
            {
                Create.Column(nameof(IndividualProfile.CityId))
                    .OnTable(nameof(IndividualProfile))
                    .AsInt32();
            }
        }

        public override void Down()
        {
        }
    }
}
