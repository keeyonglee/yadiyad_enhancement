using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Payments.IPay88.Domain;
using Nop.Data.Extensions;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Data
{
    public class IPay88PaymentRecordBuilder : NopEntityBuilder<IPay88PaymentRecord>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {

            table
                .WithColumn(nameof(IPay88PaymentRecord.PaymentNo)).AsString().Nullable()
                .WithColumn(nameof(IPay88PaymentRecord.Signature)).AsString().Nullable()
                .WithColumn(nameof(IPay88PaymentRecord.SignatureType)).AsString().Nullable()
                .WithColumn(nameof(IPay88PaymentRecord.Amount)).AsDecimal().Nullable()
                .WithColumn(nameof(IPay88PaymentRecord.CurrencyCode)).AsString().Nullable()
                .WithColumn(nameof(IPay88PaymentRecord.ErrorDesc)).AsString().Nullable()
                .WithColumn(nameof(IPay88PaymentRecord.ProdDesc)).AsString().Nullable()
                .WithColumn(nameof(IPay88PaymentRecord.UserName)).AsString().Nullable()
                .WithColumn(nameof(IPay88PaymentRecord.UserEmail)).AsString().Nullable()
                .WithColumn(nameof(IPay88PaymentRecord.UserContact)).AsString().Nullable()
                .WithColumn(nameof(IPay88PaymentRecord.Remark)).AsString().Nullable()
                .WithColumn(nameof(IPay88PaymentRecord.Status)).AsString().Nullable()
                .WithColumn(nameof(IPay88PaymentRecord.StoreId)).AsInt32().NotNullable()
                .WithColumn(nameof(IPay88PaymentRecord.UniqueId)).AsInt32().NotNullable()
                .WithColumn(nameof(IPay88PaymentRecord.CreatedBy)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(IPay88PaymentRecord.CreatedDateTime)).AsDateTime().NotNullable()
                .WithColumn(nameof(IPay88PaymentRecord.ModifiedBy)).AsInt32().ForeignKey<Customer>().Nullable()
                .WithColumn(nameof(IPay88PaymentRecord.ModifiedDateTime)).AsDateTime().Nullable()
                .WithColumn(nameof(IPay88PaymentRecord.OrderTypeId)).AsInt32().NotNullable();
        }
    }
}