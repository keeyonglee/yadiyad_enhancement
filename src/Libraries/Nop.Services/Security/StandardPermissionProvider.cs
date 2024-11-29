using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security
{
    /// <summary>
    /// Standard permission provider
    /// </summary>
    public partial class StandardPermissionProvider : IPermissionProvider
    {
        //admin area permissions
        public static readonly PermissionRecord AccessAdminPanel = new PermissionRecord { Name = "Access admin area", SystemName = "AccessAdminPanel", Category = "Standard" };
        public static readonly PermissionRecord AllowCustomerImpersonation = new PermissionRecord { Name = "Admin area. Allow Customer Impersonation", SystemName = "AllowCustomerImpersonation", Category = "Customers" };
        public static readonly PermissionRecord ManageProducts = new PermissionRecord { Name = "Admin area. Manage Products", SystemName = "ManageProducts", Category = "Catalog" };
        public static readonly PermissionRecord ManageProductsOperatorMode = new PermissionRecord { Name = "Admin area. Manage Operator Mode", SystemName = "ManageProductsOperatorMode", Category = "Catalog" };
        public static readonly PermissionRecord ManageProductsAdvancedMode = new PermissionRecord { Name = "Admin area. Manage Products Advanced Mode", SystemName = "ManageProductsAdvancedMode", Category = "Catalog" };
        public static readonly PermissionRecord ManageCategories = new PermissionRecord { Name = "Admin area. Manage Categories", SystemName = "ManageCategories", Category = "Catalog" };
        public static readonly PermissionRecord ManageJobServiceCategory = new PermissionRecord { Name = "Admin area. Manage Job Service Category", SystemName = "ManageJobServiceCategory", Category = "Catalog" };
        public static readonly PermissionRecord ManageExpertise = new PermissionRecord { Name = "Admin area. Manage Expertise", SystemName = "ManageExpertise", Category = "Catalog" };
        public static readonly PermissionRecord ManageManufacturers = new PermissionRecord { Name = "Admin area. Manage Manufacturers", SystemName = "ManageManufacturers", Category = "Catalog" };
        public static readonly PermissionRecord ManageProductReviews = new PermissionRecord { Name = "Admin area. Manage Product Reviews", SystemName = "ManageProductReviews", Category = "Catalog" };
        public static readonly PermissionRecord ManageProductTags = new PermissionRecord { Name = "Admin area. Manage Product Tags", SystemName = "ManageProductTags", Category = "Catalog" };
        public static readonly PermissionRecord ManageAttributes = new PermissionRecord { Name = "Admin area. Manage Attributes", SystemName = "ManageAttributes", Category = "Catalog" };
        public static readonly PermissionRecord ManageCustomers = new PermissionRecord { Name = "Admin area. Manage Customers", SystemName = "ManageCustomers", Category = "Customers" };
        public static readonly PermissionRecord ManageCancellationRequest = new PermissionRecord { Name = "Admin area. Manage Cancellation Request", SystemName = "ManageCancellationRequest", Category = "Customers" };
        public static readonly PermissionRecord ManageVendors = new PermissionRecord { Name = "Admin area. Manage Vendors", SystemName = "ManageVendors", Category = "Customers" };
        public static readonly PermissionRecord ManageVendorApplications = new PermissionRecord { Name = "Admin area. Manage Vendor Applications", SystemName = "ManageVendorApplications", Category = "Customers" };
        public static readonly PermissionRecord ManageCurrentCarts = new PermissionRecord { Name = "Admin area. Manage Current Carts", SystemName = "ManageCurrentCarts", Category = "Orders" };
        public static readonly PermissionRecord ManageOrders = new PermissionRecord { Name = "Admin area. Manage Orders", SystemName = "ManageOrders", Category = "Orders" };
        public static readonly PermissionRecord ManageProOrders = new PermissionRecord { Name = "Admin area. Manage ProOrders", SystemName = "ManageProOrders", Category = "Orders" };
        public static readonly PermissionRecord ManageApproveDepositRequest = new PermissionRecord { Name = "Admin area. Manage ApproveDepositRequest", SystemName = "ManageApproveDepositRequest", Category = "Orders" };
        public static readonly PermissionRecord ManageCharge = new PermissionRecord { Name = "Admin area. Manage Charge", SystemName = "ManageCharge", Category = "Orders" };
        public static readonly PermissionRecord ManageRecurringPayments = new PermissionRecord { Name = "Admin area. Manage Recurring Payments", SystemName = "ManageRecurringPayments", Category = "Orders" };
        public static readonly PermissionRecord ManageGiftCards = new PermissionRecord { Name = "Admin area. Manage Gift Cards", SystemName = "ManageGiftCards", Category = "Orders" };
        public static readonly PermissionRecord ManageReturnRequests = new PermissionRecord { Name = "Admin area. Manage Return Requests", SystemName = "ManageReturnRequests", Category = "Orders" };
        public static readonly PermissionRecord OrderCountryReport = new PermissionRecord { Name = "Admin area. Access order country report", SystemName = "OrderCountryReport", Category = "Orders" };
        public static readonly PermissionRecord ManageAffiliates = new PermissionRecord { Name = "Admin area. Manage Affiliates", SystemName = "ManageAffiliates", Category = "Promo" };
        public static readonly PermissionRecord ManageCampaigns = new PermissionRecord { Name = "Admin area. Manage Campaigns", SystemName = "ManageCampaigns", Category = "Promo" };
        public static readonly PermissionRecord ManageCampaignManagement = new PermissionRecord { Name = "Admin area. Manage CampaignManagement", SystemName = "ManageCampaignManagement", Category = "Promo" };
        public static readonly PermissionRecord ManageDiscounts = new PermissionRecord { Name = "Admin area. Manage Discounts", SystemName = "ManageDiscounts", Category = "Promo" };
        public static readonly PermissionRecord ManageNewsletterSubscribers = new PermissionRecord { Name = "Admin area. Manage Newsletter Subscribers", SystemName = "ManageNewsletterSubscribers", Category = "Promo" };
        public static readonly PermissionRecord ManagePolls = new PermissionRecord { Name = "Admin area. Manage Polls", SystemName = "ManagePolls", Category = "Content Management" };
        public static readonly PermissionRecord ManageNews = new PermissionRecord { Name = "Admin area. Manage News", SystemName = "ManageNews", Category = "Content Management" };
        public static readonly PermissionRecord ManageYadiyadNews = new PermissionRecord { Name = "Admin area. Manage YadiyadNews", SystemName = "ManageYadiyadNews", Category = "Content Management" };
        public static readonly PermissionRecord ManageBlog = new PermissionRecord { Name = "Admin area. Manage Blog", SystemName = "ManageBlog", Category = "Content Management" };
        public static readonly PermissionRecord ManageWidgets = new PermissionRecord { Name = "Admin area. Manage Widgets", SystemName = "ManageWidgets", Category = "Content Management" };
        public static readonly PermissionRecord ManageTopics = new PermissionRecord { Name = "Admin area. Manage Topics", SystemName = "ManageTopics", Category = "Content Management" };
        public static readonly PermissionRecord ManageForums = new PermissionRecord { Name = "Admin area. Manage Forums", SystemName = "ManageForums", Category = "Content Management" };
        public static readonly PermissionRecord ManageMessageTemplates = new PermissionRecord { Name = "Admin area. Manage Message Templates", SystemName = "ManageMessageTemplates", Category = "Content Management" };
        public static readonly PermissionRecord ManageCountries = new PermissionRecord { Name = "Admin area. Manage Countries", SystemName = "ManageCountries", Category = "Configuration" };
        public static readonly PermissionRecord ManageLanguages = new PermissionRecord { Name = "Admin area. Manage Languages", SystemName = "ManageLanguages", Category = "Configuration" };
        public static readonly PermissionRecord ManageSettings = new PermissionRecord { Name = "Admin area. Manage Settings", SystemName = "ManageSettings", Category = "Configuration" };
        public static readonly PermissionRecord ManagePaymentMethods = new PermissionRecord { Name = "Admin area. Manage Payment Methods", SystemName = "ManagePaymentMethods", Category = "Configuration" };
        public static readonly PermissionRecord ManageExternalAuthenticationMethods = new PermissionRecord { Name = "Admin area. Manage External Authentication Methods", SystemName = "ManageExternalAuthenticationMethods", Category = "Configuration" };
        public static readonly PermissionRecord ManageTaxSettings = new PermissionRecord { Name = "Admin area. Manage Tax Settings", SystemName = "ManageTaxSettings", Category = "Configuration" };
        public static readonly PermissionRecord ManageShippingSettings = new PermissionRecord { Name = "Admin area. Manage Shipping Settings", SystemName = "ManageShippingSettings", Category = "Configuration" };
        public static readonly PermissionRecord ManageCurrencies = new PermissionRecord { Name = "Admin area. Manage Currencies", SystemName = "ManageCurrencies", Category = "Configuration" };
        public static readonly PermissionRecord ManageActivityLog = new PermissionRecord { Name = "Admin area. Manage Activity Log", SystemName = "ManageActivityLog", Category = "Configuration" };
        public static readonly PermissionRecord ManageAcl = new PermissionRecord { Name = "Admin area. Manage ACL", SystemName = "ManageACL", Category = "Configuration" };
        public static readonly PermissionRecord ManageEmailAccounts = new PermissionRecord { Name = "Admin area. Manage Email Accounts", SystemName = "ManageEmailAccounts", Category = "Configuration" };
        public static readonly PermissionRecord ManageStores = new PermissionRecord { Name = "Admin area. Manage Stores", SystemName = "ManageStores", Category = "Configuration" };
        public static readonly PermissionRecord ManagePlugins = new PermissionRecord { Name = "Admin area. Manage Plugins", SystemName = "ManagePlugins", Category = "Configuration" };
        public static readonly PermissionRecord ManageSystemLog = new PermissionRecord { Name = "Admin area. Manage System Log", SystemName = "ManageSystemLog", Category = "Configuration" };
        public static readonly PermissionRecord ManageMessageQueue = new PermissionRecord { Name = "Admin area. Manage Message Queue", SystemName = "ManageMessageQueue", Category = "Configuration" };
        public static readonly PermissionRecord ManageMaintenance = new PermissionRecord { Name = "Admin area. Manage Maintenance", SystemName = "ManageMaintenance", Category = "Configuration" };
        public static readonly PermissionRecord HtmlEditorManagePictures = new PermissionRecord { Name = "Admin area. HTML Editor. Manage pictures", SystemName = "HtmlEditor.ManagePictures", Category = "Configuration" };
        public static readonly PermissionRecord ManageScheduleTasks = new PermissionRecord { Name = "Admin area. Manage Schedule Tasks", SystemName = "ManageScheduleTasks", Category = "Configuration" };
        public static readonly PermissionRecord ManageDashboards = new PermissionRecord { Name = "Admin area. Manage Other Dashboard", SystemName = "ManageDashboards", Category = "Dashboard" };
        public static readonly PermissionRecord ManageProDashboard = new PermissionRecord { Name = "Admin area. Manage Pro Dashboard", SystemName = "ManageProDashboard", Category = "Dashboard" };
        public static readonly PermissionRecord ManageShuqDashboard = new PermissionRecord { Name = "Admin area. Manage Shuq Dashboard", SystemName = "ManageShuqDashboard", Category = "Dashboard" };
        public static readonly PermissionRecord ManageMainDashboard = new PermissionRecord { Name = "Admin area. Manage Main Dashboard", SystemName = "ManageMainDashboard", Category = "Dashboard" };
        public static readonly PermissionRecord ManageShuqOperator = new PermissionRecord { Name = "Admin area. Manage Shuq Operator", SystemName = "ManageShuqOperator", Category = "Dashboard" };
        public static readonly PermissionRecord ManageThirdPartyPlugins = new PermissionRecord { Name = "Admin area. Manage Third Party Plugins", SystemName = "ManageThirdPartyPlugins", Category = "ThirdPartyPlugins" };
        public static readonly PermissionRecord ManageReportings = new PermissionRecord { Name = "Admin area. Manage Reportings", SystemName = "ManageReportings", Category = "Reporting" };
        public static readonly PermissionRecord ManageContacts = new PermissionRecord { Name = "Admin area. Manage Contacts", SystemName = "ManageContacts", Category = "Reporting" };
        public static readonly PermissionRecord ManageProRevenue = new PermissionRecord { Name = "Admin area. Manage Pro Revenue", SystemName = "ManageProRevenue", Category = "Reporting" };
        public static readonly PermissionRecord ManageProExpenses = new PermissionRecord { Name = "Admin area. Manage Pro Expenses", SystemName = "ManageProExpenses", Category = "Reporting" };
        public static readonly PermissionRecord ManageShuqRevenue = new PermissionRecord { Name = "Admin area. Manage Shuq Revenue", SystemName = "ManageShuqRevenue", Category = "Reporting" };
        public static readonly PermissionRecord ManageOperations = new PermissionRecord { Name = "Admin area. Manage Operations", SystemName = "ManageOperations", Category = "Operation" };
        public static readonly PermissionRecord ManageShuqOperations = new PermissionRecord { Name = "Admin area. Manage Shuq Operations", SystemName = "ManageShuqOperations", Category = "Operation" };
        public static readonly PermissionRecord ManageBankAccounts = new PermissionRecord { Name = "Admin area. Manage Bank Accounts", SystemName = "ManageBankAccounts", Category = "Operation" };
        public static readonly PermissionRecord ManageDisputes = new PermissionRecord { Name = "Admin area. Manage Disputes", SystemName = "ManageDisputes", Category = "Customers" };
        public static readonly PermissionRecord ManageShipments = new PermissionRecord { Name = "Admin area. Manage Shipments", SystemName = "ManageShipments", Category = "Orders" };


        //vendor permission
        public static readonly PermissionRecord ManageShop = new PermissionRecord { Name = "Admin area. Manage Shop", SystemName = "ManageShop", Category = "Vendor" };

        //public store permissions
        public static readonly PermissionRecord DisplayPrices = new PermissionRecord { Name = "Public store. Display Prices", SystemName = "DisplayPrices", Category = "PublicStore" };
        public static readonly PermissionRecord EnableShoppingCart = new PermissionRecord { Name = "Public store. Enable shopping cart", SystemName = "EnableShoppingCart", Category = "PublicStore" };
        public static readonly PermissionRecord EnableWishlist = new PermissionRecord { Name = "Public store. Enable wishlist", SystemName = "EnableWishlist", Category = "PublicStore" };
        public static readonly PermissionRecord PublicStoreAllowNavigation = new PermissionRecord { Name = "Public store. Allow navigation", SystemName = "PublicStoreAllowNavigation", Category = "PublicStore" };
        public static readonly PermissionRecord AccessClosedStore = new PermissionRecord { Name = "Public store. Access a closed store", SystemName = "AccessClosedStore", Category = "PublicStore" };

        //pro permissions
        public static readonly PermissionRecord JobsSummary = new PermissionRecord { Name = "Organisation Job. Jobs Summary", SystemName = "JobsSummary", Category = "OrganisationJob" };
        public static readonly PermissionRecord OrganisationProfile = new PermissionRecord { Name = "Organisation Job. Organisation Profile", SystemName = "OrganisationProfile", Category = "Organization" };
        public static readonly PermissionRecord OrganisationEditProfile = new PermissionRecord { Name = "Organisation Job. Organisation Edit Profile", SystemName = "OrganisationEditProfile", Category = "Organization" };
        public static readonly PermissionRecord CreateJobProfile = new PermissionRecord { Name = "Organisation Job. Create Job Profile", SystemName = "CreateJobProfile", Category = "OrganisationJob" };
        public static readonly PermissionRecord JobPosting = new PermissionRecord { Name = "Organisation Job. Job Posting", SystemName = "JobPosting", Category = "OrganisationJob" };
        public static readonly PermissionRecord JobInvited = new PermissionRecord { Name = "Organisation Job. Job Invited", SystemName = "JobInvited", Category = "OrganisationJob" };
        public static readonly PermissionRecord JobApplicants = new PermissionRecord { Name = "Organisation Job. Job Applicants", SystemName = "JobApplicants", Category = "OrganisationJob" };

        public static readonly PermissionRecord ConsultationPosting = new PermissionRecord { Name = "Organisation Consultation. Consultation Posting", SystemName = "ConsultationPosting", Category = "OrganisationConsultation" };
        public static readonly PermissionRecord ConsultationInvited = new PermissionRecord { Name = "Organisation Consultation. Consultation Invited", SystemName = "ConsultationInvited", Category = "OrganisationConsultation" };
        public static readonly PermissionRecord ConsultationApplicants = new PermissionRecord { Name = "Organisation Consultation. Consultation Applicants", SystemName = "ConsultationApplicants", Category = "OrganisationConsultation" };
        public static readonly PermissionRecord ConsultationConfirmed = new PermissionRecord { Name = "Organisation Consultation. Consultation Confirmed", SystemName = "ConsultationConfirmed", Category = "OrganisationConsultation" };
        public static readonly PermissionRecord ConsultationNew = new PermissionRecord { Name = "Organisation Consultation. Consultation New", SystemName = "ConsultationNew", Category = "OrganisationConsultation" };
        public static readonly PermissionRecord ConsultationSearchCandidates = new PermissionRecord { Name = "Organisation Consultation. Consultation Search Candidates", SystemName = "ConsultationSearchCandidates", Category = "OrganisationConsultation" };

        public static readonly PermissionRecord IndividualProfile = new PermissionRecord { Name = "Individual. Profile", SystemName = "IndividualProfile", Category = "Individual" };
        public static readonly PermissionRecord IndividualProfileEdit = new PermissionRecord { Name = "Individual. Individual Profile Edit", SystemName = "IndividualProfileEdit", Category = "Individual" };
        public static readonly PermissionRecord JobProfileEdit = new PermissionRecord { Name = "Individual Job. Job Profile Edit", SystemName = "JobProfileEdit", Category = "IndividualJob" };
        public static readonly PermissionRecord JobInvites = new PermissionRecord { Name = "Individual Job. Job Invites", SystemName = "JobInvites", Category = "IndividualJob" };
        public static readonly PermissionRecord JobApplied = new PermissionRecord { Name = "Individual Job. Job Applied", SystemName = "JobApplied", Category = "IndividualJob" };
        public static readonly PermissionRecord JobSearch = new PermissionRecord { Name = "Individual Job. Job Search", SystemName = "JobSearch", Category = "IndividualJob" };

        public static readonly PermissionRecord ServiceProfiles = new PermissionRecord { Name = "Individual Service. Service Profiles", SystemName = "ServiceProfiles", Category = "IndividualService" };
        public static readonly PermissionRecord ReceivedRequests = new PermissionRecord { Name = "Individual Service. Received Requests", SystemName = "ReceivedRequests", Category = "IndividualService" };
        public static readonly PermissionRecord HireRequests = new PermissionRecord { Name = "Individual Service. Hire Requests", SystemName = "HireRequests", Category = "IndividualService" };
        public static readonly PermissionRecord CreateServiceProfiles = new PermissionRecord { Name = "Individual Service. Create Service Requests", SystemName = "CreateServiceProfiles", Category = "IndividualService" };
        public static readonly PermissionRecord EditServiceProfiles = new PermissionRecord { Name = "Individual Service. Create Service Requests", SystemName = "CreateServiceProfiles", Category = "IndividualService" };
        public static readonly PermissionRecord SearchSellers = new PermissionRecord { Name = "Individual Service. Search Sellers", SystemName = "SearchSellers", Category = "IndividualService" };
        public static readonly PermissionRecord RequestedOrders = new PermissionRecord { Name = "Individual Service. Requested Orders", SystemName = "RequestedOrders", Category = "IndividualService" };
        public static readonly PermissionRecord ConfirmedOrders = new PermissionRecord { Name = "Individual Service. Confirmed Orders", SystemName = "ConfirmedOrders", Category = "IndividualService" };

        public static readonly PermissionRecord OrganizationProfile = new PermissionRecord { Name = "Organization. Profile", SystemName = "OrganizationProfile", Category = "Organization" };
        public static readonly PermissionRecord OrganizationJob = new PermissionRecord { Name = "Organization. Job", SystemName = "OrganizationJob", Category = "Organization" };
        public static readonly PermissionRecord OrganizationConsultation = new PermissionRecord { Name = "Organization. Consultation", SystemName = "OrganizationConsultation", Category = "Organization" };

        public static readonly PermissionRecord IndividualJob = new PermissionRecord { Name = "Individual. Job", SystemName = "IndividualJob", Category = "Individual" };
        public static readonly PermissionRecord IndividualService = new PermissionRecord { Name = "Individual. Service", SystemName = "IndividualService", Category = "Individual" };

        public static readonly PermissionRecord ModeratorReview = new PermissionRecord { Name = "Moderator. Review", SystemName = "ModeratorReview", Category = "Moderator" };

        public static readonly PermissionRecord FacilitateAllSession = new PermissionRecord { Name = "Moderator. Facilitate All Session", SystemName = "ConsultationFacilitateAllSession", Category = "Moderator" };



        /// <summary>
        /// Get permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                AccessAdminPanel,
                AllowCustomerImpersonation,
                ManageProducts,
                ManageProductsOperatorMode,
                ManageProductsAdvancedMode,
                ManageCategories,
                ManageExpertise,
                ManageManufacturers,
                ManageProductReviews,
                ManageProductTags,
                ManageAttributes,
                ManageCustomers,
                ManageVendors,
                ManageCurrentCarts,
                ManageOrders,
                ManageProOrders,
                ManageCharge,
                ManageRecurringPayments,
                ManageGiftCards,
                ManageReturnRequests,
                OrderCountryReport,
                ManageAffiliates,
                ManageCampaigns,
                ManageDiscounts,
                ManageNewsletterSubscribers,
                ManagePolls,
                ManageNews,
                ManageBlog,
                ManageWidgets,
                ManageTopics,
                ManageForums,
                ManageMessageTemplates,
                ManageCountries,
                ManageLanguages,
                ManageSettings,
                ManagePaymentMethods,
                ManageExternalAuthenticationMethods,
                ManageTaxSettings,
                ManageShippingSettings,
                ManageCurrencies,
                ManageActivityLog,
                ManageAcl,
                ManageEmailAccounts,
                ManageStores,
                ManagePlugins,
                ManageSystemLog,
                ManageMessageQueue,
                ManageMaintenance,
                HtmlEditorManagePictures,
                ManageScheduleTasks,
                DisplayPrices,
                EnableShoppingCart,
                EnableWishlist,
                PublicStoreAllowNavigation,
                AccessClosedStore,
                JobsSummary,
                ManageCancellationRequest,
                ManageCampaignManagement,
                ManageApproveDepositRequest,
                ManageDashboards,
                ManageProDashboard,
                ManageShuqDashboard,
                ManageMainDashboard,
                ManageThirdPartyPlugins,
                ManageReportings,
                ManageContacts,
                ManageProRevenue,
                ManageProExpenses,
                ManageShuqRevenue,
                ManageOperations,
                ManageShuqOperations,
                ManageVendorApplications,
                ManageDisputes,
                ManageShipments
            };
        }

        /// <summary>
        /// Get default permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return new HashSet<(string, PermissionRecord[])>
            {
                (
                    NopCustomerDefaults.AdministratorRoleName,
                    new[]
                    {
                        AccessAdminPanel,
                        AllowCustomerImpersonation,
                        ManageProducts,
                        ManageProductsAdvancedMode,
                        ManageProductsOperatorMode,
                        ManageCategories,
                        ManageExpertise,
                        ManageManufacturers,
                        ManageProductReviews,
                        ManageProductTags,
                        ManageAttributes,
                        ManageCustomers,
                        ManageVendors,
                        ManageCurrentCarts,
                        ManageOrders,
                        ManageProOrders,
                        ManageCharge,
                        ManageRecurringPayments,
                        ManageGiftCards,
                        ManageReturnRequests,
                        OrderCountryReport,
                        ManageAffiliates,
                        ManageCampaigns,
                        ManageDiscounts,
                        ManageNewsletterSubscribers,
                        ManagePolls,
                        ManageNews,
                        ManageBlog,
                        ManageWidgets,
                        ManageTopics,
                        ManageForums,
                        ManageMessageTemplates,
                        ManageCountries,
                        ManageLanguages,
                        ManageSettings,
                        ManagePaymentMethods,
                        ManageExternalAuthenticationMethods,
                        ManageTaxSettings,
                        ManageShippingSettings,
                        ManageCurrencies,
                        ManageActivityLog,
                        ManageAcl,
                        ManageEmailAccounts,
                        ManageStores,
                        ManagePlugins,
                        ManageSystemLog,
                        ManageMessageQueue,
                        ManageMaintenance,
                        HtmlEditorManagePictures,
                        ManageScheduleTasks,
                        DisplayPrices,
                        EnableShoppingCart,
                        EnableWishlist,
                        PublicStoreAllowNavigation,
                        AccessClosedStore,
                        ManageCancellationRequest,
                        ManageCampaignManagement,
                        ManageApproveDepositRequest,
                        ManageDashboards,
                        ManageProDashboard,
                        ManageShuqDashboard,
                        ManageMainDashboard,
                        ManageThirdPartyPlugins,
                        ManageReportings,
                        ManageContacts,
                        ManageProRevenue,
                        ManageProExpenses,
                        ManageShuqRevenue,
                        ManageOperations,
                        ManageShuqOperations,
                        ManageVendorApplications,
                        ManageDisputes,
                        ManageShipments
                    }
                ),
                (
                    NopCustomerDefaults.ModeratorRoleName,
                    new[]
                    {
                        DisplayPrices,
                        EnableShoppingCart,
                        EnableWishlist,
                        PublicStoreAllowNavigation
                    }
                ),
                (
                    NopCustomerDefaults.GuestRoleName,
                    new[]
                    {
                        DisplayPrices,
                        EnableShoppingCart,
                        EnableWishlist,
                        PublicStoreAllowNavigation
                    }
                ),
                (
                    NopCustomerDefaults.RegisteredRoleName,
                    new[]
                    {
                        DisplayPrices,
                        EnableShoppingCart,
                        EnableWishlist,
                        PublicStoreAllowNavigation
                    }
                ),
                (
                    NopCustomerDefaults.VendorRoleName,
                    new[]
                    {
                        AccessAdminPanel,
                        ManageProducts,
                        ManageProductReviews,
                        ManageOrders,
                        ManageShop,
                        ManageDiscounts
                    }
                ),
                  (
                    NopCustomerDefaults.IndividualRoleName,
                    new[]
                    {
                        AccessAdminPanel,
                        ManageProducts,
                        ManageProductReviews,
                        ManageOrders
                    }
                ),
                (
                    NopCustomerDefaults.OrganizationRoleName,
                    new[]
                    {
                        AccessAdminPanel,
                        ManageProducts,
                        ManageProductReviews,
                        ManageOrders,
                        JobsSummary

                    }
                )
            };
        }
    }
}