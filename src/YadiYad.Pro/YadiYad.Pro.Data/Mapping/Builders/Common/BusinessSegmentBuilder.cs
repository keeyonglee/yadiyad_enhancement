using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using Nop.Core.Domain.Directory;

namespace YadiYad.Pro.Data.Mapping.Builders.Common
{
    public class BusinessSegmentBuilder : NopEntityBuilder<BusinessSegment>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(BusinessSegment.Name)).AsString(200).NotNullable();
        }

        #endregion
    }
}
