using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Payout;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Builders.Payout
{
    public class InvoiceBuilderr : NopEntityBuilder<Invoice>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Invoice.InvoiceNumber)).AsString(50).Nullable()
                .WithColumn(nameof(Invoice.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(Invoice.CreatedById)).AsInt32().NotNullable()
                .WithColumn(nameof(Invoice.UpdatedById)).AsInt32().Nullable()
                .WithColumn(nameof(Invoice.CreatedOnUTC)).AsDateTime().NotNullable()
                .WithColumn(nameof(Invoice.UpdatedOnUTC)).AsDateTime().Nullable(); ;
        }

        #endregion
    }
}
