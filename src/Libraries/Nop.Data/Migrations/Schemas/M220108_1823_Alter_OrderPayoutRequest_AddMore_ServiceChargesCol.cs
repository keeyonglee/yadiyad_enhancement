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
    [NopMigration("2022/01/08 18:23:00", "M220108_1823_Alter_OrderPayoutRequest_AddMore_ServiceChargesCol")]
    public class M220108_1823_Alter_OrderPayoutRequest_AddMore_ServiceChargesCol : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220108_1823_Alter_OrderPayoutRequest_AddMore_ServiceChargesCol(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(OrderPayoutRequest))
                 .Column(nameof(OrderPayoutRequest.ServiceChargesWaiver))
                 .Exists() == false)
            {
                Create.Column(nameof(OrderPayoutRequest.ServiceChargesWaiver))
                    .OnTable(nameof(OrderPayoutRequest))
                    .AsDecimal()
                    .Nullable();
            }

            if (Schema.Table(nameof(OrderPayoutRequest))
                 .Column(nameof(OrderPayoutRequest.ServiceChargesUTC))
                 .Exists() == false)
            {
                Create.Column(nameof(OrderPayoutRequest.ServiceChargesUTC))
                    .OnTable(nameof(OrderPayoutRequest))
                    .AsDateTime()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
