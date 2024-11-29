using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/11/12 05:33:00", "M211112_0533_Alter_ServiceApplication_Add_HasRehired")]
    public class M211112_0533_Alter_ServiceApplication_Add_HasRehired : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211112_0533_Alter_ServiceApplication_Add_HasRehired(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(ServiceApplication))
               .Column(nameof(ServiceApplication.HasRehired))
               .Exists())
            {
                Create.Column(nameof(ServiceApplication.HasRehired))
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
