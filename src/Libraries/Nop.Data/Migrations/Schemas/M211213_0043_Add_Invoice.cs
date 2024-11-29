using FluentMigrator;
using Nop.Core.Domain.Documents;
using Nop.Core.Domain.Payout;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/12/12 12:43:00", "M211213_0043_Add_Invoice")]
    public class M211213_0043_Add_Invoice : Migration
    {
        private readonly IMigrationManager _migrationManager;
        private readonly IRepository<RunningNumber> _runningNumberRepository;

        public M211213_0043_Add_Invoice(
            IMigrationManager migrationManager,
            IRepository<RunningNumber> runningNumberRepository)
        {
            _migrationManager = migrationManager;
            _runningNumberRepository = runningNumberRepository;
        }


        public M211213_0043_Add_Invoice(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(Invoice)).Exists() == false)
            {
                _migrationManager.BuildTable<Invoice>(Create);
            }

            if (Schema.Table(nameof(OrderPayoutRequest))
                 .Column(nameof(OrderPayoutRequest.InvoiceId))
                 .Exists() == false)
            {
                Create.Column(nameof(OrderPayoutRequest.InvoiceId))
                    .OnTable(nameof(OrderPayoutRequest))
                    .AsInt32()
                    .Nullable()
                    .ForeignKey(
                        $"FK_{nameof(OrderPayoutRequest)}_{nameof(OrderPayoutRequest.InvoiceId)}",
                        nameof(Invoice),
                        nameof(Invoice.Id));
            }

        }

        public override void Down()
        {
        }
    }
}
