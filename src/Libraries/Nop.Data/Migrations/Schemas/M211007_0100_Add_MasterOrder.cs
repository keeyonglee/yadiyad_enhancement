using FluentMigrator;
using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/10/07 00:45:00", "Add MasterOrder")]
    public class M211007_0100_Add_MasterOrder : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211007_0100_Add_MasterOrder(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(MasterOrder)).Exists() == false)
            {
                _migrationManager.BuildTable<MasterOrder>(Create);
            }
        }

        public override void Down()
        {
        }
    }
}
