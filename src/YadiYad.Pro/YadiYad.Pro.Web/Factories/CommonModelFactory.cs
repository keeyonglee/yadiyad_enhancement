using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Services.Blogs;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.News;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Themes;
using Nop.Services.Topics;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI;
using YadiYad.Pro.Core.Infrastructure.Cache;
using YadiYad.Pro.Web.Models.Common;
using YadiYad.Pro.Web.Models.Customer;

namespace YadiYad.Pro.Web.Factories
{
    /// <summary>
    /// Represents the common models factory
    /// </summary>
    public partial class CommonModelFactory
    {
        #region Fields

        private readonly BlogSettings _blogSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly CommonSettings _commonSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly DisplayDefaultFooterItemSettings _displayDefaultFooterItemSettings;
        private readonly ForumSettings _forumSettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IBlogService _blogService;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly ICategoryService _categoryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IForumService _forumService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly INewsService _newsService;
        private readonly INopFileProvider _fileProvider;
        private readonly IPageHeadBuilder _pageHeadBuilder;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ISitemapGenerator _sitemapGenerator;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IThemeContext _themeContext;
        private readonly IThemeProvider _themeProvider;
        private readonly ITopicService _topicService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly LocalizationSettings _localizationSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly NewsSettings _newsSettings;
        private readonly SitemapSettings _sitemapSettings;
        private readonly SitemapXmlSettings _sitemapXmlSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public CommonModelFactory(BlogSettings blogSettings,
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            CustomerSettings customerSettings,
            DisplayDefaultFooterItemSettings displayDefaultFooterItemSettings,
            ForumSettings forumSettings,
            IActionContextAccessor actionContextAccessor,
            IBlogService blogService,
            ICacheKeyService cacheKeyService,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IForumService forumService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            INewsService newsService,
            INopFileProvider fileProvider,
            IPageHeadBuilder pageHeadBuilder,
            IPermissionService permissionService,
            IPictureService pictureService,
            IProductService productService,
            IProductTagService productTagService,
            IShoppingCartService shoppingCartService,
            ISitemapGenerator sitemapGenerator,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IThemeContext themeContext,
            IThemeProvider themeProvider,
            ITopicService topicService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            LocalizationSettings localizationSettings,
            MediaSettings mediaSettings,
            NewsSettings newsSettings,
            SitemapSettings sitemapSettings,
            SitemapXmlSettings sitemapXmlSettings,
            StoreInformationSettings storeInformationSettings,
            VendorSettings vendorSettings)
        {
            _blogSettings = blogSettings;
            _captchaSettings = captchaSettings;
            _catalogSettings = catalogSettings;
            _commonSettings = commonSettings;
            _customerSettings = customerSettings;
            _displayDefaultFooterItemSettings = displayDefaultFooterItemSettings;
            _forumSettings = forumSettings;
            _actionContextAccessor = actionContextAccessor;
            _blogService = blogService;
            _cacheKeyService = cacheKeyService;
            _categoryService = categoryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _forumService = forumService;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _languageService = languageService;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _newsService = newsService;
            _fileProvider = fileProvider;
            _pageHeadBuilder = pageHeadBuilder;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _productService = productService;
            _productTagService = productTagService;
            _shoppingCartService = shoppingCartService;
            _sitemapGenerator = sitemapGenerator;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _themeContext = themeContext;
            _themeProvider = themeProvider;
            _topicService = topicService;
            _urlHelperFactory = urlHelperFactory;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _localizationSettings = localizationSettings;
            _newsSettings = newsSettings;
            _sitemapSettings = sitemapSettings;
            _sitemapXmlSettings = sitemapXmlSettings;
            _storeInformationSettings = storeInformationSettings;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get the number of unread private messages
        /// </summary>
        /// <returns>Number of private messages</returns>
        protected virtual int GetUnreadPrivateMessages()
        {
            var result = 0;
            var customer = _workContext.CurrentCustomer;
            if (_forumSettings.AllowPrivateMessages && !_customerService.IsGuest(customer))
            {
                var privateMessages = _forumService.GetAllPrivateMessages(_storeContext.CurrentStore.Id,
                    0, customer.Id, false, null, false, string.Empty, 0, 1);

                if (privateMessages.TotalCount > 0)
                {
                    result = privateMessages.TotalCount;
                }
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the footer model
        /// </summary>
        /// <returns>Footer model</returns>
        public virtual FooterModel PrepareFooterModel()
        {
            //footer topics
            var topicModels = _topicService.GetAllTopics(_storeContext.CurrentStore.Id)
                    .Where(t => t.IncludeInFooterColumn1 || t.IncludeInFooterColumn2 || t.IncludeInFooterColumn3)
                    .Select(t => new FooterModel.FooterTopicModel
                    {
                        Id = t.Id,
                        Name = _localizationService.GetLocalized(t, x => x.Title),
                        SeName = _urlRecordService.GetSeName(t),
                        IncludeInFooterColumn1 = t.IncludeInFooterColumn1,
                        IncludeInFooterColumn2 = t.IncludeInFooterColumn2,
                        IncludeInFooterColumn3 = t.IncludeInFooterColumn3
                    }).ToList();

            //model
            var model = new FooterModel
            {
                StoreName = _localizationService.GetLocalized(_storeContext.CurrentStore, x => x.Name),
                WishlistEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableWishlist),
                ShoppingCartEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart),
                SitemapEnabled = _sitemapSettings.SitemapEnabled,
                WorkingLanguageId = _workContext.WorkingLanguage.Id,
                BlogEnabled = _blogSettings.Enabled,
                CompareProductsEnabled = _catalogSettings.CompareProductsEnabled,
                ForumEnabled = _forumSettings.ForumsEnabled,
                NewsEnabled = _newsSettings.Enabled,
                RecentlyViewedProductsEnabled = _catalogSettings.RecentlyViewedProductsEnabled,
                NewProductsEnabled = _catalogSettings.NewProductsEnabled,
                DisplayTaxShippingInfoFooter = _catalogSettings.DisplayTaxShippingInfoFooter,
                HidePoweredByNopCommerce = _storeInformationSettings.HidePoweredByNopCommerce,
                AllowCustomersToApplyForVendorAccount = _vendorSettings.AllowCustomersToApplyForVendorAccount,
                AllowCustomersToCheckGiftCardBalance = _customerSettings.AllowCustomersToCheckGiftCardBalance && _captchaSettings.Enabled,
                Topics = topicModels,
                DisplaySitemapFooterItem = _displayDefaultFooterItemSettings.DisplaySitemapFooterItem,
                DisplayContactUsFooterItem = _displayDefaultFooterItemSettings.DisplayContactUsFooterItem,
                DisplayProductSearchFooterItem = _displayDefaultFooterItemSettings.DisplayProductSearchFooterItem,
                DisplayNewsFooterItem = _displayDefaultFooterItemSettings.DisplayNewsFooterItem,
                DisplayBlogFooterItem = _displayDefaultFooterItemSettings.DisplayBlogFooterItem,
                DisplayForumsFooterItem = _displayDefaultFooterItemSettings.DisplayForumsFooterItem,
                DisplayRecentlyViewedProductsFooterItem = _displayDefaultFooterItemSettings.DisplayRecentlyViewedProductsFooterItem,
                DisplayCompareProductsFooterItem = _displayDefaultFooterItemSettings.DisplayCompareProductsFooterItem,
                DisplayNewProductsFooterItem = _displayDefaultFooterItemSettings.DisplayNewProductsFooterItem,
                DisplayCustomerInfoFooterItem = _displayDefaultFooterItemSettings.DisplayCustomerInfoFooterItem,
                DisplayCustomerOrdersFooterItem = _displayDefaultFooterItemSettings.DisplayCustomerOrdersFooterItem,
                DisplayCustomerAddressesFooterItem = _displayDefaultFooterItemSettings.DisplayCustomerAddressesFooterItem,
                DisplayShoppingCartFooterItem = _displayDefaultFooterItemSettings.DisplayShoppingCartFooterItem,
                DisplayWishlistFooterItem = _displayDefaultFooterItemSettings.DisplayWishlistFooterItem,
                DisplayApplyVendorAccountFooterItem = _displayDefaultFooterItemSettings.DisplayApplyVendorAccountFooterItem
            };

            return model;
        }

        /// <summary>
        /// Get the sitemap in XML format
        /// </summary>
        /// <param name="id">Sitemap identifier; pass null to load the first sitemap or sitemap index file</param>
        /// <returns>Sitemap as string in XML format</returns>
        public virtual string PrepareSitemapXml(int? id)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.SitemapSeoModelKey, id,
                _workContext.WorkingLanguage,
                _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer),
                _storeContext.CurrentStore);

            var siteMap = _staticCacheManager.Get(cacheKey, () => _sitemapGenerator.Generate(id));

            return siteMap;
        }

        /// <summary>
        /// Get robots.txt file
        /// </summary>
        /// <returns>Robots.txt file as string</returns>
        public virtual string PrepareRobotsTextFile()
        {
            var sb = new StringBuilder();

            //if robots.custom.txt exists, let's use it instead of hard-coded data below
            var robotsFilePath = _fileProvider.Combine(_fileProvider.MapPath("~/"), "robots.custom.txt");
            if (_fileProvider.FileExists(robotsFilePath))
            {
                //the robots.txt file exists
                var robotsFileContent = _fileProvider.ReadAllText(robotsFilePath, Encoding.UTF8);
                sb.Append(robotsFileContent);
            }
            else
            {
                //doesn't exist. Let's generate it (default behavior)

                var disallowPaths = new List<string>
                {
                    "/admin",
                    "/bin/",
                    "/files/",
                    "/files/exportimport/",
                    "/country/getstatesbycountryid",
                    "/install",
                    "/setproductreviewhelpfulness",
                };
                var localizableDisallowPaths = new List<string>
                {
                    "/addproducttocart/catalog/",
                    "/addproducttocart/details/",
                    "/backinstocksubscriptions/manage",
                    "/boards/forumsubscriptions",
                    "/boards/forumwatch",
                    "/boards/postedit",
                    "/boards/postdelete",
                    "/boards/postcreate",
                    "/boards/topicedit",
                    "/boards/topicdelete",
                    "/boards/topiccreate",
                    "/boards/topicmove",
                    "/boards/topicwatch",
                    "/cart$",
                    "/changecurrency",
                    "/changelanguage",
                    "/changetaxtype",
                    "/checkout",
                    "/checkout/billingaddress",
                    "/checkout/completed",
                    "/checkout/confirm",
                    "/checkout/shippingaddress",
                    "/checkout/shippingmethod",
                    "/checkout/paymentinfo",
                    "/checkout/paymentmethod",
                    "/clearcomparelist",
                    "/compareproducts",
                    "/compareproducts/add/*",
                    "/customer/avatar",
                    "/customer/activation",
                    "/customer/addresses",
                    "/customer/changepassword",
                    "/customer/checkusernameavailability",
                    "/customer/downloadableproducts",
                    "/customer/info",
                    "/deletepm",
                    "/emailwishlist",
                    "/eucookielawaccept",
                    "/inboxupdate",
                    "/newsletter/subscriptionactivation",
                    "/onepagecheckout",
                    "/order/history",
                    "/orderdetails",
                    "/passwordrecovery/confirm",
                    "/poll/vote",
                    "/privatemessages",
                    "/returnrequest",
                    "/returnrequest/history",
                    "/rewardpoints/history",
                    "/search?",
                    "/sendpm",
                    "/sentupdate",
                    "/shoppingcart/*",
                    "/storeclosed",
                    "/subscribenewsletter",
                    "/topic/authenticate",
                    "/viewpm",
                    "/uploadfilecheckoutattribute",
                    "/uploadfileproductattribute",
                    "/uploadfilereturnrequest",
                    "/wishlist",
                };

                const string newLine = "\r\n"; //Environment.NewLine
                sb.Append("User-agent: *");
                sb.Append(newLine);
                //sitemaps
                if (_sitemapXmlSettings.SitemapXmlEnabled)
                {
                    if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                    {
                        //URLs are localizable. Append SEO code
                        foreach (var language in _languageService.GetAllLanguages(storeId: _storeContext.CurrentStore.Id))
                        {
                            sb.AppendFormat("Sitemap: {0}{1}/sitemap.xml", _webHelper.GetStoreLocation(), language.UniqueSeoCode);
                            sb.Append(newLine);
                        }
                    }
                    else
                    {
                        //localizable paths (without SEO code)
                        sb.AppendFormat("Sitemap: {0}sitemap.xml", _webHelper.GetStoreLocation());
                        sb.Append(newLine);
                    }
                }
                //host
                sb.AppendFormat("Host: {0}", _webHelper.GetStoreLocation());
                sb.Append(newLine);

                //usual paths
                foreach (var path in disallowPaths)
                {
                    sb.AppendFormat("Disallow: {0}", path);
                    sb.Append(newLine);
                }
                //localizable paths (without SEO code)
                foreach (var path in localizableDisallowPaths)
                {
                    sb.AppendFormat("Disallow: {0}", path);
                    sb.Append(newLine);
                }

                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    //URLs are localizable. Append SEO code
                    foreach (var language in _languageService.GetAllLanguages(storeId: _storeContext.CurrentStore.Id))
                    {
                        foreach (var path in localizableDisallowPaths)
                        {
                            sb.AppendFormat("Disallow: /{0}{1}", language.UniqueSeoCode, path);
                            sb.Append(newLine);
                        }
                    }
                }

                //load and add robots.txt additions to the end of file.
                var robotsAdditionsFile = _fileProvider.Combine(_fileProvider.MapPath("~/"), "robots.additions.txt");
                if (_fileProvider.FileExists(robotsAdditionsFile))
                {
                    var robotsFileContent = _fileProvider.ReadAllText(robotsAdditionsFile, Encoding.UTF8);
                    sb.Append(robotsFileContent);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Prepare the contact us model
        /// </summary>
        /// <param name="model">Contact us model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Contact us model</returns>
        public virtual ContactUsProModel PrepareContactUsModel(ContactUsProModel model, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties)
            {
                model.Email = _workContext.CurrentCustomer.Email;
                model.FullName = _customerService.GetCustomerFullName(_workContext.CurrentCustomer);
            }

            model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;

            return model;
        }
        #endregion
    }
}