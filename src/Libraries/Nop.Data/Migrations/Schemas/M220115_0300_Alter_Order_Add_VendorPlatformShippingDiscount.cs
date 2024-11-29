using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2022/01/15 03:00:00", "M220115_0300_Alter_Order_Add_VendorPlatformShippingDiscount")]
    public class M220115_0300_Alter_Order_Add_VendorPlatformShippingDiscount : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220115_0300_Alter_Order_Add_VendorPlatformShippingDiscount(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Order))
                .Column(nameof(Order.VendorShippingDiscount))
                .Exists())
            {
                Create.Column(nameof(Order.VendorShippingDiscount))
                    .OnTable(nameof(Order))
                    .AsDecimal()
                    .NotNullable();
            }

            if (!Schema.Table(nameof(Order))
                .Column(nameof(Order.PlatformShippingDiscount))
                .Exists())
            {
                Create.Column(nameof(Order.PlatformShippingDiscount))
                    .OnTable(nameof(Order))
                    .AsDecimal()
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
