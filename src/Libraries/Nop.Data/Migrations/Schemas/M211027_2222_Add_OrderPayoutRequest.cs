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
    [NopMigration("2021/10/27 22:22:00", "M211027_2222_Add_OrderPayoutRequest")]
    public class M211027_2222_Add_OrderPayoutRequest : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211027_2222_Add_OrderPayoutRequest(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(Invoice)).Exists() == false)
            {
                _migrationManager.BuildTable<Invoice>(Create);
            }
            if (Schema.Table(nameof(OrderPayoutRequest)).Exists() == false)
            {
                _migrationManager.BuildTable<OrderPayoutRequest>(Create);
            }
        }

        public override void Down()
        {
        }
    }
}
