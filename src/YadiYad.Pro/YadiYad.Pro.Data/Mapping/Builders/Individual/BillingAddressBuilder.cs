using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;

namespace YadiYad.Pro.Data.Mapping.Builders.Individual
{
    public class BillingAddressBuilder : NopEntityBuilder<BillingAddress>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(BillingAddress.CustomerId)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(BillingAddress.Address1)).AsString(500).NotNullable()
                .WithColumn(nameof(BillingAddress.Address2)).AsString(500).Nullable()
                .WithColumn(nameof(BillingAddress.ZipPostalCode)).AsString(20).NotNullable()
                .WithColumn(nameof(BillingAddress.CityId)).AsInt32().ForeignKey<City>().Nullable()
                .WithColumn(nameof(BillingAddress.StateProvinceId)).AsInt32().ForeignKey<StateProvince>().Nullable()
                .WithColumn(nameof(BillingAddress.CountryId)).AsInt32().ForeignKey<Country>().NotNullable()
                .WithColumn(nameof(BillingAddress.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(BillingAddress.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(BillingAddress.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                .WithColumn(nameof(BillingAddress.CreatedOnUTC)).AsDateTime().NotNullable()
                .WithColumn(nameof(BillingAddress.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
