using FluentMigrator;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    
    [NopMigration("2021/10/26 10:15:00", "M211026_1015_Alter_ProductReview_Add_OrderId")]
    public class M211026_1015_Alter_ProductReview_Add_OrderId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211026_1015_Alter_ProductReview_Add_OrderId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(ProductReview))
                .Column(nameof(ProductReview.OrderId))
                .Exists())
            {
                Create.Column(nameof(ProductReview.OrderId))
                    .OnTable(nameof(ProductReview))
                    .AsInt32()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
