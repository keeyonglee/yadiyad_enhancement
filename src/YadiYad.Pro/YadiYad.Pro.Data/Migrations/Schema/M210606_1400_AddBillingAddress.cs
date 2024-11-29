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
    [NopMigration("2021/06/06 14:00:00", "add BillingAddress table")]

    public class M210606_1400_AddBillingAddress : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210606_1400_AddBillingAddress(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            // _migrationManager.BuildTable<BillingAddress>(Create);
            if (Schema.Table(nameof(BillingAddress)).Exists() == false)
            {
                _migrationManager.BuildTable<BillingAddress>(Create);
            }
        }

        public override void Down()
        {
        }


    }
}
