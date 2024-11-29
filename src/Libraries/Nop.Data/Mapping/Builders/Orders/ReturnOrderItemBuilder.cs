using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a return request entity builder
    /// </summary>
    public partial class ReturnOrderItemBuilder : NopEntityBuilder<ReturnRequest>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ReturnOrderItem.ReturnOrderId)).AsInt32().ForeignKey<ReturnOrder>()
                .WithColumn(nameof(ReturnOrderItem.ReturnRequestId)).AsInt32().ForeignKey<ReturnRequest>();
        }

        #endregion
    }
}