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
    public class JobSeekerProfileBuilder : NopEntityBuilder<JobSeekerProfile>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(JobSeekerProfile.CustomerId)).AsInt32().ForeignKey<Customer>().NotNullable()

            .WithColumn(nameof(JobSeekerProfile.EmploymentStatus)).AsInt16().NotNullable()
            .WithColumn(nameof(JobSeekerProfile.Company)).AsString().Nullable()
            .WithColumn(nameof(JobSeekerProfile.Position)).AsString().Nullable()
            .WithColumn(nameof(JobSeekerProfile.TenureStart)).AsDate().Nullable()
            .WithColumn(nameof(JobSeekerProfile.TenureEnd)).AsDate().Nullable()
            .WithColumn(nameof(JobSeekerProfile.AchievementAward)).AsString().Nullable()

            .WithColumn(nameof(JobSeekerProfile.IsFreelanceDaily)).AsBoolean().NotNullable()
            .WithColumn(nameof(JobSeekerProfile.IsFreelanceHourly)).AsBoolean().NotNullable()
            .WithColumn(nameof(JobSeekerProfile.IsProjectBased)).AsBoolean().NotNullable()

            .WithColumn(nameof(JobSeekerProfile.IsOnSite)).AsBoolean().NotNullable()
            .WithColumn(nameof(JobSeekerProfile.IsPartialOnSite)).AsBoolean().NotNullable()
            .WithColumn(nameof(JobSeekerProfile.IsRemote)).AsBoolean().NotNullable()

            .WithColumn(nameof(JobSeekerProfile.DailyPayAmount)).AsDecimal().Nullable()
            .WithColumn(nameof(JobSeekerProfile.HourlyPayAmount)).AsDecimal().Nullable()

            .WithColumn(nameof(JobSeekerProfile.AvailableDays)).AsInt32().Nullable()
            .WithColumn(nameof(JobSeekerProfile.AvailableHours)).AsInt32().Nullable()

            .WithColumn(nameof(JobSeekerProfile.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(JobSeekerProfile.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(JobSeekerProfile.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(JobSeekerProfile.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(JobSeekerProfile.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
