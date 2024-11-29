using FluentMigrator;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/11/02 11:48:00", "Alter VendorApplication Alter CategoryId")]
    public class M211102_1148_Alter_VendorApplication_Alter_CategoryId : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M211102_1148_Alter_VendorApplication_Alter_CategoryId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            Alter.Column(nameof(VendorApplication.CategoryId))
            .OnTable(nameof(VendorApplication))
            .AsInt32()
            .Nullable();
        }

        public override void Down()
        {
        }
    }
}
