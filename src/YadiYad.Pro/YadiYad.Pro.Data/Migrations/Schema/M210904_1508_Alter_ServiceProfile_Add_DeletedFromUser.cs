using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/09/04 15:08:00", "Alter ServiceProfile Add DeletedFromUser")]
    public class M210829_0130_Alter_ServiceProfile_Add_TaxCol : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210829_0130_Alter_ServiceProfile_Add_TaxCol(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(ServiceProfile))
               .Column(nameof(ServiceProfile.DeletedFromUser))
               .Exists())
            {
                Create.Column(nameof(ServiceProfile.DeletedFromUser))
                    .OnTable(nameof(ServiceProfile))
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
