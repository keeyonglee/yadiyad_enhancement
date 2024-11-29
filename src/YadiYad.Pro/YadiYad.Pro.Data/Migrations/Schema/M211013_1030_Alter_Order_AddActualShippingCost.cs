using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Orders;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Payout;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    
    [NopMigration("2021/10/13 10:30:00", "M211013_1030_Alter_Order_AddActualShippingCost")]
    public class M211013_1030_Alter_Order_AddActualShippingCost : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211013_1030_Alter_Order_AddActualShippingCost(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Order))
               .Column(nameof(Order.ActualOrderShippingExclTax))
               .Exists())
            {
                Create.Column(nameof(Order.ActualOrderShippingExclTax))
                    .OnTable(nameof(Order))
                    .AsDecimal()
                    .WithDefaultValue(0)
                    .NotNullable();
            }

            if (!Schema.Table(nameof(Order))
                .Column(nameof(Order.ActualOrderShippingInclTax))
                .Exists())
            {
                Create.Column(nameof(Order.ActualOrderShippingInclTax))
                    .OnTable(nameof(Order))
                    .AsDecimal()
                    .WithDefaultValue(0)
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
