using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Individual;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/11/08 19:27:00", "M211108_1926_Alter_Customer_Add_NumberOfCancellation")]
    public class M211108_1926_Alter_Customer_Add_NumberOfCancellation : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211108_1926_Alter_Customer_Add_NumberOfCancellation(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(IndividualProfile))
               .Column(nameof(IndividualProfile.NumberOfCancellation))
               .Exists())
            {
                Create.Column(nameof(IndividualProfile.NumberOfCancellation))
                    .OnTable(nameof(IndividualProfile))
                    .AsInt32()
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
