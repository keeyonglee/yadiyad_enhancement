using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/11/09 18:13:00", "Alter Return Request Image Delete Return Request Id")]
    public class M211109_1813_Alter_ReturnRequestImage_Delete_ReturnRequestId : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211109_1813_Alter_ReturnRequestImage_Delete_ReturnRequestId(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(ReturnRequestImage)).
                Constraint("FK_403D4A5337BA1FCA38089646A293A873E62E8693").
                Exists())
            {
                Delete.ForeignKey("FK_403D4A5337BA1FCA38089646A293A873E62E8693")
                    .OnTable(nameof(ReturnRequestImage));
            }

            if (Schema.Table(nameof(ReturnRequestImage))
                .Column("ReturnRequestId")
                .Exists())
            {
                Delete.Column("ReturnRequestId")
                    .FromTable(nameof(ReturnRequestImage));
            }
        }

        public override void Down()
        {
        }
    }
}
