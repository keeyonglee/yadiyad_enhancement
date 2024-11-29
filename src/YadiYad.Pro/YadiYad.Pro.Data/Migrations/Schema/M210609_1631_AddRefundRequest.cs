using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Refund;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/09 16:31:00", "Add Refund Request")]

    public class M210609_1631_AddRefundRequest : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210609_1631_AddRefundRequest(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            // _migrationManager.BuildTable<RefundRequest>(Create);
            if (Schema.Table(nameof(RefundRequest)).Exists() == false)
            {
                _migrationManager.BuildTable<RefundRequest>(Create);
            }
        }

        public override void Down()
        {
        }


    }
}
