using FluentMigrator;
using Nop.Core.Domain.Vendors;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/09/04 14:55:00", "Add VendorApplication")]
    public class M210904_1455_Add_VendorApplication : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210904_1455_Add_VendorApplication(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            _migrationManager.BuildTable<VendorApplication>(Create);
            _migrationManager.BuildTable<VendorApplicationSampleProductPicture>(Create);
        }

        public override void Down()
        {
        }
    }
}
