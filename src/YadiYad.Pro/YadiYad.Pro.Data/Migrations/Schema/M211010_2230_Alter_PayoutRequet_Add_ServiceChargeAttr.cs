using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Payout;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    
    [NopMigration("2021/10/10 22:30:00", "M211010_2230_Alter_PayoutRequet_Add_ServiceChargeAttr")]
    public class M211010_2230_Alter_PayoutRequet_Add_ServiceChargeAttr : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211010_2230_Alter_PayoutRequet_Add_ServiceChargeAttr(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(PayoutRequest))
               .Column(nameof(PayoutRequest.ServiceChargeRate))
               .Exists())
            {
                Create.Column(nameof(PayoutRequest.ServiceChargeRate))
                    .OnTable(nameof(PayoutRequest))
                    .AsDecimal()
                    .WithDefaultValue(0)
                    .NotNullable();
            }

            Alter.Column(nameof(PayoutRequest.ServiceChargeRate))
           .OnTable(nameof(PayoutRequest))
                    .AsDecimal()
                    .NotNullable();

            if (!Schema.Table(nameof(PayoutRequest))
               .Column(nameof(PayoutRequest.ServiceChargeType))
               .Exists())
            {
                Create.Column(nameof(PayoutRequest.ServiceChargeType))
                    .OnTable(nameof(PayoutRequest))
                    .AsInt16()
                    .WithDefaultValue((int)ChargeValueType.Rate)
                    .NotNullable();
            }

            Alter.Column(nameof(PayoutRequest.ServiceChargeType))
           .OnTable(nameof(PayoutRequest))
                    .AsInt16()
                    .NotNullable();
        }

        public override void Down()
        {
        }
    }
}
