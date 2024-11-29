using Nop.Data.Mapping.Builders;
using FluentMigrator.Builders.Create.Table;
using Nop.Plugin.NopStation.WebApi.Domains;

namespace Nop.Plugin.NopStation.WebApi.Data
{
    public class ApiDeviceRecordBuilder : NopEntityBuilder<ApiDevice>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ApiDevice.DeviceToken))
                .AsString().Nullable()
                .WithColumn(nameof(ApiDevice.DeviceTypeId))
                .AsInt32()
                .WithColumn(nameof(ApiDevice.CustomerId))
                .AsInt32()
                .WithColumn(nameof(ApiDevice.StoreId))
                .AsInt32()
                .WithColumn(nameof(ApiDevice.SubscriptionId))
                .AsString().Nullable()
                .WithColumn(nameof(ApiDevice.IsRegistered))
                .AsBoolean()
                .WithColumn(nameof(ApiDevice.CreatedOnUtc))
                .AsDateTime()
                .WithColumn(nameof(ApiDevice.UpdatedOnUtc))
                .AsDateTime()
                .WithColumn(nameof(ApiDevice.AppTypeId))
                .AsInt32().NotNullable();
        }
    }
}
