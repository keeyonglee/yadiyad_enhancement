using FluentMigrator;
using Nop.Core.Domain.Payout;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/12/16 00:29:00", "M211216_1229_Alter_Invoice_Add_InvoiceTo")]
    public class M211216_1229_Alter_Invoice_Add_InvoiceTo : Migration
    {
        private readonly IMigrationManager _migrationManager;


        public M211216_1229_Alter_Invoice_Add_InvoiceTo(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(Invoice))
                 .Column(nameof(Invoice.InvoiceTo))
                 .Exists() == false)
            {
                Create.Column(nameof(Invoice.InvoiceTo))
                    .OnTable(nameof(Invoice))
                    .AsInt32()
                    .NotNullable();
            }

        }

        public override void Down()
        {
        }
    }
}
