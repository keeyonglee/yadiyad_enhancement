using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/09/10 11:27:00", "Add Group Return Request Id")]
    public class M210910_1127_Alter_ReturnRequest_Add_GroupReturnRequestId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210910_1127_Alter_ReturnRequest_Add_GroupReturnRequestId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(ReturnRequest))
               .Column(nameof(ReturnRequest.GroupReturnRequestId))
               .Exists())
            {
                Create.Column(nameof(ReturnRequest.GroupReturnRequestId))
                    .OnTable(nameof(ReturnRequest))
                    .AsInt32()
                    .ForeignKey(nameof(GroupReturnRequest), nameof(GroupReturnRequest.Id))
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
