using FluentValidation;
using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Validators
{
    public class AppPushNotificationTemplateValidator : BaseNopValidator<AppPushNotificationTemplateModel>
    {
        public AppPushNotificationTemplateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage(localizationService.GetResource("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.Title.Required"));
            RuleFor(x => x.Name)
                .NotEmpty()
                .When(x => x.Id == 0)
                .WithMessage(localizationService.GetResource("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.Name.Required"));
            RuleFor(x => x.DelayBeforeSend)
                .NotNull()
                .When(x => !x.SendImmediately)
                .WithMessage(localizationService.GetResource("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.DelayBeforeSend.Required"));
            RuleFor(x => x.DelayBeforeSend)
                .GreaterThan(0)
                .When(x => !x.SendImmediately)
                .WithMessage(localizationService.GetResource("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.DelayBeforeSend.GreaterThanZero"));
        }
    }
}
