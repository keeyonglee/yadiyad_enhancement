using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.JobSeeker;
using Nop.Core.Domain.Directory;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Mapping.Builders.JobSeeker
{
    public class JobSeekerAcademicQualificationBuilder : NopEntityBuilder<JobSeekerAcademicQualification>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
            .WithColumn(nameof(JobSeekerAcademicQualification.JobSeekerProfileId)).AsInt32().ForeignKey<JobSeekerProfile>().NotNullable()
            .WithColumn(nameof(JobSeekerAcademicQualification.AcademicQualificationType)).AsInt16().NotNullable()
            .WithColumn(nameof(JobSeekerAcademicQualification.AcademicQualificationName)).AsString().NotNullable()
            .WithColumn(nameof(JobSeekerAcademicQualification.AcademicInstitution)).AsString().Nullable()
            .WithColumn(nameof(JobSeekerAcademicQualification.IsHighest)).AsBoolean().NotNullable()
            .WithColumn(nameof(JobSeekerAcademicQualification.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(JobSeekerAcademicQualification.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(JobSeekerAcademicQualification.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(JobSeekerAcademicQualification.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(JobSeekerAcademicQualification.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
