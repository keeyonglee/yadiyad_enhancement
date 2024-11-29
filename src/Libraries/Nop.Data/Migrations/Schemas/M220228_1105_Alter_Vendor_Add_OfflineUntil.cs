using FluentMigrator;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2022/02/28 11:05:00", "M220228_1105_Alter_Vendor_Add_OfflineUntil")]
    public class M220228_1105_Alter_Vendor_Add_OfflineUntil : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220228_1105_Alter_Vendor_Add_OfflineUntil(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(Vendor))
                 .Column(nameof(Vendor.OfflineUntil))
                 .Exists() == false)
            {
                Create.Column(nameof(Vendor.OfflineUntil))
                    .OnTable(nameof(Vendor))
                    .AsDateTime()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
