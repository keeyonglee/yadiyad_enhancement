using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using YadiYad.Pro.Core.Domain.Order;
using Nop.Data.Extensions;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Mapping.Builders.Order
{
    public class ProOrderItemBuilder : NopEntityBuilder<ProOrderItem>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProOrderItem.OrderId)).AsInt32().ForeignKey<ProOrder>().NotNullable()
                .WithColumn(nameof(ProOrderItem.ProductTypeId)).AsInt32().NotNullable()
                .WithColumn(nameof(ProOrderItem.RefId)).AsInt32().NotNullable()
                .WithColumn(nameof(ProOrderItem.ItemName)).AsString().NotNullable()
                .WithColumn(nameof(ProOrderItem.Quantity)).AsInt32().NotNullable()
                .WithColumn(nameof(ProOrderItem.UnitPrice)).AsDecimal().NotNullable()
                .WithColumn(nameof(ProOrderItem.Tax)).AsDecimal().NotNullable()
                .WithColumn(nameof(ProOrderItem.Price)).AsDecimal().NotNullable()
                .WithColumn(nameof(ProOrderItem.Discount)).AsDecimal().NotNullable()
                .WithColumn(nameof(ProOrderItem.Status)).AsInt32().WithDefaultValue(0).NotNullable()
                .WithColumn(nameof(ProOrderItem.OffsetProOrderItemId)).AsInt32().Nullable()
                .WithColumn(nameof(ProOrderItem.InvoiceId)).AsInt32().ForeignKey<ProInvoice>().Nullable()
                .WithColumn(nameof(ProOrderItem.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(ProOrderItem.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(ProOrderItem.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                .WithColumn(nameof(ProOrderItem.CreatedOnUTC)).AsDateTime().NotNullable()
                .WithColumn(nameof(ProOrderItem.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
