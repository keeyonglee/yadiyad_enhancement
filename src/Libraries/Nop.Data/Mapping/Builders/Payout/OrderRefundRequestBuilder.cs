using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payout;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Builders.Payout
{
    public class OrderRefundRequestBuilder : NopEntityBuilder<OrderRefundRequest>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OrderRefundRequest.OrderId)).AsInt32()
                    .ForeignKey(
                        $"FK_{nameof(OrderRefundRequest)}_{nameof(OrderRefundRequest.OrderId)}", 
                        nameof(Order),
                        nameof(Order.Id))
                .WithColumn(nameof(OrderRefundRequest.CustomerId)).AsInt32()
                .WithColumn(nameof(OrderRefundRequest.RefundStatusId)).AsInt32()
                .WithColumn(nameof(OrderRefundRequest.DocumentNumber)).AsString(50).Nullable()
                .WithColumn(nameof(OrderRefundRequest.Amount)).AsDecimal()
                .WithColumn(nameof(OrderRefundRequest.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(OrderRefundRequest.TransactionDate​)).AsDateTime().NotNullable()
                .WithColumn(nameof(OrderRefundRequest.CreatedById)).AsInt32().NotNullable()
                .WithColumn(nameof(OrderRefundRequest.UpdatedById)).AsInt32().Nullable()
                .WithColumn(nameof(OrderRefundRequest.CreatedOnUTC)).AsDateTime().NotNullable()
                .WithColumn(nameof(OrderRefundRequest.UpdatedOnUTC)).AsDateTime().Nullable(); ;
    }

        #endregion
    }
}
