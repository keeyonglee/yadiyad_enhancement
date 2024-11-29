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
    [NopMigration("2021/07/29 10:28:00", "Alter ServiceLicenseCertificate")]
    public class M210729_1028_Alter_ServiceLicenseCertificate : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210729_1028_Alter_ServiceLicenseCertificate(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (!Schema.Table(nameof(ServiceLicenseCertificate))
               .Column(nameof(ServiceLicenseCertificate.DownloadId))
               .Exists())
            {
                Create.Column(nameof(ServiceLicenseCertificate.DownloadId))
                    .OnTable(nameof(ServiceLicenseCertificate))
                    .AsInt32()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
