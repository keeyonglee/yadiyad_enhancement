using Nop.Data.Mapping.Builders;
using FluentMigrator.Builders.Create.Table;
using Nop.Plugin.NopStation.WebApi.Domains;
using FluentMigrator.SqlServer;
using Nop.Plugin.NopStation.AppPushNotification.Domains;

namespace Nop.Plugin.NopStation.AppPushNotification.Data
{
    public class AppPushNotificationCampaignRecordBuilder : NopEntityBuilder<AppPushNotificationCampaign>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AppPushNotificationCampaign.ImageId))
                .AsInt32()
                .WithColumn(nameof(AppPushNotificationCampaign.SendingWillStartOnUtc))
                .AsDateTime()
                .WithColumn(nameof(AppPushNotificationCampaign.AddedToQueueOnUtc))
                .AsDateTime().Nullable()
                .WithColumn(nameof(AppPushNotificationCampaign.LimitedToStoreId))
                .AsInt32()
                .WithColumn(nameof(AppPushNotificationCampaign.Deleted))
                .AsBoolean()
                .WithColumn(nameof(AppPushNotificationCampaign.ActionTypeId))
                .AsInt32()
                .WithColumn(nameof(AppPushNotificationCampaign.ActionValue))
                .AsString().Nullable()
                .WithColumn(nameof(AppPushNotificationCampaign.CreatedOnUtc))
                .AsDateTime();



        }
    }
}
