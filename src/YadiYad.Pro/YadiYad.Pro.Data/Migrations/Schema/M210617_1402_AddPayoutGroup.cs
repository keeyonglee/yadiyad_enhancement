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
    [NopMigration("2021/06/17 14:02:00", "Add Payout Group")]

    public class M210617_1402_AddPayoutGroup : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210617_1402_AddPayoutGroup(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<PayoutGroup>(Create);
        }

        public override void Down()
        {
        }

       
    }
}
