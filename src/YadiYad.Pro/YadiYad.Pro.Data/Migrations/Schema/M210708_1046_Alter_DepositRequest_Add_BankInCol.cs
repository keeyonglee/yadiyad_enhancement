using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Deposit;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/07/08 10:46:00", "Alter DepositRequest Add BankInCol")]

    public class M210708_1046_Alter_DepositRequest_Add_BankInCol : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210708_1046_Alter_DepositRequest_Add_BankInCol(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(DepositRequest))
               .Column(nameof(DepositRequest.PaymentChannelId))
               .Exists() == false)
            {
                Create.Column(nameof(DepositRequest.PaymentChannelId))
                    .OnTable(nameof(DepositRequest))
                    .AsInt32()
                    .WithDefaultValue(0)
                    .Nullable();
            }

            if (Schema.Table(nameof(DepositRequest))
               .Column(nameof(DepositRequest.BankId))
               .Exists() == false)
            {
                Create.Column(nameof(DepositRequest.BankId))
                    .OnTable(nameof(DepositRequest))
                    .AsInt32()
                    .ForeignKey(
                        "FK_DepositRequest_BankId", 
                        nameof(Bank), 
                        nameof(Bank.Id))
                    .Nullable();
            }

            if (Schema.Table(nameof(DepositRequest))
               .Column(nameof(DepositRequest.BankInDate))
               .Exists() == false)
            {
                Create.Column(nameof(DepositRequest.BankInDate))
                    .OnTable(nameof(DepositRequest))
                    .AsDateTime()
                    .Nullable();
            }

            if (Schema.Table(nameof(DepositRequest))
               .Column(nameof(DepositRequest.BankInReference))
               .Exists() == false)
            {
                Create.Column(nameof(DepositRequest.BankInReference))
                    .OnTable(nameof(DepositRequest))
                    .AsString(255)
                    .Nullable();
            }

            if (Schema.Table(nameof(DepositRequest))
               .Column(nameof(DepositRequest.BankInSlipDownloadId))
               .Exists() == false)
            {
                Create.Column(nameof(DepositRequest.BankInSlipDownloadId))
                .OnTable(nameof(DepositRequest))
                .AsInt32()
                .ForeignKey(
                    "FK_DepositRequest_BankInSlipDownloadId", 
                    nameof(Download), 
                    nameof(Download.Id))
                .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
