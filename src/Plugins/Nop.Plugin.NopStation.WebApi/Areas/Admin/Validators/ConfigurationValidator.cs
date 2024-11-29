using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Validators
{
    public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
    {
        #region Ctor

        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.IOSProductPriceTextSize)
                .GreaterThan(7)
                .LessThan(17)
                .WithMessage(localizationService.GetResource("Admin.NopStation.WebApi.Configuration.Fields.IOSProductPriceTextSize.GreaterThanAndLessThanLimit"));
            RuleFor(model => model.AndroidProductPriceTextSize)
                .GreaterThan(9)
                .LessThan(15)
                .WithMessage(localizationService.GetResource("Admin.NopStation.WebApi.Configuration.Fields.AndroidProductPriceTextSize.GreaterThanAndLessThanLimit"));
            RuleFor(model => model.IonicProductPriceTextSize)
                .GreaterThan(15)
                .LessThan(25)
                .WithMessage(localizationService.GetResource("Admin.NopStation.WebApi.Configuration.Fields.IonicProductPriceTextSize.GreaterThanAndLessThanLimit"));
        }

        #endregion
    }
}
