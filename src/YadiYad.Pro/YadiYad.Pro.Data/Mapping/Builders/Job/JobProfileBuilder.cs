using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Job;
using Nop.Core.Domain.Directory;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Mapping.Builders.Job
{
    public class JobProfileBuilder : NopEntityBuilder<JobProfile>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(JobProfile.CustomerId)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(JobProfile.CategoryId)).AsInt32().ForeignKey<JobServiceCategory>().NotNullable()
            .WithColumn(nameof(JobProfile.JobTitle)).AsString(200).NotNullable()
            .WithColumn(nameof(JobProfile.PreferredExperience)).AsInt16().NotNullable()
            .WithColumn(nameof(JobProfile.JobType)).AsInt16().NotNullable()
            .WithColumn(nameof(JobProfile.JobRequired)).AsInt16().Nullable()
            .WithColumn(nameof(JobProfile.IsImmediate)).AsBoolean().NotNullable()
            .WithColumn(nameof(JobProfile.StartDate)).AsDateTime().Nullable()
            .WithColumn(nameof(JobProfile.IsOnSite)).AsBoolean().NotNullable()
            .WithColumn(nameof(JobProfile.IsPartialOnSite)).AsBoolean().NotNullable()
            .WithColumn(nameof(JobProfile.IsRemote)).AsBoolean().NotNullable()
            .WithColumn(nameof(JobProfile.CityId)).AsInt32().ForeignKey<City>().Nullable()
            .WithColumn(nameof(JobProfile.StateProvinceId)).AsInt32().ForeignKey<StateProvince>().Nullable()
            .WithColumn(nameof(JobProfile.CountryId)).AsInt32().ForeignKey<Country>().Nullable()
            .WithColumn(nameof(JobProfile.PayAmount)).AsDecimal().NotNullable()
            .WithColumn(nameof(JobProfile.Status)).AsInt32().WithDefaultValue((int)JobProfileStatus.Draft).NotNullable()
            .WithColumn(nameof(JobProfile.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(JobProfile.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(JobProfile.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(JobProfile.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(JobProfile.UpdatedOnUTC)).AsDateTime().Nullable()
            .WithColumn(nameof(JobProfile.JobSchedule)).AsInt16().NotNullable();
        }
        #endregion
    }
}
