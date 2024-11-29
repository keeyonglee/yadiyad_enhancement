using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Dashboard
{
    public partial class YadiyadKeyStatisticsModel : BaseNopModel
    {
        public int NumberOfJobPostsThisMonth { get; set; }
        public int NumberOfJobPostsLastMonth { get; set; }
        public int NumberOfJobHiredThisMonth { get; set; }
        public int NumberOfJobHiredLastMonth { get; set; }
        public int NumberOfServiceHiredThisMonth { get; set; }
        public int NumberOfServiceHiredLastMonth { get; set; }
        public int NumberOfShuqOrdersThisMonth { get; set; }
        public int NumberOfShuqOrdersLastMonth { get; set; }
    }
}
