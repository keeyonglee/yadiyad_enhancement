using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Data.Mapping.Builders.Payout
{
    public class PayoutRequestBuilder : NopEntityBuilder<PayoutRequest>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                    .WithColumn(nameof(PayoutRequest.OrderItemId)).AsInt32().ForeignKey<ProOrderItem>().NotNullable()
                    .WithColumn(nameof(PayoutRequest.ProductTypeId)).AsInt32().NotNullable()
                    .WithColumn(nameof(PayoutRequest.RefId)).AsInt32().NotNullable()
                    .WithColumn(nameof(PayoutRequest.StartDate)).AsDateTime().Nullable()
                    .WithColumn(nameof(PayoutRequest.EndDate)).AsDateTime().Nullable()
                    .WithColumn(nameof(PayoutRequest.JobApplicationMilestoneId)).AsInt32().ForeignKey<JobApplicationMilestone>().Nullable()
                    .WithColumn(nameof(PayoutRequest.PayoutTo)).AsInt32().ForeignKey<Customer>().NotNullable()
                    .WithColumn(nameof(PayoutRequest.Status)).AsInt32().NotNullable()

                    .WithColumn(nameof(PayoutRequest.TimeSheetJson)).AsString(int.MaxValue).NotNullable()
                    .WithColumn(nameof(PayoutRequest.WorkDesc)).AsString(int.MaxValue).NotNullable()
                    .WithColumn(nameof(PayoutRequest.AttachmentDownloadId)).AsInt32().Nullable()
                    .WithColumn(nameof(PayoutRequest.OnsiteDuration)).AsInt32().WithDefaultValue(0).Nullable()
                    .WithColumn(nameof(PayoutRequest.ProratedWorkDuration)).AsInt32().Nullable()

                    .WithColumn(nameof(PayoutRequest.Remark)).AsString(int.MaxValue).Nullable()

                    .WithColumn(nameof(PayoutRequest.Fee)).AsDecimal().NotNullable()
                    .WithColumn(nameof(PayoutRequest.ServiceCharge)).AsDecimal().WithDefaultValue(0).NotNullable()

                    .WithColumn(nameof(PayoutRequest.InvoiceId)).AsInt32().Nullable()

                    .WithColumn(nameof(PayoutRequest.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                    .WithColumn(nameof(PayoutRequest.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                    .WithColumn(nameof(PayoutRequest.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                    .WithColumn(nameof(PayoutRequest.CreatedOnUTC)).AsDateTime().NotNullable()
                    .WithColumn(nameof(PayoutRequest.UpdatedOnUTC)).AsDateTime().Nullable();
        }

        #endregion
    }
}
