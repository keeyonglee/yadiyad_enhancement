using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/11/14 14:03:00", "M211114_1403_Alter_ServiceApplication_Add_RehiredId")]
    public class M211114_1403_Alter_ServiceApplication_Add_RehiredId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211114_1403_Alter_ServiceApplication_Add_RehiredId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(ServiceApplication))
               .Column(nameof(ServiceApplication.RehiredServiceApplicationId))
               .Exists())
            {
                Create.Column(nameof(ServiceApplication.RehiredServiceApplicationId))
                    .OnTable(nameof(ServiceApplication))
                    .AsInt32()
                    .Nullable();
            }

            if (!Schema.Table(nameof(ServiceApplication))
                .Column(nameof(ServiceApplication.HiredTime))
                .Exists())
            {
                Create.Column(nameof(ServiceApplication.HiredTime))
                    .OnTable(nameof(ServiceApplication))
                    .AsDateTime()
                    .Nullable();
            }

            if (!Schema.Table(nameof(ServiceApplication))
                .Column(nameof(ServiceApplication.HasCancelledTwice))
                .Exists())
            {
                Create.Column(nameof(ServiceApplication.HasCancelledTwice))
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
