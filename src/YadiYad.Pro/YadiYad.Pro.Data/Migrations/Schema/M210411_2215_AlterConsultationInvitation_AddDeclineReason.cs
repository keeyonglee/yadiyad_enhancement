using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Consultation;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/04/11 22:16:00", "alter ConsultationInvite - add decline reasons")]
    public class M210411_2215_AlterConsultationInvitation_AddDeclineReason : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;
        public M210411_2215_AlterConsultationInvitation_AddDeclineReason(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ConsultationInvitation))
                .Column(nameof(ConsultationInvitation.DeclineReasons))
                .Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.DeclineReasons))
                .OnTable(nameof(ConsultationInvitation))
                .AsString(int.MaxValue).Nullable()
                .Nullable();
            }
        }
    }
}

