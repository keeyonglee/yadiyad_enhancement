using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2022/01/15 13:39:00", "M220115_1339_Alter_MasterOrder_Add_TotalVendorPlatformSubTotalDiscount")]
    public class M220115_1339_Alter_MasterOrder_Add_TotalVendorPlatformSubTotalDiscount : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220115_1339_Alter_MasterOrder_Add_TotalVendorPlatformSubTotalDiscount(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {

            if (!Schema.Table(nameof(MasterOrder))
                .Column(nameof(MasterOrder.TotalPlatformSubTotalDiscount))
                .Exists())
            {
                Create.Column(nameof(MasterOrder.TotalPlatformSubTotalDiscount))
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
