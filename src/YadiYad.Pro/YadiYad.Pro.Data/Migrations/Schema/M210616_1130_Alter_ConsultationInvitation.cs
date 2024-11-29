using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Consultation;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/16 11:30:00", "Alter ConsultationInvitation")]

    public class M210616_1130_Alter_ConsultationInvitation : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210616_1130_Alter_ConsultationInvitation(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ConsultationInvitation))
               .Column(nameof(ConsultationInvitation.ModeratorReviewText))
               .Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.ModeratorReviewText))
                .OnTable(nameof(ConsultationInvitation))
                .AsString(int.MaxValue)
                .Nullable();
            }

            if (Schema.Table(nameof(ConsultationInvitation))
               .Column(nameof(ConsultationInvitation.ModeratorClearnessRating))
               .Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.ModeratorClearnessRating))
                .OnTable(nameof(ConsultationInvitation))
                .AsDouble()
                .Nullable();
            }

            if (Schema.Table(nameof(ConsultationInvitation))
              .Column(nameof(ConsultationInvitation.ModeratorKnowledgenessRating))
              .Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.ModeratorKnowledgenessRating))
                .OnTable(nameof(ConsultationInvitation))
                .AsDouble()
                .Nullable();
            }

            if (Schema.Table(nameof(ConsultationInvitation))
              .Column(nameof(ConsultationInvitation.ModeratorProfessionalismRating))
              .Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.ModeratorProfessionalismRating))
                .OnTable(nameof(ConsultationInvitation))
                .AsDouble()
                .Nullable();
            }

            if (Schema.Table(nameof(ConsultationInvitation))
              .Column(nameof(ConsultationInvitation.ModeratorRelevanceRating))
              .Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.ModeratorRelevanceRating))
                .OnTable(nameof(ConsultationInvitation))
                .AsDouble()
                .Nullable();
            }

            if (Schema.Table(nameof(ConsultationInvitation))
              .Column(nameof(ConsultationInvitation.ModeratorRespondingRating))
              .Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.ModeratorRespondingRating))
                .OnTable(nameof(ConsultationInvitation))
                .AsDouble()
                .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
