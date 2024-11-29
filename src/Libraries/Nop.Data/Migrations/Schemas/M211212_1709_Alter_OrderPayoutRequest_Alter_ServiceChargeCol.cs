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
using Nop.Core.Domain.Payout;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/12/12 17:09:00", "M211212_1709_Alter_OrderPayoutRequest_Alter_ServiceChargeCol")]
    public class M211212_1709_Alter_OrderPayoutRequest_Alter_ServiceChargeCol : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211212_1709_Alter_OrderPayoutRequest_Alter_ServiceChargeCol(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(OrderPayoutRequest))
                 .Column(nameof(OrderPayoutRequest.ServiceCharge))
                 .Exists())
            {
                Alter.Column(nameof(OrderPayoutRequest.ServiceCharge))
                    .OnTable(nameof(OrderPayoutRequest))
                    .AsDecimal()
                    .Nullable();
            }

            if (Schema.Table(nameof(OrderPayoutRequest))
                 .Column(nameof(OrderPayoutRequest.ServiceChargeRate))
                 .Exists())
            {
                Alter.Column(nameof(OrderPayoutRequest.ServiceChargeRate))
                    .OnTable(nameof(OrderPayoutRequest))
                    .AsDecimal()
                    .Nullable();
            }

            if (Schema.Table(nameof(OrderPayoutRequest))
                 .Column(nameof(OrderPayoutRequest.ServiceChargeSST))
                 .Exists())
            {
                Alter.Column(nameof(OrderPayoutRequest.ServiceChargeSST))
                    .OnTable(nameof(OrderPayoutRequest))
                    .AsDecimal()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
