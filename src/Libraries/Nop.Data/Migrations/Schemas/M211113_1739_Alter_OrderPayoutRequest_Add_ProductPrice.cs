using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payout;
using Nop.Core.Domain.Vendors;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/11/13 17:39:00", "M211113_1739_Alter_OrderPayoutRequest_Add_ProductPrice")]
    public class M211113_1739_Alter_OrderPayoutRequest_Add_ProductPrice : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211113_1739_Alter_OrderPayoutRequest_Add_ProductPrice(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(OrderPayoutRequest))
                .Column(nameof(OrderPayoutRequest.ProductPriceExclTax))
                .Exists())
            {
                Create.Column(nameof(OrderPayoutRequest.ProductPriceExclTax))
                    .OnTable(nameof(OrderPayoutRequest))
                    .AsDecimal()
                    .NotNullable();
            }

            if (!Schema.Table(nameof(OrderPayoutRequest))
                .Column(nameof(OrderPayoutRequest.ProductPriceInclTax))
                .Exists())
            {
                Create.Column(nameof(OrderPayoutRequest.ProductPriceInclTax))
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
