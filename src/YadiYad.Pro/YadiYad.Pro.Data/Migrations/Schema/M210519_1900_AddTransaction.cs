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
    [NopMigration("2021/05/19 19:00:00", "add Transaction table")]

    public class M210519_1900_AddTransaction : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210519_1900_AddTransaction(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            // _migrationManager.BuildTable<Transaction>(Create);
            if (Schema.Table(nameof(Transaction)).Exists() == false)
            {
                _migrationManager.BuildTable<Transaction>(Create);
            }
        }

        public override void Down()
        {
        }


    }
}
