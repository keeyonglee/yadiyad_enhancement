using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/09/15 00:36:00", "Alter SpecificationAttributeOption Add PictureId")]
    public class M210915_0036_Alter_SpecificationAttributeOption_Add_PictureId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210915_0036_Alter_SpecificationAttributeOption_Add_PictureId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(SpecificationAttributeOption))
               .Column(nameof(SpecificationAttributeOption.PictureId))
               .Exists())
            {
                Create.Column(nameof(SpecificationAttributeOption.PictureId))
                    .OnTable(nameof(SpecificationAttributeOption))
                    .AsInt32()
                    .ForeignKey(
                        $"{nameof(SpecificationAttributeOption)}_{nameof(SpecificationAttributeOption.PictureId)}",
                        nameof(Picture), 
                        nameof(Picture.Id))
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
