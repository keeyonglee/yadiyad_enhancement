using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a order entity builder
    /// </summary>
    public partial class MasterOrderBuilder : NopEntityBuilder<MasterOrder>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(MasterOrder.MasterOrderGuid)).AsGuid().NotNullable()
                .WithColumn(nameof(MasterOrder.StoreId)).AsInt32().ForeignKey<Store>().NotNullable()
                .WithColumn(nameof(MasterOrder.CustomerId)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None).NotNullable()
                .WithColumn(nameof(MasterOrder.BillingAddressId)).AsInt32().ForeignKey<Address>(onDelete: Rule.None).NotNullable()
                .WithColumn(nameof(MasterOrder.PaymentStatusId)).AsInt32().NotNullable()
                .WithColumn(nameof(MasterOrder.PaymentMethodSystemName)).AsString(100).NotNullable()
                .WithColumn(nameof(MasterOrder.CustomerCurrencyCode)).AsString(100).NotNullable()
                .WithColumn(nameof(MasterOrder.CurrencyRate)).AsDecimal().NotNullable()
                .WithColumn(nameof(MasterOrder.OrderSubtotalInclTax)).AsDecimal().NotNullable()
                .WithColumn(nameof(MasterOrder.OrderSubtotalExclTax)).AsDecimal().NotNullable()
                .WithColumn(nameof(MasterOrder.OrderSubTotalDiscountInclTax)).AsDecimal().NotNullable()
                .WithColumn(nameof(MasterOrder.OrderSubTotalDiscountExclTax)).AsDecimal().NotNullable()
                .WithColumn(nameof(MasterOrder.OrderShippingInclTax)).AsDecimal().NotNullable()
                .WithColumn(nameof(MasterOrder.OrderShippingExclTax)).AsDecimal().NotNullable()
                .WithColumn(nameof(MasterOrder.PaymentMethodAdditionalFeeInclTax)).AsDecimal().NotNullable()
                .WithColumn(nameof(MasterOrder.PaymentMethodAdditionalFeeExclTax)).AsDecimal().NotNullable()
                .WithColumn(nameof(MasterOrder.TaxRates)).AsString().NotNullable()
                .WithColumn(nameof(MasterOrder.OrderTax)).AsDecimal().NotNullable()
                .WithColumn(nameof(MasterOrder.OrderDiscount)).AsDecimal().NotNullable()
                .WithColumn(nameof(MasterOrder.TotalVendorShippingDiscount)).AsDecimal().NotNullable()
                .WithColumn(nameof(MasterOrder.TotalPlatformShippingDiscount)).AsDecimal().NotNullable()
                .WithColumn(nameof(MasterOrder.TotalPlatformSubTotalDiscount)).AsDecimal().NotNullable()
                .WithColumn(nameof(MasterOrder.OrderTotal)).AsDecimal().NotNullable()
                .WithColumn(nameof(MasterOrder.RefundedAmount)).AsDecimal().NotNullable()
                .WithColumn(nameof(MasterOrder.PaidDateUtc)).AsDateTime().Nullable()
                .WithColumn(nameof(MasterOrder.CustomValuesXml)).AsString(1000).Nullable()
                .WithColumn(nameof(MasterOrder.Deleted)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(MasterOrder.CreatedOnUtc)).AsDateTime().NotNullable()
                .WithColumn(nameof(MasterOrder.CustomOrderNumber)).AsString(int.MaxValue).NotNullable();
    }

    #endregion
}
}