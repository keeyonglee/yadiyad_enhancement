using FluentMigrator;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.NopStation.WebApi.Domains;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.NopStation.WebApi.Data
{
    [NopMigration("2022/03/01 23:15:00", "Alter_AppDevice_Add_AppTypeId")]
    public class M220301_2315_Alter_ApiDevice_Add_AppTypeId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220301_2315_Alter_ApiDevice_Add_AppTypeId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ApiDevice)))
                    .Column(NameCompatibilityManager.GetColumnName(typeof(ApiDevice), nameof(ApiDevice.AppTypeId)))
                    .Exists())
            {
                Create.Column(NameCompatibilityManager.GetColumnName(typeof(ApiDevice), nameof(ApiDevice.AppTypeId)))
                    .OnTable(NameCompatibilityManager.GetTableName(typeof(ApiDevice)))
                    .AsInt32()
                    .NotNullable()
                    .WithDefaultValue(10);
            }
        }

        public override void Down()
        {
        }
    }
}
