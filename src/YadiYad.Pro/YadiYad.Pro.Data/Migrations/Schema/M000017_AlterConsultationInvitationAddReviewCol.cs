using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Consultation;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/03/20 15:17:00", "alter ConsultationInvitation table add review col")]
    public class M000017_AlterConsultationInvitationAddReviewCol : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M000017_AlterConsultationInvitationAddReviewCol(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ConsultationInvitation)).Column(nameof(ConsultationInvitation.ReviewText)).Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.ReviewText))
                .OnTable(nameof(ConsultationInvitation))
                .AsString(int.MaxValue)
                .Nullable();
            }

            if (Schema.Table(nameof(ConsultationInvitation)).Column(nameof(ConsultationInvitation.ReviewDateTime)).Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.ReviewDateTime))
                .OnTable(nameof(ConsultationInvitation))
                .AsDateTime()
                .Nullable();
            }
        }
    }
}
