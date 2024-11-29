using FluentMigrator;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/11/27 11:07:00", "Alter Vendor Add Online")]
    public class M211127_1107_Alter_Vendor_Add_Online : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211127_1107_Alter_Vendor_Add_Online(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Vendor))
                .Column(nameof(Vendor.Online))
                .Exists())
            {
                Create.Column(nameof(Vendor.Online))
                    .OnTable(nameof(Vendor))
                    .AsBoolean()
                    .WithDefaultValue(true)
                    .NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}
