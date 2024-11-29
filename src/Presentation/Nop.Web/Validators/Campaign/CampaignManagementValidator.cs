using FluentValidation;
using Nop.Web.Areas.Pro.Models.CampaignManagement;
using Nop.Web.Framework.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Validators.Campaign
{
    public partial class CampaignManagementValidator : BaseNopValidator<CampaignManagementModel>
    {
        public CampaignManagementValidator()
        {
            RuleFor(x => x.TransactionLimit)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Transaction Limit must not be negative");
            RuleFor(x => x.Value1)
                .GreaterThan(0)
                .WithMessage("Primary value must be more than zero");
            RuleFor(x => x.Value2)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Secondary value must not be negative");
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required");
            RuleFor(x => x.Beneficiary)
                .NotEmpty()
                .WithMessage("Beneficiary is required");
            RuleFor(x => x.Type)
                .NotEmpty()
                .WithMessage("Type is required");
            RuleFor(x => x.Activity)
                .NotEmpty()
                .WithMessage("Activity is required");

            RuleFor(x => x.From).Must((x, context) =>
            {
                if (x.From >= x.Until)
                {
                    return false;
                }

                return true;
            }).WithMessage("Effective Until must be later than Effective From");
        }
    }
}
