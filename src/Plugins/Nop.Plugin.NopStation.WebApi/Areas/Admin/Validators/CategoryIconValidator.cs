using FluentValidation;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Validators
{
    public class CategoryIconValidator : BaseNopValidator<CategoryIconModel>
    {
        public CategoryIconValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.IconId)
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.NopStation.WebApi.CategoryIcons.Fields.Picture.Required"));
            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.NopStation.WebApi.CategoryIcons.Fields.Category.Required"));
        }
    }
}
