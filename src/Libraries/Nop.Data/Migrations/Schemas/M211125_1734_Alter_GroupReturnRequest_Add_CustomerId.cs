using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/11/25 17:34:00", "Alter group return add customer id")]
    public class M211125_1734_Alter_GroupReturnRequest_Add_CustomerId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211125_1734_Alter_GroupReturnRequest_Add_CustomerId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(GroupReturnRequest))
                .Column(nameof(GroupReturnRequest.CustomerId))
                .Exists())
            {
                Create.Column(nameof(GroupReturnRequest.CustomerId))
                    .OnTable(nameof(GroupReturnRequest))
                    .AsInt32()
                    .ForeignKey(nameof(Customer), nameof(Customer.Id))
                    .Nullable();
            }

        }

        public override void Down()
        {
        }
    }
}
