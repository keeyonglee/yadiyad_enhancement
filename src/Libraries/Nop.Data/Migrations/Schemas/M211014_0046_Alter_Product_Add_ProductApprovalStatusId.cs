using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/10/14 00:46:00", "M211014_0046_Alter_Product_Add_ProductApprovalStatusId")]
    public class M211014_0046_Alter_Product_Add_ProductApprovalStatusId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211014_0046_Alter_Product_Add_ProductApprovalStatusId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Product))
                .Column(nameof(Product.ProductApprovalStatusId))
                .Exists())
            {
                Create.Column(nameof(Product.ProductApprovalStatusId))
                    .OnTable(nameof(Product))
                    .AsInt32()
                    .WithDefaultValue(0)
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
