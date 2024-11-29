using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/11/09 22:34:00", "M211109_2234_Alter_JobApplication_Add_RehiredobApplicationId")]
    public class M211109_2234_Alter_JobApplication_Add_RehiredobApplicationId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211109_2234_Alter_JobApplication_Add_RehiredobApplicationId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(JobApplication))
               .Column(nameof(JobApplication.RehiredobApplicationId))
               .Exists())
            {
                Create.Column(nameof(JobApplication.RehiredobApplicationId))
                    .OnTable(nameof(JobApplication))
                    .AsInt32()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
