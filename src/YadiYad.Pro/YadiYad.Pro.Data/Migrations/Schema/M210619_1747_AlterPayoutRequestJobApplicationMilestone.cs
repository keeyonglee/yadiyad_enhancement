using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/19 17:56:00", "Alter PayoutRequest Add JobApplicationMilestone")]

    public class M210619_1747_AlterPayoutRequestJobApplicationMilestone : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210619_1747_AlterPayoutRequestJobApplicationMilestone(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(JobApplicationMilestone)).Exists() == false)
            {
                _migrationManager.BuildTable<JobApplicationMilestone>(Create);
            }

            if (Schema.Table(nameof(PayoutRequest))
               .Column(nameof(PayoutRequest.JobApplicationMilestoneId))
               .Exists() == false)
            {
                Create.Column(nameof(PayoutRequest.JobApplicationMilestoneId))
                .OnTable(nameof(PayoutRequest))
                .AsInt32()
                .ForeignKey(nameof(JobApplicationMilestone), nameof(JobApplicationMilestone.Id))
                .Nullable();
            }
        }

        public override void Down()
        {
        }


    }
}
