using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/29 01:00:00", "alter DepositRequest nullable orderitemid")]

    public class M210629_0100_Alter_DepositRequest_Nullable_OrderItemId : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210629_0100_Alter_DepositRequest_Nullable_OrderItemId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            Alter.Column(nameof(DepositRequest.OrderItemId))
            .OnTable(nameof(DepositRequest))
            .AsInt32()
            .Nullable();
        }

        public override void Down()
        {
        }
    }
}
