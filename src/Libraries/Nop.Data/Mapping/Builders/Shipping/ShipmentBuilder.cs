using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Shipping
{
    /// <summary>
    /// Represents a shipment entity builder
    /// </summary>
    public partial class ShipmentBuilder : NopEntityBuilder<Shipment>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Shipment.OrderId)).AsInt32().ForeignKey<Order>()
                .WithColumn(nameof(Shipment.Type)).AsInt32().NotNullable()
                .WithColumn(nameof(Shipment.ReturnOrderId)).AsInt32().Nullable()
                .WithColumn(nameof(Shipment.Insurance)).AsDecimal().NotNullable()
                .WithColumn(nameof(Shipment.ShippingTotal)).AsDecimal().NotNullable()
                .WithColumn(nameof(Shipment.ShippingMethodId)).AsInt32().Nullable()
                .WithColumn(nameof(Shipment.RequireInsurance)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(Shipment.DeliveryModeId)).AsInt32().NotNullable().WithDefaultValue((int)DeliveryMode.Bike)
                .WithColumn(nameof(Shipment.RetryCount)).AsInt32().NotNullable().WithDefaultValue(0);
        }

        #endregion
    }
}