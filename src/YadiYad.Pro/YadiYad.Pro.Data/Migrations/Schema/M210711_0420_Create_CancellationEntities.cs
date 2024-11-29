using FluentMigrator;
using Nop.Data.Migrations;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/07/11 04:20:00", "Create_CancellationEntities")]

    public class M210711_0420_Create_CancellationEntities : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210711_0420_Create_CancellationEntities(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<CancellationDefinition>(Create);
            _migrationManager.BuildTable<CancellationPayout>(Create);
            _migrationManager.BuildTable<CancellationRequest>(Create);

            if (!Schema.Table(nameof(Reason))
               .Column(nameof(Reason.OptionalRehireOrRefund))
               .Exists())
            {
                Create.Column(nameof(Reason.OptionalRehireOrRefund))
                    .OnTable(nameof(Reason))
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
