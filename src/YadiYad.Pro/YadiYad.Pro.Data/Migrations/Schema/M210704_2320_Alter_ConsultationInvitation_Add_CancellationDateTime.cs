using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Consultation;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/07/04 23:20:00", "Alter ConsultationInvitation Add CancellationDateTime")]

    public class M210704_2320_Alter_ConsultationInvitation_Add_CancellationDateTime : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210704_2320_Alter_ConsultationInvitation_Add_CancellationDateTime(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ConsultationInvitation))
               .Column(nameof(ConsultationInvitation.CancellationDateTime))
               .Exists() == false)
            {
                Create.Column(nameof(ConsultationInvitation.CancellationDateTime))
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
