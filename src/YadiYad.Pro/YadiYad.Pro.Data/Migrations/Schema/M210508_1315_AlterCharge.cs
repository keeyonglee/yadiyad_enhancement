using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/05/08 13:15:00", "alter Charge - add EndDate nullable")]

    public class M210508_1315_AlterCharge : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210508_1315_AlterCharge(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            Alter.Column(nameof(Charge.EndDate))
            .OnTable(nameof(Charge))
            .AsDateTime()
            .Nullable();
        }

        public override void Down()
        {
        }

       
    }
}
