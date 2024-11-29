using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Consultation;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/10/22 20:37:00", "Alter Consultation Profile Add Years Of Experience")]
    public class M211022_2037_Alter_ConsultationProfile_AddYearsOfExperience : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211022_2037_Alter_ConsultationProfile_AddYearsOfExperience(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        
        public override void Up()
        {
            if (!Schema.Table(nameof(ConsultationProfile))
               .Column(nameof(ConsultationProfile.YearExperience))
               .Exists())
            {
                Create.Column(nameof(ConsultationProfile.YearExperience))
                    .OnTable(nameof(ConsultationProfile))
                    .AsInt32()
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}