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
    [NopMigration("2021/06/17 23:24:00", "Alter ServiceApplication Add EndDate")]

    public class M210617_2324_AlterServiceApplication : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210617_2324_AlterServiceApplication(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ServiceApplication))
               .Column(nameof(ServiceApplication.EndDate))
               .Exists() == false)
            {
                Create.Column(nameof(ServiceApplication.EndDate))
                .OnTable(nameof(ServiceApplication))
                .AsDateTime()
                .Nullable();
            }
        }

        public override void Down()
        {
        }

       
    }
}
