using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/08/11 00:04:00", "Alter ServiceApplication Add CancellationDateTime")]
    public class M210811_0004_Alter_ServiceApplication_Add_CancellationDateTime : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210811_0004_Alter_ServiceApplication_Add_CancellationDateTime(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(ServiceApplication))
               .Column(nameof(ServiceApplication.CancellationDateTime))
               .Exists())
            {
                Create.Column(nameof(ServiceApplication.CancellationDateTime))
                    .OnTable(nameof(ServiceApplication))
                    .AsDateTime()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
