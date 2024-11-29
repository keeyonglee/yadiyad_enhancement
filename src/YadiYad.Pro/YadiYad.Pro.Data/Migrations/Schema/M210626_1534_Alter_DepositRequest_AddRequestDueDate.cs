using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Deposit;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/26 15:34:00", "Alter DepositRequest Add Request/Due Date")]

    public class M210626_1534_Alter_DepositRequest_AddRequestDueDate : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210626_1534_Alter_DepositRequest_AddRequestDueDate(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(DepositRequest))
               .Column(nameof(DepositRequest.DueDate))
               .Exists() == false)
            {
                Create.Column(nameof(DepositRequest.DueDate))
                .OnTable(nameof(DepositRequest))
                .AsDateTime()
                .NotNullable();
            }
            if (Schema.Table(nameof(DepositRequest))
               .Column(nameof(DepositRequest.RequestDate))
               .Exists() == false)
            {
                Create.Column(nameof(DepositRequest.RequestDate))
                .OnTable(nameof(DepositRequest))
                .AsDateTime()
                .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
