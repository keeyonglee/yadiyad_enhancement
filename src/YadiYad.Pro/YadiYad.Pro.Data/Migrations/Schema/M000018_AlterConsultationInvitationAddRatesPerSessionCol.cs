using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Consultation;

namespace YadiYad.Pro.Data.Migrations.Schema
{

    [NopMigration("2021/03/21 13:17:00", "alter ConsultationInvitation table add ratespersession col")]
    public class M000018_AlterConsultationInvitationAddRatesPerSessionCol : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M000018_AlterConsultationInvitationAddRatesPerSessionCol(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ConsultationInvitation)).Column(nameof(ConsultationInvitation.RatesPerSession)).Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.RatesPerSession))
                .OnTable(nameof(ConsultationInvitation))
                .AsDecimal(11, 2)
                .Nullable();
            }
        }
    }
}
