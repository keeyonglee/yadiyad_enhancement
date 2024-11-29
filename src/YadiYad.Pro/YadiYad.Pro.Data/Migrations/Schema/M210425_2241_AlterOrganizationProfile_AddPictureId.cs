using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Organization;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/04/25 22:41:00", "alter OgranizationProfile - add Picture id")]

    public class M210425_2241_AlterOrganizationProfile_AddPictureId : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210425_2241_AlterOrganizationProfile_AddPictureId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(OrganizationProfile))
               .Column(nameof(OrganizationProfile.PictureId))
               .Exists() == false)
            {
                Create.Column(nameof(OrganizationProfile.PictureId))
                .OnTable(nameof(OrganizationProfile))
                .AsInt32().ForeignKey(nameof(Picture), nameof(Picture.Id))
                .Nullable();
            }

            if (Schema.Table(nameof(OrganizationProfile))
               .Column("LogoImage")
               .Exists() == true)
            {
                Delete.Column("LogoImage").FromTable(nameof(OrganizationProfile));
            }
        }

        public override void Down()
        {
        }
    }
}

