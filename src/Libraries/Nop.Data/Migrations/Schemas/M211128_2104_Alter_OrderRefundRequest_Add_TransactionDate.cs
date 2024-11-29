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
    [NopMigration("2021/11/28 21:04:00", "M211128_2104_Alter_OrderRefundRequest_Add_TransactionDate")]
    public class M211128_2104_Alter_OrderRefundRequest_Add_TransactionDate : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211128_2104_Alter_OrderRefundRequest_Add_TransactionDate(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(OrderRefundRequest))
                 .Column(nameof(OrderRefundRequest.TransactionDate​))
                 .Exists())
            {
                Create.Column(nameof(OrderRefundRequest.TransactionDate​))
                    .OnTable(nameof(OrderRefundRequest))
                    .AsDateTime().NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
