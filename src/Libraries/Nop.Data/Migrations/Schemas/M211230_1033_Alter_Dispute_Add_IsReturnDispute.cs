using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/12/30 10:33:00", "M211230_1033_Alter_Dispute_Add_IsReturnDispute")]
    public class M211230_1033_Alter_Dispute_Add_IsReturnDispute : Migration
    {
        private readonly IMigrationManager _migrationManager;


        public M211230_1033_Alter_Dispute_Add_IsReturnDispute(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(Dispute))
                 .Column(nameof(Dispute.IsReturnDispute))
                 .Exists() == false)
            {
                Create.Column(nameof(Dispute.IsReturnDispute))
                    .OnTable(nameof(Dispute))
                    .AsBoolean()
                    .NotNullable();
            }

        }

        public override void Down()
        {
        }
    }
}
