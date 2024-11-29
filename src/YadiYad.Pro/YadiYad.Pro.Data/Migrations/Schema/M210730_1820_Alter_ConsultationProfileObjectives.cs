using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/07/30 18:20:00", "Alter ConsultationProfileObjectives")]
    public class M210730_1820_Alter_ConsultationProfileObjectives : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210730_1820_Alter_ConsultationProfileObjectives(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            Alter.Column(nameof(ConsultationProfile.Objective))
                .OnTable(nameof(ConsultationProfile))
                .AsString(2000)
                .NotNullable();
        }

        public override void Down()
        {
        }
    }
}
