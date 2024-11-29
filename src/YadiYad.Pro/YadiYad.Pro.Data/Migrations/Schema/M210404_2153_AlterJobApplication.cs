using FluentMigrator;
using Nop.Core.Domain.Directory;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/04/04 21:56:00", "Alter JobApplication Add Col IsEscrow")]
    public class M210404_2153_AlterJobApplication : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210404_2153_AlterJobApplication(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema
                .Table(nameof(JobApplication))
                .Column(nameof(JobApplication.IsEscrow))
                .Exists() == false)
            {
                Create
                    .Column(nameof(JobApplication.IsEscrow))
                    .OnTable(nameof(JobApplication))
                    .AsBoolean()
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
