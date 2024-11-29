using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/11/16 19:20:00", "Alter Address Alter LatLong")]
    public class M211116_1920_Alter_Address_Alter_LatLong : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211116_1920_Alter_Address_Alter_LatLong(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            Alter.Column(nameof(Address.Latitude))
                .OnTable(nameof(Address))
                .AsDecimal(19, 6)
                .Nullable();

            Alter.Column(nameof(Address.Longitude))
                .OnTable(nameof(Address))
                .AsDecimal(19, 6)
                .Nullable();

        }

        public override void Down()
        {
        }
    }
}
