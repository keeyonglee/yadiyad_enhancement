using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Consultation;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/16 11:50:00", "Alter ConsultationInvitation")]

    public class M210616_1155_Alter_ConsultationInvitation : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210616_1155_Alter_ConsultationInvitation(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ConsultationInvitation))
               .Column(nameof(ConsultationInvitation.RescheduleRemarks))
               .Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.RescheduleRemarks))
                .OnTable(nameof(ConsultationInvitation))
                .AsString(int.MaxValue)
                .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
