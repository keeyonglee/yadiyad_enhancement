using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/11/28 00:08:00", "M211128_0008_Alter_Order_Add_CancellationDateUtc")]
    public class M211128_0008_Alter_Order_Add_CancellationDateUtc : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211128_0008_Alter_Order_Add_CancellationDateUtc(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Order))
                .Column(nameof(Order.CancellationDateUtc))
                .Exists())
            {
                Create.Column(nameof(Order.CancellationDateUtc))
                    .OnTable(nameof(Order))
                    .AsDateTime()
                    .Nullable();
            }

        }

        public override void Down()
        {
        }
    }
}
