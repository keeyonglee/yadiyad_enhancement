using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2022/02/19 22:00:00", "M220219_2200_Alter_Bank_Add_ShortName")]
    public class M220219_2200_Alter_Bank_Add_ShortName : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220219_2200_Alter_Bank_Add_ShortName(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Bank))
               .Column(nameof(Bank.ShortName))
               .Exists())
            {
                Create.Column(nameof(Bank.ShortName))
                    .OnTable(nameof(Bank))
                    .AsString(200)
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
