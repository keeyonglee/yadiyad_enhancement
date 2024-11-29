using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/05/11 01:17:00", "alter Individual - add cols")]

    public class M210511_0117_AlterIndividualProfile_AddCol : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210511_0117_AlterIndividualProfile_AddCol(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(IndividualProfile))
               .Column(nameof(IndividualProfile.ProfileImageViewModeId))
               .Exists() == false)
            {
                Create.Column(nameof(IndividualProfile.ProfileImageViewModeId))
                .OnTable(nameof(IndividualProfile))
                .AsInt32()  
                .NotNullable()
                .WithDefaultValue(0);
            }
        }

        public override void Down()
        {
        }

    }
}
