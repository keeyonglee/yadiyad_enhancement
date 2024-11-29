using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using YadiYad.Pro.Core.Domain.JobSeeker;

namespace YadiYad.Pro.Data.Mapping.Builders.JobSeeker
{
    public class JobSeekerCVBuilder: NopEntityBuilder<JobSeekerCV>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(JobSeekerCV.JobSeekerProfileId)).AsInt32().ForeignKey<JobSeekerProfile>().NotNullable()
                .WithColumn(nameof(JobSeekerCV.DownloadId)).AsInt32().NotNullable()
                .WithColumn(nameof(JobSeekerCV.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(JobSeekerCV.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(JobSeekerCV.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                .WithColumn(nameof(JobSeekerCV.CreatedOnUTC)).AsDateTime().NotNullable()
                .WithColumn(nameof(JobSeekerCV.UpdatedOnUTC)).AsDateTime().Nullable();
        }
    }
}