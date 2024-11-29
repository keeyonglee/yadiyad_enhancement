using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/11/15 01:22:00", "M211115_0122_Alter_Order_Add_InvoiceNumber")]
    public class M211115_0122_Alter_Order_Add_InvoiceNumber : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211115_0122_Alter_Order_Add_InvoiceNumber(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Order))
                .Column(nameof(Order.InvoiceNumber))
                .Exists())
            {
                Create.Column(nameof(Order.InvoiceNumber))
                    .OnTable(nameof(Order))
                    .AsString(50)
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
