using FluentMigrator;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/09/26 23:59:00", "Add ShippingMethodId")]
    public class M210926_2359_Alter_Shipment_Add_ShippingMethodId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210926_2359_Alter_Shipment_Add_ShippingMethodId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Shipment))
                .Column(nameof(Shipment.ShippingMethodId))
                .Exists())
            {
                Create.Column(nameof(Shipment.ShippingMethodId))
                    .OnTable(nameof(Shipment))
                    .AsInt32()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
