using FluentMigrator;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/09/15 17:55:00", "Alter Shipping, Add ShippingTotal")]

    public class M210915_1755_Alter_Shipment_Add_ShippingAmount : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210915_1755_Alter_Shipment_Add_ShippingAmount(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            Alter.Column(nameof(Shipment.Insurance))
            .OnTable(nameof(Shipment))
            .AsDecimal()
            .NotNullable();

            if (!Schema.Table(nameof(Shipment))
            .Column(nameof(Shipment.ShippingTotal))
            .Exists())
            {
                Create.Column(nameof(Shipment.ShippingTotal))
                    .OnTable(nameof(Shipment))
                    .AsDecimal()
                    .NotNullable()
                    .WithDefaultValue(false);
            }
        }

        public override void Down()
        {
        }
    }
}
