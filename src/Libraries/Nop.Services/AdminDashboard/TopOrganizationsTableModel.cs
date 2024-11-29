using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.AdminDashboard
{
    public class TopOrganizationsTableModel
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ContactPersonPhone { get; set; }
        public int JobPostCount { get; set; }
        public decimal JobEngagementAmount { get; set; }
        public int TotalCandidateHired { get; set; }
    }
}
