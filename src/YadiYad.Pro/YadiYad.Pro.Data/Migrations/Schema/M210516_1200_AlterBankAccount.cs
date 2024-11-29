using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Individual;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/05/16 12:00:00", "alter BankAccount - add IsVerified nullable")]

    public class M210516_1200_AlterBankAccount : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210516_1200_AlterBankAccount(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            Alter.Column(nameof(BankAccount.IsVerified))
            .OnTable(nameof(BankAccount))
            .AsBoolean()
            .Nullable();
        }

        public override void Down()
        {
        }


    }
}
