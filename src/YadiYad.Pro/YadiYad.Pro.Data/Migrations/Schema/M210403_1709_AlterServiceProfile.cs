using FluentMigrator;
using Nop.Core.Domain.Directory;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/04/03 17:10:00", "alter ServiceProfile add city, state, country")]
    public class M210403_1709_AlterServiceProfile : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;
        public M210403_1709_AlterServiceProfile(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ServiceProfile)).Column(nameof(ServiceProfile.CityId)).Exists() == false)
            {
                Create.Column(nameof(ServiceProfile.CityId))
                .OnTable(nameof(ServiceProfile))
                .AsInt32()
                .ForeignKey("FK_ServiceProfile_CityId", "City", "Id")
                .Nullable();
            }


            if (Schema.Table(nameof(ServiceProfile)).Column(nameof(ServiceProfile.StateProvinceId)).Exists() == false)
            {
                Create.Column(nameof(ServiceProfile.StateProvinceId))
                .OnTable(nameof(ServiceProfile))
                .AsInt32()
                .ForeignKey("FK_ServiceProfile_StateProvinceId", "StateProvince", "Id")
                .Nullable();
            }

            if (Schema.Table(nameof(ServiceProfile)).Column(nameof(ServiceProfile.CountryId)).Exists() == false)
            {
                Create.Column(nameof(ServiceProfile.CountryId))
                .OnTable(nameof(ServiceProfile))
                .AsInt32()
                .ForeignKey("FK_ServiceProfile_CountryId", "Country", "Id")
                .Nullable();
            }
        }
    }
}
