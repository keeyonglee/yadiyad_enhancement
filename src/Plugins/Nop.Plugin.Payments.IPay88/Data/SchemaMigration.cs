using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Payments.IPay88.Domain;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Data
{
    [NopMigration("2021/05/04 22:40:55:1687541", "IPay88 base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<IPay88PaymentRecord>(Create);
        }
    }
}