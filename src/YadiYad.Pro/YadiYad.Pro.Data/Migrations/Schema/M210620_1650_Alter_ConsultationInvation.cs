using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/20 16:50:00", "Alter ConsultationInvitation add CancellationDownloadId")]

    public class M210620_1650_Alter_ConsultationInvation : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210620_1650_Alter_ConsultationInvation(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ConsultationInvitation))
               .Column(nameof(ConsultationInvitation.CancellationDownloadId))
               .Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.CancellationDownloadId))
                .OnTable(nameof(ConsultationInvitation))
                .AsInt32()
                .ForeignKey(nameof(Picture), nameof(Picture.Id))
                .Nullable();
            }

            if (Schema.Table(nameof(ServiceApplication))
               .Column(nameof(ServiceApplication.CancellationDownloadId))
               .Exists() == false)
            {
                Create.Column(nameof(ServiceApplication.CancellationDownloadId))
                .OnTable(nameof(ServiceApplication))
                .AsInt32()
                .ForeignKey(nameof(Picture), nameof(Picture.Id))
                .Nullable();
            }

            if (Schema.Table(nameof(JobApplication))
              .Column(nameof(JobApplication.CancellationDownloadId))
              .Exists() == false)
            {
                Create.Column(nameof(JobApplication.CancellationDownloadId))
                .OnTable(nameof(JobApplication))
                .AsInt32()
                .ForeignKey(nameof(Picture), nameof(Picture.Id))
                .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
