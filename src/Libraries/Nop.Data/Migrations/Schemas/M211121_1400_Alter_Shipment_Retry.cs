using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/11/21 14:00:00", "Alter Shipment Retry")]
    public class M211121_1400_Alter_Shipment_Retry : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211121_1400_Alter_Shipment_Retry(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Shipment))
                .Column(nameof(Shipment.RequireInsurance))
                .Exists())
            {
                Create.Column(nameof(Shipment.RequireInsurance))
                    .OnTable(nameof(Shipment))
                    .AsBoolean()
                    .NotNullable()
                    .WithDefaultValue(false);
            }
            
            if (!Schema.Table(nameof(Shipment))
                .Column(nameof(Shipment.DeliveryModeId))
                .Exists())
            {
                Create.Column(nameof(Shipment.DeliveryModeId))
                    .OnTable(nameof(Shipment))
                    .AsInt32()
                    .NotNullable()
                    .WithDefaultValue((int)DeliveryMode.Bike);
            }
            
            if (!Schema.Table(nameof(Shipment))
                .Column(nameof(Shipment.RetryCount))
                .Exists())
            {
                Create.Column(nameof(Shipment.RetryCount))
                    .OnTable(nameof(Shipment))
                    .AsInt32()
                    .NotNullable()
                    .WithDefaultValue(0);
            }
            
            if (!Schema.Table(nameof(Category))
                .Column(nameof(Category.NonReturnable))
                .Exists())
            {
                Create.Column(nameof(Category.NonReturnable))
                    .OnTable(nameof(Category))
                    .AsBoolean()
                    .NotNullable()
                    .WithDefaultValue(false);
            }

        }

        public override void Down()
        {
        }
    }
}
