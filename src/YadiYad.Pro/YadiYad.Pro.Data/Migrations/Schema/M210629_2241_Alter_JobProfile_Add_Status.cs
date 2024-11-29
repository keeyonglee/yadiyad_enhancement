using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/29 22:41:00", "Alter JobProfile Add Status")]

    public class M210629_2241_Alter_JobProfile_Add_Status : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210629_2241_Alter_JobProfile_Add_Status(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(JobProfile))
               .Column(nameof(JobProfile.Status))
               .Exists() == false)
            {
                Create.Column(nameof(JobProfile.Status))
                .OnTable(nameof(JobProfile))
                .AsInt32()
                .WithDefaultValue((int)JobProfileStatus.Draft)
                .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
