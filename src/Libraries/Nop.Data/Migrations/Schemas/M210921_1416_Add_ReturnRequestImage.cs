using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/09/21 14:17:00", "Add Return Request Image")]
    public class M210921_1416_Add_ReturnRequestImage : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210921_1416_Add_ReturnRequestImage(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            _migrationManager.BuildTable<ReturnRequestImage>(Create);
        }

        public override void Down()
        {
        }
    }
}
