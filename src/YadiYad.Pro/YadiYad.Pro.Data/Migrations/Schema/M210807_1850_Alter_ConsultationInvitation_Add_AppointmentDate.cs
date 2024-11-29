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
    [NopMigration("2021/08/07 18:50:00", "Alter ConsultationInvitation Add Apointment Date")]
    public class M210807_1850_Alter_ConsultationInvitation_Add_AppointmentDate : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210807_1850_Alter_ConsultationInvitation_Add_AppointmentDate(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(ConsultationInvitation))
               .Column(nameof(ConsultationInvitation.AppointmentStartDate))
               .Exists())
            {
                Create.Column(nameof(ConsultationInvitation.AppointmentStartDate))
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
