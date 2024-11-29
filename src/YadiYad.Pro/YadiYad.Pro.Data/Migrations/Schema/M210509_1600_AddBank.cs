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
    [NopMigration("2021/05/09 17:00:00", "add Bank table")]

    public class M210509_1600_AddBank : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210509_1600_AddBank(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            // _migrationManager.BuildTable<Bank>(Create);
            if (Schema.Table(nameof(Bank)).Exists() == false)
            {
                _migrationManager.BuildTable<Bank>(Create);
            }
        }

        public override void Down()
        {
        }


    }
}
