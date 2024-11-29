using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Data
{
    [NopMigration("2021/09/07 00:12:00", "Alter ShippingByWeightByTotalRecord")]
    public class M210907_0005_Alter_ShippingByWeightByTotalRecord : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210907_0005_Alter_ShippingByWeightByTotalRecord(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            Alter.Column(nameof(ShippingByWeightByTotalRecord.RatePerWeightUnit))
                .OnTable(nameof(ShippingByWeightByTotalRecord))
                .AsDecimal(18, 4)
                .NotNullable();
        }

        public override void Down()
        {
        }
    }
}
