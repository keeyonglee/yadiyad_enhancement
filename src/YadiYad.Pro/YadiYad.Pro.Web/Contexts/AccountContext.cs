using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Http.Extensions;
using Nop.Services.Customers;
using Nop.Services.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Services.Organization;
using YadiYad.Pro.Services.Services.Subscription;
using YadiYad.Pro.Web.DTO;
using YadiYad.Pro.Web.Enums;

namespace YadiYad.Pro.Web.Contexts
{
    public class AccountContext
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWorkContext _workContext;
        private readonly OrganizationProfileService _organizationProfileService;
        private readonly IndividualProfileService _individualProfileService;
        private readonly DashboardService _dashboardService;
        private readonly ServiceSubscriptionService _serviceSubscriptionService;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        public AccountContext(
            IHttpContextAccessor httpContextAccessor,
            IWorkContext workContext,
            ICustomerService customerService,
            IPictureService pictureService,
            OrganizationProfileService organizationProfileService,
            IndividualProfileService individualProfileService,
            DashboardService dashboardService,
            ServiceSubscriptionService serviceSubscriptionService,
            MediaSettings mediaSettings)
        {
            _workContext = workContext;
            _customerService = customerService;
            _httpContextAccessor = httpContextAccessor;
            _organizationProfileService = organizationProfileService;
            _individualProfileService = individualProfileService;
            _dashboardService = dashboardService;
            _serviceSubscriptionService = serviceSubscriptionService;
            _pictureService = pictureService;
            _mediaSettings = mediaSettings;
        }

        #endregion


        public AccountDTO CurrentAccount
        {
            get
            {
                var currentCustomerId = _workContext.CurrentCustomer.Id;
                var currentCustomerEmail = _workContext.CurrentCustomer.Email;
                var roles = _customerService.GetCustomerRoles(_workContext.CurrentCustomer);

                if (string.IsNullOrWhiteSpace(currentCustomerEmail))
                {
                    ClearAccountSession();
                    return new AccountDTO();
                }

                var currentAccount = _httpContextAccessor.HttpContext?.Session.Get<AccountDTO>("CurrentProAccount");

                if (currentAccount == null
                    || currentAccount.AccountType == null)
                {
                    currentAccount = new AccountDTO();
                    int? pictureId = null;

                    if (roles.Any(x => x.SystemName == NopCustomerDefaults.IndividualRoleName))
                    {
                        var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(currentCustomerId);
                        var applyJobEntitledEndDateTime = _serviceSubscriptionService.GetApplyJobEntitledEndDateTime(currentCustomerId);

                        if (individualProfile != null)
                        {
                            currentAccount.AccountType = AccountType.Individual;
                            currentAccount.Name = individualProfile.NickName;
                            currentAccount.Salutation = individualProfile.TitleName;
                            currentAccount.AccountImageURL = individualProfile.ProfileImage;
                            currentAccount.Gender = individualProfile.GenderText;
                            currentAccount.ApplyJobEntitledEndDateTime = applyJobEntitledEndDateTime;
                            if (individualProfile.IsOnline == true)
                                currentAccount.Status = "Online";
                            else
                                currentAccount.Status = "Offline";
                            pictureId = individualProfile.PictureId;

                        }
                    } 
                    else if (roles.Any(x => x.SystemName == NopCustomerDefaults.OrganizationRoleName))
                    {
                        var organizationProfile = _organizationProfileService.GetOrganizationProfileByCustomerId(currentCustomerId);

                        if (organizationProfile != null)
                        {
                            currentAccount.AccountType = AccountType.Organization;
                            currentAccount.Name = currentAccount.Name ?? organizationProfile.Name;
                            currentAccount.OrgRegNo = organizationProfile.RegistrationNo;
                            currentAccount.OrgOfficeState = organizationProfile.Website;
                            pictureId = organizationProfile.PictureId;
                            currentAccount.OrganizationProfileId = organizationProfile.Id;
                        }
                    }
                    else if (roles.Any(x => x.SystemName == NopCustomerDefaults.ModeratorRoleName))
                    {
                        var customerName = _customerService.GetCustomerFullName(_workContext.CurrentCustomer);

                        currentAccount.AccountType = AccountType.Moderator;
                        currentAccount.Name = string.IsNullOrWhiteSpace(customerName)?_workContext.CurrentCustomer.Email: customerName;
                    }
                    else
                    {
                        currentAccount.AccountType = null;
                    }


                    if (pictureId.HasValue)
                    {
                        var picture = _pictureService.GetPictureById(pictureId.Value)
                                      ?? throw new Exception("Picture cannot be loaded");

                        currentAccount.AccountImageURL = _pictureService.GetPictureUrl(ref picture, _mediaSettings.ProductDetailsPictureSize);
                    }
                    currentAccount.Email = _workContext.CurrentCustomer.Email;
                    _httpContextAccessor.HttpContext?.Session.Set<AccountDTO>("CurrentProAccount", currentAccount);
                }
                
                if (currentAccount != null && roles.Any(x => x.SystemName == NopCustomerDefaults.IndividualRoleName))
                {
                    currentAccount = GetOnlineStatus();
                }

                return currentAccount;
            }
        }

        public AccountDTO GetOnlineStatus()
        {
            var currentCustomerId = _workContext.CurrentCustomer.Id;
            var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(currentCustomerId);

            var currentAccount = _httpContextAccessor.HttpContext?.Session.Get<AccountDTO>("CurrentProAccount");

            if (individualProfile != null)
            {
                if (individualProfile.IsOnline == true)
                    currentAccount.Status = "Online";
                else
                    currentAccount.Status = "Offline";
            }
            
            return currentAccount;
        }

        public void ClearAccountSession()
        {
            _httpContextAccessor.HttpContext?.Session.Remove("CurrentProAccount");
        }

    }
}
