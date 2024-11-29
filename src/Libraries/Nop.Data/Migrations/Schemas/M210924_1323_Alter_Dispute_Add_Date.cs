using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/09/24 13:24:00", "Add Dispute")]
    public class M210924_1323_Alter_Dispute_Add_Date : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210924_1323_Alter_Dispute_Add_Date(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Dispute))
                .Column(nameof(Dispute.CreatedOnUtc))
                .Exists())
            {
                Create.Column(nameof(Dispute.CreatedOnUtc))
                    .OnTable(nameof(Dispute))
                    .AsDateTime()
                    .Nullable();
            }

            if (!Schema.Table(nameof(Dispute))
                .Column(nameof(Dispute.UpdatedOnUtc))
                .Exists())
            {
                Create.Column(nameof(Dispute.UpdatedOnUtc))
                    .OnTable(nameof(Dispute))
                    .AsDateTime()
                    .Nullable();
            }

            if (!Schema.Table(nameof(Dispute))
                .Column(nameof(Dispute.OrderId))
                .Exists())
            {
                Create.Column(nameof(Dispute.OrderId))
                    .OnTable(nameof(Dispute))
                    .AsInt32()
                    .ForeignKey(nameof(Order), nameof(Order.Id))
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
