using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/11/14 23:25:00", "M211114_2335_Alter_Product_Add_DeliveryModeId")]
    public class M211114_2335_Alter_Product_Add_DeliveryModeId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211114_2335_Alter_Product_Add_DeliveryModeId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Product))
                .Column(nameof(Product.DeliveryModeId))
                .Exists())
            {
                Create.Column(nameof(Product.DeliveryModeId))
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
