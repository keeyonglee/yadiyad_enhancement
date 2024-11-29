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
using Nop.Core.Domain.Media;

namespace YadiYad.Pro.Data.Mapping.Builders.Job
{
    public class JobApplicationBuilder : NopEntityBuilder<JobApplication>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(JobApplication.JobSeekerProfileId)).AsInt32().ForeignKey<JobSeekerProfile>().NotNullable()
                .WithColumn(nameof(JobApplication.JobProfileId)).AsInt32().ForeignKey<JobProfile>().NotNullable()
                .WithColumn(nameof(JobApplication.OrganizationProfileId)).AsInt32().ForeignKey<OrganizationProfile>().NotNullable()
                .WithColumn(nameof(JobApplication.IsRead)).AsBoolean().NotNullable()
                .WithColumn(nameof(JobApplication.JobInvitationId)).AsInt32().ForeignKey<JobInvitation>().Nullable()
                .WithColumn(nameof(JobApplication.JobApplicationStatus)).AsInt16().NotNullable()
                .WithColumn(nameof(JobApplication.IsEscrow)).AsBoolean().NotNullable()
                .WithColumn(nameof(JobApplication.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(JobApplication.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(JobApplication.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                .WithColumn(nameof(JobApplication.CreatedOnUTC)).AsDateTime().NotNullable()
                .WithColumn(nameof(JobApplication.UpdatedOnUTC)).AsDateTime().Nullable()
                .WithColumn(nameof(JobApplication.KnowledgenessRating)).AsDouble().Nullable()
                .WithColumn(nameof(JobApplication.ClearnessRating)).AsDouble().Nullable()
                .WithColumn(nameof(JobApplication.ProfessionalismRating)).AsDouble().Nullable()
                .WithColumn(nameof(JobApplication.RelevanceRating)).AsDouble().Nullable()
                .WithColumn(nameof(JobApplication.RespondingRating)).AsDouble().Nullable()
                .WithColumn(nameof(JobApplication.ReviewText)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(JobApplication.ReviewDateTime)).AsDateTime().Nullable()
                .WithColumn(nameof(JobApplication.Rating)).AsDouble().Nullable()
                .WithColumn(nameof(JobApplication.CancellationRemarks)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(JobApplication.StartDate)).AsDateTime().Nullable()
                .WithColumn(nameof(JobApplication.EndDate)).AsDateTime().Nullable()
                .WithColumn(nameof(JobApplication.CancellationDateTime)).AsDateTime().Nullable()
                .WithColumn(nameof(JobApplication.EndMilestoneId)).AsInt32().Nullable()
                .WithColumn(nameof(JobApplication.PayAmount)).AsDecimal().NotNullable()
                .WithColumn(nameof(JobApplication.JobType)).AsInt32().NotNullable()
                .WithColumn(nameof(JobApplication.JobRequired)).AsInt32().Nullable()
                .WithColumn(nameof(JobApplication.CancellationReasonId)).AsInt32().ForeignKey<Reason>().Nullable()
                .WithColumn(nameof(JobApplication.CancellationDownloadId)).AsInt32().ForeignKey<Picture>().Nullable()
                .WithColumn(nameof(JobApplication.NumberOfHiring)).AsInt32().NotNullable();

        }
        #endregion
    }
}
