using FluentMigrator;
using Nop.Core.Domain.Discounts;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2022/01/14 14:08:00", "M220114_1408_Alter_Discount_AlterVendorId")]
    public class M220114_1408_Alter_Discount_AlterVendorId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220114_1408_Alter_Discount_AlterVendorId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            Alter.Column(nameof(Discount.VendorId))
                .OnTable(nameof(Discount))
                .AsInt32()
                .Nullable();
        }

        public override void Down()
        {
        }
    }
}
