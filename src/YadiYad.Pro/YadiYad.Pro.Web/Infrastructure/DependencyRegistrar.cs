using Autofac;
using AutoMapper;
using Nop.Core.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Security;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Services.Common;
using YadiYad.Pro.Services.Consultation;
using YadiYad.Pro.Services.Deposit;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Services.Job;
using YadiYad.Pro.Services.JobSeeker;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Services.Organization;
using YadiYad.Pro.Services.Payment;
using YadiYad.Pro.Services.Refund;
using YadiYad.Pro.Services.Service;
using YadiYad.Pro.Services.Services.Common;
using YadiYad.Pro.Services.Services.Customer;
using YadiYad.Pro.Services.Services.Messages;
using YadiYad.Pro.Services.Payout;
using YadiYad.Pro.Services.Services.Subscription;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Web.Factories;
using YadiYad.Pro.Web.FactoriesPro;
using YadiYad.Pro.Web.Validators;
using YadiYad.Pro.Services.Services.Moderator;
using YadiYad.Pro.Services.Services.Campaign;
using YadiYad.Pro.Services.Services.Engagement;
using YadiYad.Pro.Services.Engagement;
using YadiYad.Pro.Services.Services.Order;
using Nop.Services.ShippingShuq;
using YadiYad.Pro.Services.Services.Operator;
using YadiYad.Pro.Services.Services.Attentions;

namespace YadiYad.Pro.Web.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {

            #region common
            builder.RegisterType<BusinessSegmentService>().As<BusinessSegmentService>().InstancePerLifetimeScope();
            builder.RegisterType<ExpertiseService>().As<ExpertiseService>().InstancePerLifetimeScope();
            builder.RegisterType<CityService>().As<CityService>().InstancePerLifetimeScope();
            builder.RegisterType<InterestHobbyService>().As<InterestHobbyService>().InstancePerLifetimeScope();
            builder.RegisterType<JobServiceCategoryService>().As<JobServiceCategoryService>().InstancePerLifetimeScope();
            builder.RegisterType<TimeZoneService>().As<TimeZoneService>().InstancePerLifetimeScope();
            builder.RegisterType<ChargeService>().As<ChargeService>().InstancePerLifetimeScope();
            builder.RegisterType<CommonModelFactory>().As<CommonModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ProMessageTokenProvider>().As<ProMessageTokenProvider>().InstancePerLifetimeScope();
            builder.RegisterType<ProWorkflowMessageService>().As<ProWorkflowMessageService>().InstancePerLifetimeScope();
            builder.RegisterType<CommunicateLanguageService>().As<CommunicateLanguageService>().InstancePerLifetimeScope();
            builder.RegisterType<BankService>().As<BankService>().InstancePerLifetimeScope();
            builder.RegisterType<CancellationReasonService>().As<CancellationReasonService>().InstancePerLifetimeScope();
            builder.RegisterType<CampaignManagementService>().As<CampaignManagementService>().InstancePerLifetimeScope();
            #endregion

            #region individual
            builder.RegisterType<IndividualInterestHobbyService>().As<IndividualInterestHobbyService>().InstancePerLifetimeScope();
            builder.RegisterType<IndividualProfileService>().As<IndividualProfileService>().InstancePerLifetimeScope();
            builder.RegisterType<DashboardService>().As<DashboardService>().InstancePerLifetimeScope();
            builder.RegisterType<BankAccountService>().As<BankAccountService>().InstancePerLifetimeScope();
            builder.RegisterType<BillingAddressService>().As<BillingAddressService>().InstancePerLifetimeScope();
            #endregion

            #region job
            builder.RegisterType<JobProfileService>().As<JobProfileService>().InstancePerLifetimeScope();
            //builder.RegisterType<JobProfileExpertiseService>().As<JobProfileExpertiseService>().InstancePerLifetimeScope();
            builder.RegisterType<JobInvitationService>().As<JobInvitationService>().InstancePerLifetimeScope();
            builder.RegisterType<JobApplicationService>().As<JobApplicationService>().As<IEngagementService>().InstancePerLifetimeScope();
            builder.RegisterType<PermissionService>().As<PermissionService>().InstancePerLifetimeScope();
            #endregion

            #region jobseeker
            builder.RegisterType<JobSeekerProfileService>().As<JobSeekerProfileService>().InstancePerLifetimeScope();
            #endregion

            #region organization
            builder.RegisterType<OrganizationProfileService>().As<OrganizationProfileService>().InstancePerLifetimeScope();
            #endregion

            #region nopcommerce
            builder.RegisterType<CountryService>().As<CountryService>().InstancePerLifetimeScope();
            builder.RegisterType<StateProvinceService>().As<StateProvinceService>().InstancePerLifetimeScope();
            builder.RegisterType<LanguageService>().As<LanguageService>().InstancePerLifetimeScope();
            #endregion

            #region service
            builder.RegisterType<ServiceProfileService>().As<ServiceProfileService>().InstancePerLifetimeScope();
            builder.RegisterType<ServiceApplicationService>().As<ServiceApplicationService>().As<IEngagementService>().InstancePerLifetimeScope();

            #endregion

            #region consultation
            builder.RegisterType<ConsultationProfileService>().As<ConsultationProfileService>().InstancePerLifetimeScope();
            builder.RegisterType<ConsultationInvitationService>().As<ConsultationInvitationService>().As<IEngagementService>().InstancePerLifetimeScope();
            #endregion

            #region subscription
            builder.RegisterType<ServiceSubscriptionService>().As<ServiceSubscriptionService>().InstancePerLifetimeScope();
            #endregion    

            #region subscription
            builder.RegisterType<OrderService>().As<OrderService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderProcessingService>().As<OrderProcessingService>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentService_Pro>().As<Nop.Services.Payments.IPaymentService_Pro>().InstancePerLifetimeScope();
            builder.RegisterType<InvoiceService>().As<InvoiceService>().InstancePerLifetimeScope();
            builder.RegisterType<DepositRequestService>().As<DepositRequestService>().InstancePerLifetimeScope();
            builder.RegisterType<PayoutBatchService>().As<PayoutBatchService>().InstancePerLifetimeScope();
            builder.RegisterType<RefundRequestService>().As<RefundRequestService>().InstancePerLifetimeScope();
            builder.RegisterType<ChargeService>().As<ChargeService>().InstancePerLifetimeScope();
            builder.RegisterType<TransactionService>().As<TransactionService>().InstancePerLifetimeScope();
            #endregion

            #region global
            builder.RegisterType<AccountContext>().As<AccountContext>().InstancePerLifetimeScope();
            #endregion

            #region customer

            builder.RegisterType<CustomerProModelFactory>().As<CustomerProModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerRegistrationRoleService>().As<CustomerRegistrationRoleService>().InstancePerLifetimeScope();

            #endregion

            #region category
            builder.RegisterType<CategoryModelFactory>().As<CategoryModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<BaseAdminModelFactory>().As<BaseAdminModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SettingModelFactory>().As<SettingModelFactory>().InstancePerLifetimeScope();

            #endregion

            #region expertise

            builder.RegisterType<ExpertiseModelFactory>().As<ExpertiseModelFactory>().InstancePerLifetimeScope();


            #endregion

            #region News

            builder.RegisterType<NewsModelFactory>().As<NewsModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<NewsProService>().As<NewsProService>().InstancePerLifetimeScope();


            #endregion

            #region Payout Request
            builder.RegisterType<PayoutRequestService>().As<PayoutRequestService>().InstancePerLifetimeScope();
            builder.RegisterType<FeeCalculationService>().As<FeeCalculationService>().InstancePerLifetimeScope();
            #endregion

            #region Moderator

            builder.RegisterType<ModeratorCancellationRequestService>().As<ModeratorCancellationRequestService>().InstancePerLifetimeScope();
            builder.RegisterType<BlockCustomerService>().As<BlockCustomerService>().InstancePerLifetimeScope();

            #endregion

            #region Statement
            builder.RegisterType<StatementService>().As<StatementService>().InstancePerLifetimeScope();
            #endregion

            #region Cancellation
            builder.RegisterType<EngagementCancellationManager>().As<EngagementCancellationManager>().InstancePerLifetimeScope();
            builder.RegisterType<CancellationRequestService>().As<CancellationRequestService>().InstancePerLifetimeScope();
            #endregion

            #region Engagement
            builder.RegisterType<EngagementService>().As<EngagementService>().InstancePerLifetimeScope();

            builder.RegisterType<EngagementResolver>().As<EngagementResolver>().InstancePerLifetimeScope();
            #endregion
            
            #region Campaign
            builder.RegisterType<CampaignProcessingService>()
                .As<CampaignProcessingService>()
                .As<IConsumer<CustomerRegisteredEvent>>()
                .As<IConsumer<JobPublishedEvent>>()
                .As<IConsumer<ExtendPviEvent>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CampaignSubscriptionService>().As<CampaignSubscriptionService>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<PayToApplyJobsCampaign>().As<ICampaignProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<PayToViewInviteJobsCampaign>().As<ICampaignProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<ExtendPayToViewInviteJobCampaign>().As<ICampaignProcessor>().InstancePerLifetimeScope();
            //builder.RegisterType<CreditVoucherCampaign>().As<ICampaignProcessor>().InstancePerLifetimeScope();
            //builder.RegisterType<CreditVoucherReferralCampaign>().As<ICampaignProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<ChargesWaiverCampaign>().As<ICampaignProcessor>().InstancePerLifetimeScope();

            #endregion

            #region Shipping

            builder.RegisterType<WarehouseService>().As<WarehouseService>().InstancePerLifetimeScope();


            #endregion

            #region Operator

            builder.RegisterType<OperatorService>().As<IOperatorService>().InstancePerLifetimeScope();


            #endregion

            #region Operator

            builder.RegisterType<IndividualAttentionService>().As<IndividualAttentionService>().InstancePerLifetimeScope();
            builder.RegisterType<OrganizationAttentionService>().As<OrganizationAttentionService>().InstancePerLifetimeScope();


            #endregion
        }

        public int Order => 1;
    }
}
