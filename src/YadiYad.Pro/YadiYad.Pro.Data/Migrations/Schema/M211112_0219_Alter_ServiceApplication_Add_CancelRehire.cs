using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/11/12 02:19:00", "M211112_0219_Alter_ServiceApplication_Add_CancelRehire")]
    public class M211112_0219_Alter_ServiceApplication_Add_CancelRehire : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211112_0219_Alter_ServiceApplication_Add_CancelRehire(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(ServiceApplication))
               .Column(nameof(ServiceApplication.CancelRehire))
               .Exists())
            {
                Create.Column(nameof(ServiceApplication.CancelRehire))
                    .OnTable(nameof(ServiceApplication))
                    .AsBoolean()
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
