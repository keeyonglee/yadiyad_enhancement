using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Deposit;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/07/13 11:47:00", "Alter JobServiceCategory")]

    public class M210713_1147_Alter_JobServiceCategory : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210713_1147_Alter_JobServiceCategory(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(JobServiceCategory))
               .Column(nameof(JobServiceCategory.ImageUrl))
               .Exists() == false)
            {
                Create.Column(nameof(JobServiceCategory.ImageUrl))
                    .OnTable(nameof(JobServiceCategory))
                    .AsString(500)
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
