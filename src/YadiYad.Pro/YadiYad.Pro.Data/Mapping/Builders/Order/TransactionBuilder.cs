using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using YadiYad.Pro.Core.Domain.Order;
using Nop.Data.Extensions;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Mapping.Builders.Order
{
    public class TransactionBuilder : NopEntityBuilder<Transaction>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
        table
                .WithColumn(nameof(Transaction.Amount)).AsDecimal().NotNullable()
                .WithColumn(nameof(Transaction.TransactionTypeId)).AsInt32().NotNullable()
                .WithColumn(nameof(Transaction.TransactionRefTypeId)).AsInt32().NotNullable()
                .WithColumn(nameof(Transaction.TransactionRefId)).AsInt32().NotNullable()
                .WithColumn(nameof(Transaction.Account)).AsInt32().NotNullable()
                .WithColumn(nameof(Transaction.Remarks)).AsString().Nullable()
                .WithColumn(nameof(Transaction.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(Transaction.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(Transaction.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                .WithColumn(nameof(Transaction.CreatedOnUTC)).AsDateTime().NotNullable()
                .WithColumn(nameof(Transaction.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
