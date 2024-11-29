using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/04/18 17:35:00", "alter JobApplication - add review details")]
    public class M210418_1735_AlterJobApplication_AlterReviewColumn : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;
        public M210418_1735_AlterJobApplication_AlterReviewColumn(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(JobApplication))
                .Column(nameof(JobApplication.KnowledgenessRating))
                .Exists() == false)
            {
                Create.Column(nameof(JobApplication.KnowledgenessRating))
                .OnTable(nameof(JobApplication))
                .AsDouble()
                .Nullable();
            }

            if (Schema.Table(nameof(JobApplication))
                .Column(nameof(JobApplication.ClearnessRating))
                .Exists() == false)
            {
                Create.Column(nameof(JobApplication.ClearnessRating))
                .OnTable(nameof(JobApplication))
                .AsDouble()
                .Nullable();
            }

            if (Schema.Table(nameof(JobApplication))
                .Column(nameof(JobApplication.ProfessionalismRating))
                .Exists() == false)
            {
                Create.Column(nameof(JobApplication.ProfessionalismRating))
                .OnTable(nameof(JobApplication))
                .AsDouble()
                .Nullable();
            }

            if (Schema.Table(nameof(JobApplication))
                .Column(nameof(JobApplication.RelevanceRating))
                .Exists() == false)
            {
                Create.Column(nameof(JobApplication.RelevanceRating))
                .OnTable(nameof(JobApplication))
                .AsDouble()
                .Nullable();
            }

            if (Schema.Table(nameof(JobApplication))
                .Column(nameof(JobApplication.RespondingRating))
                .Exists() == false)
            {
                Create.Column(nameof(JobApplication.RespondingRating))
                .OnTable(nameof(JobApplication))
                .AsDouble()
                .Nullable();
            }

            if (Schema.Table(nameof(JobApplication))
                .Column(nameof(JobApplication.Rating))
                .Exists() == false)
            {
                Create.Column(nameof(JobApplication.Rating))
                .OnTable(nameof(JobApplication))
                .AsDouble()
                .Nullable();
            }

            if (Schema.Table(nameof(JobApplication))
                .Column(nameof(JobApplication.ReviewText))
                .Exists() == false)
            {
                Create.Column(nameof(JobApplication.ReviewText))
                .OnTable(nameof(JobApplication))
                .AsString(int.MaxValue)
                .Nullable();
            }

            if (Schema.Table(nameof(JobApplication))
                .Column(nameof(JobApplication.ReviewDateTime))
                .Exists() == false)
            {
                Create.Column(nameof(JobApplication.ReviewDateTime))
                .OnTable(nameof(JobApplication))
                .AsDateTime()
                .Nullable();
            }
        }
    }
}

