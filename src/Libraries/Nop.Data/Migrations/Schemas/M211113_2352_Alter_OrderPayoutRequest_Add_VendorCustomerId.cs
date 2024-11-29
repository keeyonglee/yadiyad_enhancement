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
    [NopMigration("2021/11/13 23:52:00", "M211113_2352_Alter_OrderPayoutRequest_Add_VendorCustomerId")]
    public class M211113_2352_Alter_OrderPayoutRequest_Add_VendorCustomerId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211113_2352_Alter_OrderPayoutRequest_Add_VendorCustomerId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(OrderPayoutRequest))
                .Column(nameof(OrderPayoutRequest.VendorCustomerId))
                .Exists())
            {
                Create.Column(nameof(OrderPayoutRequest.VendorCustomerId))
                    .OnTable(nameof(OrderPayoutRequest))
                    .AsInt32()
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
