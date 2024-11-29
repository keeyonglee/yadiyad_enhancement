using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Consultation;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/03/30 17:30:00", "alter ConsultationInvite. changed remarks name to ApprovalRemark and add col StatusRemark")]
    public class M000022_AlterConsultationInvitation : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;
        public M000022_AlterConsultationInvitation(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ConsultationInvitation)).Column(nameof(ConsultationInvitation.StatusRemarks)).Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.StatusRemarks))
                .OnTable(nameof(ConsultationInvitation))
                .AsString(int.MaxValue)
                .Nullable();
            }

            Rename.Column(nameof(ConsultationInvitation.ApprovalRemarks))
                .OnTable(nameof(ConsultationInvitation))
                .To("ApprovalRemarks");
            
        }
    }
}
