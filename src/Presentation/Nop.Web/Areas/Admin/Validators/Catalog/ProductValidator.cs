using FluentValidation;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Catalog
{
    public partial class ProductValidator : BaseNopValidator<ProductModel>
    {
        public ProductValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Name.Required"));
            
            RuleFor(x => x.SeName)
                .Length(0, NopSeoDefaults.SearchEngineNameLength)
                .WithMessage(string.Format(localizationService.GetResource("Admin.SEO.SeName.MaxLengthValidation"), NopSeoDefaults.SearchEngineNameLength));
            
            RuleFor(x => x.RentalPriceLength)
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.RentalPriceLength.ShouldBeGreaterThanZero"))
                .When(x => x.IsRental);

            RuleFor(x => x.Weight)
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Weight.ShouldBeGreaterThanZero"))
                .When(x => x.IsShipEnabled || x.IsShipMandatory);

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Weight.Required"))
                .When(x => x.IsShipEnabled || x.IsShipMandatory);

            RuleFor(x => x.AdvancedOrderDay)
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.AdvancedOrderDay.ShouldBeGreaterThanZero"))
                .When(x => x.AdvancedOrderDay.HasValue);

            RuleFor(x => x.MonMaxNoOrderQty)
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.MonMaxNoOrderQty.ShouldBeGreaterThanZero"))
                .When(x => x.MonMaxNoOrderQty.HasValue);

            RuleFor(x => x.TueMaxNoOrderQty)
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.TueMaxNoOrderQty.ShouldBeGreaterThanZero"))
                .When(x => x.TueMaxNoOrderQty.HasValue);

            RuleFor(x => x.WedMaxNoOrderQty)
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.WedMaxNoOrderQty.ShouldBeGreaterThanZero"))
                .When(x => x.WedMaxNoOrderQty.HasValue);

            RuleFor(x => x.ThuMaxNoOrderQty)
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.ThuMaxNoOrderQty.ShouldBeGreaterThanZero"))
                .When(x => x.ThuMaxNoOrderQty.HasValue);

            RuleFor(x => x.FriMaxNoOrderQty)
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.FriMaxNoOrderQty.ShouldBeGreaterThanZero"))
                .When(x => x.FriMaxNoOrderQty.HasValue);

            RuleFor(x => x.SatMaxNoOrderQty)
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.SatMaxNoOrderQty.ShouldBeGreaterThanZero"))
                .When(x => x.SatMaxNoOrderQty.HasValue);

            RuleFor(x => x.SunMaxNoOrderQty)
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.SunMaxNoOrderQty.ShouldBeGreaterThanZero"))
                .When(x => x.SunMaxNoOrderQty.HasValue);

            RuleFor(x => x.AdminComment)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.AdminComment.Required.Rejection"))
                .When(x => x.IsRejectAction);

            RuleFor(x => x.SelectedCategoryIds.Count)
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.SelectedCategoryIds.ShouldBeAtLeastOne"));

            SetDatabaseValidationRules<Product>(dataProvider);
        }
    }
}