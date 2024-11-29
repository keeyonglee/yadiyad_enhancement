using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Organization;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/03/20 13:13:00", "alter ConsultationProfile table add organization profile id")]
    public class M000016_AlterConsultationProfileTable : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public M000016_AlterConsultationProfileTable(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ConsultationProfile)).Column(nameof(ConsultationProfile.OrganizationProfileId)).Exists() == false)
            {
                Create.Column(nameof(ConsultationProfile.OrganizationProfileId))
                .OnTable(nameof(ConsultationProfile))
                .AsInt32()
                .ForeignKey("FK_ConsultationProfile_OrganizationProfileId", "OrganizationProfile", "Id")
                .NotNullable()
                .WithDefaultValue(1);
            }
        }
    }
}
