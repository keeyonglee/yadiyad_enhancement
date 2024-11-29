using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.ShippingShuq;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/09/23 11:30:00", "Add VendorAttributeValue_ShippingMethod_Mapping")]
    public class M210923_1130_Add_VendorAttributeValue_ShippingMethod_Mapping : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210923_1130_Add_VendorAttributeValue_ShippingMethod_Mapping(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<VendorAttributeValue_ShippingMethod_Mapping>(Create);
        }

        public override void Down()
        {
        }
    }
}
