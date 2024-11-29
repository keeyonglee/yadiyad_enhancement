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
    [NopMigration("2021/12/04 17:50:00", "M211204_1705_Alter_OrderRefundRequest_Alter_Amount")]
    public class M211204_1705_Alter_OrderRefundRequest_Alter_Amount : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211204_1705_Alter_OrderRefundRequest_Alter_Amount(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(OrderRefundRequest))
                 .Column(nameof(OrderRefundRequest.Amount))
                 .Exists())
            {
                Alter.Column(nameof(OrderRefundRequest.Amount))
                    .OnTable(nameof(OrderRefundRequest))
                    .AsDecimal().NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
