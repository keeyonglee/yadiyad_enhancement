using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models;
using Nop.Plugin.NopStation.AppPushNotification.Domains;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Infrastructure
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        public MapperConfiguration()
        {
            #region App app device

            #endregion

            #region Push notf template

            CreateMap<AppPushNotificationTemplate, AppPushNotificationTemplateModel>()
                .ForMember(model => model.AllowedTokens, options => options.Ignore());
            CreateMap<AppPushNotificationTemplateModel, AppPushNotificationTemplate>()
                .ForMember(entity => entity.Name, options => options.Ignore());

            #endregion

            #region Queued notification

            CreateMap<AppQueuedPushNotification, AppQueuedPushNotificationModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.SentOn, options => options.Ignore());
            CreateMap<AppQueuedPushNotificationModel, AppQueuedPushNotification>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.SentOnUtc, options => options.Ignore());

            #endregion 

            #region Configuration

            CreateMap<AppPushNotificationSettings, ConfigurationModel>()
                .ForMember(model => model.GoogleConsoleApiAccessKey_OverrideForStore, options => options.Ignore());
            CreateMap<ConfigurationModel, AppPushNotificationSettings>();

            #endregion

            #region Push notification campaign

            CreateMap<AppPushNotificationCampaign, AppPushNotificationCampaignModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.AddedToQueueOn, options => options.Ignore())
                .ForMember(model => model.AvailableSmartGroups, options => options.Ignore())
                .ForMember(model => model.CustomerRoles, options => options.Ignore())
                .ForMember(model => model.DeviceTypes, options => options.Ignore())
                .ForMember(model => model.AvailableCustomerRoles, options => options.Ignore())
                .ForMember(model => model.SendingWillStartOn, options => options.Ignore());
            CreateMap<AppPushNotificationCampaignModel, AppPushNotificationCampaign>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.CustomerRoles, options => options.Ignore())
                .ForMember(entity => entity.DeviceTypes, options => options.Ignore())
                .ForMember(entity => entity.AddedToQueueOnUtc, options => options.Ignore())
                .ForMember(entity => entity.SendingWillStartOnUtc, options => options.Ignore());

            #endregion
        }

        public int Order => 0;
    }
}
