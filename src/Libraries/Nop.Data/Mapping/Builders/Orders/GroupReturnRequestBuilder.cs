using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a return request entity builder
    /// </summary>
    public partial class GroupReturnRequestBuilder : NopEntityBuilder<GroupReturnRequest>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(GroupReturnRequest.CustomerId)).AsInt32().ForeignKey<Customer>()
                .WithColumn(nameof(GroupReturnRequest.IsInsuranceClaim)).AsBoolean().NotNullable()
                .WithColumn(nameof(GroupReturnRequest.HasInsuranceCover)).AsBoolean().NotNullable()
                .WithColumn(nameof(GroupReturnRequest.InsuranceClaimAmt)).AsDecimal().Nullable()
                .WithColumn(nameof(GroupReturnRequest.NeedReturnShipping)).AsBoolean().NotNullable()
                .WithColumn(nameof(GroupReturnRequest.ApproveStatusId)).AsInt32().Nullable()
                .WithColumn(nameof(GroupReturnRequest.ApprovalDateUtc)).AsDateTime().Nullable()
                .WithColumn(nameof(GroupReturnRequest.ReturnConditionId)).AsInt32().Nullable()
                .WithColumn(nameof(GroupReturnRequest.InsuranceRef)).AsInt32().Nullable()
                .WithColumn(nameof(GroupReturnRequest.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(GroupReturnRequest.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                .WithColumn(nameof(GroupReturnRequest.InsurancePayoutDate)).AsDateTime().Nullable()
                .WithColumn(nameof(GroupReturnRequest.CreatedOnUtc)).AsDateTime().NotNullable()
                .WithColumn(nameof(GroupReturnRequest.UpdatedOnUtc)).AsDateTime().Nullable();
        }

        #endregion
    }
}