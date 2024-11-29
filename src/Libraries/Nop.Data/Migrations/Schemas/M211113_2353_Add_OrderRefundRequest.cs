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
    [NopMigration("2021/11/13 23:53:00", "M211113_2353_Add_OrderRefundRequest")]
    public class M211113_2353_Add_OrderRefundRequest : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211113_2353_Add_OrderRefundRequest(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(OrderRefundRequest)).Exists() == false)
            {
                _migrationManager.BuildTable<OrderRefundRequest>(Create);
            }
        }

        public override void Down()
        {
        }
    }
}
