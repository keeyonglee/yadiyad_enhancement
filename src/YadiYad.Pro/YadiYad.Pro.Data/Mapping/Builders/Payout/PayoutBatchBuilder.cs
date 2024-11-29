using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using YadiYad.Pro.Core.Domain.Payout;
using Nop.Core.Domain.Media;

namespace YadiYad.Pro.Data.Mapping.Builders.Payout
{
    public class PayoutBatchBuilder : NopEntityBuilder<PayoutBatch>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                    .WithColumn(nameof(PayoutBatch.GeneratedDateTime)).AsDateTime().NotNullable()
                    .WithColumn(nameof(PayoutBatch.DownloadDateTime)).AsDateTime().Nullable()
                    .WithColumn(nameof(PayoutBatch.ReconDateTime)).AsDateTime().Nullable()
                    .WithColumn(nameof(PayoutBatch.Status)).AsInt32().NotNullable()
                    .WithColumn(nameof(PayoutBatch.ReconFileDownloadId)).AsInt32().ForeignKey<Download>().Nullable()
                    .WithColumn(nameof(PayoutBatch.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                    .WithColumn(nameof(PayoutBatch.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                    .WithColumn(nameof(PayoutBatch.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                    .WithColumn(nameof(PayoutBatch.CreatedOnUTC)).AsDateTime().NotNullable()
                    .WithColumn(nameof(PayoutBatch.UpdatedOnUTC)).AsDateTime().Nullable()
                    .WithColumn(nameof(PayoutBatch.PlatformId)).AsInt32().NotNullable();
        }

        #endregion
    }
}
