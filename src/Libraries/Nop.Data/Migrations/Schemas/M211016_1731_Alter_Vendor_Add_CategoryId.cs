using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/10/16 17:31:00", "M211016_1731_Alter_Vendor_Add_CategoryId")]
    public class M211016_1731_Alter_Vendor_Add_CategoryId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211016_1731_Alter_Vendor_Add_CategoryId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Vendor))
                .Column(nameof(Vendor.CategoryId))
                .Exists())
            {
                Create.Column(nameof(Vendor.CategoryId))
                    .OnTable(nameof(Vendor))
                    .AsInt32()
                   .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
