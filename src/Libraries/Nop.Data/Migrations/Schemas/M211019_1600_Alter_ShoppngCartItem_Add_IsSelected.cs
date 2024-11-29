using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/10/19 16:00:00", "M211019_1600_Alter_ShoppngCartItem_Add_IsSelected")]
    public class M211019_1600_Alter_ShoppngCartItem_Add_IsSelected : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211019_1600_Alter_ShoppngCartItem_Add_IsSelected(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(ShoppingCartItem))
                .Column(nameof(ShoppingCartItem.IsSelected))
                .Exists())
            {
                Create.Column(nameof(ShoppingCartItem.IsSelected))
                    .OnTable(nameof(ShoppingCartItem))
                    .AsBoolean()
                    .WithDefaultValue(false)
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
