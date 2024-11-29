using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using YadiYad.Pro.Core.Domain.Payout;

namespace YadiYad.Pro.Data.Mapping.Builders.Payout
{
    public class PayoutGroupBuilder : NopEntityBuilder<PayoutGroup>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                    .WithColumn(nameof(PayoutGroup.Amount)).AsDecimal().NotNullable()
                    .WithColumn(nameof(PayoutGroup.Status)).AsInt32().Nullable()
                    .WithColumn(nameof(PayoutGroup.PayoutTo)).AsInt32().ForeignKey<Customer>().Nullable()
                    .WithColumn(nameof(PayoutGroup.PayoutBatchId)).AsInt32().ForeignKey<PayoutBatch>().NotNullable()
                    .WithColumn(nameof(PayoutGroup.Remarks)).AsString().Nullable()
                    .WithColumn(nameof(PayoutGroup.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                    .WithColumn(nameof(PayoutGroup.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                    .WithColumn(nameof(PayoutGroup.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                    .WithColumn(nameof(PayoutGroup.CreatedOnUTC)).AsDateTime().NotNullable()
                    .WithColumn(nameof(PayoutGroup.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}