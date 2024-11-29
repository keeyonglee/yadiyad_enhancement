using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Refund;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/08/21 20:43:00", "Alter RefundRequest Add Status")]
    public class M210821_2043_Alter_RefundRequest_Add_Status : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210821_2043_Alter_RefundRequest_Add_Status(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(RefundRequest))
               .Column(nameof(RefundRequest.Status))
               .Exists())
            {
                Create.Column(nameof(RefundRequest.Status))
                    .OnTable(nameof(RefundRequest))
                    .AsInt32()
                    .NotNullable()
                    .WithDefaultValue(0);
            }
        }

        public override void Down()
        {
        }
    }
}
