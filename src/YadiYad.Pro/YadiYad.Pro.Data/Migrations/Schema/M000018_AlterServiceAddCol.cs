using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{

    [NopMigration("2021/03/20 17:49:00", "alter ServiceProfile table add col rating")]
    public class M000018_AlterServiceAddCol : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M000018_AlterServiceAddCol(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ServiceProfile)).Column(nameof(ServiceProfile.Rating)).Exists() == false)
            {
                Create.Column(nameof(ServiceProfile.Rating))
                .OnTable(nameof(ServiceProfile))
                .AsDecimal().NotNullable().WithDefaultValue(0);
            }
        }
    }
}
