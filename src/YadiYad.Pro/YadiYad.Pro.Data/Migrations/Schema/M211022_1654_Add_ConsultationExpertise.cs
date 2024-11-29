using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Consultation;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/10/22 16:54:00", "Add Consultation Expertise")]
    public class M211022_1654_Add_ConsultationExpertise : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211022_1654_Add_ConsultationExpertise(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        
        public override void Up()
        {
            _migrationManager.BuildTable<ConsultationExpertise>(Create);
        }

        public override void Down()
        {
        }
    }
}