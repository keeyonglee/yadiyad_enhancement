using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a return request entity builder
    /// </summary>
    public partial class ReturnRequestBuilder : NopEntityBuilder<ReturnRequest>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ReturnRequest.GroupReturnRequestId)).AsInt32().ForeignKey<GroupReturnRequest>()
                .WithColumn(nameof(ReturnRequest.ReturnRequestStatusId)).AsInt32().NotNullable()
                .WithColumn(nameof(ReturnRequest.ReasonForReturn)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(ReturnRequest.RequestedAction)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(ReturnRequest.ActionAfterReturn)).AsInt32().Nullable()
                .WithColumn(nameof(ReturnRequest.CustomerId)).AsInt32().ForeignKey<Customer>();
        }

        #endregion
    }
}