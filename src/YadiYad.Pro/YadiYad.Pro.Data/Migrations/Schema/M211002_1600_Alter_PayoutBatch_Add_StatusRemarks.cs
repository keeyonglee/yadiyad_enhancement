using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Payout;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    
    [NopMigration("2021/10/02 16:00:00", "Alter PayoutBatch Add StatusRemarks")]
    public class M211002_1600_Alter_PayoutBatch_Add_StatusRemarks : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211002_1600_Alter_PayoutBatch_Add_StatusRemarks(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(PayoutBatch))
             .Column(nameof(PayoutBatch.StatusRemarks))
             .Exists())
            {
                Create.Column(nameof(PayoutBatch.StatusRemarks))
                    .OnTable(nameof(PayoutBatch))
                    .AsString(1000)
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
