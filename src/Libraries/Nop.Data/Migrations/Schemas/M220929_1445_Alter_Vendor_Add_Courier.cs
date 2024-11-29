using FluentMigrator;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2022/09/29 14:45:00", "M220929_1445_Alter_Vendor_Add_Courier")]
    public class M220929_1445_Alter_Vendor_Add_Courier : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220929_1445_Alter_Vendor_Add_Courier(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(Vendor))
                    .Column(nameof(Vendor.CourierId))
                    .Exists() == false)
            {
                Create.Column(nameof(Vendor.CourierId))
                    .OnTable(nameof(Vendor))
                    .AsInt32()
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}