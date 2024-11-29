using FluentMigrator;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/10/03 17:30:00", "Alter ProductReview Add PictureId")]
    public class M211003_1730_Alter_ProductReview_Add_PictureId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211003_1730_Alter_ProductReview_Add_PictureId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(ProductReview))
                .Column(nameof(ProductReview.PictureId))
                .Exists())
            {
                Create.Column(nameof(ProductReview.PictureId))
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
