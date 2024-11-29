using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Refund;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/11/21 17:53:00", "M211121_1753_Alter_ProInvoiceDepositRefundRequest_Alter_BaseNumber")]
    public class M211121_1753_Alter_ProInvoiceDepositRefundRequest_Alter_BaseNumber : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211121_1753_Alter_ProInvoiceDepositRefundRequest_Alter_BaseNumber(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(DepositRequest))
               .Column(nameof(DepositRequest.BaseDepositNumber))
               .Exists())
            {
                Alter.Column(nameof(DepositRequest.BaseDepositNumber))
                    .OnTable(nameof(DepositRequest))
                    .AsInt32()
                    .WithDefaultValue(0);
            }

            if (Schema.Table(nameof(ProInvoice))
                .Column(nameof(ProInvoice.BaseInvoiceNumber))
                .Exists())
            {
                Alter.Column(nameof(ProInvoice.BaseInvoiceNumber))
                    .OnTable(nameof(ProInvoice))
                    .AsInt32()
                    .WithDefaultValue(0);
            }

            if (Schema.Table(nameof(RefundRequest))
                .Column(nameof(RefundRequest.BaseRefundNumber))
                .Exists())
            {
                Alter.Column(nameof(RefundRequest.BaseRefundNumber))
                    .OnTable(nameof(RefundRequest))
                    .AsInt32()
                    .WithDefaultValue(0);
            }
        }

        public override void Down()
        {
        }
    }
}
