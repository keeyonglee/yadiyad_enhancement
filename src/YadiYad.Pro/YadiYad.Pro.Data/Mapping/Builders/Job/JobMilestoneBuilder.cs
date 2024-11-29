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
    public class ServiceMilestoneBuilder : NopEntityBuilder<JobMilestone>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(JobMilestone.JobProfileId)).AsInt32().ForeignKey<JobProfile>().Nullable()
            .WithColumn(nameof(JobMilestone.Sequence)).AsInt16().NotNullable()
            .WithColumn(nameof(JobMilestone.Description)).AsString(200).NotNullable()
            .WithColumn(nameof(JobMilestone.Amount)).AsDecimal().NotNullable()
            .WithColumn(nameof(JobMilestone.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(JobMilestone.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(JobMilestone.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(JobMilestone.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(JobMilestone.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
