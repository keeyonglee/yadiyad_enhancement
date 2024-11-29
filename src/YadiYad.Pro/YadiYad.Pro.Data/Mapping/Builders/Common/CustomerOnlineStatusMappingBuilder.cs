using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Mapping.Builders.Common
{
    public class CustomerOnlineStatusMappingBuilder : NopEntityBuilder<CustomerOnlineStatusMapping>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CustomerOnlineStatusMapping.CustomerId)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(CustomerOnlineStatusMapping.IsOnline)).AsBoolean().WithDefaultValue(true).NotNullable()
                .WithColumn(nameof(CustomerOnlineStatusMapping.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(CustomerOnlineStatusMapping.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(CustomerOnlineStatusMapping.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                .WithColumn(nameof(CustomerOnlineStatusMapping.CreatedOnUTC)).AsDateTime().NotNullable()
                .WithColumn(nameof(CustomerOnlineStatusMapping.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
