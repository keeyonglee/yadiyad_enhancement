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
    [NopMigration("2021/06/17 16:36:00", "Add Payout And Group")]

    public class M210617_1636_AddPayoutAndGroup : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210617_1636_AddPayoutAndGroup(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<PayoutAndGroup>(Create);
        }

        public override void Down()
        {
        }

       
    }
}
