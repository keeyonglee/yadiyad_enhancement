using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using Nop.Core.Domain.Customers;

namespace YadiYad.Pro.Data.Mapping.Builders.Common
{
    public class ExpertiseBuilder : NopEntityBuilder<Expertise>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(Expertise.Name)).AsString(200).NotNullable()
            .WithColumn(nameof(Expertise.JobServiceCategoryId)).AsInt32().ForeignKey<JobServiceCategory>().NotNullable()
            .WithColumn(nameof(Expertise.IsOthers)).AsBoolean().Nullable()
            .WithColumn(nameof(Expertise.Published)).AsBoolean().NotNullable();
        }

        #endregion
    }

}
