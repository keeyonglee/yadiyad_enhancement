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
    [NopMigration("2021/11/25 00:50:00", "M211125_0049_Alter_OrderRefundRequest_Add_MissingCol")]
    public class M211125_0050_Alter_OrderRefundRequest_Add_MissingCol : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211125_0050_Alter_OrderRefundRequest_Add_MissingCol(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(OrderRefundRequest))
                 .Column(nameof(OrderRefundRequest.Deleted))
                 .Exists())
            {
                Create.Column(nameof(OrderRefundRequest.Deleted))
                    .OnTable(nameof(OrderRefundRequest))
                    .AsBoolean().WithDefaultValue(false).NotNullable();
            }

            if (!Schema.Table(nameof(OrderRefundRequest))
                .Column(nameof(OrderRefundRequest.CreatedById))
                .Exists())
            {
                Create.Column(nameof(OrderRefundRequest.CreatedById))
                    .OnTable(nameof(OrderRefundRequest))
                    .AsInt32().NotNullable();
            }

            if (!Schema.Table(nameof(OrderRefundRequest))
                .Column(nameof(OrderRefundRequest.UpdatedById))
                .Exists())
            {
                Create.Column(nameof(OrderRefundRequest.UpdatedById))
                    .OnTable(nameof(OrderRefundRequest))
                    .AsInt32().Nullable();
            }

            if (!Schema.Table(nameof(OrderRefundRequest))
                .Column(nameof(OrderRefundRequest.CreatedOnUTC))
                .Exists())
            {
                Create.Column(nameof(OrderRefundRequest.CreatedOnUTC))
                    .OnTable(nameof(OrderRefundRequest))
                    .AsDateTime().NotNullable();
            }

            if (!Schema.Table(nameof(OrderRefundRequest))
                .Column(nameof(OrderRefundRequest.UpdatedOnUTC))
                .Exists())
            {
                Create.Column(nameof(OrderRefundRequest.UpdatedOnUTC))
                    .OnTable(nameof(OrderRefundRequest))
                    .AsDateTime().Nullable();
            }

            if (!Schema.Table(nameof(OrderRefundRequest))
                .Column(nameof(OrderRefundRequest.OrderId))
                .Exists())
            {
                Create.Column(nameof(OrderRefundRequest.OrderId))
                    .OnTable(nameof(OrderRefundRequest))
                    .AsInt32()
                    .ForeignKey(
                        $"FK_{nameof(OrderRefundRequest)}_{nameof(OrderRefundRequest.OrderId)}",
                        nameof(Order),
                        nameof(Order.Id));
            }
        }

        public override void Down()
        {
        }
    }
}
