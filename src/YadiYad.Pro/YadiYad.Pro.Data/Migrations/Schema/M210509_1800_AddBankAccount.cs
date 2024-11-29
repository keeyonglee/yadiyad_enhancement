using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/05/09 18:00:00", "add BankAccount table")]

    public class M210509_1800_AddBankAccount : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210509_1800_AddBankAccount(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            // _migrationManager.BuildTable<BankAccount>(Create);
            if (Schema.Table(nameof(BankAccount)).Exists() == false)
            {
                _migrationManager.BuildTable<BankAccount>(Create);
            }
        }

        public override void Down()
        {
        }


    }
}
