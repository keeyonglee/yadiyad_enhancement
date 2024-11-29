using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/03/25 14:17:00", "alter JobServiceCategory and Expertise table. Both add published col")]

    public class M000020_AlterJobServiceCategoryAddPublished : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;
        public M000020_AlterJobServiceCategoryAddPublished(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(JobServiceCategory)).Column(nameof(JobServiceCategory.Published)).Exists() == false)
            {
                Create.Column(nameof(JobServiceCategory.Published))
                .OnTable(nameof(JobServiceCategory))
                .AsBoolean()
                .NotNullable();
            }

            if (Schema.Table(nameof(Expertise)).Column(nameof(Expertise.Published)).Exists() == false)
            {
                Create.Column(nameof(Expertise.Published))
                .OnTable(nameof(Expertise))
                .AsBoolean()
                .NotNullable();
            }
        }
    }
}
