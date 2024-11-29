using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Web.Models.Customer;

namespace YadiYad.Pro.Web.Validators
{
    public partial class PasswordChangeProValidator : BaseNopValidator<PasswordChangeProModel>
    {
        public PasswordChangeProValidator(ILocalizationService localizationService, CustomerSettings customerSettings)
        {
            RuleFor(x => x.OldPassword).NotEmpty().WithMessage(localizationService.GetResource("Account.ChangePassword.Fields.OldPassword.Required"));
            RuleFor(x => x.NewPassword).IsPassword(localizationService, customerSettings);
            RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessage(localizationService.GetResource("Account.ChangePassword.Fields.ConfirmNewPassword.Required"));
            RuleFor(x => x.ConfirmNewPassword).Equal(x => x.NewPassword).WithMessage(localizationService.GetResource("Account.ChangePassword.Fields.NewPassword.EnteredPasswordsDoNotMatch"));
        }
                
    }
}
