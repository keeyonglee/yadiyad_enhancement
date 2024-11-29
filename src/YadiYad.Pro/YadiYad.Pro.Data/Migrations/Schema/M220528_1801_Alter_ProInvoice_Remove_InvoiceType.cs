using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2022/05/28 18:01:00", "Alter ProInvoice Remove InvoiceType")]
    public class M220528_1801_Alter_ProInvoice_Remove_InvoiceType : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M220528_1801_Alter_ProInvoice_Remove_InvoiceType(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(ProInvoice))
               .Column("InvoiceType")
               .Exists())
            {
                Delete.Column("InvoiceType")
                    .FromTable(nameof(ProInvoice));
            }
        }

        public override void Down()
        {
        }
    }
}
