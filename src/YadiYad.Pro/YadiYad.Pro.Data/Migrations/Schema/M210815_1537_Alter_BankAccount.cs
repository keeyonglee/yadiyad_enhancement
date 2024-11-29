using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Domain.Individual;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/08/15 15:37:00", "Alter BankAccount")]
    public class M210815_1537_Alter_BankAccount : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210815_1537_Alter_BankAccount(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(BankAccount))
               .Column(nameof(BankAccount.IdentityType))
               .Exists())
            {
                Create.Column(nameof(BankAccount.IdentityType))
                    .OnTable(nameof(BankAccount))
                    .AsInt32()
                    .NotNullable();
            }

            if (!Schema.Table(nameof(BankAccount))
               .Column(nameof(BankAccount.Identity))
               .Exists())
            {
                Create.Column(nameof(BankAccount.Identity))
                    .OnTable(nameof(BankAccount))
                    .AsString(200)
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
