using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using YadiYad.Pro.Core.Domain.Order;
using Nop.Data.Extensions;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Mapping.Builders.Order
{
    public class ProOrderBuilder : NopEntityBuilder<ProOrder>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProOrder.CustomerId)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(ProOrder.OrderStatusId)).AsInt32().NotNullable()
                .WithColumn(nameof(ProOrder.PaymentStatusId)).AsInt32().NotNullable()
                .WithColumn(nameof(ProOrder.OrderTax)).AsDecimal().NotNullable()
                .WithColumn(nameof(ProOrder.OrderDiscount)).AsDecimal().NotNullable()
                .WithColumn(nameof(ProOrder.OrderTotal)).AsDecimal().NotNullable()
                .WithColumn(nameof(ProOrder.RefundedAmount)).AsDecimal().NotNullable()
                .WithColumn(nameof(ProOrder.PaidOnUTC)).AsDateTime().Nullable()
                .WithColumn(nameof(ProOrder.CustomOrderNumber)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(ProOrder.MoreInfo)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(ProOrder.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(ProOrder.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(ProOrder.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                .WithColumn(nameof(ProOrder.CreatedOnUTC)).AsDateTime().NotNullable()
                .WithColumn(nameof(ProOrder.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
