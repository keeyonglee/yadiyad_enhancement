using FluentMigrator;
using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Migrations.Schemas
{
    [NopMigration("2022/04/26 10:35:00", "M220426_1035_Alter_Order_Add_OrderShippingDeliverySlot")]
    public class M220426_1035_Alter_Order_Add_OrderShippingDeliverySlot : Migration
    {
        private readonly IMigrationManager _migrationManager;

        public M220426_1035_Alter_Order_Add_OrderShippingDeliverySlot(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            if (Schema.Table(nameof(Order))
                 .Column(nameof(Order.OrderShippingDeliverySlot))
                 .Exists() == false)
            {
                Create.Column(nameof(Order.OrderShippingDeliverySlot))
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
