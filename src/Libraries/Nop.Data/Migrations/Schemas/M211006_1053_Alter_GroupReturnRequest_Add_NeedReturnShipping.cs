using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/10/06 10:53:00", "Alter group return add need return shipping")]
    public class M211006_1053_Alter_GroupReturnRequest_Add_NeedReturnShipping : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211006_1053_Alter_GroupReturnRequest_Add_NeedReturnShipping(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(GroupReturnRequest))
                .Column(nameof(GroupReturnRequest.NeedReturnShipping))
                .Exists())
            {
                Create.Column(nameof(GroupReturnRequest.NeedReturnShipping))
                    .OnTable(nameof(GroupReturnRequest))
                    .AsInt32()
                    .Nullable();
            }

            if (Schema.Table(nameof(GroupReturnRequest))
                .Column("IsApproved")
                .Exists())
            {
                Delete.Column("IsApproved")
                    .FromTable(nameof(GroupReturnRequest));
            }

        }

        public override void Down()
        {
        }
    }
}
