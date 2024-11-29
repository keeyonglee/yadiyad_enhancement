using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using YadiYad.Pro.Core.Domain.Deposit;
using Nop.Data.Extensions;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Order;
using Nop.Core.Domain.Media;

namespace YadiYad.Pro.Data.Mapping.Builders.Deposit
{
    public class DepositRequestBuilder : NopEntityBuilder<DepositRequest>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                    .WithColumn(nameof(DepositRequest.OrderItemId)).AsInt32().ForeignKey<ProOrderItem>().NotNullable()
                    // .WithColumn(nameof(DepositRequest.DepositNumber)).AsString(int.MaxValue).Unique().NotNullable()
                    .WithColumn(nameof(DepositRequest.DepositNumber)).AsString(int.MaxValue).NotNullable()
                    .WithColumn(nameof(DepositRequest.DepositTo)).AsInt32().NotNullable()
                    .WithColumn(nameof(DepositRequest.DepositFrom)).AsInt32().NotNullable()
                    .WithColumn(nameof(DepositRequest.ReminderCount)).AsInt32().WithDefaultValue(0).NotNullable()
                    .WithColumn(nameof(DepositRequest.CycleStart)).AsDateTime().Nullable()
                    .WithColumn(nameof(DepositRequest.CycleEnd)).AsDateTime().Nullable()
                    .WithColumn(nameof(DepositRequest.DueDate)).AsDateTime().NotNullable()
                    .WithColumn(nameof(DepositRequest.RequestDate)).AsDateTime().NotNullable()
                    .WithColumn(nameof(DepositRequest.ProductTypeId)).AsInt32().NotNullable()
                    .WithColumn(nameof(DepositRequest.RefId)).AsInt32().NotNullable()
                    .WithColumn(nameof(DepositRequest.Status)).AsInt32().WithDefaultValue(0).NotNullable()
                    .WithColumn(nameof(DepositRequest.Amount)).AsDecimal().NotNullable()

                    .WithColumn(nameof(DepositRequest.PaymentChannelId)).AsInt32().WithDefaultValue(0).Nullable()
                    .WithColumn(nameof(DepositRequest.BankId)).AsInt32()
                    .ForeignKey(
                            "FK_DepositRequest_BankId",
                            nameof(Bank),
                            nameof(Bank.Id))
                    .Nullable()
                    .WithColumn(nameof(DepositRequest.BankInDate)).AsDateTime().Nullable()
                    .WithColumn(nameof(DepositRequest.BankInReference)).AsString(255).Nullable()
                    .WithColumn(nameof(DepositRequest.BankInSlipDownloadId)).AsInt32()
                    .ForeignKey(
                        "FK_DepositRequest_BankInSlipDownloadId",
                        nameof(Download),
                        nameof(Download.Id))
                    .Nullable()

                    .WithColumn(nameof(DepositRequest.Deleted)).AsBoolean().WithDefaultValue(false).NotNullable()
                    .WithColumn(nameof(DepositRequest.CreatedById)).AsInt32().ForeignKey<Customer>().NotNullable()
                    .WithColumn(nameof(DepositRequest.UpdatedById)).AsInt32().ForeignKey<Customer>().Nullable()
                    .WithColumn(nameof(DepositRequest.CreatedOnUTC)).AsDateTime().NotNullable()
                    .WithColumn(nameof(DepositRequest.UpdatedOnUTC)).AsDateTime().Nullable()
                    .WithColumn(nameof(DepositRequest.ApproveRemarks)).AsString(int.MaxValue).Nullable();

        }

        #endregion
    }
}