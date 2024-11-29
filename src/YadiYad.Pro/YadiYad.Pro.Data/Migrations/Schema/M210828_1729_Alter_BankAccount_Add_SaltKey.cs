using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Payout;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/08/28 17:29:00", "Alter BankAccount Add SaltKey")]
    public class M210828_1729_Alter_BankAccount_Add_SaltKey : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210828_1729_Alter_BankAccount_Add_SaltKey(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(BankAccount))
               .Column(nameof(BankAccount.SaltKey))
               .Exists())
            {
                Create.Column(nameof(BankAccount.SaltKey))
                    .OnTable(nameof(BankAccount))
                    .AsString(200)
                    .NotNullable()
                    .WithDefaultValue("");
            }
            
            Alter.Column(nameof(BankAccount.SaltKey))
                .OnTable(nameof(BankAccount))
                .AsString(200)
                .NotNullable();
        }

        public override void Down()
        {
        }
    }
}
