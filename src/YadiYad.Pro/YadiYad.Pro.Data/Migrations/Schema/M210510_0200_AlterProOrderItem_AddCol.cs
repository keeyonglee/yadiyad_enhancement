using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Data.Migrations.Schema
{
    [NopMigration("2021/05/10 02:00:00", "alter ProOrderItem - add cols")]

    public class M210510_0200_AlterProOrderItem_AddCol : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210510_0200_AlterProOrderItem_AddCol(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ProOrderItem))
               .Column(nameof(ProOrderItem.InvoiceId))
               .Exists() == false)
            {
                Create.Column(nameof(ProOrderItem.InvoiceId))
                .OnTable(nameof(ProOrderItem))
                .AsString()
                .Nullable();
            }
        }

        public override void Down()
        {
        }

    }
}
