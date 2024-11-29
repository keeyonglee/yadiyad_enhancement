using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Core.Domain.Refund;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/11/27 20:20:00", "M211127_2020_Alter_PayoutBatch_Add_PlatformId")]
    public class M211127_2020_Alter_PayoutBatch_Add_PlatformId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211127_2020_Alter_PayoutBatch_Add_PlatformId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(PayoutBatch))
               .Column(nameof(PayoutBatch.PlatformId))
               .Exists())
            {
                Create.Column(nameof(PayoutBatch.PlatformId))
                    .OnTable(nameof(PayoutBatch))
                    .AsInt32()
                    .WithDefaultValue((int)Platform.Pro);
            }

            if (Schema.Table(nameof(PayoutBatch))
               .Column(nameof(PayoutBatch.PlatformId))
               .Exists())
            {
                Alter.Column(nameof(PayoutBatch.PlatformId))
                    .OnTable(nameof(PayoutBatch))
                    .AsInt32();
            }
        }

        public override void Down()
        {
        }
    }
}
