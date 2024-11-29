using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a order entity builder
    /// </summary>
    public partial class DisputeBuilder : NopEntityBuilder<Dispute>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Dispute.GroupReturnRequestId)).AsInt32().NotNullable().ForeignKey<GroupReturnRequest>()
                .WithColumn(nameof(Dispute.DisputeReasonId)).AsInt32().Nullable()
                .WithColumn(nameof(Dispute.DisputeAction)).AsInt32().Nullable()
                .WithColumn(nameof(Dispute.PartialAmount)).AsDecimal().Nullable()
                .WithColumn(nameof(Dispute.VendorId)).AsInt32().ForeignKey<Vendor>()
                .WithColumn(nameof(Dispute.RaiseClaim)).AsBoolean().NotNullable()
                .WithColumn(nameof(Dispute.IsReturnDispute)).AsBoolean().NotNullable()
                .WithColumn(nameof(Dispute.OrderId)).AsInt32().ForeignKey<Order>()
                .WithColumn(nameof(Dispute.DisputeDetail)).AsString(1000).Nullable()
                .WithColumn(nameof(Dispute.CreatedOnUtc)).AsDateTime().NotNullable()
                .WithColumn(nameof(Dispute.UpdatedOnUtc)).AsDateTime().Nullable();
        }

        #endregion
    }
}