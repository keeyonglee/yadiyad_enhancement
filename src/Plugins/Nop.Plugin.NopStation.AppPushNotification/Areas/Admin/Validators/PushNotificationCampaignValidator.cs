using System;
using FluentValidation;
using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Validators
{
    public class AppPushNotificationCampaignValidator : BaseNopValidator<AppPushNotificationCampaignModel>
    {
        public AppPushNotificationCampaignValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage(localizationService.GetResource("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.Title.Required"));
            RuleFor(x => x.Body).NotEmpty().WithMessage(localizationService.GetResource("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.Body.Required"));
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.Name.Required"));
            RuleFor(x => x.SendingWillStartOn).NotEqual(DateTime.MinValue).WithMessage(localizationService.GetResource("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.SendingWillStartOn.Required"));
        }
    }
}
