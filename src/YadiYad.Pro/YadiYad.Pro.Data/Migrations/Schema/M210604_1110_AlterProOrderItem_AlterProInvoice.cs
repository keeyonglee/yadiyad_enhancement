using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/04 11:10:00", "AlterProOrderItem_AlterProInvoice")]

    public class M210604_1110_AlterProOrderItem_AlterProInvoice : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210604_1110_AlterProOrderItem_AlterProInvoice(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ProOrderItem))
               .Column(nameof(ProOrderItem.ItemName))
               .Exists() == false)
            {
                Create.Column(nameof(ProOrderItem.ItemName))
                .OnTable(nameof(ProOrderItem))
                .AsString()
                .NotNullable();
            }

            if (Schema.Table(nameof(ProInvoice))
               .Column(nameof(ProInvoice.RefType))
               .Exists() == false)
            {
                Create.Column(nameof(ProInvoice.RefType))
                .OnTable(nameof(ProInvoice))
                .AsInt32()
                .NotNullable();
            }

            if (Schema.Table(nameof(ProInvoice))
               .Column(nameof(ProInvoice.InvoiceTo))
               .Exists() == false)
            {
                Create.Column(nameof(ProInvoice.InvoiceTo))
                .OnTable(nameof(ProInvoice))
                .AsInt32()
                .NotNullable();
            }

            if (Schema.Table(nameof(ProInvoice))
               .Column(nameof(ProInvoice.InvoiceFrom))
               .Exists() == false)
            {
                Create.Column(nameof(ProInvoice.InvoiceFrom))
                .OnTable(nameof(ProInvoice))
                .AsInt32()
                .Nullable();
            }
        }

        public override void Down()
        {
        }


    }
}
