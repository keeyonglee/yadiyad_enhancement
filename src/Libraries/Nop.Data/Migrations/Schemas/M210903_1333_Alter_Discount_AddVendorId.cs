using FluentMigrator;
using Nop.Core.Domain.Discounts;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/09/03 13:23:00", "Alter Discount Add Vendor Id")]
    public class M210903_1333_Alter_Discount_AddVendorId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210903_1333_Alter_Discount_AddVendorId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Discount))
               .Column(nameof(Discount.VendorId))
               .Exists())
            {
                Create.Column(nameof(Discount.VendorId))
                    .OnTable(nameof(Discount))
                    .AsInt32()
                    .WithDefaultValue(false);
            }
        }

        public override void Down()
        {
        }
    }
}
