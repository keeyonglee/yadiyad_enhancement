using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.Payout;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/17 13:19:00", "Add Payout Batch")]

    public class M210617_1318_AddPayoutBatch : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210617_1318_AddPayoutBatch(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            // _migrationManager.BuildTable<PayoutBatch>(Create);
            if (Schema.Table(nameof(PayoutBatch)).Exists() == false)
            {
                _migrationManager.BuildTable<PayoutBatch>(Create);
            }
        }

        public override void Down()
        {
        }


    }
}
