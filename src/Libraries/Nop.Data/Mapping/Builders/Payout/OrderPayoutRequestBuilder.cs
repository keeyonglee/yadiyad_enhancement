using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Payout;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Builders.Payout
{
    public class OrderPayoutRequestBuilder : NopEntityBuilder<OrderPayoutRequest>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OrderPayoutRequest.InvoiceId))
                    .AsInt32()
                    .Nullable()
                    .ForeignKey(
                        $"FK_{nameof(OrderPayoutRequest)}_{nameof(OrderPayoutRequest.InvoiceId)}",
                        nameof(Invoice),
                        nameof(Invoice.Id));
        }

        #endregion
    }
}
