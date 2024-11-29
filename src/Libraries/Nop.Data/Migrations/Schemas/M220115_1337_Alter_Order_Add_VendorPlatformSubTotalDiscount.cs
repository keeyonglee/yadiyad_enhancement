using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2022/01/15 13:37:00", "M220115_1337_Alter_Order_Add_VendorPlatformSubTotalDiscount")]
    public class M220115_1337_Alter_Order_Add_VendorPlatformSubTotalDiscount : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220115_1337_Alter_Order_Add_VendorPlatformSubTotalDiscount(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Order))
                .Column(nameof(Order.PlatformSubTotalDiscount))
                .Exists())
            {
                Create.Column(nameof(Order.PlatformSubTotalDiscount))
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
