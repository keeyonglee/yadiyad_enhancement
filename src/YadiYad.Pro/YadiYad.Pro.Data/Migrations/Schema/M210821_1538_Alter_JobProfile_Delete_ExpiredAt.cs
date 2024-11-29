using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/08/21 15:38:00", "Alter JobProfile Delete ExpiredAt")]
    public class M210821_1538_Alter_JobProfile_Delete_ExpiredAt : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210821_1538_Alter_JobProfile_Delete_ExpiredAt(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(JobProfile))
               .Column("ExpiredAt")
               .Exists())
            {
                Delete.Column("ExpiredAt")
                    .FromTable(nameof(JobProfile));
            }
        }

        public override void Down()
        {
        }
    }
}
