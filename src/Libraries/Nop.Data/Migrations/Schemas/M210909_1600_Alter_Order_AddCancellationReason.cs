using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/09/09 16:00:00", "Add Cancellation reason")]
    public class M210909_1600_Alter_Order_AddCancellationReason : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210909_1600_Alter_Order_AddCancellationReason(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Order))
               .Column(nameof(Order.CancellationReason))
               .Exists())
            {
                Create.Column(nameof(Order.CancellationReason))
                    .OnTable(nameof(Order))
                    .AsInt32()
                    .Nullable()
                    .WithDefaultValue(false);
            }
        }

        public override void Down()
        {
        }
    }
}
