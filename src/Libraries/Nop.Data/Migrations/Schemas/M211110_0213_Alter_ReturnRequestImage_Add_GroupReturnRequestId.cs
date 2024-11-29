using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/11/10 02:13:00", "Alter return request image add group return request Id")]
    public class M211110_0213_Alter_ReturnRequestImage_Add_GroupReturnRequestId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211110_0213_Alter_ReturnRequestImage_Add_GroupReturnRequestId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(ReturnRequestImage))
                .Column(nameof(ReturnRequestImage.GroupReturnRequestId))
                .Exists())
            {
                Create.Column(nameof(ReturnRequestImage.GroupReturnRequestId))
                    .OnTable(nameof(ReturnRequestImage))
                    .AsInt32()
                    .ForeignKey(nameof(GroupReturnRequest), nameof(GroupReturnRequest.Id));
            }

        }

        public override void Down()
        {
        }
    }
}
