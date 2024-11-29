using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a return request entity builder
    /// </summary>
    public partial class ReturnOrderBuilder : NopEntityBuilder<ReturnRequest>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ReturnOrder.GroupReturnRequestId)).AsInt32().ForeignKey<GroupReturnRequest>()
                .WithColumn(nameof(ReturnOrder.EstimatedShippingExclTax)).AsDecimal().Nullable()
                .WithColumn(nameof(ReturnOrder.EstimatedShippingInclTax)).AsDecimal().Nullable()
                .WithColumn(nameof(ReturnOrder.ActualShippingExclTax)).AsDecimal().Nullable()
                .WithColumn(nameof(ReturnOrder.ActualShippingInclTax)).AsDecimal().Nullable()
                .WithColumn(nameof(ReturnOrder.IsShipped)).AsBoolean().NotNullable()
                .WithColumn(nameof(ReturnOrder.CreatedOnUtc)).AsDateTime().Nullable()
                .WithColumn(nameof(ReturnOrder.UpdatedOnUtc)).AsDateTime().Nullable();
        }

        #endregion
    }
}