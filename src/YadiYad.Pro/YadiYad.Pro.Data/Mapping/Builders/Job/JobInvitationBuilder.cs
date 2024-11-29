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
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Core.Domain.JobSeeker;

namespace YadiYad.Pro.Data.Mapping.Builders.Job
{
    public class JobInvitationBuilder : NopEntityBuilder<JobInvitation>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(JobInvitation.OrganizationProfileId)).AsInt32().ForeignKey<OrganizationProfile>().NotNullable()
            .WithColumn(nameof(JobInvitation.JobProfileId)).AsInt32().ForeignKey<JobProfile>().NotNullable()
            .WithColumn(nameof(JobInvitation.JobSeekerProfileId)).AsInt32().ForeignKey<JobSeekerProfile>().NotNullable()
            .WithColumn(nameof(JobInvitation.IsRead)).AsBoolean().NotNullable()
            .WithColumn(nameof(JobInvitation.JobInvitationStatus)).AsInt16().NotNullable()
            .WithColumn(nameof(JobInvitation.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn(nameof(JobInvitation.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
            .WithColumn(nameof(JobInvitation.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
            .WithColumn(nameof(JobInvitation.CreatedOnUTC)).AsDateTime().NotNullable()
            .WithColumn(nameof(JobInvitation.UpdatedOnUTC)).AsDateTime().Nullable();
        }
        #endregion
    }
}
