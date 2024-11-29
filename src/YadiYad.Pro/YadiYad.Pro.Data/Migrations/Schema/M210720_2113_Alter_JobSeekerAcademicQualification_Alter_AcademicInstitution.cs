using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.JobSeeker;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/07/20 21:14:00", "Alter JobSeekerAcademicQualification Alter AcademicInstitution")]

    public class M210720_2113_Alter_JobSeekerAcademicQualification_Alter_AcademicInstitution : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210720_2113_Alter_JobSeekerAcademicQualification_Alter_AcademicInstitution(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            Alter.Column(nameof(JobSeekerAcademicQualification.AcademicInstitution))
                .OnTable(nameof(JobSeekerAcademicQualification))
                .AsString()
                .Nullable();
        }

        public override void Down()
        {
        }
    }
}
