using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/24 12:00:00", "create tables (BlockCustomer)")]

    public class M210624_1200_Add_BlockCustomer : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M210624_1200_Add_BlockCustomer(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<BlockCustomer>(Create);
        }
    }
}
