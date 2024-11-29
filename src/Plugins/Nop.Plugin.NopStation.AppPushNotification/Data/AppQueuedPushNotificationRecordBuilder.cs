using Nop.Data.Mapping.Builders;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.SqlServer;
using Nop.Plugin.NopStation.AppPushNotification.Domains;

namespace Nop.Plugin.NopStation.AppPushNotification.Data
{
    public class AppQueuedPushNotificationRecordBuilder : NopEntityBuilder<AppQueuedPushNotification>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AppQueuedPushNotification.CustomerId))
                .AsInt32()
                .WithColumn(nameof(AppQueuedPushNotification.StoreId))
                .AsInt32()
                .WithColumn(nameof(AppQueuedPushNotification.ImageUrl))
                .AsString().Nullable()
                .WithColumn(nameof(AppQueuedPushNotification.SentTries))
                .AsInt32()
                .WithColumn(nameof(AppQueuedPushNotification.AppDeviceId))
                .AsInt32()
                .WithColumn(nameof(AppQueuedPushNotification.DeviceTypeId))
                .AsInt32()
                .WithColumn(nameof(AppQueuedPushNotification.ActionTypeId))
                .AsInt32()
                .WithColumn(nameof(AppQueuedPushNotification.ActionValue))
                .AsString().Nullable()
                .WithColumn(nameof(AppQueuedPushNotification.SubscriptionId))
                .AsString().Nullable()
                .WithColumn(nameof(AppQueuedPushNotification.CreatedOnUtc))
                .AsDateTime()
                .WithColumn(nameof(AppQueuedPushNotification.SentOnUtc))
                .AsDateTime().Nullable()
                .WithColumn(nameof(AppQueuedPushNotification.DontSendBeforeDateUtc))
                .AsDateTime().Nullable()
                .WithColumn(nameof(AppQueuedPushNotification.AppTypeId))
                .AsInt32().NotNullable();
        }
    }
}