using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Organization;
using Nop.Core.Domain.Directory;
using YadiYad.Pro.Core.Domain.Common;
using Nop.Core.Domain.Media;

namespace YadiYad.Pro.Data.Mapping.Builders.Organization
{
    public class OrganizationProfileBuilder : NopEntityBuilder<OrganizationProfile>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(OrganizationProfile.CustomerId)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(OrganizationProfile.Name)).AsString(200).NotNullable()
            .WithColumn(nameof(OrganizationProfile.SegmentId)).AsInt32().ForeignKey<BusinessSegment>().NotNullable()
            .WithColumn(nameof(OrganizationProfile.RegistrationNo)).AsString(200).NotNullable()
            .WithColumn(nameof(OrganizationProfile.DateEstablished)).AsDateTime().NotNullable()
            .WithColumn(nameof(OrganizationProfile.Website)).AsString().Nullable()
            .WithColumn(nameof(OrganizationProfile.IsListedCompany)).AsInt16().NotNullable()
            .WithColumn(nameof(OrganizationProfile.CompanySize)).AsInt16().NotNullable()
            .WithColumn(nameof(OrganizationProfile.Address)).AsString(500).NotNullable()
            .WithColumn(nameof(OrganizationProfile.StateProvinceId)).AsInt32().ForeignKey<StateProvince>().NotNullable()
            .WithColumn(nameof(OrganizationProfile.CountryId)).AsInt32().ForeignKey<Country>().NotNullable()
            .WithColumn(nameof(OrganizationProfile.ContactPersonTitle)).AsInt16().NotNullable()
            .WithColumn(nameof(OrganizationProfile.ContactPersonName)).AsString(200).NotNullable()
            .WithColumn(nameof(OrganizationProfile.ContactPersonPosition)).AsString(200).NotNullable()
            .WithColumn(nameof(OrganizationProfile.ContactPersonEmail)).AsString(200).NotNullable()
            .WithColumn(nameof(OrganizationProfile.ContactPersonContact)).AsString(200).NotNullable()
            .WithColumn(nameof(OrganizationProfile.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(OrganizationProfile.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(OrganizationProfile.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(OrganizationProfile.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(OrganizationProfile.UpdatedOnUTC)).AsDateTime().Nullable()
            .WithColumn(nameof(OrganizationProfile.PictureId)).AsInt32().ForeignKey<Picture>().Nullable();
        }

        #endregion
    }
}
