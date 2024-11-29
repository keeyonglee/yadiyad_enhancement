using FluentMigrator;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/09/30 17:16:00", "Alter VendorAttribute Add IsRequiredManageVendorsPermission")]
    public class M210930_1716_Alter_VendorAttribute_Add_IsRequiredManageVendorsPermission : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210930_1716_Alter_VendorAttribute_Add_IsRequiredManageVendorsPermission(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(VendorAttribute))
                .Column(nameof(VendorAttribute.IsRequiredManageVendorsPermission))
                .Exists())
            {
                Create.Column(nameof(VendorAttribute.IsRequiredManageVendorsPermission))
                    .OnTable(nameof(VendorAttribute))
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
