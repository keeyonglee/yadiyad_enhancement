using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Individual;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Mapping.Builders.Individual
{
    public class IndividualProfileBuilder : NopEntityBuilder<IndividualProfile>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(IndividualProfile.CustomerId)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(IndividualProfile.Title)).AsInt16().NotNullable()
            .WithColumn(nameof(IndividualProfile.FullName)).AsString(200).NotNullable()
            .WithColumn(nameof(IndividualProfile.NickName)).AsString(200).NotNullable()
            .WithColumn(nameof(IndividualProfile.Gender)).AsInt16().NotNullable()
            .WithColumn(nameof(IndividualProfile.DateOfBirth)).AsDateTime().NotNullable()
            .WithColumn(nameof(IndividualProfile.Email)).AsString(200).NotNullable()
            .WithColumn(nameof(IndividualProfile.ContactNo)).AsString(200).NotNullable()
            .WithColumn(nameof(IndividualProfile.NationalityId)).AsInt32().ForeignKey<Country>().NotNullable()
            .WithColumn(nameof(IndividualProfile.Address)).AsString().NotNullable()
            .WithColumn(nameof(BillingAddress.Address1)).AsString(500).NotNullable()
            .WithColumn(nameof(BillingAddress.Address2)).AsString(500).Nullable()
            .WithColumn(nameof(BillingAddress.ZipPostalCode)).AsString(20).NotNullable()
            .WithColumn(nameof(IndividualProfile.CityId)).AsInt32().ForeignKey<City>().NotNullable()
            .WithColumn(nameof(IndividualProfile.StateProvinceId)).AsInt32().ForeignKey<StateProvince>().NotNullable()
            .WithColumn(nameof(IndividualProfile.CountryId)).AsInt32().ForeignKey<Country>().NotNullable()
            .WithColumn(nameof(IndividualProfile.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(IndividualProfile.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(IndividualProfile.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(IndividualProfile.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(IndividualProfile.UpdatedOnUTC)).AsDateTime().Nullable()
            .WithColumn(nameof(IndividualProfile.IsOnline)).AsBoolean().WithDefaultValue(true).NotNullable()
            .WithColumn(nameof(IndividualProfile.PictureId)).AsInt32().ForeignKey<Picture>().Nullable()
            .WithColumn(nameof(IndividualProfile.ProfileImageViewModeId)).AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn(nameof(IndividualProfile.SSTRegNo)).AsString().Nullable()
            .WithColumn(nameof(IndividualProfile.NumberOfCancellation)).AsInt32().NotNullable()
            .WithColumn(nameof(IndividualProfile.IsTourCompleted)).AsBoolean().WithDefaultValue(false).NotNullable();

        }

        #endregion
    }
}
