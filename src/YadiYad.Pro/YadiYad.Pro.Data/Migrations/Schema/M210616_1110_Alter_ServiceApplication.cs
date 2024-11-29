using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/16 11:10:00", "Alter ServiceApplication")]
    public class M210616_1110_Alter_ServiceApplication : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210616_1110_Alter_ServiceApplication(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ServiceApplication))
               .Column(nameof(ServiceApplication.CancellationReasonId))
               .Exists() == false)
            {
                Create.Column(nameof(ServiceApplication.CancellationReasonId))
                .OnTable(nameof(ServiceApplication))
                .AsInt32()
                .ForeignKey(nameof(Reason), nameof(Reason.Id))
                .Nullable();
            }

            if (Schema.Table(nameof(ServiceApplication))
               .Column(nameof(ServiceApplication.CancellationRemarks))
               .Exists() == false)
            {
                Create.Column(nameof(ServiceApplication.CancellationRemarks))
                .OnTable(nameof(ServiceApplication))
                .AsString(int.MaxValue)
                .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
