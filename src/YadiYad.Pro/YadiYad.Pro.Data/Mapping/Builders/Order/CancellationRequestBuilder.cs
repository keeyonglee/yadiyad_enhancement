using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using YadiYad.Pro.Core.Domain.Order;
using Nop.Data.Extensions;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Data.Mapping.Builders.Order
{
    public class CancellationRequestBuilder : NopEntityBuilder<CancellationRequest>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CancellationRequest.EngagementId)).AsInt32().NotNullable()
                .WithColumn(nameof(CancellationRequest.EngagementType)).AsInt32().NotNullable()
                .WithColumn(nameof(CancellationRequest.CancelledBy)).AsInt32().NotNullable()
                .WithColumn(nameof(CancellationRequest.ReasonId)).AsInt32().NotNullable()
                .WithColumn(nameof(CancellationRequest.UserRemarks)).AsString().Nullable()
                .WithColumn(nameof(CancellationRequest.SubmissionDate)).AsDateTime().NotNullable()
                .WithColumn(nameof(CancellationRequest.Status)).AsInt32().Nullable()
                .WithColumn(nameof(CancellationRequest.PostCancellationAction)).AsInt32().NotNullable()
                .WithColumn(nameof(CancellationRequest.AdminRemarks)).AsString().Nullable()
                .WithColumn(nameof(CancellationRequest.AttachmentId)).AsInt32().Nullable()
                .WithColumn(nameof(CancellationRequest.CustomerBlockDays)).AsInt32().Nullable()
                .WithColumn(nameof(CancellationRequest.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(nameof(CancellationRequest.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(CancellationRequest.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                .WithColumn(nameof(CancellationRequest.CreatedOnUTC)).AsDateTime().NotNullable()
                .WithColumn(nameof(CancellationRequest.UpdatedOnUTC)).AsDateTime().Nullable();
        }
        #endregion
    }
}
