using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Service;
using Nop.Core.Domain.Directory;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Data.Mapping.Builders.Job
{
    public class JobProfileExpertiseBuilder : NopEntityBuilder<JobProfileExpertise>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(JobProfileExpertise.CustomerId)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(JobProfileExpertise.JobProfieId)).AsInt32().ForeignKey<JobProfile>().NotNullable()
            .WithColumn(nameof(JobProfileExpertise.ExpertiseId)).AsInt32().ForeignKey<Expertise>().NotNullable()
            .WithColumn(nameof(JobProfileExpertise.OtherName)).AsString().Nullable()
            .WithColumn(nameof(JobProfileExpertise.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(JobProfileExpertise.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(JobProfileExpertise.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(JobProfileExpertise.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(JobProfileExpertise.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
