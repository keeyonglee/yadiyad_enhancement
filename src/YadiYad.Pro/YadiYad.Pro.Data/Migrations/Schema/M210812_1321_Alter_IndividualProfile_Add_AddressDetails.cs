using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Domain.Individual;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/08/12 13:21:00", "Alter IndividualProfile Add AddressDetails")]
    public class M210812_1321_Alter_IndividualProfile_Add_AddressDetails : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210812_1321_Alter_IndividualProfile_Add_AddressDetails(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(IndividualProfile))
               .Column(nameof(IndividualProfile.Address1))
               .Exists())
            {
                Create.Column(nameof(IndividualProfile.Address1))
                    .OnTable(nameof(IndividualProfile))
                    .AsString(500)
                    .NotNullable();
            }

            if (!Schema.Table(nameof(IndividualProfile))
               .Column(nameof(IndividualProfile.Address2))
               .Exists())
            {
                Create.Column(nameof(IndividualProfile.Address2))
                    .OnTable(nameof(IndividualProfile))
                    .AsString(500)
                    .Nullable();
            }

            if (!Schema.Table(nameof(IndividualProfile))
               .Column(nameof(IndividualProfile.ZipPostalCode))
               .Exists())
            {
                Create.Column(nameof(IndividualProfile.ZipPostalCode))
                    .OnTable(nameof(IndividualProfile))
                    .AsString(20)
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
