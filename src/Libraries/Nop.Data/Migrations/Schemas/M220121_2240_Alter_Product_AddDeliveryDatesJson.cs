using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2022/01/21 22:40:00", "M220121_2240_Alter_Product_AddDeliveryDatesJson")]
    public class M220121_2240_Alter_Product_AddDeliveryDatesJson : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220121_2240_Alter_Product_AddDeliveryDatesJson(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(Product))
                 .Column(nameof(Product.DeliveryDatesJson))
                 .Exists() == false)
            {
                Create.Column(nameof(Product.DeliveryDatesJson))
                    .OnTable(nameof(Product))
                    .AsString()
                    .Nullable();
            }


            Alter.Column(nameof(Product.DeliveryDatesJson))
                .OnTable(nameof(Product))
                .AsString(int.MaxValue)
                .Nullable();
        }

        public override void Down()
        {
        }
    }
}
