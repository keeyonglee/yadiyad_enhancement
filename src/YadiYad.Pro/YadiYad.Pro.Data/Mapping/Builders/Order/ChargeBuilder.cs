using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using YadiYad.Pro.Core.Domain.Order;
using Nop.Data.Extensions;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Mapping.Builders.Order
{
    public class ChargeBuilder : NopEntityBuilder<Charge>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Charge.ProductTypeId)).AsInt32().NotNullable()
                .WithColumn(nameof(Charge.SubProductTypeId)).AsInt32().NotNullable()
                .WithColumn(nameof(Charge.ValidityDays)).AsInt32().NotNullable()
                .WithColumn(nameof(Charge.ValueType)).AsInt32().NotNullable()
                .WithColumn(nameof(Charge.Value)).AsDecimal().NotNullable()
                .WithColumn(nameof(Charge.StartDate)).AsDateTime().NotNullable()
                .WithColumn(nameof(Charge.EndDate)).AsDateTime().Nullable()
                .WithColumn(nameof(Charge.IsActive)).AsBoolean().NotNullable()
                .WithColumn(nameof(Charge.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(Charge.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(Charge.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                .WithColumn(nameof(Charge.CreatedOnUTC)).AsDateTime().NotNullable()
                .WithColumn(nameof(Charge.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
