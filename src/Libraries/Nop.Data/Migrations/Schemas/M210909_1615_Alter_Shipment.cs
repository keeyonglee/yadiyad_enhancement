using FluentMigrator;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/09/09 16:15:00", "Add Insurance, Type and ReturnOrderId")]
    public class M210909_1615_Alter_Shipment : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210909_1615_Alter_Shipment(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Shipment))
               .Column(nameof(Shipment.Insurance))
               .Exists())
            {
                Create.Column(nameof(Shipment.Insurance))
                    .OnTable(nameof(Shipment))
                    .AsDecimal()
                    .Nullable()
                    .WithDefaultValue(false);
            }
            if (!Schema.Table(nameof(Shipment))
               .Column(nameof(Shipment.ReturnOrderId))
               .Exists())
            {
                Create.Column(nameof(Shipment.ReturnOrderId))
                    .OnTable(nameof(Shipment))
                    .AsInt32()
                    .Nullable()
                    .WithDefaultValue(false);
            }
            if (!Schema.Table(nameof(Shipment))
               .Column(nameof(Shipment.Type))
               .Exists())
            {
                Create.Column(nameof(Shipment.Type))
                    .OnTable(nameof(Shipment))
                    .AsInt32()
                    .NotNullable()
                    .WithDefaultValue(false);
            }
        }

        public override void Down()
        {
        }
    }
}
