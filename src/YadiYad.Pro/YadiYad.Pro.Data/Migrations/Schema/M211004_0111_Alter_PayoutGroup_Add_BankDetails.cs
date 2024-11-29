using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Payout;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    
    [NopMigration("2021/10/04 01:11:00", "M211004_0111_Alter_PayoutGroup_Add_BankDetails")]
    public class M211004_0111_Alter_PayoutGroup_Add_BankDetails : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211004_0111_Alter_PayoutGroup_Add_BankDetails(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(PayoutGroup))
               .Column(nameof(PayoutGroup.AccountHolderName))
               .Exists())
            {
                Create.Column(nameof(PayoutGroup.AccountHolderName))
                    .OnTable(nameof(PayoutGroup))
                    .AsString(512)
                    .WithDefaultValue("")
                    .NotNullable();
            }

            Alter.Column(nameof(PayoutGroup.AccountHolderName))
           .OnTable(nameof(PayoutGroup))
                    .AsString(512)
                    .NotNullable();

            if (!Schema.Table(nameof(PayoutGroup))
               .Column(nameof(PayoutGroup.AccountNumber))
               .Exists())
            {
                Create.Column(nameof(PayoutGroup.AccountNumber))
                    .OnTable(nameof(PayoutGroup))
                    .AsString(512)
                    .WithDefaultValue("")
                    .NotNullable();
            }

            Alter.Column(nameof(PayoutGroup.AccountNumber))
           .OnTable(nameof(PayoutGroup))
                    .AsString(512)
                    .NotNullable();

            if (!Schema.Table(nameof(PayoutGroup))
               .Column(nameof(PayoutGroup.BankName))
               .Exists())
            {
                Create.Column(nameof(PayoutGroup.BankName))
                    .OnTable(nameof(PayoutGroup))
                    .AsString(512)
                    .WithDefaultValue("")
                    .NotNullable();
            }

            Alter.Column(nameof(PayoutGroup.BankName))
           .OnTable(nameof(PayoutGroup))
                    .AsString(512)
                    .NotNullable();
        }

        public override void Down()
        {
        }
    }
}
