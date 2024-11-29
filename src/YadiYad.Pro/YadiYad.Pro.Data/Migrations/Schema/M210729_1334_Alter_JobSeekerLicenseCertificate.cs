using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/07/29 13:34:00", "Alter JobSeekerLicenseCertificate")]
    public class M210729_1334_Alter_JobSeekerLicenseCertificate : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210729_1334_Alter_JobSeekerLicenseCertificate(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (!Schema.Table(nameof(JobSeekerLicenseCertificate))
               .Column(nameof(JobSeekerLicenseCertificate.DownloadId))
               .Exists())
            {
                Create.Column(nameof(JobSeekerLicenseCertificate.DownloadId))
                    .OnTable(nameof(JobSeekerLicenseCertificate))
                    .AsInt32()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
