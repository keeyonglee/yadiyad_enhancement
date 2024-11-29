using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using YadiYad.Pro.Core.Domain.Refund;
using Nop.Data.Extensions;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Data.Mapping.Builders.Refund
{
    public class RefundRequestBuilder : NopEntityBuilder<RefundRequest>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                    .WithColumn(nameof(RefundRequest.OrderItemId)).AsInt32().ForeignKey<ProOrderItem>().NotNullable()
                    // .WithColumn(nameof(RefundRequest.RefundNumber)).AsString(int.MaxValue).Unique().NotNullable()
                    .WithColumn(nameof(RefundRequest.RefundNumber)).AsString(int.MaxValue).NotNullable()
                    .WithColumn(nameof(RefundRequest.RefundTo)).AsInt32().NotNullable()
                    .WithColumn(nameof(RefundRequest.Status)).AsInt32().NotNullable().WithDefaultValue(0)
                    .WithColumn(nameof(RefundRequest.Amount)).AsDecimal().NotNullable()
                    .WithColumn(nameof(RefundRequest.ServiceCharge)).AsDecimal().NotNullable()
                    .WithColumn(nameof(RefundRequest.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                    .WithColumn(nameof(RefundRequest.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                    .WithColumn(nameof(RefundRequest.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                    .WithColumn(nameof(RefundRequest.CreatedOnUTC)).AsDateTime().NotNullable()
                    .WithColumn(nameof(RefundRequest.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}