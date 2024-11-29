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
    [NopMigration("2022/01/16 00:00:00", "M220116_0000_Alter_OrderPayoutRequest_AddPlatformDiscount")]
    public class M220116_0000_Alter_OrderPayoutRequest_AddPlatformDiscount : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220116_0000_Alter_OrderPayoutRequest_AddPlatformDiscount(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(OrderPayoutRequest))
                 .Column(nameof(OrderPayoutRequest.PlatformShippingDiscount))
                 .Exists() == false)
            {
                Create.Column(nameof(OrderPayoutRequest.PlatformShippingDiscount))
                    .OnTable(nameof(OrderPayoutRequest))
                    .AsDecimal()
                    .NotNullable();
            }

            if (Schema.Table(nameof(OrderPayoutRequest))
                 .Column(nameof(OrderPayoutRequest.PlatformSubTotalDiscount))
                 .Exists() == false)
            {
                Create.Column(nameof(OrderPayoutRequest.PlatformSubTotalDiscount))
                    .OnTable(nameof(OrderPayoutRequest))
                    .AsDecimal()
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
