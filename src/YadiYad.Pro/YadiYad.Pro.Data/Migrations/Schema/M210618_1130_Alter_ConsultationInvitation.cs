using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/18 11:30:00", "Alter ConsultationInvitation")]

    public class M210618_1130_Alter_ConsultationInvitation : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210618_1130_Alter_ConsultationInvitation(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ConsultationInvitation))
               .Column(nameof(ConsultationInvitation.CancellationReasonId))
               .Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.CancellationReasonId))
                .OnTable(nameof(ConsultationInvitation))
                .AsInt32()
                .ForeignKey(nameof(Reason), nameof(Reason.Id))
                .Nullable();
            }

            if (Schema.Table(nameof(ConsultationInvitation))
               .Column(nameof(ConsultationInvitation.CancellationRemarks))
               .Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.CancellationRemarks))
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
