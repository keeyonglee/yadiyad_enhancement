using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/06/28 23:33:00", "Alter ProInvoice Remove OrderId")]

    public class M210628_2333_Alter_ProInvoice_RemoveOrderId : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210628_2333_Alter_ProInvoice_RemoveOrderId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ProInvoice))
               .Column("OrderId")
               .Exists() == true)
            {
                Delete.ForeignKey()
                    .FromTable(nameof(ProInvoice))
                    .ForeignColumn("OrderId")
                    .ToTable(nameof(ProOrder))
                    .PrimaryColumn(nameof(ProOrder.Id));
                Delete.Column("OrderId").FromTable(nameof(ProInvoice));
            }
        }

        public override void Down()
        {
        }
    }
}
