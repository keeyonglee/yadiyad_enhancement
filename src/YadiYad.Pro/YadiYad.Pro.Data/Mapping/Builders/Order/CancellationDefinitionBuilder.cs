using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using YadiYad.Pro.Core.Domain.Order;
using Nop.Data.Extensions;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Mapping.Builders.Order
{
    public class CancellationDefinitionBuilder : NopEntityBuilder<CancellationDefinition>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CancellationDefinition.EngagementId)).AsInt32().NotNullable()
                .WithColumn(nameof(CancellationDefinition.EngagementType)).AsInt32().NotNullable()
                .WithColumn(nameof(CancellationDefinition.ChargeValueType)).AsInt32().NotNullable()
                .WithColumn(nameof(CancellationDefinition.BasedOnCondition)).AsInt32().NotNullable()
                .WithColumn(nameof(CancellationDefinition.BlockCustomer)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(CancellationDefinition.StartDate)).AsDateTime().NotNullable()
                .WithColumn(nameof(CancellationDefinition.EndDate)).AsDateTime().Nullable()
                .WithColumn(nameof(CancellationDefinition.IsActive)).AsBoolean().NotNullable()
                .WithColumn(nameof(CancellationDefinition.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(CancellationDefinition.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(CancellationDefinition.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                .WithColumn(nameof(CancellationDefinition.CreatedOnUTC)).AsDateTime().NotNullable()
                .WithColumn(nameof(CancellationDefinition.UpdatedOnUTC)).AsDateTime().Nullable();
        }
        #endregion
    }
}