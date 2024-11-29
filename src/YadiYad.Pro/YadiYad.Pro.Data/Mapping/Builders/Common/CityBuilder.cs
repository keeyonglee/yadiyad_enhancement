using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;

namespace YadiYad.Pro.Data.Mapping.Builders.Common
{
    public class CityBuilder : NopEntityBuilder<City>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(City.Name)).AsString(200).NotNullable()
            .WithColumn(nameof(City.Abbreviation)).AsString(200).Nullable()
            .WithColumn(nameof(City.Published)).AsBoolean().NotNullable()
            .WithColumn(nameof(City.DisplayOrder)).AsInt32().NotNullable()
            .WithColumn(nameof(City.CountryId)).AsInt32().ForeignKey<Country>().Nullable()
            .WithColumn(nameof(City.StateProvinceId)).AsInt32().ForeignKey<StateProvince>().Nullable();
        }

        #endregion
    }

}
