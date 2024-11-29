using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using YadiYad.Pro.Core.Domain.Order;
using Nop.Data.Extensions;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Mapping.Builders.Order
{
    public class ProInvoiceBuilder : NopEntityBuilder<ProInvoice>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                    // .WithColumn(nameof(ProInvoice.InvoiceNumber)).AsString(int.MaxValue).Unique().NotNullable()
                    .WithColumn(nameof(ProInvoice.InvoiceNumber)).AsString(int.MaxValue).NotNullable()
                    .WithColumn(nameof(ProInvoice.RefType)).AsInt32().NotNullable()
                    .WithColumn(nameof(ProInvoice.InvoiceTo)).AsInt32().NotNullable()
                    .WithColumn(nameof(ProInvoice.InvoiceFrom)).AsInt32().Nullable()
                    .WithColumn(nameof(ProInvoice.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                    .WithColumn(nameof(ProInvoice.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                    .WithColumn(nameof(ProInvoice.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                    .WithColumn(nameof(ProInvoice.CreatedOnUTC)).AsDateTime().NotNullable()
                    .WithColumn(nameof(ProInvoice.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}