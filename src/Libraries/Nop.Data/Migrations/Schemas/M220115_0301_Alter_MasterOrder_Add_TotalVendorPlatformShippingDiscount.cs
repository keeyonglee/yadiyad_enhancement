using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2022/01/15 03:01:00", "M220115_0301_Alter_MasterOrder_Add_TotalVendorPlatformShippingDiscount")]
    public class M220115_0301_Alter_MasterOrder_Add_TotalVendorPlatformShippingDiscount : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220115_0301_Alter_MasterOrder_Add_TotalVendorPlatformShippingDiscount(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(MasterOrder))
                .Column(nameof(MasterOrder.TotalVendorShippingDiscount))
                .Exists())
            {
                Create.Column(nameof(MasterOrder.TotalVendorShippingDiscount))
                    .OnTable(nameof(MasterOrder))
                    .AsDecimal()
                    .NotNullable();
            }

            if (!Schema.Table(nameof(MasterOrder))
                .Column(nameof(MasterOrder.TotalPlatformShippingDiscount))
                .Exists())
            {
                Create.Column(nameof(MasterOrder.TotalPlatformShippingDiscount))
                    .OnTable(nameof(MasterOrder))
                    .AsDecimal()
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
