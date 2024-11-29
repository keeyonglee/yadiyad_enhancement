using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Service;
using Nop.Core.Domain.Directory;

namespace YadiYad.Pro.Data.Mapping.Builders.JobSeeker
{
    public class ServiceLicenseCertificateBuilder : NopEntityBuilder<ServiceLicenseCertificate>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ServiceLicenseCertificate.ServiceProfileId)).AsInt32().ForeignKey<ServiceProfile>().Nullable()
            .WithColumn(nameof(ServiceLicenseCertificate.ProfessionalAssociationName)).AsString(200).NotNullable()
            .WithColumn(nameof(ServiceLicenseCertificate.LicenseCertificateName)).AsString(200).NotNullable()
            .WithColumn(nameof(ServiceLicenseCertificate.DownloadId)).AsInt32().Nullable()
            .WithColumn(nameof(ServiceLicenseCertificate.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(ServiceLicenseCertificate.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(ServiceLicenseCertificate.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(ServiceLicenseCertificate.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(ServiceLicenseCertificate.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
