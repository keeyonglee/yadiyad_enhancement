using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Data.Mapping.Builders.Common
{
    public class BlockCustomerBuilder : NopEntityBuilder<BlockCustomer>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(BlockCustomer.CustomerId)).AsInt32().ForeignKey<Customer>().NotNullable()
                 .WithColumn(nameof(BlockCustomer.StartDate)).AsDateTime().NotNullable()
                 .WithColumn(nameof(BlockCustomer.EndDate)).AsDateTime().NotNullable()
                 .WithColumn(nameof(BlockCustomer.Reason)).AsInt32().ForeignKey<Reason>().NotNullable()
                 .WithColumn(nameof(BlockCustomer.Remarks)).AsString(int.MaxValue).Nullable()
                 .WithColumn(nameof(BlockCustomer.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                 .WithColumn(nameof(BlockCustomer.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                 .WithColumn(nameof(BlockCustomer.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                 .WithColumn(nameof(BlockCustomer.CreatedOnUTC)).AsDateTime().NotNullable()
                 .WithColumn(nameof(BlockCustomer.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
