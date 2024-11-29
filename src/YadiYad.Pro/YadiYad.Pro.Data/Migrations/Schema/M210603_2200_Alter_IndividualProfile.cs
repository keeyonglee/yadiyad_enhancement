using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.JobSeeker;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/03 22:00:00", "Alter IndividualProfile")]

    public class M210603_2200_Alter_IndividualProfile : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210603_2200_Alter_IndividualProfile(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(IndividualProfile))
               .Column(nameof(IndividualProfile.SSTRegNo))
               .Exists() == false)
            {
                Create.Column(nameof(IndividualProfile.SSTRegNo))
                .OnTable(nameof(IndividualProfile))
                .AsString()
                .Nullable();
            }
        }

        public override void Down()
        {
        }

       
    }
}
