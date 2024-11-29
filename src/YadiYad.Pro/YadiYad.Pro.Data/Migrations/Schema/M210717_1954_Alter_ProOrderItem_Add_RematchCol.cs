using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/07/17 19:54:00", "Alter ProOrderItem Add RematchCol")]
    public class M210717_1954_Alter_ProOrderItem_Add_RematchCol : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210717_1954_Alter_ProOrderItem_Add_RematchCol(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (!Schema.Table(nameof(ProOrderItem))
               .Column(nameof(ProOrderItem.Status))
               .Exists())
            {
                Create.Column(nameof(ProOrderItem.Status))
                    .OnTable(nameof(ProOrderItem))
                    .AsInt32()
                    .WithDefaultValue(0)
                    .NotNullable();
            }

            if (!Schema.Table(nameof(ProOrderItem))
               .Column(nameof(ProOrderItem.OffsetProOrderItemId))
               .Exists())
            {
                Create.Column(nameof(ProOrderItem.OffsetProOrderItemId))
                    .OnTable(nameof(ProOrderItem))
                    .AsInt32()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
