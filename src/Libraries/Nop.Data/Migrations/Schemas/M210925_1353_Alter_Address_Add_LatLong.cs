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
    [NopMigration("2021/09/25 13:53:00", "Alter Address Add LatLong")]
    public class M210925_1353_Alter_Address_Add_LatLong : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210925_1353_Alter_Address_Add_LatLong(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Address))
                .Column(nameof(Address.Latitude))
                .Exists())
            {
                Create.Column(nameof(Address.Latitude))
                    .OnTable(nameof(Address))
                    .AsDecimal(19, 7)
                    .Nullable();
            }

            if (!Schema.Table(nameof(Address))
                .Column(nameof(Address.Longitude))
                .Exists())
            {
                Create.Column(nameof(Address.Longitude))
                    .OnTable(nameof(Address))
                    .AsDecimal(19, 7)
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
