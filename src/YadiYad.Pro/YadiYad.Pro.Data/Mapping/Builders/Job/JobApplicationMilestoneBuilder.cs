using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Directory;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Data.Mapping.Builders.Job
{
    public class JobApplicationMilestoneBuilder : NopEntityBuilder<JobApplicationMilestone>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(JobApplicationMilestone.JobApplicationId)).AsInt32().ForeignKey<JobApplication>().Nullable()
            .WithColumn(nameof(JobApplicationMilestone.Sequence)).AsInt16().NotNullable()
            .WithColumn(nameof(JobApplicationMilestone.Description)).AsString(200).NotNullable()
            .WithColumn(nameof(JobApplicationMilestone.Amount)).AsDecimal().NotNullable()
            .WithColumn(nameof(JobApplicationMilestone.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(JobApplicationMilestone.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(JobApplicationMilestone.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(JobApplicationMilestone.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(JobApplicationMilestone.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
