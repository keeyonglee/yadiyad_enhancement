using Nop.Data.Mapping.Builders;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.SqlServer;
using Nop.Plugin.NopStation.AppPushNotification.Domains;

namespace Nop.Plugin.NopStation.AppPushNotification.Data
{
    public class AppPushNotificationTemplateRecordBuilder : NopEntityBuilder<AppPushNotificationTemplate>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AppPushNotificationTemplate.ImageId))
                .AsInt32()
                .WithColumn(nameof(AppPushNotificationTemplate.Active))
                .AsBoolean()
                .WithColumn(nameof(AppPushNotificationTemplate.LimitedToStores))
                .AsBoolean()
                .WithColumn(nameof(AppPushNotificationTemplate.SendImmediately))
                .AsBoolean()
                .WithColumn(nameof(AppPushNotificationTemplate.DelayBeforeSend))
                .AsInt32().Nullable()
                .WithColumn(nameof(AppPushNotificationTemplate.DelayPeriodId))
                .AsInt32()
                .WithColumn(nameof(AppPushNotificationTemplate.ActionTypeId))
                .AsInt32()
                .WithColumn(nameof(AppPushNotificationTemplate.ActionValue))
                .AsString().Nullable()
                .WithColumn(nameof(AppPushNotificationTemplate.AppTypeId))
                .AsInt32().NotNullable();
        }
    }
}