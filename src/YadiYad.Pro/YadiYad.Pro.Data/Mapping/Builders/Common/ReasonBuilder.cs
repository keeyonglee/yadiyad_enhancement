using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Mapping.Builders.Common
{
    public class ReasonBuilder : NopEntityBuilder<Reason>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
               .WithColumn(nameof(Reason.Name)).AsString(200).NotNullable()
               .WithColumn(nameof(Reason.EngagementType)).AsInt16().NotNullable()
               .WithColumn(nameof(Reason.Party)).AsInt16().NotNullable()
               .WithColumn(nameof(Reason.Published)).AsBoolean().NotNullable();

        }

        #endregion
    }
}
