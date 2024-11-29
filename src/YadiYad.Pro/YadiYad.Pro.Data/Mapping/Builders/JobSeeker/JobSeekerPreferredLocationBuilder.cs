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
    public class JobSeekerPreferredLocationBuilder : NopEntityBuilder<JobSeekerPreferredLocation>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
            .WithColumn(nameof(JobSeekerPreferredLocation.JobSeekerProfileId)).AsInt32().ForeignKey<JobSeekerProfile>().NotNullable()
            .WithColumn(nameof(JobSeekerPreferredLocation.CityId)).AsInt32().ForeignKey<City>().Nullable()
            .WithColumn(nameof(JobSeekerPreferredLocation.StateProvinceId)).AsInt32().ForeignKey<StateProvince>().Nullable()
            .WithColumn(nameof(JobSeekerPreferredLocation.CountryId)).AsInt32().ForeignKey<Country>().NotNullable()
            .WithColumn(nameof(JobSeekerPreferredLocation.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(JobSeekerPreferredLocation.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(JobSeekerPreferredLocation.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(JobSeekerPreferredLocation.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(JobSeekerPreferredLocation.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
