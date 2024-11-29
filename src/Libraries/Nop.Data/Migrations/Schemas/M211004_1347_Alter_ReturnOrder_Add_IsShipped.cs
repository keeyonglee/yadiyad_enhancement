using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/10/04 13:47:00", "Alter ReturnOrder Add IsShipped")]
    public class M211004_1347_Alter_ReturnOrder_Add_IsShipped : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211004_1347_Alter_ReturnOrder_Add_IsShipped(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(ReturnOrder))
                .Column(nameof(ReturnOrder.IsShipped))
                .Exists())
            {
                Create.Column(nameof(ReturnOrder.IsShipped))
                    .OnTable(nameof(ReturnOrder))
                    .AsBoolean()
                    .NotNullable();
            }


        }

        public override void Down()
        {
        }
    }
}
