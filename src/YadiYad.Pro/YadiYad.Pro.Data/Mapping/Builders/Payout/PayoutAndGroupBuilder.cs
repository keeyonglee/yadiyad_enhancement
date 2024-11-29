using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using YadiYad.Pro.Core.Domain.Payout;

namespace YadiYad.Pro.Data.Mapping.Builders.Payout
{
    public class PayoutAndGroupBuilder : NopEntityBuilder<PayoutAndGroup>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                    .WithColumn(nameof(PayoutAndGroup.PayoutGroupId)).AsInt32().ForeignKey<PayoutGroup>().NotNullable()
                    .WithColumn(nameof(PayoutAndGroup.RefTypeId)).AsInt32().Nullable()
                    .WithColumn(nameof(PayoutAndGroup.RefId)).AsInt32()
                    .WithColumn(nameof(PayoutAndGroup.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                    .WithColumn(nameof(PayoutAndGroup.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                    .WithColumn(nameof(PayoutAndGroup.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                    .WithColumn(nameof(PayoutAndGroup.CreatedOnUTC)).AsDateTime().NotNullable()
                    .WithColumn(nameof(PayoutAndGroup.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}