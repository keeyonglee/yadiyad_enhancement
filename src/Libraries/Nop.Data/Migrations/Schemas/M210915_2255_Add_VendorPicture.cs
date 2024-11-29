using FluentMigrator;
using Nop.Core.Domain.Vendors;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/09/15 22:55:00", "Add VendorPicture")]
    public class M210915_2255_Add_VendorPicture : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210915_2255_Add_VendorPicture(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            _migrationManager.BuildTable<VendorPicture>(Create);
        }

        public override void Down()
        {
        }
    }
}
