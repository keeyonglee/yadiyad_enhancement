using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Consultation;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/04/11 15:40:00", "alter ConsultationInvite - add review details")]
    public class M210411_1535_AlterConsultationInvitation_AddReviewDetails : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;
        public M210411_1535_AlterConsultationInvitation_AddReviewDetails(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ConsultationInvitation))
                .Column(nameof(ConsultationInvitation.KnowledgenessRating))
                .Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.KnowledgenessRating))
                .OnTable(nameof(ConsultationInvitation))
                .AsDouble()
                .Nullable();
            }

            if (Schema.Table(nameof(ConsultationInvitation))
                .Column(nameof(ConsultationInvitation.ClearnessRating))
                .Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.ClearnessRating))
                .OnTable(nameof(ConsultationInvitation))
                .AsDouble()
                .Nullable();
            }

            if (Schema.Table(nameof(ConsultationInvitation))
                .Column(nameof(ConsultationInvitation.ProfessionalismRating))
                .Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.ProfessionalismRating))
                .OnTable(nameof(ConsultationInvitation))
                .AsDouble()
                .Nullable();
            }

            if (Schema.Table(nameof(ConsultationInvitation))
                .Column(nameof(ConsultationInvitation.RelevanceRating))
                .Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.RelevanceRating))
                .OnTable(nameof(ConsultationInvitation))
                .AsDouble()
                .Nullable();
            }

            if (Schema.Table(nameof(ConsultationInvitation))
                .Column(nameof(ConsultationInvitation.RespondingRating))
                .Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.RespondingRating))
                .OnTable(nameof(ConsultationInvitation))
                .AsDouble()
                .Nullable();
            }
        }
    }
}

