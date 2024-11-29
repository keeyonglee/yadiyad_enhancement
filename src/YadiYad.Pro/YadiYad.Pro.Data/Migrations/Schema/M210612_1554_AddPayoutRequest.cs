using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/12 15:54:00", "Add Payout Request")]

    public class M210612_1554_AddPayoutRequest : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210612_1554_AddPayoutRequest(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            // _migrationManager.BuildTable<PayoutRequest>(Create);
            if (Schema.Table(nameof(JobApplicationMilestone)).Exists() == false)
            {
                _migrationManager.BuildTable<JobApplicationMilestone>(Create);
            }
            if (Schema.Table(nameof(PayoutRequest)).Exists() == false)
            {
                _migrationManager.BuildTable<PayoutRequest>(Create);
            }
        }

        public override void Down()
        {
        }


    }
}
