using FluentValidation;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Messages
{
    public partial class DisputeValidator : BaseNopValidator<DisputeSubmitModel>
    {
        public DisputeValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.PartialAmount).LessThanOrEqualTo(z => z.TotalReturnAmount).WithMessage(localizationService.GetResource("Admin.Dispute.Fields.PartialAmount.MoreThanOrder"));

            RuleFor(x => x.PartialAmount).NotEmpty().When(z => z.DisputeAction == (int)DisputeActionEnum.PartialRefund).WithMessage(localizationService.GetResource("Admin.Dispute.Fields.PartialAmount.Required"));
            SetDatabaseValidationRules<Dispute>(dataProvider);
        }
    }
}