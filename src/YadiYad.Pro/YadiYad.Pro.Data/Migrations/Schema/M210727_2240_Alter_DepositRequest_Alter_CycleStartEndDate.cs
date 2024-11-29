using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.JobSeeker;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/07/27 22:20:00", "Alter DepositRequest Alter Cycle Start End Date")]

    public class M210727_2240_Alter_DepositRequest_Alter_CycleStartEndDate : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210727_2240_Alter_DepositRequest_Alter_CycleStartEndDate(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            Alter.Column(nameof(DepositRequest.CycleStart))
                .OnTable(nameof(DepositRequest))
                .AsDateTime()
                .Nullable();

            Alter.Column(nameof(DepositRequest.CycleEnd))
                .OnTable(nameof(DepositRequest))
                .AsDateTime()
                .Nullable();
        }

        public override void Down()
        {
        }
    }
}
