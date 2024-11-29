using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Deposit;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/26 15:44:00", "Alter DepositRequest Add Status/ReminderCount")]

    public class M210626_1544_Alter_DepositRequest_AddStatusReminderCount : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210626_1544_Alter_DepositRequest_AddStatusReminderCount(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(DepositRequest))
               .Column(nameof(DepositRequest.Status))
               .Exists() == false)
            {
                Create.Column(nameof(DepositRequest.Status))
                .OnTable(nameof(DepositRequest))
                .AsInt32()
                .WithDefaultValue(0)
                .NotNullable();
            }
            if (Schema.Table(nameof(DepositRequest))
               .Column(nameof(DepositRequest.ReminderCount))
               .Exists() == false)
            {
                Create.Column(nameof(DepositRequest.ReminderCount))
                .OnTable(nameof(DepositRequest))
                .AsInt32()
                .WithDefaultValue(0)
                .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
