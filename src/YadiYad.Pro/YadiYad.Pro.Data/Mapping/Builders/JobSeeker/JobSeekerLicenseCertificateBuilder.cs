using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.JobSeeker;
using Nop.Core.Domain.Directory;

namespace YadiYad.Pro.Data.Mapping.Builders.JobSeeker
{
    public class JobSeekerLicenseCertificateBuilder : NopEntityBuilder<JobSeekerLicenseCertificate>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
            .WithColumn(nameof(JobSeekerLicenseCertificate.JobSeekerProfileId)).AsInt32().ForeignKey<JobSeekerProfile>().NotNullable()
            .WithColumn(nameof(JobSeekerLicenseCertificate.ProfessionalAssociationName)).AsString(200).NotNullable()
            .WithColumn(nameof(JobSeekerLicenseCertificate.LicenseCertificateName)).AsString(200).NotNullable()
            .WithColumn(nameof(JobSeekerLicenseCertificate.DownloadId)).AsInt32().Nullable()
            .WithColumn(nameof(JobSeekerLicenseCertificate.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(JobSeekerLicenseCertificate.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(JobSeekerLicenseCertificate.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(JobSeekerLicenseCertificate.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(JobSeekerLicenseCertificate.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
