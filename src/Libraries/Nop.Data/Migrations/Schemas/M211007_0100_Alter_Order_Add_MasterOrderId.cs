using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/10/07 01:00:00", "Alter order add masterOrderId")]
    public class M211007_0100_Alter_Order_Add_MasterOrderId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211007_0100_Alter_Order_Add_MasterOrderId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(Order))
                .Column(nameof(Order.MasterOrderId))
                .Exists())
            {
                if (Schema.Table(nameof(GroupReturnRequest))
                    .Column("ReturnRequestStatusId")
                    .Exists())
                {
                    Delete.Column("ReturnRequestStatusId")
                        .FromTable(nameof(GroupReturnRequest));
                }
            }

            if (!Schema.Table(nameof(Order))
                .Column(nameof(Order.MasterOrderId))
                .Exists())
            {
                Create.Column(nameof(Order.MasterOrderId))
                    .OnTable(nameof(Order))
                    .AsInt32()
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
