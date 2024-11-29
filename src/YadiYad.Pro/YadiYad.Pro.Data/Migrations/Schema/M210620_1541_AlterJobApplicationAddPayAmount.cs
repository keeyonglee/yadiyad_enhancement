
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
    [NopMigration("2021/06/20 15:42:00", "Alter JobApplication Add PayAmount")]

    public class M210620_1541_AlterJobApplicationAddPayAmount : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210620_1541_AlterJobApplicationAddPayAmount(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(JobApplication))
               .Column(nameof(JobApplication.PayAmount))
               .Exists() == false)
            {
                Create.Column(nameof(JobApplication.PayAmount))
                .OnTable(nameof(JobApplication))
                .AsDecimal()
                .NotNullable();
            }

            Alter.Column(nameof(JobApplication.PayAmount))
            .OnTable(nameof(JobApplication))
            .AsDecimal()
            .NotNullable();
        }

        public override void Down()
        {
        }


    }
}
