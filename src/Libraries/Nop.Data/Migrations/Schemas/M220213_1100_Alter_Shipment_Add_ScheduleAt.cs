using FluentMigrator;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2022/02/13 11:00:00", "M220213_1100_Alter_Shipment_Add_ScheduleAt")]
    public  class M220213_1100_Alter_Shipment_Add_ScheduleAt : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220213_1100_Alter_Shipment_Add_ScheduleAt(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(Shipment))
                 .Column(nameof(Shipment.ScheduleAt))
                 .Exists() == false)
            {
                Create.Column(nameof(Shipment.ScheduleAt))
                    .OnTable(nameof(Shipment))
                    .AsString()
                    .Nullable();
            }

            Alter.Column(nameof(Shipment.ScheduleAt))
                .OnTable(nameof(Shipment))
                .AsString()
                .Nullable();
        }

        public override void Down()
        {
        }
    }
}
