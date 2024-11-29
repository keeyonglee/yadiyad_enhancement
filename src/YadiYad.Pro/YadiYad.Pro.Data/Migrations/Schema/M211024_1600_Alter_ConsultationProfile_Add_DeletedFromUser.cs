using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Consultation;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/10/24 16:00:00", "Alter ConsultationProfile Add DeletedFromUser")]
    public class M211024_1600_Alter_ConsultationProfile_Add_DeletedFromUser : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211024_1600_Alter_ConsultationProfile_Add_DeletedFromUser(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(ConsultationProfile))
               .Column(nameof(ConsultationProfile.DeletedFromUser))
               .Exists())
            {
                Create.Column(nameof(ConsultationProfile.DeletedFromUser))
                    .OnTable(nameof(ConsultationProfile))
                    .AsBoolean()
                    .WithDefaultValue(false)
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
