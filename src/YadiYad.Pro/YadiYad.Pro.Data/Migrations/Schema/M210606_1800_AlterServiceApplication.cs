using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/06 18:00:00", "Alter Serviceapplication")]

    public class M210606_1800_AlterServiceApplication : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210606_1800_AlterServiceApplication(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ServiceApplication))
               .Column(nameof(ServiceApplication.ZipPostalCode))
               .Exists() == false)
            {
                Create.Column(nameof(ServiceApplication.ZipPostalCode))
                .OnTable(nameof(ServiceApplication))
                .AsString(20)
                .NotNullable();
            }

            if (Schema.Table(nameof(ServiceApplication))
               .Column(nameof(ServiceApplication.CityId))
               .Exists() == false)
            {
                Create.Column(nameof(ServiceApplication.CityId))
                .OnTable(nameof(ServiceApplication))
                .AsInt32()
                .ForeignKey(nameof(City), nameof(City.Id))
                .NotNullable()
                .WithDefaultValue(0);
            }
        }

        public override void Down()
        {
        }


    }
}
