using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/10/04 11:25:00", "Alter ReturnOrder Add CreatedDate")]
    public class M211004_1125_Alter_ReturnOrder_Add_CreatedDate : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211004_1125_Alter_ReturnOrder_Add_CreatedDate(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(ReturnOrder))
                .Column(nameof(ReturnOrder.CreatedOnUtc))
                .Exists())
            {
                Create.Column(nameof(ReturnOrder.CreatedOnUtc))
                    .OnTable(nameof(ReturnOrder))
                    .AsDateTime()
                    .Nullable();
            }

            if (!Schema.Table(nameof(ReturnOrder))
                .Column(nameof(ReturnOrder.UpdatedOnUtc))
                .Exists())
            {
                Create.Column(nameof(ReturnOrder.UpdatedOnUtc))
                    .OnTable(nameof(ReturnOrder))
                    .AsDateTime()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
