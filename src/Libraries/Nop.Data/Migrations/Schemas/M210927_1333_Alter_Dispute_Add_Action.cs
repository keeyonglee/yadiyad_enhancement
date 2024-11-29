using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/09/27 13:33:00", "Add Action")]
    public class M210927_1333_Alter_Dispute_Add_Action : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210927_1333_Alter_Dispute_Add_Action(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Dispute))
                .Column(nameof(Dispute.DisputeAction))
                .Exists())
            {
                Create.Column(nameof(Dispute.DisputeAction))
                    .OnTable(nameof(Dispute))
                    .AsInt32()
                    .Nullable();
            }

            if (!Schema.Table(nameof(Dispute))
                .Column(nameof(Dispute.PartialAmount))
                .Exists())
            {
                Create.Column(nameof(Dispute.PartialAmount))
                    .OnTable(nameof(Dispute))
                    .AsDecimal()
                    .Nullable();
            }

            if (!Schema.Table(nameof(Dispute))
                .Column(nameof(Dispute.RaiseClaim))
                .Exists())
            {
                Create.Column(nameof(Dispute.RaiseClaim))
                    .OnTable(nameof(Dispute))
                    .AsBoolean()
                    .NotNullable();
            }

            if (!Schema.Table(nameof(Dispute))
                .Column(nameof(Dispute.VendorId))
                .Exists())
            {
                Create.Column(nameof(Dispute.VendorId))
                    .OnTable(nameof(Dispute))
                    .AsInt32()
                    .ForeignKey(nameof(Vendor), nameof(Vendor.Id))
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
