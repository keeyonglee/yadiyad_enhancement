using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2021/11/21 10:29:00", "M211121_1029_Alter_Order_Add_ReceivedDateUtc")]
    public class M211121_1029_Alter_Order_Add_ReceivedDateUtc : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M211121_1029_Alter_Order_Add_ReceivedDateUtc(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (!Schema.Table(nameof(Order))
               .Column(nameof(Order.ReceivedDateUtc))
               .Exists())
            {
                Create.Column(nameof(Order.ReceivedDateUtc))
                    .OnTable(nameof(Order))
                    .AsDateTime()
                    .Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
