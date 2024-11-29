using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Individual;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/04/21 15:20:00", "alter IndividualProfile - add IsOnline")]

    public class M210421_1520_AlterIndividualProfile_AddIsOnline : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;
        public M210421_1520_AlterIndividualProfile_AddIsOnline(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(IndividualProfile))
               .Column(nameof(IndividualProfile.IsOnline))
               .Exists() == false)
            {
                Create.Column(nameof(IndividualProfile.IsOnline))
                .OnTable(nameof(IndividualProfile))
                .AsBoolean()
                .NotNullable();
            }
        }
    }
}
