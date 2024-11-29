using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/11/27 19:51:00", "M211127_1951_Alter_GroupReturnRequest_Add_ApprovalDateUtc")]
    public class M211127_1951_Alter_GroupReturnRequest_Add_ApprovalDateUtc : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211127_1951_Alter_GroupReturnRequest_Add_ApprovalDateUtc(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(GroupReturnRequest))
                .Column(nameof(GroupReturnRequest.ApprovalDateUtc))
                .Exists())
            {
                Create.Column(nameof(GroupReturnRequest.ApprovalDateUtc))
                    .OnTable(nameof(GroupReturnRequest))
                    .AsDateTime()
                    .Nullable();
            }

        }

        public override void Down()
        {
        }
    }
}
