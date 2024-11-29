using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.JobSeeker;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/05/20 00:00:00", "Add Job Seeker Profile")]

    public class M210520_0000_AddJobSeekerProfile : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210520_0000_AddJobSeekerProfile(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            // _migrationManager.BuildTable<JobSeekerProfile>(Create);
            if (Schema.Table(nameof(JobSeekerProfile)).Exists() == false)
            {
                _migrationManager.BuildTable<JobSeekerProfile>(Create);
            }
            _migrationManager.BuildTable<JobSeekerCategory>(Create);
            _migrationManager.BuildTable<JobSeekerAcademicQualification>(Create);
            _migrationManager.BuildTable<JobSeekerLicenseCertificate>(Create);
            _migrationManager.BuildTable<JobSeekerLanguageProficiency>(Create);
            _migrationManager.BuildTable<JobSeekerPreferredLocation>(Create);
        }

        public override void Down()
        {
        }

    }
}
