using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Payout;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2022/05/28 18:00:00", "Alter PayoutRequest Add ServiceChargeInvoice")]
    public class M220528_1800_Alter_PayoutRequest_Add_ServiceChargeInvoice : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M220528_1800_Alter_PayoutRequest_Add_ServiceChargeInvoice(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(PayoutRequest))
               .Column(nameof(PayoutRequest.ServiceChargeInvoiceId))
               .Exists())
            {
                Create.Column(nameof(PayoutRequest.ServiceChargeInvoiceId))
                    .OnTable(nameof(PayoutRequest))
                    .AsInt32()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
