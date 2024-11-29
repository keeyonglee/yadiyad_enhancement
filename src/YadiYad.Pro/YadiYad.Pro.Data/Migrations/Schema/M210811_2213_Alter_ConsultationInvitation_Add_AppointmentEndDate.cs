using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/08/11 22:13:00", "Alter ConsultationInvitation Add AppointmentEndDate")]
    public class M210811_2213_Alter_ConsultationInvitation_Add_AppointmentEndDate : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210811_2213_Alter_ConsultationInvitation_Add_AppointmentEndDate(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(ConsultationInvitation))
               .Column(nameof(ConsultationInvitation.AppointmentEndDate))
               .Exists())
            {
                Create.Column(nameof(ConsultationInvitation.AppointmentEndDate))
                    .OnTable(nameof(ConsultationInvitation))
                    .AsDateTime()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
