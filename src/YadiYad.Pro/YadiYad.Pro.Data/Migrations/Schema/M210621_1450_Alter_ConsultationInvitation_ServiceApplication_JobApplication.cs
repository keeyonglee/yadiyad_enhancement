using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/21 14:45:00", "alter ConsultationInvitation, ServiceApplication, JobApplication")]

    public class M210621_1450_Alter_ConsultationInvitation_ServiceApplication_JobApplication : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210621_1450_Alter_ConsultationInvitation_ServiceApplication_JobApplication(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            //Delete.ForeignKey()
            //    .FromTable(nameof(ConsultationInvitation))
            //    .ForeignColumn(nameof(ConsultationInvitation.CancellationDownloadId))
            //    .ToTable(nameof(Download))
            //    .PrimaryColumn(nameof(Download.Id));

            //Delete.ForeignKey()
            //    .FromTable(nameof(ServiceApplication))
            //    .ForeignColumn(nameof(ServiceApplication.CancellationDownloadId))
            //    .ToTable(nameof(Download))
            //    .PrimaryColumn(nameof(Download.Id));

            //Delete.ForeignKey()
            //    .FromTable(nameof(JobApplication))
            //    .ForeignColumn(nameof(JobApplication.CancellationDownloadId))
            //    .ToTable(nameof(Download))
            //    .PrimaryColumn(nameof(Download.Id));

            //Alter.Column(nameof(ConsultationInvitation.CancellationDownloadId))
            //.OnTable(nameof(ConsultationInvitation))
            //.AsInt32()
            //.ForeignKey(nameof(Picture), nameof(Picture.Id))
            //.Nullable();

            //Alter.Column(nameof(ServiceApplication.CancellationDownloadId))
            //.OnTable(nameof(ServiceApplication))
            //.AsInt32()
            //.ForeignKey(nameof(Picture), nameof(Picture.Id))
            //.Nullable();

            //Alter.Column(nameof(JobApplication.CancellationDownloadId))
            //.OnTable(nameof(JobApplication))
            //.AsInt32()
            //.ForeignKey(nameof(Picture), nameof(Picture.Id))
            //.Nullable();
        }

        public override void Down()
        {
        }
    }
}
