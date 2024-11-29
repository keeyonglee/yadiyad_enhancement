using FluentMigrator;
using Nop.Core.Domain.Discounts;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2022/01/14 15:51:00", "M220114_1551_Alter_Discount_AddMinimumSubTotalAmount")]
    public class M220114_1551_Alter_Discount_AddMinimumSubTotalAmount : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220114_1551_Alter_Discount_AddMinimumSubTotalAmount(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(Discount))
                 .Column(nameof(Discount.MinimmumSubTotalAmount))
                 .Exists() == false)
            {
                Create.Column(nameof(Discount.MinimmumSubTotalAmount))
                    .OnTable(nameof(Discount))
                    .AsDecimal()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
