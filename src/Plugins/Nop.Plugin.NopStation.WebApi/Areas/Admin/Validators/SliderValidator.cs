using FluentValidation;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Models;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Validators
{
    public class SliderValidator : BaseNopValidator<SliderModel>
    {
        public SliderValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.PictureId)
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.NopStation.WebApi.Sliders.Fields.Picture.Required"));

            RuleFor(x => x.ActiveEndDate)
                .GreaterThan(x => x.ActiveStartDate)
                .When(x => x.ActiveEndDate.HasValue && x.ActiveStartDate.HasValue)
                .WithMessage(localizationService.GetResource("Admin.NopStation.WebApi.Sliders.Fields.ActiveEndDate.GreaterThanStartDate"));

            RuleFor(x => x.EntityId)
                .GreaterThan(0)
                .When(x => x.SliderTypeId == (int)SliderType.Category ||
                    x.SliderTypeId == (int)SliderType.Manufacturer ||
                    x.SliderTypeId == (int)SliderType.Product ||
                    x.SliderTypeId == (int)SliderType.Vendor ||
                    x.SliderTypeId == (int)SliderType.Topic)
                .WithMessage(localizationService.GetResource("Admin.NopStation.WebApi.Sliders.Fields.EntityId.Required"));
        }
    }
}
