using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/10/06 11:47:00", "Alter returnrequest delete return request status")]
    public class M211006_1147_Alter_ReturnRequest_Delete_ReturnRequestStatus : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211006_1147_Alter_ReturnRequest_Delete_ReturnRequestStatus(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(GroupReturnRequest))
                .Column("ReturnRequestStatusId")
                .Exists())
            {
                Delete.Column("ReturnRequestStatusId")
                    .FromTable(nameof(GroupReturnRequest));
            }

        }

        public override void Down()
        {
        }
    }
}
