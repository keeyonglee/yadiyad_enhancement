using FluentValidation;
using Nop.Web.Areas.Pro.Models.ApproveDepositRequest;
using Nop.Web.Framework.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Validators.DepositRequest
{
    public partial class ApproveDepositRequestValidator : BaseNopValidator<ApproveDepositRequestModel>
    {
        public ApproveDepositRequestValidator()
        {
            RuleFor(x => x.Validity)
                .NotEmpty()
                .WithMessage("Validity of Bank in Slip is required");
        }
    }
}
