using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using YadiYad.Pro.Core.Domain.Order;
using Nop.Data.Extensions;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Mapping.Builders.Order
{
    public class CancellationPayoutBuilder : NopEntityBuilder<CancellationPayout>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CancellationPayout.CancellationDefinitionId)).AsInt32().NotNullable()
                .WithColumn(nameof(CancellationPayout.EngagementParty)).AsInt32().NotNullable()
                .WithColumn(nameof(CancellationPayout.ChargeValueType)).AsInt32().NotNullable()
                .WithColumn(nameof(CancellationPayout.Value)).AsDecimal().NotNullable()
                .WithColumn(nameof(CancellationPayout.HasProcessingCharge)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(CancellationPayout.RefundOnCharge)).AsInt32().NotNullable();
        }
        #endregion
    }
}