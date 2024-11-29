using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/17 23:53:00", "Alter PayoutRequest Add ProratedWorkDuration")]

    public class M210617_2353_AlterPatyoutRequest : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210617_2353_AlterPatyoutRequest(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(PayoutRequest))
               .Column(nameof(PayoutRequest.ProratedWorkDuration))
               .Exists() == false)
            {
                Create.Column(nameof(PayoutRequest.ProratedWorkDuration))
                .OnTable(nameof(PayoutRequest))
                .AsInt32()
                .Nullable();
            }
        }

        public override void Down()
        {
        }

       
    }
}
