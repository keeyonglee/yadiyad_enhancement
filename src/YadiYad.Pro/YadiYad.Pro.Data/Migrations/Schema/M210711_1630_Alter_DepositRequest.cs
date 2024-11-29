using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Deposit;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/07/11 16:30:00", "Alter DepositRequest (add ApproveRemarks)")]

    public class M210711_1630_Alter_DepositRequest : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210711_1630_Alter_DepositRequest(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(DepositRequest))
               .Column(nameof(DepositRequest.ApproveRemarks))
               .Exists() == false)
            {
                Create.Column(nameof(DepositRequest.ApproveRemarks))
                    .OnTable(nameof(DepositRequest))
                    .AsString(int.MaxValue)
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
