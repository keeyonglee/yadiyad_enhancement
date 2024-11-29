using FluentMigrator;
using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/09/17 11:55:00", "Alter Group Return Request, Add Insurance")]

    public class M210917_1155_Alter_GroupReturnRequest_Add_Insurance : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M210917_1155_Alter_GroupReturnRequest_Add_Insurance(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(GroupReturnRequest))
            .Column(nameof(GroupReturnRequest.IsInsuranceClaim))
            .Exists())
            {
                Create.Column(nameof(GroupReturnRequest.IsInsuranceClaim))
                    .OnTable(nameof(GroupReturnRequest))
                    .AsBoolean();
            }

            if (!Schema.Table(nameof(GroupReturnRequest))
            .Column(nameof(GroupReturnRequest.HasInsuranceCover))
            .Exists())
            {
                Create.Column(nameof(GroupReturnRequest.HasInsuranceCover))
                    .OnTable(nameof(GroupReturnRequest))
                    .AsBoolean();
            }

            if (!Schema.Table(nameof(GroupReturnRequest))
            .Column(nameof(GroupReturnRequest.InsuranceClaimAmt))
            .Exists())
            {
                Create.Column(nameof(GroupReturnRequest.InsuranceClaimAmt))
                    .OnTable(nameof(GroupReturnRequest))
                    .AsDecimal()
                    .Nullable();
            }

            if (!Schema.Table(nameof(GroupReturnRequest))
            .Column(nameof(GroupReturnRequest.InsuranceRef))
            .Exists())
            {
                Create.Column(nameof(GroupReturnRequest.InsuranceRef))
                    .OnTable(nameof(GroupReturnRequest))
                    .AsInt32()
                    .Nullable();
            }

            if (!Schema.Table(nameof(GroupReturnRequest))
            .Column(nameof(GroupReturnRequest.InsurancePayoutDate))
            .Exists())
            {
                Create.Column(nameof(GroupReturnRequest.InsurancePayoutDate))
                    .OnTable(nameof(GroupReturnRequest))
                    .AsDateTime()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
