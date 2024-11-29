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
    [NopMigration("2021/06/24 02:00:00", "Alter DepositRequest")]

    public class M210624_0200_Alter_DepositRequest : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210624_0200_Alter_DepositRequest(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(DepositRequest))
               .Column(nameof(DepositRequest.CycleStart))
               .Exists() == false)
            {
                Create.Column(nameof(DepositRequest.CycleStart))
                .OnTable(nameof(DepositRequest))
                .AsDateTime()
                .NotNullable();
            }
            if (Schema.Table(nameof(DepositRequest))
               .Column(nameof(DepositRequest.CycleEnd))
               .Exists() == false)
            {
                Create.Column(nameof(DepositRequest.CycleEnd))
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
