using Nop.Web.Areas.Pro.Models.Charge;
using Nop.Web.Framework.Validators;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Validators.Order
{
    public partial class ChargeValidator : BaseNopValidator<ChargeModel>
    {
        public ChargeValidator()
        {
            RuleFor(x => x.Value).Must((x, context) =>
            {
                if (x.Value < 0)
                {
                    return false;
                }

                return true;
            }).WithMessage("Value must be positive");

            RuleFor(x => x.ValidityDays).Must((x, context) =>
            {
                if (x.ValidityDays < 0)
                {
                    return false;
                }

                return true;
            }).WithMessage("Validity Days must be positive");

            RuleFor(x => x.StartDate).Must((x, context) =>
            {
                if (x.EndDateNull == false)
                {
                    if (x.StartDate >= x.EndDate)
                    {
                        return false;
                    }

                }

                return true;
            }).WithMessage("Start Date must be before End Date");

            RuleFor(x => x.MinRange).Must((x, context) =>
            {
                if (x.MinRange < 0)
                {
                    return false;
                }

                return true;
            }).WithMessage("Min Range must be positive");

            RuleFor(x => x.MaxRange).Must((x, context) =>
            {
                if (x.MaxRange < 0)
                {
                    return false;
                }

                return true;
            }).WithMessage("Max Range must be positive");

            RuleFor(x => x.MinRange).Must((x, context) =>
            {
                if (x.MinRange >= x.MaxRange)
                {
                    return false;
                }

                return true;
            }).WithMessage("Max Range must be more than Min Range");

        }
    }
}
