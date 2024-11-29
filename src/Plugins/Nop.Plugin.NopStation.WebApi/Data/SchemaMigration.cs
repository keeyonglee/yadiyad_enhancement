using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.NopStation.WebApi.Domains;

namespace Nop.Plugin.NopStation.WebApi.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/06/08 08:30:55:1687541", "NopStation.WebApi base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<ApiSlider>(Create);
            _migrationManager.BuildTable<ApiCategoryIcon>(Create);
            _migrationManager.BuildTable<ApiDevice>(Create);
            _migrationManager.BuildTable<ApiStringResource>(Create);
        }
    }
}
