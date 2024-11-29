using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/16 10:25:00", "Add Reason")]

    public class M210616_1025_Add_Reason : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210616_1025_Add_Reason(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            // _migrationManager.BuildTable<Reason>(Create);
            if (Schema.Table(nameof(Reason)).Exists() == false)
            {
                _migrationManager.BuildTable<Reason>(Create);
            }
        }

        public override void Down()
        {
        }
    }
}
