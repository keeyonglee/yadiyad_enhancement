using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Consultation;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/03/29 10:30:00", "alter ConsultationProfile and ConsultationInvite. Both add published col. Profile changed IsApprove to nullable" +
        "Invitation add ModeratorCustomerId")]

    public class M000021_AlterConsultationProfile_ConsultationInvite : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;
        public M000021_AlterConsultationProfile_ConsultationInvite(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ConsultationProfile)).Column(nameof(ConsultationProfile.Remarks)).Exists() == false)
            {
                Create.Column(nameof(ConsultationProfile.Remarks))
            .OnTable(nameof(ConsultationProfile))
            .AsString(int.MaxValue)
            .Nullable();
            }

            if (Schema.Table(nameof(ConsultationProfile)).Column(nameof(ConsultationProfile.IsApproved)).Exists() == false)
            {
                Alter.Column(nameof(ConsultationProfile.IsApproved))
                .OnTable(nameof(ConsultationProfile))
                .AsBoolean()
                .Nullable();
            }

            if (Schema.Table(nameof(ConsultationInvitation)).Column(nameof(ConsultationInvitation.ApprovalRemarks)).Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.ApprovalRemarks))
                .OnTable(nameof(ConsultationInvitation))
                .AsString(int.MaxValue)
                .Nullable();
            }

            if (Schema.Table(nameof(ConsultationInvitation)).Column(nameof(ConsultationInvitation.ModeratorCustomerId)).Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.ModeratorCustomerId))
               .OnTable(nameof(ConsultationInvitation))
               .AsInt32()
               .ForeignKey("FK_ConsultationInvitation_ModeratorCustomerId", "Customer", "Id")
               .Nullable();
            }
        }
    }
}
