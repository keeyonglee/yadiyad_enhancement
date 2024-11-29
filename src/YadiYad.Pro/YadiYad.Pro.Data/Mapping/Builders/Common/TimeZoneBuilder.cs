using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using TimeZone = YadiYad.Pro.Core.Domain.Common.TimeZone;

namespace YadiYad.Pro.Data.Mapping.Builders.Common
{
    public class TimeZoneBuilder : NopEntityBuilder<TimeZone>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(TimeZone.Name)).AsString(200).NotNullable()
                .WithColumn(nameof(TimeZone.Offset)).AsInt16().NotNullable();
        }

        #endregion
    }

}
