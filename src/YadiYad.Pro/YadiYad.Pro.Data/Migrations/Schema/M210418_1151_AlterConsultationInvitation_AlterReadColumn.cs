using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Consultation;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/04/18 11:51:00", "alter ConsultationInvite - alter is read")]
    public class M210418_1151_AlterConsultationInvitation_AlterReadColumn : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;
        public M210418_1151_AlterConsultationInvitation_AlterReadColumn(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ConsultationInvitation))
                .Column("IsRead")
                .Exists() == true)
            {
                Rename
                    .Column("IsRead")
                    .OnTable(nameof(ConsultationInvitation))
                    .To(nameof(ConsultationInvitation.IsIndividualRead));
            }

            if (Schema.Table(nameof(ConsultationInvitation))
                .Column(nameof(ConsultationInvitation.IsOrganizationRead))
                .Exists() == false)
            {
                Create
                .Column(nameof(ConsultationInvitation.IsOrganizationRead))
                .OnTable(nameof(ConsultationInvitation))
                .AsBoolean()
                .WithDefaultValue(false)
                .NotNullable();
            }
        }
    }
}

