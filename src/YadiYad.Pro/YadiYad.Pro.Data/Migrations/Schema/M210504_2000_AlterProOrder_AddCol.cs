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
    [NopMigration("2021/05/03 20:00:00", "alter ProOrder - add cols")]

    public class M210504_2000_AlterProOrder_AddCol : Migration
    {
        private readonly IMigrationManager _migrationManager;
        public M210504_2000_AlterProOrder_AddCol(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (Schema.Table(nameof(ProOrder))
               .Column(nameof(ProOrder.PaymentMethodSystemName))
               .Exists() == false)
            {
                Create.Column(nameof(ProOrder.PaymentMethodSystemName))
                .OnTable(nameof(ProOrder))
                .AsString()
                .Nullable();
            }

            if (Schema.Table(nameof(ProOrder))
               .Column(nameof(ProOrder.StoreId))
               .Exists() == false)
            {
                Create.Column(nameof(ProOrder.StoreId))
                .OnTable(nameof(ProOrder))
                .AsInt32()
                .NotNullable();
            }
            if (Schema.Table(nameof(ProOrder))
               .Column(nameof(ProOrder.CustomValuesXml))
               .Exists() == false)
            {
                Create.Column(nameof(ProOrder.CustomValuesXml))
                .OnTable(nameof(ProOrder))
                .AsString()
                .Nullable();
            }

        }

        public override void Down()
        {
        }

    }
}
