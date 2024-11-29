using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Mapping.Builders.JobSeeker
{
    public class JobSeekerCategoryBuilder : NopEntityBuilder<JobSeekerCategory>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
            .WithColumn(nameof(JobSeekerCategory.JobSeekerProfileId)).AsInt32().ForeignKey<JobSeekerProfile>().NotNullable()
            .WithColumn(nameof(JobSeekerCategory.CategoryId)).AsInt32().ForeignKey<JobServiceCategory>().NotNullable()
            .WithColumn(nameof(JobSeekerCategory.YearExperience)).AsInt32().NotNullable()
            .WithColumn(nameof(JobSeekerCategory.Expertises)).AsString().NotNullable()
            .WithColumn(nameof(JobSeekerCategory.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(JobSeekerCategory.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(JobSeekerCategory.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(JobSeekerCategory.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(JobSeekerCategory.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
