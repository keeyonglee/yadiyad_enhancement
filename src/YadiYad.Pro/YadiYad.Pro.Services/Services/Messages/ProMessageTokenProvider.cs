using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.DepositRequest;
using YadiYad.Pro.Services.DTO.Engagement;
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Services.DTO.Job;
using YadiYad.Pro.Services.DTO.Moderator;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Services.DTO.Service;

namespace YadiYad.Pro.Services.Services.Messages
{
    /// <summary>
    /// Message token provider
    /// </summary>
    public partial class ProMessageTokenProvider
    {
        #region Fields

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ICustomerAttributeFormatter _customerAttributeFormatter;
        private readonly ICustomerService _customerService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly ConsultationJobSettings _consultationJobSettings;

        private readonly IDateTimeHelper _dateTimeHelper;

        private readonly PayoutRequestSettings _payoutRequestSettings;

        #endregion

        #region Ctor

        public ProMessageTokenProvider(
            PayoutRequestSettings payoutRequestSettings,
            ConsultationJobSettings consultationJobSettings,
            IDateTimeHelper dateTimeHelper,
            IActionContextAccessor actionContextAccessor,
            ICustomerAttributeFormatter customerAttributeFormatter,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IStoreService storeService,
            IUrlHelperFactory urlHelperFactory,
            StoreInformationSettings storeInformationSettings)
        {
            _consultationJobSettings = consultationJobSettings;
            _dateTimeHelper = dateTimeHelper;
            _payoutRequestSettings = payoutRequestSettings;
            _actionContextAccessor = actionContextAccessor;
            _customerAttributeFormatter = customerAttributeFormatter;
            _customerService = customerService;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _storeService = storeService;
            _urlHelperFactory = urlHelperFactory;
            _storeInformationSettings = storeInformationSettings;
        }

        #endregion

        /// <summary>
        /// Add store tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="store">Store</param>
        /// <param name="emailAccount">Email account</param>
        public virtual void AddStoreTokens(IList<Token> tokens, Store store, EmailAccount emailAccount)
        {
            if (emailAccount == null)
                throw new ArgumentNullException(nameof(emailAccount));

            tokens.Add(new Token("Store.Name", _localizationService.GetLocalized(store, x => x.Name)));
            tokens.Add(new Token("Store.URL", store.Url, true));
            tokens.Add(new Token("Store.Email", emailAccount.Email));
            tokens.Add(new Token("Store.CompanyName", store.CompanyName));
            tokens.Add(new Token("Store.CompanyAddress", store.CompanyAddress));
            tokens.Add(new Token("Store.CompanyPhoneNumber", store.CompanyPhoneNumber));
            tokens.Add(new Token("Store.CompanyVat", store.CompanyVat));

            tokens.Add(new Token("Facebook.URL", _storeInformationSettings.FacebookLink));
            tokens.Add(new Token("Twitter.URL", _storeInformationSettings.TwitterLink));
            tokens.Add(new Token("YouTube.URL", _storeInformationSettings.YoutubeLink));

            //event notification
            _eventPublisher.EntityTokensAdded(store, tokens);
        }

        /// <summary>
        /// Add customer tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="customer">Customer</param>
        public virtual void AddCustomerTokens(IList<Token> tokens, Nop.Core.Domain.Customers.Customer customer)
        {
            tokens.Add(new Token("Customer.Email", customer.Email));
            tokens.Add(new Token("Customer.Username", customer.Username));
            tokens.Add(new Token("Customer.FullName", _customerService.GetCustomerFullName(customer)));
            tokens.Add(new Token("Customer.FirstName", _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute)));
            tokens.Add(new Token("Customer.LastName", _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute)));
            tokens.Add(new Token("Customer.VatNumber", _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.VatNumberAttribute)));
            tokens.Add(new Token("Customer.VatNumberStatus", ((VatNumberStatus)_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.VatNumberStatusIdAttribute)).ToString()));

            var customAttributesXml = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CustomCustomerAttributes);
            tokens.Add(new Token("Customer.CustomAttributes", _customerAttributeFormatter.FormatAttributes(customAttributesXml), true));

            //note: we do not use SEO friendly URLS for these links because we can get errors caused by having .(dot) in the URL (from the email address)
            var passwordRecoveryUrl = RouteUrl(
                routeName: "PasswordRecoveryConfirm",
                controller: "Account",
                routeValues: new
                {
                    token = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute),
                    guid = customer.CustomerGuid,
                    area = "Pro"
                });
            var accountActivationUrl = RouteUrl(
                routeName: "AccountActivation",
                controller: "Account",
                routeValues: new
                {
                    token = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.AccountActivationTokenAttribute),
                    guid = customer.CustomerGuid,
                    area = "Pro"
                });
            var emailRevalidationUrl = RouteUrl(
                routeName: "EmailRevalidation",
                controller: "Account",
                routeValues: new
                {
                    token = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute),
                    guid = customer.CustomerGuid
                });
            var wishlistUrl = RouteUrl(routeName: "Wishlist", routeValues: new { customerGuid = customer.CustomerGuid });
            tokens.Add(new Token("Customer.PasswordRecoveryURL", passwordRecoveryUrl, true));
            tokens.Add(new Token("Customer.AccountActivationURL", accountActivationUrl, true));
            tokens.Add(new Token("Customer.EmailRevalidationURL", emailRevalidationUrl, true));
            tokens.Add(new Token("Wishlist.URLForCustomer", wishlistUrl, true));

            //event notification
            _eventPublisher.EntityTokensAdded(customer, tokens);
        }


        /// <summary>
        /// Generates an absolute URL for the specified store, routeName and route values
        /// </summary>
        /// <param name="storeId">Store identifier; Pass 0 to load URL of the current store</param>
        /// <param name="routeName">The name of the route that is used to generate URL</param>
        /// <param name="routeValues">An object that contains route values</param>
        /// <returns>Generated URL</returns>
        protected virtual string RouteUrl(int storeId = 0, string routeName = null, object routeValues = null, string controller = null)
        {
            //try to get a store by the passed identifier
            var store = _storeService.GetStoreById(storeId) ?? _storeContext.CurrentStore
                ?? throw new Exception("No store could be loaded");

            //ensure that the store URL is specified
            if (string.IsNullOrEmpty(store.Url))
                throw new Exception("URL cannot be null");

            //generate a URL with an absolute path
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            PathString url = null;

            if (string.IsNullOrWhiteSpace(controller) == false)
            {
                url = new PathString(urlHelper.Action(routeName, controller, routeValues));
            }
            else
            {
                url = new PathString(urlHelper.RouteUrl(routeName, routeValues));
            }

            //remove the application path from the generated URL if exists
            var pathBase = _actionContextAccessor.ActionContext?.HttpContext?.Request?.PathBase ?? PathString.Empty;
            url.StartsWithSegments(pathBase, out url);

            //compose the result
            return Uri.EscapeUriString(WebUtility.UrlDecode($"{store.Url.TrimEnd('/')}{url}"));
        }

        /// <summary>
        /// Add customer tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="customer">Customer</param>
        public virtual void AddJobServiceTokens(IList<Token> tokens, Nop.Core.Domain.Customers.Customer customer, JobProfile jobProfile,
            IndividualProfileDTO individualProfile)
        {
            tokens.Add(new Token("JobInvitation.JobTitle", jobProfile.JobTitle));
            tokens.Add(new Token("Individual.Fullname", individualProfile.FullName));

            //note: we do not use SEO friendly URLS for these links because we can get errors caused by having .(dot) in the URL (from the email address)
            var jobInvitationInviteUrl = RouteUrl(
                routeName: "Invites",
                controller: "JobInvitation",
                routeValues: new
                {
                    guid = customer.CustomerGuid,
                    area = "Pro"
                });
            var jobInvitationAcceptedUrl = RouteUrl(
               routeName: "Applicants",
               controller: "JobInvitation",
               routeValues: new
               {
                   guid = customer.CustomerGuid,
                   area = "Pro"
               });
            var jobInvitationRejectedUrl = RouteUrl(
               routeName: "Invited",
               controller: "JobInvitation",
               routeValues: new
               {
                   guid = customer.CustomerGuid,
                   area = "Pro"
               });
            var jobApplicationAppliedUrl = RouteUrl(
               routeName: "List",
               controller: "JobApplication",
               routeValues: new
               {
                   guid = customer.CustomerGuid,
                   area = "Pro"
               });
            tokens.Add(new Token("JobInvitation.InviteUrl", jobInvitationInviteUrl, true));
            tokens.Add(new Token("JobInvitation.AcceptUrl", jobInvitationAcceptedUrl, true));
            tokens.Add(new Token("JobInvitation.RejectUrl", jobInvitationRejectedUrl, true));
            tokens.Add(new Token("JobApplication.AppliedUrl", jobApplicationAppliedUrl, true));

            //event notification
            _eventPublisher.EntityTokensAdded(customer, tokens);
        }

        public virtual void AddServiceApplicationTokens(IList<Token> tokens, Nop.Core.Domain.Customers.Customer customer,
            ServiceProfileDTO dto)
        {
            tokens.Add(new Token("JobServiceCategory", dto.CategoryName));

            //note: we do not use SEO friendly URLS for these links because we can get errors caused by having .(dot) in the URL (from the email address)
            var serviceApplicationReceivesUrl = RouteUrl(
              routeName: "Receives",
              controller: "ServiceApplication",
              routeValues: new
              {
                  guid = customer.CustomerGuid,
                  area = "Pro"
              });
            var serviceApplicationRequestsUrl = RouteUrl(
             routeName: "Requests",
             controller: "ServiceApplication",
             routeValues: new
             {
                 guid = customer.CustomerGuid,
                 area = "Pro"
             });
            var serviceApplicationConfirmsUrl = RouteUrl(
             routeName: "Confirms",
             controller: "ServiceApplication",
             routeValues: new
             {
                 guid = customer.CustomerGuid,
                 area = "Pro"
             });
            var serviceApplicationHiresUrl = RouteUrl(
             routeName: "Hires",
             controller: "ServiceApplication",
             routeValues: new
             {
                 guid = customer.CustomerGuid,
                 area = "Pro"
             });
            tokens.Add(new Token("ServiceApplication.ReceivesUrl", serviceApplicationReceivesUrl, true));
            tokens.Add(new Token("ServiceApplication.RequestsUrl", serviceApplicationRequestsUrl, true));
            tokens.Add(new Token("ServiceApplication.ConfirmsUrl", serviceApplicationConfirmsUrl, true));
            tokens.Add(new Token("ServiceApplication.HiresUrl", serviceApplicationHiresUrl, true));

            //event notification
            _eventPublisher.EntityTokensAdded(customer, tokens);
        }

        public virtual void AddConsultationTokens(IList<Token> tokens, Nop.Core.Domain.Customers.Customer customer,
        ConsultationProfileDTO consultProfileDto, IndividualProfileDTO individualDto)
        {
            tokens.Add(new Token("ConsultationProfile.SegmentName", consultProfileDto.SegmentName));
            tokens.Add(new Token("ConsultationProfile.Topic", consultProfileDto.Topic));
            tokens.Add(new Token("ConsultationProfile.Objective", consultProfileDto.Objective));
            tokens.Add(new Token("Individual.Fullname", individualDto.FullName));

            //note: we do not use SEO friendly URLS for these links because we can get errors caused by having .(dot) in the URL (from the email address)
            //TODO change link to consultation
            var consultationCandidateInvitedUrl = RouteUrl(
              routeName: "Invites",
              controller: "JobInvitation",
              routeValues: new
              {
                  guid = customer.CustomerGuid,
                  area = "Pro"
              });
            var consultationOrganizationAcceptedUrl = RouteUrl(
             routeName: "Applicant",
             controller: "Consultation",
             routeValues: new
             {
                 guid = customer.CustomerGuid,
                 area = "Pro"
             });
            var consultationOrganizationDeclinedUrl = RouteUrl(
             routeName: "Applicant",
             controller: "Consultation",
             routeValues: new
             {
                 guid = customer.CustomerGuid,
                 area = "Pro"
             });
            var consultationCandidatePaidUrl = RouteUrl(
             routeName: "List",
             controller: "JobApplication",
             routeValues: new
             {
                 guid = customer.CustomerGuid,
                 area = "Pro"
             });
            tokens.Add(new Token("Consultation.Invite.Url", consultationCandidateInvitedUrl, true));
            tokens.Add(new Token("Consultation.Accepted.Url", consultationOrganizationAcceptedUrl, true));
            tokens.Add(new Token("Consultation.Declined.Url", consultationOrganizationDeclinedUrl, true));
            tokens.Add(new Token("Consultation.Paid.Url", consultationCandidatePaidUrl, true));

            //event notification
            _eventPublisher.EntityTokensAdded(customer, tokens);
        }

        public virtual void AddModeratorConsultationTokens(IList<Token> tokens, Nop.Core.Domain.Customers.Customer customerBuyer,
            Nop.Core.Domain.Customers.Customer customerSeller, ConsultationProfileDTO profile, ModeratorCancellationRequestDTO engagement)
        {

            tokens.Add(new Token("ConsultationInvite.InvitationAutoRejectAfterWorkingDays", _consultationJobSettings.InvitationAutoRejectAfterWorkingDays));
            tokens.Add(new Token("ConsultationProfile.SegmentName", profile.SegmentName));
            tokens.Add(new Token("ConsultationProfile.Topic", profile.Topic));
            tokens.Add(new Token("ConsultationProfile.Objective", profile.Objective));
            tokens.Add(new Token("Buyer.Name", engagement.BuyerName));
            tokens.Add(new Token("Seller.Name", engagement.SellerName));
            tokens.Add(new Token("ConsultationInvite.CancelledBy", engagement.CancelledBy));
            tokens.Add(new Token("ConsultationInvite.Reason", engagement.Reason));
            tokens.Add(new Token("ConsultationInvite.StartTime", engagement.AppointmentStartDate?.ToString("dd MMM yyyy hh:mm tt")));
            tokens.Add(new Token("ConsultationInvite.EndTime", engagement.AppointmentEndDate?.ToString("dd MMM yyyy hh:mm tt")));

            if (String.IsNullOrWhiteSpace(engagement.Remarks))
            {
                tokens.Add(new Token("ConsultationInvite.Remarks", engagement.Remarks));
            }
            else
            {
                tokens.Add(new Token("ConsultationInvite.Remarks", "-"));
            }

            //note: we do not use SEO friendly URLS for these links because we can get errors caused by having .(dot) in the URL (from the email address)
            //TODO change link to consultation
            var consultationCandidateInvitedUrl = RouteUrl(
              routeName: "Invites",
              controller: "JobInvitation",
              routeValues: new
              {
                  guid = customerBuyer.CustomerGuid,
                  area = "Pro"
              });

            tokens.Add(new Token("Consultation.Invite.Url", consultationCandidateInvitedUrl, true));

            //event notification
            //_eventPublisher.EntityTokensAdded(customer, tokens);
        }

        public virtual void AddBlockCustomerTokens(IList<Token> tokens, Nop.Core.Domain.Customers.Customer customerSeller, DateTime? endDate, int numberOfTimes, string remarks)
        {

            tokens.Add(new Token("Block.NumberOfTimes", numberOfTimes));
            tokens.Add(new Token("Block.EndDate", endDate));
            if (String.IsNullOrWhiteSpace(remarks))
            {
                tokens.Add(new Token("Block.Remarks", remarks));
            }
            else
            {
                tokens.Add(new Token("Block.Remarks", "-"));
            }

            //note: we do not use SEO friendly URLS for these links because we can get errors caused by having .(dot) in the URL (from the email address)
            //TODO change link to consultation
            var consultationCandidateInvitedUrl = RouteUrl(
              routeName: "Invites",
              controller: "JobInvitation",
              routeValues: new
              {
                  guid = customerSeller.CustomerGuid,
                  area = "Pro"
              });

            tokens.Add(new Token("Consultation.Invite.Url", consultationCandidateInvitedUrl, true));

            //event notification
            //_eventPublisher.EntityTokensAdded(customer, tokens);
        }

        public virtual void AddDepositRequestConfirmationTokens(IList<Token> tokens, Nop.Core.Domain.Customers.Customer customerSeller, int jobId, string remarks = "")
        {

            tokens.Add(new Token("Project.EngagementNo", jobId));
            
            if (String.IsNullOrWhiteSpace(remarks))
            {
                tokens.Add(new Token("Project.Deposit.Remarks", remarks));
            }
        }

        public virtual void AddDepositRequestNotificationTokens(IList<Token> tokens, Nop.Core.Domain.Customers.Customer customerSeller, DepositRequestDTO depositRequest)
        {
            tokens.Add(new Token("DepositRequest.DepositNumber", depositRequest.DepositNumber));
            tokens.Add(new Token("DepositRequest.OrderNumber", depositRequest.CustomOrderNumber));
            tokens.Add(new Token("DepositRequest.OrderItem.ItemName", depositRequest.ItemName));
            tokens.Add(new Token("DepositRequest.Amount", $"RM {depositRequest.Amount}"));
            tokens.Add(new Token("DepositRequest.StartToEnd", $"{depositRequest.CycleStart.ToShortDateString()} - {depositRequest.CycleEnd.ToShortDateString()}"));
            tokens.Add(new Token("DepositRequest.DueDate", depositRequest.DueDate.ToShortDateString()));
            if (depositRequest.ReminderCount != 0)
            {
                var ordinal = $"{depositRequest.ReminderCount}";
                switch (depositRequest.ReminderCount % 10)
                {
                    case 1:
                        ordinal += "st";
                        break;
                    case 2:
                        ordinal += "nd";
                        break;
                    case 3:
                        ordinal += "rd";
                        break;
                    default:
                        ordinal += "th";
                        break;
                }

                tokens.Add(new Token("DepositRequest.ReminderCount", ordinal));
            }

            //note: we do not use SEO friendly URLS for these links because we can get errors caused by having .(dot) in the URL (from the email address)
            //TODO change link to consultation
            string url = null;
            if (depositRequest.ProductTypeId == (int)ProductType.ServiceEnagegementFee)
            {
                url = RouteUrl(
                    routeName: "Confirms",
                    controller: "ServiceApplication",
                    routeValues: new
                    {
                        guid = customerSeller.CustomerGuid,
                        area = "Pro"
                    }
                );
            }
            else if (depositRequest.ProductTypeId == (int)ProductType.JobEnagegementFee)
            {
                url = RouteUrl(
                    routeName: "Applicants",
                    controller: "JobApplication",
                    routeValues: new
                    {
                        guid = customerSeller.CustomerGuid,
                        area = "Pro"
                    }
                );
            }

            tokens.Add(new Token("DepositRequest.ConfirmedOrder.Url", url, true));

            //event notification
            //_eventPublisher.EntityTokensAdded(customer, tokens);
        }

        public virtual void AddDepositRequestTerminatinAppNotificationTokensBuyer(IList<Token> tokens, Nop.Core.Domain.Customers.Customer customerSeller, DepositRequestDTO depositRequest, DateTime endDate)
        {
            tokens.Add(new Token("DepositRequest.DepositNumber", depositRequest.DepositNumber));
            tokens.Add(new Token("DepositRequest.OrderNumber", depositRequest.CustomOrderNumber));
            tokens.Add(new Token("DepositRequest.OrderItem.ItemName", depositRequest.ItemName));
            tokens.Add(new Token("DepositRequest.EndDate", endDate.ToShortDateString()));
            tokens.Add(new Token("ProductType", depositRequest.ProductTypeId == (int)ProductType.ServiceEnagegementFee ? "Service Application" : "Candidate Hired"));

            //note: we do not use SEO friendly URLS for these links because we can get errors caused by having .(dot) in the URL (from the email address)
            //TODO change link to consultation
            string url = null;
            if (depositRequest.ProductTypeId == (int)ProductType.ServiceEnagegementFee)
            {
                url = RouteUrl(
                    routeName: "Confirms",
                    controller: "ServiceApplication",
                    routeValues: new
                    {
                        guid = customerSeller.CustomerGuid,
                        area = "Pro"
                    }
                );
            }
            else if (depositRequest.ProductTypeId == (int)ProductType.JobEnagegementFee)
            {
                url = RouteUrl(
                    routeName: "Hired",
                    controller: "JobApplication",
                    routeValues: new
                    {
                        guid = customerSeller.CustomerGuid,
                        area = "Pro"
                    }
                );
            }

            tokens.Add(new Token("DepositRequest.ConfirmedOrder.Url", url, true));

            //event notification
            //_eventPublisher.EntityTokensAdded(customer, tokens);
        }

        public virtual void AddDepositRequestTerminatinAppNotificationTokensSeller(IList<Token> tokens, Nop.Core.Domain.Customers.Customer customerSeller, DepositRequestDTO depositRequest, DateTime endDate)
        {
            tokens.Add(new Token("DepositRequest.OrderNumber", depositRequest.CustomOrderNumber));
            tokens.Add(new Token("DepositRequest.OrderItem.ItemName", depositRequest.ItemName));
            tokens.Add(new Token("DepositRequest.EndDate", endDate.ToShortDateString()));
            tokens.Add(new Token("ProductType", depositRequest.ProductTypeId == (int)ProductType.ServiceEnagegementFee ? "Service Sold" : "Job Applied"));

            //note: we do not use SEO friendly URLS for these links because we can get errors caused by having .(dot) in the URL (from the email address)
            //TODO change link to consultation
            string url = null;
            if (depositRequest.ProductTypeId == (int)ProductType.ServiceEnagegementFee)
            {
                url = RouteUrl(
                    routeName: "Hires",
                    controller: "ServiceApplication",
                    routeValues: new
                    {
                        guid = customerSeller.CustomerGuid,
                        area = "Pro"
                    }
                );
            }
            else if (depositRequest.ProductTypeId == (int)ProductType.JobEnagegementFee)
            {
                url = RouteUrl(
                    routeName: "List",
                    controller: "JobApplication",
                    routeValues: new
                    {
                        guid = customerSeller.CustomerGuid,
                        area = "Pro"
                    }
                );
            }

            tokens.Add(new Token("DepositRequest.ConfirmedOrder.Url", url, true));

            //event notification
            //_eventPublisher.EntityTokensAdded(customer, tokens);
        }
        public virtual void AddDepositRequestPaymentVerificationNotificationTokens(IList<Token> tokens, DepositRequestDTO depositRequest)
        {
            tokens.Add(new Token("DepositRequest.DepositNumber", depositRequest.RefId));
            tokens.Add(new Token("DepositRequest.Amount", $"RM {depositRequest.Amount}"));

            string url = RouteUrl(
                    routeName: "List",
                    controller: "ApproveDepositRequest",
                    routeValues: new
                    {
                        area = "Admin"
                    }
                );

            tokens.Add(new Token("DepositRequest.PaymentVerification.Url", url, true));
        }

        public virtual void AddPayoutRequestTokens(
            IList<Token> tokens, 
            Nop.Core.Domain.Customers.Customer customer, 
            EngagementDTO engagement,
            PayoutRequestDTO payoutRequest)
        {
            tokens.Add(new Token("PayoutRequest.EngagementNo", engagement.EngagementNo));
            tokens.Add(new Token("PayoutRequest.StartDate", payoutRequest.StartDate?.ToShortDateString()));
            tokens.Add(new Token("PayoutRequest.EndDate", payoutRequest.EndDate?.ToShortDateString()));
            tokens.Add(new Token("PayoutRequest.JobMilestonePhase", payoutRequest.JobMilestonePhase));
            tokens.Add(new Token("PayoutRequest.JobMilestoneName", payoutRequest.JobMilestoneName));
            tokens.Add(new Token("PayoutRequest.AutoApprovalDays", _payoutRequestSettings.AutoApprovalDays));

            var timezone = _dateTimeHelper.DefaultStoreTimeZone;
            var hoursDiff = timezone.BaseUtcOffset.TotalHours;
            var payoutRequestAutoApprovalDate = payoutRequest.UpdatedOnUTC != null
                ? payoutRequest.UpdatedOnUTC.AddHours(hoursDiff).Date.AddDays(_payoutRequestSettings.AutoApprovalDays + 1)
                : payoutRequest.CreatedOnUTC.AddHours(hoursDiff).Date.AddDays(_payoutRequestSettings.AutoApprovalDays + 1);

            tokens.Add(new Token("PayoutRequest.AutoApprovalDate", payoutRequestAutoApprovalDate.ToShortDateString()));

            //note: we do not use SEO friendly URLS for these links because we can get errors caused by having .(dot) in the URL (from the email address)
            //TODO change link to consultation
            string url = null;
            if (payoutRequest.ProductTypeId == (int)ProductType.ServiceEnagegementFee)
            {
                url = RouteUrl(
                    routeName: "Confirms",
                    controller: "ServiceApplication",
                    routeValues: new
                    {
                        guid = customer.CustomerGuid,
                        area = "Pro"
                    }
                );
            }
            else if (payoutRequest.ProductTypeId == (int)ProductType.JobEnagegementFee)
            {
                url = RouteUrl(
                    routeName: "Applicants",
                    controller: "JobApplication",
                    routeValues: new
                    {
                        guid = customer.CustomerGuid,
                        area = "Pro"
                    }
                );
            }

            tokens.Add(new Token("PayoutRequest.ConfirmedOrder.Url", url, true));
        }

    }
}
