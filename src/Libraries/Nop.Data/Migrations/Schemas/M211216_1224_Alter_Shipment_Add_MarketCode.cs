using FluentMigrator;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/12/16 12:45:00", "M211216_1245_Alter_Invoice_Add_InvoiceTo")]
    public class M211216_1224_Alter_Shipment_Add_MarketCode : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211216_1224_Alter_Shipment_Add_MarketCode(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Shipment))
             .Column(nameof(Shipment.MarketCode))
             .Exists())
            {
                Create.Column(nameof(Shipment.MarketCode))
                    .OnTable(nameof(Shipment))
                    .AsString()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
