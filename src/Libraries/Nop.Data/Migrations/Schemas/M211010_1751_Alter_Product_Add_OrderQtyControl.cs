using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/10/10 17:51:00", "M211010_1751_Alter_Product_Add_OrderQtyControl")]
    public class M211010_1751_Alter_Product_Add_OrderQtyControl : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211010_1751_Alter_Product_Add_OrderQtyControl(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Product))
                .Column(nameof(Product.MonMaxNoOrderQty))
                .Exists())
            {
                Create.Column(nameof(Product.MonMaxNoOrderQty))
                    .OnTable(nameof(Product))
                    .AsInt32()
                    .Nullable();
            }

            if (!Schema.Table(nameof(Product))
                .Column(nameof(Product.TueMaxNoOrderQty))
                .Exists())
            {
                Create.Column(nameof(Product.TueMaxNoOrderQty))
                    .OnTable(nameof(Product))
                    .AsInt32()
                    .Nullable();
            }

            if (!Schema.Table(nameof(Product))
                .Column(nameof(Product.WedMaxNoOrderQty))
                .Exists())
            {
                Create.Column(nameof(Product.WedMaxNoOrderQty))
                    .OnTable(nameof(Product))
                    .AsInt32()
                    .Nullable();
            }

            if (!Schema.Table(nameof(Product))
                .Column(nameof(Product.ThuMaxNoOrderQty))
                .Exists())
            {
                Create.Column(nameof(Product.ThuMaxNoOrderQty))
                    .OnTable(nameof(Product))
                    .AsInt32()
                    .Nullable();
            }

            if (!Schema.Table(nameof(Product))
                .Column(nameof(Product.FriMaxNoOrderQty))
                .Exists())
            {
                Create.Column(nameof(Product.FriMaxNoOrderQty))
                    .OnTable(nameof(Product))
                    .AsInt32()
                    .Nullable();
            }

            if (!Schema.Table(nameof(Product))
                .Column(nameof(Product.SatMaxNoOrderQty))
                .Exists())
            {
                Create.Column(nameof(Product.SatMaxNoOrderQty))
                    .OnTable(nameof(Product))
                    .AsInt32()
                    .Nullable();
            }

            if (!Schema.Table(nameof(Product))
                .Column(nameof(Product.SunMaxNoOrderQty))
                .Exists())
            {
                Create.Column(nameof(Product.SunMaxNoOrderQty))
                    .OnTable(nameof(Product))
                    .AsInt32()
                    .Nullable();
            }

            if (!Schema.Table(nameof(Product))
                .Column(nameof(Product.AdvancedOrderDay))
                .Exists())
            {
                Create.Column(nameof(Product.AdvancedOrderDay))
                    .OnTable(nameof(Product))
                    .AsInt32()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
