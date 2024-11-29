using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Pro.Models.AdminDashboard
{
    public class AdminDashboardSettings : ISettings
    {
        public List<string> CategoryChartColourMapping { get; set; }
    }
}
