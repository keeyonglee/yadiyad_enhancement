using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/09/23 12:38:00", "Add Dispute")]
    public class M210923_1237_Add_Dispute : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210923_1237_Add_Dispute(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            _migrationManager.BuildTable<Dispute>(Create);
        }

        public override void Down()
        {
        }
    }
}
