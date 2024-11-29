using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.JobSeeker;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/05/23 18:00:00", "Alter JobApplication and JobInvitation")]

    public class M210523_1800_Alter_JobApplication_JobInvitation : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210523_1800_Alter_JobApplication_JobInvitation(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(JobApplication))
               .Column(nameof(JobApplication.JobSeekerProfileId))
               .Exists() == false)
            {
                Create.Column(nameof(JobApplication.JobSeekerProfileId))
                .OnTable(nameof(JobApplication))
                .AsInt32()
                .ForeignKey(nameof(JobSeekerProfile), nameof(JobSeekerProfile.Id))
                .NotNullable()
                .WithDefaultValue(9);
            }
            if (Schema.Table(nameof(JobInvitation))
               .Column(nameof(JobInvitation.JobSeekerProfileId))
               .Exists() == false)
            {
                Create.Column(nameof(JobInvitation.JobSeekerProfileId))
                .OnTable(nameof(JobInvitation))
                .AsInt32()
                .ForeignKey(nameof(JobSeekerProfile), nameof(JobSeekerProfile.Id))
                .NotNullable()
                .WithDefaultValue(9);
            }

            if (Schema.Table(nameof(JobApplication))
               .Column("ServiceProfileId")
               .Exists() == true)
            {
                Delete.Column("ServiceProfileId").FromTable(nameof(JobApplication));
            }
            if (Schema.Table(nameof(JobInvitation))
               .Column("ServiceProfileId")
               .Exists() == true)
            {
                Delete.Column("ServiceProfileId").FromTable(nameof(JobInvitation));
            }
        }

        public override void Down()
        {
        }

       
    }
}
