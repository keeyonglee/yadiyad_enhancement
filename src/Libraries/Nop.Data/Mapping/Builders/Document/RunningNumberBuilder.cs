using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Builders.Document
{
    public class RunningNumberBuilder : NopEntityBuilder<RunningNumber>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(RunningNumber.Name)).AsString(255)
                .WithColumn(nameof(RunningNumber.LastId)).AsInt32()
                .WithColumn(nameof(RunningNumber.LastYear)).AsInt32();
        }

        #endregion
    }
}
