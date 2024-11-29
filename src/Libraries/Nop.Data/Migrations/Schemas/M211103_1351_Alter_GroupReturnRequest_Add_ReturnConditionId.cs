using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/11/03 13:51:00", "Alter group return add return condition")]
    public class M211103_1351_Alter_GroupReturnRequest_Add_ReturnConditionId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211103_1351_Alter_GroupReturnRequest_Add_ReturnConditionId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(GroupReturnRequest))
                .Column(nameof(GroupReturnRequest.ReturnConditionId))
                .Exists())
            {
                Create.Column(nameof(GroupReturnRequest.ReturnConditionId))
                    .OnTable(nameof(GroupReturnRequest))
                    .AsInt32()
                    .Nullable();
            }

        }

        public override void Down()
        {
        }
    }
}
