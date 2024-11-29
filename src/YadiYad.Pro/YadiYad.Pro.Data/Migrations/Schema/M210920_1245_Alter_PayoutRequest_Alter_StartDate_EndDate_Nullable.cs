using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Payout;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    
    [NopMigration("2021/09/20 12:45:00", "Alter_PayoutRequest_Alter_StartDate_EndDate_Nullable")]
    public class M210920_1245_Alter_PayoutRequest_Alter_StartDate_EndDate_Nullable : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210920_1245_Alter_PayoutRequest_Alter_StartDate_EndDate_Nullable(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {

            Alter.Column(nameof(PayoutRequest.StartDate))
                .OnTable(nameof(PayoutRequest))
                .AsDateTime()
                .Nullable();

            Alter.Column(nameof(PayoutRequest.EndDate))
           .OnTable(nameof(PayoutRequest))
           .AsDateTime()
           .Nullable();
        }

        public override void Down()
        {
        }
    }
}
