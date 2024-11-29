using FluentMigrator;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/10/07 11:35:00", "Add ProductReviewPictureMapping")]
    public class M211007_1135_Add_ProductReviewPictureMapping : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211007_1135_Add_ProductReviewPictureMapping(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            _migrationManager.BuildTable<ProductReviewPictureMapping>(Create);
        }

        public override void Down()
        {
        }
    }
}
