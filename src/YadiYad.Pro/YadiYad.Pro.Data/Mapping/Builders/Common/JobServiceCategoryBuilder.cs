using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Mapping.Builders.Common
{
    public class JobServiceCategoryBuilder : NopEntityBuilder<JobServiceCategory>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
               .WithColumn(nameof(JobServiceCategory.Name)).AsString(200).NotNullable()
               .WithColumn(nameof(JobServiceCategory.ImageUrl)).AsString(500).Nullable()
               .WithColumn(nameof(JobServiceCategory.Published)).AsBoolean().NotNullable();
        }

        #endregion
    }
}
