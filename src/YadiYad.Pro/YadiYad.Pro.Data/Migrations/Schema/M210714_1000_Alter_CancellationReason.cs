using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Deposit;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/07/14 10:00:00", "Alter CancellationReason")]

    public class M210714_1000_Alter_CancellationReason : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210714_1000_Alter_CancellationReason(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (!Schema.Table(nameof(Reason))
               .Column(nameof(Reason.AllowedAfterStart))
               .Exists())
            {
                Create.Column(nameof(Reason.AllowedAfterStart))
                    .OnTable(nameof(Reason))
                    .AsBoolean()
                    .WithDefaultValue(false)
                    .NotNullable();
            }

            if (!Schema.Table(nameof(Reason))
               .Column(nameof(Reason.BlameSeller))
               .Exists())
            {
                Create.Column(nameof(Reason.BlameSeller))
                    .OnTable(nameof(Reason))
                    .AsBoolean()
                    .WithDefaultValue(false)
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
