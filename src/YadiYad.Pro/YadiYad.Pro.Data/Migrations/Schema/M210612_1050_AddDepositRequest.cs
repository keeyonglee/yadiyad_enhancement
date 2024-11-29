using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Deposit;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/12 10:20:00", "Add Deposit Request")]

    public class M210612_1050_AddDepositRequest : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210612_1050_AddDepositRequest(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            // _migrationManager.BuildTable<DepositRequest>(Create);
            if (Schema.Table(nameof(DepositRequest)).Exists() == false)
            {
                _migrationManager.BuildTable<DepositRequest>(Create);
            }
        }

        public override void Down()
        {
        }


    }
}
