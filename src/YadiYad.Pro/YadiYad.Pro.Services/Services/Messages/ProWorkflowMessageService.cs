using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Messages;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Core.Domain.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Messages;
using YadiYad.Pro.Services.DTO.Job;
using YadiYad.Pro.Services.Job;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.Service;
using YadiYad.Pro.Services.Organization;
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.Consultation;
using YadiYad.Pro.Services.JobSeeker;
using YadiYad.Pro.Services.Services.Moderator;
using YadiYad.Pro.Services.Services.Common;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Services.DTO.DepositRequest;
using YadiYad.Pro.Core.Domain.Order;
using System.Net;
using YadiYad.Pro.Core.Domain.Home;
using YadiYad.Pro.Services.Services.Engagement;

namespace YadiYad.Pro.Services.Services.Messages
{
    public class ProWorkflowMessageService
    {
        #region Fields

        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly ICustomerService _customerService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly ProMessageTokenProvider _proMessageTokenProvider;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ITokenizer _tokenizer;
        private readonly JobProfileService _jobProfileService;
        private readonly IndividualProfileService _individualProfileService;
        private readonly ServiceApplicationService _serviceApplicationService;
        private readonly ServiceProfileService _serviceProfileService;
        private readonly JobApplicationService _jobApplicationService;
        private readonly JobInvitationService _jobInvitationService;
        private readonly OrganizationProfileService _organizationProfileService;
        private readonly ConsultationProfileService _consultationProfileService;
        private readonly JobSeekerProfileService _jobSeekerProfileService;
        private readonly ModeratorCancellationRequestService _moderatorCancellationRequestService;
        private readonly BlockCustomerService _blockCustomerService;
        private readonly CancellationReasonService _cancellationReasonService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly CommonSettings _commonSettings;
        private readonly EngagementService _engagementService;
        #endregion

        #region Ctor

        public ProWorkflowMessageService(
            EmailAccountSettings emailAccountSettings,
            ICustomerService customerService,
            IEmailAccountService emailAccountService,
            IEventPublisher eventPublisher,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IMessageTemplateService messageTemplateService,
            ProMessageTokenProvider proMessageTokenProvider,
            IOrderService orderService,
            IProductService productService,
            IQueuedEmailService queuedEmailService,
            IStoreContext storeContext,
            IStoreService storeService,
            ITokenizer tokenizer,
            JobProfileService jobProfileService,
            IndividualProfileService individualProfileService,
            ServiceApplicationService serviceApplicationService,
            ServiceProfileService serviceProfileService,
            JobApplicationService jobApplicationService,
            JobInvitationService jobInvitationService,
            OrganizationProfileService organizationProfileService,
            ConsultationProfileService consultationProfileService,
            JobSeekerProfileService jobSeekerProfileService,
            ModeratorCancellationRequestService moderatorCancellationRequestService,
            BlockCustomerService blockCustomerService,
            CancellationReasonService cancellationReasonService,
            IMessageTokenProvider messageTokenProvider,
            CommonSettings commonSettings,
            EngagementService engagementService)
        {
            _engagementService = engagementService;
            _emailAccountSettings = emailAccountSettings;
            _customerService = customerService;
            _emailAccountService = emailAccountService;
            _eventPublisher = eventPublisher;
            _languageService = languageService;
            _localizationService = localizationService;
            _messageTemplateService = messageTemplateService;
            _proMessageTokenProvider = proMessageTokenProvider;
            _orderService = orderService;
            _productService = productService;
            _queuedEmailService = queuedEmailService;
            _storeContext = storeContext;
            _storeService = storeService;
            _tokenizer = tokenizer;
            _jobProfileService = jobProfileService;
            _individualProfileService = individualProfileService;
            _serviceApplicationService = serviceApplicationService;
            _serviceProfileService = serviceProfileService;
            _jobApplicationService = jobApplicationService;
            _jobInvitationService = jobInvitationService;
            _organizationProfileService = organizationProfileService;
            _consultationProfileService = consultationProfileService;
            _jobSeekerProfileService = jobSeekerProfileService;
            _moderatorCancellationRequestService = moderatorCancellationRequestService;
            _blockCustomerService = blockCustomerService;
            _cancellationReasonService = cancellationReasonService;
            _messageTokenProvider = messageTokenProvider;
            _commonSettings = commonSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get active message templates by the name
        /// </summary>
        /// <param name="messageTemplateName">Message template name</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>List of message templates</returns>
        protected virtual IList<MessageTemplate> GetActiveMessageTemplates(string messageTemplateName, int storeId)
        {
            //get message templates by the name
            var messageTemplates = _messageTemplateService.GetMessageTemplatesByName(messageTemplateName, storeId);

            //no template found
            if (!messageTemplates?.Any() ?? true)
                return new List<MessageTemplate>();

            //filter active templates
            messageTemplates = messageTemplates.Where(messageTemplate => messageTemplate.IsActive).ToList();

            return messageTemplates;
        }

        /// <summary>
        /// Get EmailAccount to use with a message templates
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>EmailAccount</returns>
        protected virtual EmailAccount GetEmailAccountOfMessageTemplate(MessageTemplate messageTemplate, int languageId)
        {
            var emailAccountId = _localizationService.GetLocalized(messageTemplate, mt => mt.EmailAccountId, languageId);
            //some 0 validation (for localizable "Email account" dropdownlist which saves 0 if "Standard" value is chosen)
            if (emailAccountId == 0)
                emailAccountId = messageTemplate.EmailAccountId;

            var emailAccount = (_emailAccountService.GetEmailAccountById(emailAccountId) ?? _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId)) ??
                               _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
            return emailAccount;
        }

        /// <summary>
        /// Ensure language is active
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Return a value language identifier</returns>
        protected virtual int EnsureLanguageIsActive(int languageId, int storeId)
        {
            //load language by specified ID
            var language = _languageService.GetLanguageById(languageId);

            if (language == null || !language.Published)
            {
                //load any language from the specified store
                language = _languageService.GetAllLanguages(storeId: storeId).FirstOrDefault();
            }

            if (language == null || !language.Published)
            {
                //load any language
                language = _languageService.GetAllLanguages().FirstOrDefault();
            }

            if (language == null)
                throw new Exception("No active language could be loaded");

            return language.Id;
        }

        #endregion

        #region Methods

        #region Contact Us

        public virtual IList<int> SendContactUsMessage(int languageId, string senderEmail,
            string senderName, string subject, string body, int contactUsSubject)
        {
            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var template = "";
            switch (contactUsSubject)
            {
                case (int)ContactUsSubject.GeneralEnquiry:
                case (int)ContactUsSubject.Collaboration:
                case (int)ContactUsSubject.CSR:
                case (int)ContactUsSubject.Others:
                    template = MessageTemplateProNames.ContactUsEnquiry;
                    break;
                case (int)ContactUsSubject.Complains:
                case (int)ContactUsSubject.Dispute:
                case (int)ContactUsSubject.CancellationAndRefund:
                    template = MessageTemplateProNames.ContactUsIssues;
                    break;
                case (int)ContactUsSubject.Feedbacks:
                case (int)ContactUsSubject.Suggestions:
                case (int)ContactUsSubject.Testimonials:
                    template = MessageTemplateProNames.ContactUsFeedbacks;
                    break;
                default:
                    break;
            }

            var messageTemplates = GetActiveMessageTemplates(template, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>
            {
                new Token("ContactUs.SenderEmail", senderEmail),
                new Token("ContactUs.SenderName", senderName),
                new Token("ContactUs.Body", body, true)
            };

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                string fromEmail;
                string fromName;

                fromEmail = emailAccount.Email;
                fromName = emailAccount.DisplayName;
                body = $"<strong>From</strong>: {WebUtility.HtmlEncode(senderName)} - {WebUtility.HtmlEncode(senderEmail)}<br /><br />{body}";

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                    fromEmail: fromEmail,
                    fromName: fromName,
                    subject: subject,
                    replyToEmailAddress: senderEmail,
                    replyToName: senderName);
            }).ToList();
        }

        #endregion

        #region Account

        /// <summary>
        /// Sends password recovery message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendCustomerPasswordRecoveryMessage(Nop.Core.Domain.Customers.Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.CustomerPasswordRecoveryMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddCustomerTokens(commonTokens, customer);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = customer.Email;
                var toName = _customerService.GetCustomerFullName(customer);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="toEmailAddress">Recipient email address</param>
        /// <param name="toName">Recipient name</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name</param>
        /// <param name="replyToEmailAddress">"Reply to" email</param>
        /// <param name="replyToName">"Reply to" name</param>
        /// <param name="fromEmail">Sender email. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="fromName">Sender name. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="subject">Subject. If specified, then it overrides subject of a message template</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNotification(MessageTemplate messageTemplate,
            EmailAccount emailAccount, int languageId, IEnumerable<Token> tokens,
            string toEmailAddress, string toName,
            string attachmentFilePath = null, string attachmentFileName = null,
            string replyToEmailAddress = null, string replyToName = null,
            string fromEmail = null, string fromName = null, string subject = null)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException(nameof(messageTemplate));

            if (emailAccount == null)
                throw new ArgumentNullException(nameof(emailAccount));

            //retrieve localized message template data
            var bcc = _localizationService.GetLocalized(messageTemplate, mt => mt.BccEmailAddresses, languageId);
            if (string.IsNullOrEmpty(subject))
                subject = _localizationService.GetLocalized(messageTemplate, mt => mt.Subject, languageId);
            var body = _localizationService.GetLocalized(messageTemplate, mt => mt.Body, languageId);

            //Replace subject and body tokens 
            var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);

            //limit name length
            toName = CommonHelper.EnsureMaximumLength(toName, 300);

            var email = new QueuedEmail
            {
                Priority = QueuedEmailPriority.High,
                From = !string.IsNullOrEmpty(fromEmail) ? fromEmail : emailAccount.Email,
                FromName = !string.IsNullOrEmpty(fromName) ? fromName : emailAccount.DisplayName,
                To = toEmailAddress,
                ToName = toName,
                ReplyTo = replyToEmailAddress,
                ReplyToName = replyToName,
                CC = string.Empty,
                Bcc = bcc,
                Subject = subjectReplaced,
                Body = bodyReplaced,
                AttachmentFilePath = attachmentFilePath,
                AttachmentFileName = attachmentFileName,
                AttachedDownloadId = messageTemplate.AttachedDownloadId,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id,
                DontSendBeforeDateUtc = !messageTemplate.DelayBeforeSend.HasValue ? null
                    : (DateTime?)(DateTime.UtcNow + TimeSpan.FromHours(messageTemplate.DelayPeriod.ToHours(messageTemplate.DelayBeforeSend.Value)))
            };

            _queuedEmailService.InsertQueuedEmail(email);
            return email.Id;
        }

        /// <summary>
        /// Sends an email validation message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendCustomerEmailValidationMessage(Nop.Core.Domain.Customers.Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.CustomerEmailValidationMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddCustomerTokens(commonTokens, customer);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = customer.Email;
                var toName = _customerService.GetCustomerFullName(customer);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a welcome message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendCustomerWelcomeMessage(Nop.Core.Domain.Customers.Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.CustomerWelcomeMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddCustomerTokens(commonTokens, customer);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = customer.Email;
                var toName = _customerService.GetCustomerFullName(customer);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        #endregion

        #region Job Invitation

        public virtual IList<int> SendIndividualJobInvitationInvitedMessage(JobInvitationDTO dto, int languageId)
        {
            var customer = _customerService.GetCustomerById(dto.ServiceIndividualProfile.CustomerId);
            var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(dto.ServiceIndividualProfile.CustomerId);
            var jobprofile = _jobProfileService.GetJobProfileDomainById(dto.JobProfileId);
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.JobInvitationIndividualInvite, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddJobServiceTokens(commonTokens, customer, jobprofile, individualProfile);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = individualProfile.Email;
                var toName = individualProfile.FullName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        public virtual IList<int> SendOrganisationJobInvitationAcceptedMessage(int jobInvitationId, int languageId)
        {
            var jobInvitationProfileDto = _jobInvitationService.GetJobInvitationById(jobInvitationId);
            var jobProfileDto = _jobProfileService.GetJobProfileDomainById(jobInvitationProfileDto.JobProfileId);
            var jobSeekerProfileDto = _jobSeekerProfileService.GetJobSeekerProfileById(jobInvitationProfileDto.JobSeekerProfileId);
            var customer = _customerService.GetCustomerById(jobProfileDto.CustomerId);
            var orgProfileDto = _organizationProfileService.GetOrganizationProfileByCustomerId(jobProfileDto.CustomerId);
            var individualProfileDto = _individualProfileService.GetIndividualProfileByCustomerId(jobSeekerProfileDto.CustomerId);
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.JobInvitationOrganisationAccepted, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddJobServiceTokens(commonTokens, customer, jobProfileDto, individualProfileDto);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = orgProfileDto.ContactPersonEmail;
                var toName = orgProfileDto.ContactPersonName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        public virtual IList<int> SendOrganisationJobInvitationRejectedMessage(int jobInvitationId, int languageId)
        {
            var jobInvitationProfileDto = _jobInvitationService.GetJobInvitationById(jobInvitationId);
            var jobProfileDto = _jobProfileService.GetJobProfileDomainById(jobInvitationProfileDto.JobProfileId);
            var jobSeekerProfileDto = _jobSeekerProfileService.GetJobSeekerProfileById(jobInvitationProfileDto.JobSeekerProfileId);
            var customer = _customerService.GetCustomerById(jobProfileDto.CustomerId);
            var orgProfileDto = _organizationProfileService.GetOrganizationProfileByCustomerId(jobProfileDto.CustomerId);
            var individualProfileDto = _individualProfileService.GetIndividualProfileByCustomerId(jobSeekerProfileDto.CustomerId);

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.JobInvitationOrganisationRejected, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddJobServiceTokens(commonTokens, customer, jobProfileDto, individualProfileDto);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = orgProfileDto.ContactPersonEmail;
                var toName = orgProfileDto.ContactPersonName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        #endregion

        #region Job Application

        public virtual IList<int> SendIndividualJobApplicationShortlist(int jobApplicationId, int languageId)
        {
            var jobApplicationDto = _jobApplicationService.GetJobApplicationById(jobApplicationId);
            var jobSeekerProfileDto = _jobSeekerProfileService.GetJobSeekerProfileById(jobApplicationDto.JobSeekerProfileId);
            var customer = _customerService.GetCustomerById(jobSeekerProfileDto.Id);
            var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(jobSeekerProfileDto.CustomerId);
            var jobprofile = _jobProfileService.GetJobProfileDomainById(jobApplicationDto.JobProfileId);
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.JobApplicationIndividualUnderShortlist, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddJobServiceTokens(commonTokens, customer, jobprofile, individualProfile);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = individualProfile.Email;
                var toName = individualProfile.FullName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        public virtual IList<int> SendIndividualJobApplicationFutureReference(int jobApplicationId, int languageId)
        {
            var jobApplicationDto = _jobApplicationService.GetJobApplicationById(jobApplicationId);
            var jobSeekerProfileDto = _jobSeekerProfileService.GetJobSeekerProfileById(jobApplicationDto.JobSeekerProfileId);
            var customer = _customerService.GetCustomerById(jobSeekerProfileDto.Id);
            var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(jobSeekerProfileDto.CustomerId);
            var jobprofile = _jobProfileService.GetJobProfileDomainById(jobApplicationDto.JobProfileId);
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.JobApplicationIndividualKeepForFuture, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddJobServiceTokens(commonTokens, customer, jobprofile, individualProfile);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = individualProfile.Email;
                var toName = individualProfile.FullName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        public virtual IList<int> SendIndividualJobApplicationHire(int jobApplicationId, int languageId)
        {
            var jobApplicationDto = _jobApplicationService.GetJobApplicationById(jobApplicationId);
            var jobSeekerProfileDto = _jobSeekerProfileService.GetJobSeekerProfileById(jobApplicationDto.JobSeekerProfileId);
            var customer = _customerService.GetCustomerById(jobSeekerProfileDto.Id);
            var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(jobSeekerProfileDto.CustomerId);
            var jobprofile = _jobProfileService.GetJobProfileDomainById(jobApplicationDto.JobProfileId);
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.JobApplicationIndividualHire, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddJobServiceTokens(commonTokens, customer, jobprofile, individualProfile);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = individualProfile.Email;
                var toName = individualProfile.FullName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        #endregion

        #region Service Application

        public virtual IList<int> SendServiceSellerRequest(ServiceApplicationDTO dto, int languageId)
        {
            var serviceProfileDto = _serviceProfileService.GetServiceProfileById(dto.ServiceProfileId);
            var customer = _customerService.GetCustomerById(serviceProfileDto.CustomerId);
            var individualProfileDto = _individualProfileService.GetIndividualProfileByCustomerId(serviceProfileDto.CustomerId);

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.ServiceSellerRequest, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddServiceApplicationTokens(commonTokens, customer, serviceProfileDto);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = individualProfileDto.Email;
                var toName = individualProfileDto.FullName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        public virtual IList<int> SendServiceBuyerAccepted(ServiceApplicationDTO dto, int languageId)
        {
            var serviceApplicationDto = _serviceApplicationService.GetServiceApplicationById(dto.Id);
            var serviceProfileDto = _serviceProfileService.GetServiceProfileById(serviceApplicationDto.ServiceProfileId);
            var customer = _customerService.GetCustomerById(serviceProfileDto.CustomerId);
            var individualProfileDto = _individualProfileService.GetIndividualProfileByCustomerId(serviceProfileDto.CustomerId);

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.ServiceBuyerAccepted, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddServiceApplicationTokens(commonTokens, customer, serviceProfileDto);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = individualProfileDto.Email;
                var toName = individualProfileDto.FullName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        public virtual IList<int> SendServiceBuyerDeclined(ServiceApplicationDTO dto, int languageId)
        {
            var serviceApplicationDto = _serviceApplicationService.GetServiceApplicationById(dto.Id);
            var serviceProfileDto = _serviceProfileService.GetServiceProfileById(serviceApplicationDto.ServiceProfileId);
            var customer = _customerService.GetCustomerById(serviceProfileDto.CustomerId);
            var individualProfileDto = _individualProfileService.GetIndividualProfileByCustomerId(serviceProfileDto.CustomerId);

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.ServiceBuyerDeclined, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddServiceApplicationTokens(commonTokens, customer, serviceProfileDto);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = individualProfileDto.Email;
                var toName = individualProfileDto.FullName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        public virtual IList<int> SendServiceBuyerReproposed(ServiceApplicationDTO dto, int languageId)
        {
            var serviceApplicationDto = _serviceApplicationService.GetServiceApplicationById(dto.Id);
            var serviceProfileDto = _serviceProfileService.GetServiceProfileById(serviceApplicationDto.ServiceProfileId);
            var customer = _customerService.GetCustomerById(serviceProfileDto.CustomerId);
            var individualProfileDto = _individualProfileService.GetIndividualProfileByCustomerId(serviceProfileDto.CustomerId);

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.ServiceBuyerReproposed, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddServiceApplicationTokens(commonTokens, customer, serviceProfileDto);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = individualProfileDto.Email;
                var toName = individualProfileDto.FullName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        public virtual IList<int> SendServiceBuyerConfirm(int serviceApplicationId, int languageId)
        {
            var serviceApplicationDto = _serviceApplicationService.GetServiceApplicationById(serviceApplicationId);
            var serviceProfileDto = _serviceProfileService.GetServiceProfileById(serviceApplicationDto.ServiceProfileId);
            var customer = _customerService.GetCustomerById(serviceApplicationDto.CustomerId);
            var individualProfileDto = _individualProfileService.GetIndividualProfileByCustomerId(serviceApplicationDto.CustomerId);

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.ServiceBuyerConfirm, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddServiceApplicationTokens(commonTokens, customer, serviceProfileDto);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = individualProfileDto.Email;
                var toName = individualProfileDto.FullName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        public virtual IList<int> SendServiceSellerConfirm(int serviceApplicationId, int languageId)
        {
            var serviceApplicationDto = _serviceApplicationService.GetServiceApplicationById(serviceApplicationId);
            var serviceProfileDto = _serviceProfileService.GetServiceProfileById(serviceApplicationDto.ServiceProfileId);
            var customer = _customerService.GetCustomerById(serviceProfileDto.CustomerId);
            var individualProfileDto = _individualProfileService.GetIndividualProfileByCustomerId(serviceProfileDto.CustomerId);

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.ServiceSellerConfirm, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddServiceApplicationTokens(commonTokens, customer, serviceProfileDto);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = individualProfileDto.Email;
                var toName = individualProfileDto.FullName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }


        #endregion

        #region Consultation

        public virtual IList<int> SendConsultationCandidateInvited(int languageId, ConsultationProfileDTO consultProfileDto, IndividualProfileDTO individualDto)
        {
            var customer = _customerService.GetCustomerById(individualDto.CustomerId);

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.ConsultationCandidateInvited, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddConsultationTokens(commonTokens, customer, consultProfileDto, individualDto);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = individualDto.Email;
                var toName = individualDto.FullName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        public virtual IList<int> SendConsultationOrganizationAccepted(int languageId, ConsultationInvitationDTO consultInvitationDto)
        {
            var consultProfileDto = _consultationProfileService.GetConsultationProfileById(consultInvitationDto.ConsultationProfileId);
            var individualProfileDto = _individualProfileService.GetIndividualProfileByCustomerId(consultInvitationDto.IndividualCustomerId);
            var orgProfileDto = _organizationProfileService.GetOrganizationProfileById(consultInvitationDto.OrganizationProfileId);
            var customer = _customerService.GetCustomerById(consultInvitationDto.IndividualCustomerId);

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.ConsultationOrganizationAccepted, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddConsultationTokens(commonTokens, customer, consultProfileDto, individualProfileDto);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = orgProfileDto.ContactPersonEmail;
                var toName = orgProfileDto.ContactPersonName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        public virtual IList<int> SendConsultationOrganizationDeclined(int languageId, ConsultationProfileDTO consultProfileDto, IndividualProfileDTO individualDto)
        {
            var customer = _customerService.GetCustomerById(individualDto.CustomerId);
            var orgProfileDto = _organizationProfileService.GetOrganizationProfileById(consultProfileDto.OrganizationProfileId);

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.ConsultationOrganizationDeclined, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddConsultationTokens(commonTokens, customer, consultProfileDto, individualDto);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = orgProfileDto.ContactPersonEmail;
                var toName = orgProfileDto.ContactPersonName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        public virtual IList<int> SendConsultationCandidatePaid(int languageId, ConsultationInvitationDTO consultInvitationDto)
        {
            var consultProfileDto = _consultationProfileService.GetConsultationProfileById(consultInvitationDto.ConsultationProfileId);
            var individualDto = _individualProfileService.GetIndividualProfileByCustomerId(consultInvitationDto.IndividualCustomerId);
            var customer = _customerService.GetCustomerById(individualDto.CustomerId);

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.ConsultationCandidatePaid, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddConsultationTokens(commonTokens, customer, consultProfileDto, individualDto);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = individualDto.Email;
                var toName = individualDto.FullName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        public virtual IList<int> SendConsultationOrganizationAutoDeclined(int languageId, int consultationInvitationId)
        {
            var engagement = _moderatorCancellationRequestService.GetModeratorConsultantDTO(consultationInvitationId);
            var consultationProfileDTO = _consultationProfileService.GetConsultationProfileById(engagement.ConsultationProfileId);
            var recipientBuyer = _customerService.GetCustomerById(engagement.BuyerId);
            var recipientSeller = _customerService.GetCustomerById(engagement.SellerId);
            var toEmailBuyer = engagement.BuyerEmail;
            var toNameBuyer = engagement.BuyerName;
            var toEmailSeller = engagement.SellerEmail;
            var toNameSeller = engagement.SellerName;

            if (recipientBuyer == null)
                throw new ArgumentNullException(nameof(recipientBuyer));
            if (recipientSeller == null)
                throw new ArgumentNullException(nameof(recipientSeller));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.ConsultationOrganizationAutoDeclined, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddModeratorConsultationTokens(commonTokens, recipientBuyer, recipientSeller, consultationProfileDTO, engagement);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = toEmailBuyer;
                var toName = toNameBuyer;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        public virtual IList<int> SendConsultationConsultantAutoDeclined(int languageId, int consultationInvitationId)
        {
            var engagement = _moderatorCancellationRequestService.GetModeratorConsultantDTO(consultationInvitationId);
            var consultationProfileDTO = _consultationProfileService.GetConsultationProfileById(engagement.ConsultationProfileId);
            var recipientBuyer = _customerService.GetCustomerById(engagement.BuyerId);
            var recipientSeller = _customerService.GetCustomerById(engagement.SellerId);
            var toEmailBuyer = engagement.BuyerEmail;
            var toNameBuyer = engagement.BuyerName;
            var toEmailSeller = engagement.SellerEmail;
            var toNameSeller = engagement.SellerName;

            if (recipientBuyer == null)
                throw new ArgumentNullException(nameof(recipientBuyer));
            if (recipientSeller == null)
                throw new ArgumentNullException(nameof(recipientSeller));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.ConsultationConsultantAutoDeclined, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddModeratorConsultationTokens(commonTokens, recipientBuyer, recipientSeller, consultationProfileDTO, engagement);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = toEmailSeller;
                var toName = toNameSeller;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }



        #endregion

        #region Moderator

        public virtual void SendConsultationCompleted(int languageId, int consultationInvitationId)
        {
            var engagement = _moderatorCancellationRequestService.GetModeratorConsultantDTO(consultationInvitationId);
            var consultationProfileDTO = _consultationProfileService.GetConsultationProfileById(engagement.ConsultationProfileId);
            var recipientBuyer = _customerService.GetCustomerById(engagement.BuyerId);
            var recipientSeller = _customerService.GetCustomerById(engagement.SellerId);
            var toEmailBuyer = engagement.BuyerEmail;
            var toNameBuyer = engagement.BuyerName;
            var toEmailSeller = engagement.SellerEmail;
            var toNameSeller = engagement.SellerName;

            if (recipientBuyer == null)
                throw new ArgumentNullException(nameof(recipientBuyer));
            if (recipientSeller == null)
                throw new ArgumentNullException(nameof(recipientSeller));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var messageTemplateBuyer = GetActiveMessageTemplates(MessageTemplateProNames.ConsultationBuyerCompleted, store.Id);
            var messageTemplateSeller = GetActiveMessageTemplates(MessageTemplateProNames.ConsultationSellerCompleted, store.Id);

            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddModeratorConsultationTokens(commonTokens, recipientBuyer, recipientSeller, consultationProfileDTO, engagement);

            messageTemplateBuyer.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmailBuyer, toNameBuyer);
            }).ToList();

            messageTemplateSeller.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmailSeller, toNameSeller);
            }).ToList();
        }

        public virtual void SendConsultationReschedule(int languageId, int consultationInvitationId)
        {
            var engagement = _moderatorCancellationRequestService.GetModeratorConsultantDTO(consultationInvitationId);
            var consultationProfileDTO = _consultationProfileService.GetConsultationProfileById(engagement.ConsultationProfileId);
            var recipientBuyer = _customerService.GetCustomerById(engagement.BuyerId);
            var recipientSeller = _customerService.GetCustomerById(engagement.SellerId);
            var toEmailBuyer = engagement.BuyerEmail;
            var toNameBuyer = engagement.BuyerName;
            var toEmailSeller = engagement.SellerEmail;
            var toNameSeller = engagement.SellerName;

            if (recipientBuyer == null)
                throw new ArgumentNullException(nameof(recipientBuyer));
            if (recipientSeller == null)
                throw new ArgumentNullException(nameof(recipientSeller));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var messageTemplateBuyer = GetActiveMessageTemplates(MessageTemplateProNames.ConsultationBuyerReschedule, store.Id);
            var messageTemplateSeller = GetActiveMessageTemplates(MessageTemplateProNames.ConsultationSellerReschedule, store.Id);

            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddModeratorConsultationTokens(commonTokens, recipientBuyer, recipientSeller, consultationProfileDTO, engagement);

            messageTemplateBuyer.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmailBuyer, toNameBuyer);
            }).ToList();

            messageTemplateSeller.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmailSeller, toNameSeller);
            }).ToList();
        }

        public virtual void SendConsultationCancellation(int languageId, int consultationInvitationId)
        {
            var engagement = _moderatorCancellationRequestService.GetModeratorConsultantDTO(consultationInvitationId);
            var consultationProfileDTO = _consultationProfileService.GetConsultationProfileById(engagement.ConsultationProfileId);
            var recipientBuyer = _customerService.GetCustomerById(engagement.BuyerId);
            var recipientSeller = _customerService.GetCustomerById(engagement.SellerId);
            var toEmailBuyer = engagement.BuyerEmail;
            var toNameBuyer = engagement.BuyerName;
            var toEmailSeller = engagement.SellerEmail;
            var toNameSeller = engagement.SellerName;

            if (recipientBuyer == null)
                throw new ArgumentNullException(nameof(recipientBuyer));
            if (recipientSeller == null)
                throw new ArgumentNullException(nameof(recipientSeller));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var messageTemplateBuyer = GetActiveMessageTemplates(MessageTemplateProNames.ConsultationBuyerCancellation, store.Id);
            var messageTemplateSeller = GetActiveMessageTemplates(MessageTemplateProNames.ConsultationSellerCancellation, store.Id);

            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddModeratorConsultationTokens(commonTokens, recipientBuyer, recipientSeller, consultationProfileDTO, engagement);

            messageTemplateBuyer.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmailBuyer, toNameBuyer);
            }).ToList();

            messageTemplateSeller.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmailSeller, toNameSeller);
            }).ToList();
        }

        //public virtual IList<int> SendConsultationReschedule(int languageId, int consultationInvitationId)
        //{
        //    var engagement = _moderatorCancellationRequestService.GetModeratorConsultantDTO(consultationInvitationId);
        //    var consultationProfileDTO = _consultationProfileService.GetConsultationProfileById(engagement.ConsultationProfileId);
        //    var recipient = _customerService.GetCustomerById(engagement.BuyerId);
        //    var toEmail = engagement.BuyerEmail;
        //    var toName = engagement.BuyerName;

        //    if (recipient == null)
        //        throw new ArgumentNullException(nameof(recipient));

        //    var store = _storeContext.CurrentStore;
        //    languageId = EnsureLanguageIsActive(languageId, store.Id);
        //    var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.ConsultationBuyerReschedule, store.Id);

        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    var commonTokens = new List<Token>();
        //    _proMessageTokenProvider.AddModeratorConsultationTokens(commonTokens, recipient, consultationProfileDTO, engagement);

        //    return messageTemplates.Select(messageTemplate =>
        //    {
        //        var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //        var tokens = new List<Token>(commonTokens);
        //        _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
        //        _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
        //        return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToList();
        //}

        //public virtual IList<int> SendConsultationCancellation(int languageId, int consultationInvitationId)
        //{
        //    var engagement = _moderatorCancellationRequestService.GetModeratorConsultantDTO(consultationInvitationId);
        //    var consultationProfileDTO = _consultationProfileService.GetConsultationProfileById(engagement.ConsultationProfileId);
        //    var recipient = _customerService.GetCustomerById(engagement.BuyerId);
        //    var toEmail = engagement.BuyerEmail;
        //    var toName = engagement.BuyerName;

        //    if (recipient == null)
        //        throw new ArgumentNullException(nameof(recipient));

        //    var store = _storeContext.CurrentStore;
        //    languageId = EnsureLanguageIsActive(languageId, store.Id);
        //    var messageTemplates = GetActiveMessageTemplates(MessageTemplateProNames.ConsultationBuyerCancellation, store.Id);

        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    var commonTokens = new List<Token>();
        //    _proMessageTokenProvider.AddModeratorConsultationTokens(commonTokens, recipient, consultationProfileDTO, engagement);

        //    return messageTemplates.Select(messageTemplate =>
        //    {
        //        var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //        var tokens = new List<Token>(commonTokens);
        //        _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
        //        _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
        //        return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToList();
        //}

        public virtual void SendConsultationDeclinedByOrganization(int languageId, int consultationInvitationId)
        {
            var engagement = _moderatorCancellationRequestService.GetModeratorConsultantDTO(consultationInvitationId);
            var consultationProfileDTO = _consultationProfileService.GetConsultationProfileById(engagement.ConsultationProfileId);
            var recipientBuyer = _customerService.GetCustomerById(engagement.BuyerId);
            var recipientSeller = _customerService.GetCustomerById(engagement.SellerId);
            var toEmailSeller = engagement.SellerEmail;
            var toNameSeller = engagement.SellerName;

            if (recipientBuyer == null)
                throw new ArgumentNullException(nameof(recipientBuyer));
            if (recipientSeller == null)
                throw new ArgumentNullException(nameof(recipientSeller));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var messageTemplateSeller = GetActiveMessageTemplates(MessageTemplateProNames.ConsultationSellerDeclinedByOrganization, store.Id);

            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddModeratorConsultationTokens(commonTokens, recipientBuyer, recipientSeller, consultationProfileDTO, engagement);

            messageTemplateSeller.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmailSeller, toNameSeller);
            }).ToList();
        }

        #endregion

        #region Block Customer

        public virtual void SendBlockCustomer(int languageId, int sellerId, string remarks)
        {
            var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(sellerId);
            var recipient = _customerService.GetCustomerById(sellerId);
            var blockStatus = _blockCustomerService.GetBlockStatus(sellerId);
            var toEmailSeller = individualProfile.Email;
            var toNameSeller = individualProfile.FullName;
            if (recipient == null)
                throw new ArgumentNullException(nameof(recipient));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var messageTemplateSeller = GetActiveMessageTemplates(MessageTemplateProNames.BlockSeller, store.Id);

            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddBlockCustomerTokens(commonTokens, recipient, blockStatus.EndDate, blockStatus.BlockQuantity, remarks);

            messageTemplateSeller.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmailSeller, toNameSeller);
            }).ToList();
        }

        public virtual void SendDepositRequestNotification(int languageId, DepositRequestDTO depositRequest)
        {
            var recipient = _customerService.GetCustomerById(depositRequest.DepositFrom);
            var toEmailSeller = "";
            var toNameSeller = "";

            if (depositRequest.ProductTypeId == (int)ProductType.ServiceEnagegementFee)
            {
                var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(depositRequest.DepositFrom);
                toEmailSeller = individualProfile.Email;
                toNameSeller = individualProfile.FullName;
            }
            else
            {
                var orgProfileDto = _organizationProfileService.GetOrganizationProfileByCustomerId(depositRequest.DepositFrom);
                toEmailSeller = orgProfileDto.ContactPersonEmail;
                toNameSeller = orgProfileDto.Name;
            }
            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            IList<MessageTemplate> messageTemplateSeller = null;
            if (depositRequest.ReminderCount == 0)
            {
                messageTemplateSeller = GetActiveMessageTemplates(MessageTemplateProNames.DepositRequestNotification, store.Id);
            }
            else if (depositRequest.ReminderCount == 3)
            {
                messageTemplateSeller = GetActiveMessageTemplates(MessageTemplateProNames.DepositRequestLastReminder, store.Id);
            }
            else
            {
                messageTemplateSeller = GetActiveMessageTemplates(MessageTemplateProNames.DepositRequestReminder, store.Id);
            }

            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddDepositRequestNotificationTokens(commonTokens, recipient, depositRequest);
            messageTemplateSeller.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmailSeller, toNameSeller);
            }).ToList();
        }


        public virtual void SendTerminatingApplicationNotificationBuyer(int languageId, DepositRequestDTO depositRequest, DateTime endDate)
        {
            var recipient = _customerService.GetCustomerById(depositRequest.DepositFrom);
            var toEmailSeller = "";
            var toNameSeller = "";

            if (depositRequest.ProductTypeId == (int)ProductType.ServiceEnagegementFee)
            {
                var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(depositRequest.DepositFrom);
                toEmailSeller = individualProfile.Email;
                toNameSeller = individualProfile.FullName;
            }
            else
            {
                var orgProfileDto = _organizationProfileService.GetOrganizationProfileByCustomerId(depositRequest.DepositFrom);
                toEmailSeller = orgProfileDto.ContactPersonEmail;
                toNameSeller = orgProfileDto.Name;
            }
            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            IList<MessageTemplate> messageTemplateSeller = GetActiveMessageTemplates(MessageTemplateProNames.DepositRequestTerminatingApplicationBuyer, store.Id);

            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddDepositRequestTerminatinAppNotificationTokensBuyer(commonTokens, recipient, depositRequest, endDate);
            messageTemplateSeller.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmailSeller, toNameSeller);
            }).ToList();
        }


        public virtual void SendTerminatingApplicationNotificationSeller(int languageId, DepositRequestDTO depositRequest, DateTime endDate)
        {
            var recipient = _customerService.GetCustomerById(depositRequest.DepositTo);
            var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(depositRequest.DepositTo);
            var toEmailSeller = individualProfile.Email;
            var toNameSeller = individualProfile.FullName;
            
            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            IList<MessageTemplate> messageTemplateSeller = GetActiveMessageTemplates(MessageTemplateProNames.DepositRequestTerminatingApplicationSeller, store.Id);

            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddDepositRequestTerminatinAppNotificationTokensSeller(commonTokens, recipient, depositRequest, endDate);
            messageTemplateSeller.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmailSeller, toNameSeller);
            }).ToList();
        }

        #endregion

        #region Payout Request 

        /// <summary>
        /// send email to engagement buyer on payout request submit by seller
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="payoutRequest"></param>
        public virtual void SendSubmittedPayoutRequestMessage(int languageId, PayoutRequestDTO payoutRequest)
        {
            var engagement = _engagementService.GetEngagement(payoutRequest.ProductTypeId, payoutRequest.RefId);

            var recipientCustomerId = engagement.BuyerCustomerId;
            var recipientEmail = "";
            var recipientName = "";
            var messageTemplate = engagement.IsProjectPayout
                ?MessageTemplateProNames.SubmittedProjectPayoutRequestMessage
                :MessageTemplateProNames.SubmittedNonProjectPayoutRequestMessage;

            var recipientCustomer = _customerService.GetCustomerById(recipientCustomerId);

            if (payoutRequest.ProductTypeId == (int)ProductType.ServiceEnagegementFee)
            {
                var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(recipientCustomerId);
                recipientEmail = individualProfile.Email;
                recipientName = individualProfile.FullName;
            }
            else
            {
                var orgProfileDto = _organizationProfileService.GetOrganizationProfileByCustomerId(recipientCustomerId);
                recipientEmail = orgProfileDto.ContactPersonEmail;
                recipientName = orgProfileDto.Name;
            }

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            IList<MessageTemplate> messageTemplates = GetActiveMessageTemplates(messageTemplate, store.Id); ;

            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddPayoutRequestTokens(commonTokens, recipientCustomer, engagement, payoutRequest);
            messageTemplates.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, recipientEmail, recipientName);
            }).ToList();
        }

        /// <summary>
        /// send email to engagement seller on payout request approved by buyer
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="payoutRequest"></param>
        public virtual void SendApprovedPayoutRequestMessage(int languageId, PayoutRequestDTO payoutRequest)
        {
            var engagement = _engagementService.GetEngagement(payoutRequest.ProductTypeId, payoutRequest.RefId);

            var recipientCustomerId = engagement.SellerCustomerId;
            var recipientEmail = "";
            var recipientName = "";
            var messageTemplate = engagement.IsProjectPayout
                ? MessageTemplateProNames.ApprovedProjectPayoutRequestMessage
                : MessageTemplateProNames.ApprovedNonProjectPayoutRequestMessage;

            var recipientCustomer = _customerService.GetCustomerById(recipientCustomerId);
            var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(recipientCustomerId);
            recipientEmail = individualProfile.Email;
            recipientName = individualProfile.FullName;

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            IList<MessageTemplate> messageTemplates = GetActiveMessageTemplates(messageTemplate, store.Id); ;

            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddPayoutRequestTokens(commonTokens, recipientCustomer, engagement, payoutRequest);
            messageTemplates.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, recipientEmail, recipientName);
            }).ToList();
        }

        /// <summary>
        /// send email to engagement buyer on payout request auto approved by system
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="payoutRequest"></param>
        public virtual void SendAutoApprovedPayoutRequestMessage(int languageId, PayoutRequestDTO payoutRequest)
        {
            var engagement = _engagementService.GetEngagement(payoutRequest.ProductTypeId, payoutRequest.RefId);

            var recipientCustomerId = engagement.BuyerCustomerId;
            var recipientEmail = "";
            var recipientName = "";
            var messageTemplate = engagement.IsProjectPayout
                ? MessageTemplateProNames.AutoApprovedProjectPayoutRequestMessage
                : MessageTemplateProNames.AutoApprovedNonProjectPayoutRequestMessage;

            var recipientCustomer = _customerService.GetCustomerById(recipientCustomerId);

            if (payoutRequest.ProductTypeId == (int)ProductType.ServiceEnagegementFee)
            {
                var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(recipientCustomerId);
                recipientEmail = individualProfile.Email;
                recipientName = individualProfile.FullName;
            }
            else
            {
                var orgProfileDto = _organizationProfileService.GetOrganizationProfileByCustomerId(recipientCustomerId);
                recipientEmail = orgProfileDto.ContactPersonEmail;
                recipientName = orgProfileDto.Name;
            }

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            IList<MessageTemplate> messageTemplates = GetActiveMessageTemplates(messageTemplate, store.Id); ;

            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddPayoutRequestTokens(commonTokens, recipientCustomer, engagement, payoutRequest);
            messageTemplates.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, recipientEmail, recipientName);
            }).ToList();
        }

        /// <summary>
        /// send email to engagement seller on payout request required more info by buyer
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="payoutRequest"></param>
        public virtual void SendRequiredMoreInfoPayoutRequestMessage(int languageId, PayoutRequestDTO payoutRequest)
        {
            var engagement = _engagementService.GetEngagement(payoutRequest.ProductTypeId, payoutRequest.RefId);

            var recipientCustomerId = engagement.SellerCustomerId;
            var recipientEmail = "";
            var recipientName = "";
            var messageTemplate = engagement.IsProjectPayout
                ? MessageTemplateProNames.RequiredMoreInfoProjectPayoutRequestMessage
                : MessageTemplateProNames.RequiredMoreInfoNonProjectPayoutRequestMessage;

            var recipientCustomer = _customerService.GetCustomerById(recipientCustomerId);
            var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(recipientCustomerId);
            recipientEmail = individualProfile.Email;
            recipientName = individualProfile.FullName;

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            IList<MessageTemplate> messageTemplates = GetActiveMessageTemplates(messageTemplate, store.Id); ;

            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddPayoutRequestTokens(commonTokens, recipientCustomer, engagement, payoutRequest);
            messageTemplates.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, recipientEmail, recipientName);
            }).ToList();
        }

        /// <summary>
        /// send email to engagement seller on payout request having error when make payment
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="payoutRequest"></param>
        public virtual void SendErrorPayoutRequestMessage(int languageId, PayoutRequestDTO payoutRequest)
        {
            var engagement = _engagementService.GetEngagement(payoutRequest.ProductTypeId, payoutRequest.RefId);

            var recipientCustomerId = engagement.SellerCustomerId;
            var recipientEmail = "";
            var recipientName = "";
            var messageTemplate = engagement.IsProjectPayout
                ? MessageTemplateProNames.ErrorProjectPayoutRequestMessage
                : MessageTemplateProNames.ErrorNonProjectPayoutRequestMessage;

            var recipientCustomer = _customerService.GetCustomerById(recipientCustomerId);
            var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(recipientCustomerId);
            recipientEmail = individualProfile.Email;
            recipientName = individualProfile.FullName;

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            IList<MessageTemplate> messageTemplates = GetActiveMessageTemplates(messageTemplate, store.Id); ;

            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddPayoutRequestTokens(commonTokens, recipientCustomer, engagement, payoutRequest);
            messageTemplates.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, recipientEmail, recipientName);
            }).ToList();
        }

        /// <summary>
        /// send email to engagement seller on payout request have been paid to seller
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="payoutRequest"></param>
        public virtual void SendPaidPayoutRequestMessage(int languageId, PayoutRequestDTO payoutRequest)
        {
            var engagement = _engagementService.GetEngagement(payoutRequest.ProductTypeId, payoutRequest.RefId);

            var recipientCustomerId = engagement.SellerCustomerId;
            var recipientEmail = "";
            var recipientName = "";
            var messageTemplate = engagement.IsProjectPayout
                ? MessageTemplateProNames.PaidProjectPayoutRequestMessage
                : MessageTemplateProNames.PaidNonProjectPayoutRequestMessage;

            var recipientCustomer = _customerService.GetCustomerById(recipientCustomerId);
            var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(recipientCustomerId);
            recipientEmail = individualProfile.Email;
            recipientName = individualProfile.FullName;

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            IList<MessageTemplate> messageTemplates = GetActiveMessageTemplates(messageTemplate, store.Id); ;

            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddPayoutRequestTokens(commonTokens, recipientCustomer, engagement, payoutRequest);
            messageTemplates.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, recipientEmail, recipientName);
            }).ToList();
        }

        /// <summary>
        /// send email to user on payout fail to process due to invalid bank account info
        /// </summary>
        /// <param name="bankAccount"></param>
        /// <param name="languageId"></param>
        public virtual void SendPayouRequestFailInvalidBankAccountMessage(BankAccountDTO bankAccount, int languageId)
        {
            var recipient = _customerService.GetCustomerById(bankAccount.CustomerId);

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            IList<MessageTemplate> messageTemplate = GetActiveMessageTemplates(MessageTemplateProNames.PayoutFailInvalidBankAccount, store.Id);

            var toName = _customerService.GetCustomerFullName(recipient);

            var commonTokens = new List<Token>();

            messageTemplate.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, recipient.Email, toName);
            }).ToList();
        }
        #endregion

        #region Deposit Request

        public virtual void SendDepositRequestPaymentVerificationNotification(int languageId, DepositRequestDTO depositRequest, int opCustomerId)
        {
            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var op = _customerService.GetCustomerById(opCustomerId);
            IList<MessageTemplate> messageTemplate = GetActiveMessageTemplates(MessageTemplateProNames.DepositRequestPaymentVerificationNotification, store.Id);

            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddDepositRequestPaymentVerificationNotificationTokens(commonTokens, depositRequest);

            messageTemplate.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, op.Email, op.SystemName);
            }).ToList();
        }

        public virtual void SendDepositApproved(int languageId, int productTypeId, int refId, string remarks, bool isApproved)
        {
            var engagement = _engagementService.GetEngagement(productTypeId, refId);

            var recipient = _customerService.GetCustomerById(engagement.BuyerCustomerId);
            var orgProfileDto = _organizationProfileService.GetOrganizationProfileByCustomerId(engagement.BuyerCustomerId);
            if (recipient == null)
                throw new ArgumentNullException(nameof(recipient));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            IList<MessageTemplate> messageTemplate;
            if (isApproved)
            {
                messageTemplate = GetActiveMessageTemplates(MessageTemplateProNames.DepositRequestConfirmed, store.Id);
            }
            else
            {
                messageTemplate = GetActiveMessageTemplates(MessageTemplateProNames.DepositRequestNotConfirmed, store.Id);
            }
            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddDepositRequestConfirmationTokens(commonTokens, recipient, refId, remarks);

            messageTemplate.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, recipient.Email, orgProfileDto.ContactPersonName);
            }).ToList();
        }

        public virtual void SendDepositBuyerPaid(int languageId, int productTypeId, int refId)
        {
            var engagement = _engagementService.GetEngagement(productTypeId, refId);

            var recipient = _customerService.GetCustomerById(engagement.BuyerCustomerId);
            var orgProfileDto = _organizationProfileService.GetOrganizationProfileByCustomerId(engagement.BuyerCustomerId);
            if (recipient == null)
                throw new ArgumentNullException(nameof(recipient));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var messageTemplate = GetActiveMessageTemplates(MessageTemplateProNames.DepositRequestBuyerPaid, store.Id);

            var commonTokens = new List<Token>();
            _proMessageTokenProvider.AddDepositRequestConfirmationTokens(commonTokens, recipient, refId);

            messageTemplate.Select(messageTemplate =>
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var tokens = new List<Token>(commonTokens);
                _proMessageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, recipient.Email, orgProfileDto.ContactPersonName);
            }).ToList();
        }

        #endregion

        #region Bank Account



        #endregion

        #endregion
    }
}
