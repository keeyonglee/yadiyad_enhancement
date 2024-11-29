using Nop.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YadiYad.Pro.Web.Infrastructure.Cache
{
    public class NopModelCacheDefaults
    {
        /// <summary>
        /// Key for Job Seeker Item Counter
        /// </summary>
        /// <remarks>
        /// {0} : current customer id
        /// </remarks>
        public static CacheKey JobSeekerItemCounterCacheKeyKey => new CacheKey("YadiYad.Pro.JobSeekerItemCounter-{0}", JobSeekerItemCounterPrefixCacheKey);
        public static string JobSeekerItemCounterPrefixCacheKey => "JobSeekerItemCounter";


        /// <summary>
        /// Key for Job Seeker Item Counter
        /// </summary>
        /// <remarks>
        /// {0} : current customer id
        /// </remarks>
        public static CacheKey OrganizationItemCounterMainCacheKeyKey => new CacheKey("YadiYad.Pro.OrganizationItemCounterMain-{0}", OrganizationItemCounterMainPrefixCacheKey);
        public static string OrganizationItemCounterMainPrefixCacheKey => "OrganizationItemCounterMain";


        /// <summary>
        /// Key for Job Seeker Item Counter
        /// </summary>
        /// <remarks>
        /// {0} : current customer id
        /// {2} : job profile id
        /// </remarks>
        public static CacheKey OrganizationItemCounterCacheKeyKey => new CacheKey("YadiYad.Pro.OrganizationItemCounter-{0}-{1}", OrganizationItemCounterPrefixCacheKey);
        public static string OrganizationItemCounterPrefixCacheKey => "OrganizationItemCounter";

    }
}
