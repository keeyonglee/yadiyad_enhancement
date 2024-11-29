using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Payout;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/19 13:02:00", "Alter PayoutBatch")]

    public class M210619_1302_Alter_PayoutBatch : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210619_1302_Alter_PayoutBatch(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(PayoutBatch))
               .Column(nameof(PayoutBatch.ReconFileDownloadId))
               .Exists() == false)
            {
                Create.Column(nameof(PayoutBatch.ReconFileDownloadId))
                .OnTable(nameof(PayoutBatch))
                .AsInt32()
                .ForeignKey(nameof(Download), nameof(Download.Id))
                .Nullable();
            }

            if (Schema.Table(nameof(PayoutBatch))
               .Column("ReconFileURL")
               .Exists() == true)
            {
                Delete.Column("ReconFileURL")
                .FromTable(nameof(PayoutBatch));
            }
        }

        public override void Down()
        {
        }
    }
}
