using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/09/10 11:25:00", "Add Group Return Request")]
    public class M210910_1125_Add_GroupReturnRequest : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210910_1125_Add_GroupReturnRequest(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(GroupReturnRequest)).Exists() == false)
            {
                _migrationManager.BuildTable<GroupReturnRequest>(Create);
            }
        }

        public override void Down()
        {
        }
    }
}
