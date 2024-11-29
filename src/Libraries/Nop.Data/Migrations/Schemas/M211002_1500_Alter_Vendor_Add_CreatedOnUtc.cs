using FluentMigrator;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/10/02 15:00:00", "Alter Vendor Add CreatedOnUtc")]
    public class M211002_1500_Alter_Vendor_Add_CreatedOnUtc : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211002_1500_Alter_Vendor_Add_CreatedOnUtc(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Vendor))
                .Column(nameof(Vendor.CreatedOnUtc))
                .Exists())
            {
                Create.Column(nameof(Vendor.CreatedOnUtc))
                    .OnTable(nameof(Vendor))
                    .AsDateTime()
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
