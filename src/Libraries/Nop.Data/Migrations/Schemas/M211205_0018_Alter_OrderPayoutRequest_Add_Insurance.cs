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
    [NopMigration("2021/12/05 00:18:00", "M211205_0018_Alter_OrderPayoutRequest_Add_Insurance")]
    public class M211205_0018_Alter_OrderPayoutRequest_Add_Insurance : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211205_0018_Alter_OrderPayoutRequest_Add_Insurance(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(OrderPayoutRequest))
                 .Column(nameof(OrderPayoutRequest.Insurance))
                 .Exists())
            {
                Create.Column(nameof(OrderPayoutRequest.Insurance))
                    .OnTable(nameof(OrderPayoutRequest))
                    .AsDecimal()
                    .NotNullable()
                    .WithDefaultValue(0);
            }


            Alter.Column(nameof(OrderPayoutRequest.Insurance))
                .OnTable(nameof(OrderPayoutRequest))
                .AsDecimal()
                .NotNullable();
        }

        public override void Down()
        {
        }
    }
}
