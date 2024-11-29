using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/10/11 21:30:00", "Add_MinRange_MaxRange_Charges")]
    public class M211011_2130_Add_MinRange_MaxRange_Charges : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211011_2130_Add_MinRange_MaxRange_Charges(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (!Schema.Table(nameof(Charge))
               .Column(nameof(Charge.MinRange))
               .Exists())
            {
                Create.Column(nameof(Charge.MinRange))
                    .OnTable(nameof(Charge))
                    .AsDecimal(10, 2)
                    .Nullable();
            }
            if (!Schema.Table(nameof(Charge))
               .Column(nameof(Charge.MaxRange))
               .Exists())
            {
                Create.Column(nameof(Charge.MaxRange))
                    .OnTable(nameof(Charge))
                    .AsDecimal(10, 2)
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
