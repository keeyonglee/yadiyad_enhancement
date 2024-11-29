using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.JobSeeker;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/07/21 20:51:00", "Alter Billing Address")]

    public class M210721_2051_Alter_BillingAddress : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210721_2051_Alter_BillingAddress(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            Alter.Column(nameof(BillingAddress.Address2))
                .OnTable(nameof(BillingAddress))
                .AsString(500)
                .Nullable();
        }

        public override void Down()
        {
        }
    }
}
