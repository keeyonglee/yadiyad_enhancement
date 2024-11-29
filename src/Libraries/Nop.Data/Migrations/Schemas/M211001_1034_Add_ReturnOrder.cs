using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/10/01 10:35:00", "Add Return Order")]
    public class M211001_1034_Add_ReturnOrder : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211001_1034_Add_ReturnOrder(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            _migrationManager.BuildTable<ReturnOrder>(Create);
            _migrationManager.BuildTable<ReturnOrderItem>(Create);
        }

        public override void Down()
        {
        }
    }
}
