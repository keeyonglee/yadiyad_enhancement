using Nop.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YadiYad.Pro.Core.Infrastructure.Cache
{
    public class NopModelCacheDefaults
    {
        /// <summary>
        /// Key for nopCommerce.com news cache
        /// </summary>
        public static CacheKey OfficialNewsModelKey => new CacheKey("Nop.pres.admin.official.news");

        /// <summary>
        /// Key for categories caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static CacheKey CategoriesListKey => new CacheKey("Nop.pres.admin.categories.list-{0}", CategoriesListPrefixCacheKey);
        public static string CategoriesListPrefixCacheKey => "Nop.pres.admin.categories.list";

        /// <summary>
        /// Key for manufacturers caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static CacheKey ManufacturersListKey => new CacheKey("Nop.pres.admin.manufacturers.list-{0}", ManufacturersListPrefixCacheKey);
        public static string ManufacturersListPrefixCacheKey => "Nop.pres.admin.manufacturers.list";

        /// <summary>
        /// Key for vendors caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static CacheKey VendorsListKey => new CacheKey("Nop.pres.admin.vendors.list-{0}", VendorsListPrefixCacheKey);
        public static string VendorsListPrefixCacheKey => "Nop.pres.admin.vendors.list";

        /// <summary>
        /// Key for sitemap on the sitemap SEO page
        /// </summary>
        /// <remarks>
        /// {0} : sitemap identifier
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public static CacheKey SitemapSeoModelKey => new CacheKey("Nop.pres.sitemap.seo-{0}-{1}-{2}-{3}", SitemapPrefixCacheKey);
        public static string SitemapPrefixCacheKey => "Nop.pres.sitemap";

        public static CacheKey JobServiceExpertise = new CacheKey("Nop.pro.jobServiceExpertise");
        public static CacheKey CountryDefault = new CacheKey("Nop.pro.country.{0}");
        public static CacheKey Country = new CacheKey("Nop.pro.country");
        public static CacheKey JobServiceCategory = new CacheKey("Nop.pro.jobServiceCategory");
        public static CacheKey Expertise = new CacheKey("Nop.pro.expertise");
        public static CacheKey BusinessSegment = new CacheKey("Nop.pro.businessSegment");
        public static CacheKey InterestHobby = new CacheKey("Nop.pro.interestHobby");
        public static CacheKey Location = new CacheKey("Nop.pro.location");
        public static CacheKey Language = new CacheKey("Nop.pro.language");
        public static CacheKey City = new CacheKey("Nop.pro.city");
        public static CacheKey Timezone = new CacheKey("Nop.pro.timezone");
        public static CacheKey Bank = new CacheKey("Nop.pro.bank");
        public static CacheKey CancellationReason = new CacheKey("Nop.pro.cancellationReason-{0}-{1}");

        public static CacheKey IndividualServiceSellerCounter = new CacheKey("Nop.pro.serviceSellerCounter-{0}-{1}");
        public static CacheKey IndividualServiceBuyerCounter = new CacheKey("Nop.pro.serviceBuyerCounter-{0}-{1}");
        public static CacheKey IndividualJobCounter = new CacheKey("Nop.pro.jobCounter-{0}-{1}");
        public static CacheKey OrganizationJobCounter = new CacheKey("Nop.pro.organizationJobCounter-{0}-{1}");
        public static CacheKey IndividualAttention = new CacheKey("Nop.pro.IndividualAttention-{0}");
        public static CacheKey OrganizationAttention = new CacheKey("Nop.pro.OrganizationAttention-{0}");
    }
}
