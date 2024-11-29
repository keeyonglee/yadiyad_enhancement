using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using YadiYad.Pro.Core.Domain.Subscription;

namespace YadiYad.Pro.Data.Mapping.Builders.Subscription
{
    public class ServiceSubscriptionBuilder : NopEntityBuilder<ServiceSubscription>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
            .WithColumn(nameof(ServiceSubscription.CustomerId)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(ServiceSubscription.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(ServiceSubscription.StartDate)).AsDateTime().NotNullable()
            .WithColumn(nameof(ServiceSubscription.EndDate)).AsDateTime().NotNullable()
            .WithColumn(nameof(ServiceSubscription.SubscriptionTypeId)).AsInt32().NotNullable()
            .WithColumn(nameof(ServiceSubscription.RefId)).AsInt32().NotNullable()
            .WithColumn(nameof(ServiceSubscription.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(ServiceSubscription.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(ServiceSubscription.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(ServiceSubscription.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
