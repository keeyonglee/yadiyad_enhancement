using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Web.Enums;

namespace YadiYad.Pro.Web.DTO
{
    public class AccountDTO
    {
        public AccountType? AccountType { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Salutation { get; set; }
        public string OrgRegNo { get; set; }
        public string OrgOfficeState { get; set; }
        public string AccountImageURL { get; set; }
        public int? OrganizationProfileId { get; set; }
        public string Status { get; set; }
        public bool IsEntitledApplyJob { 
            get
            {
                return ApplyJobEntitledEndDateTime == null ? false : ApplyJobEntitledEndDateTime.Value >= DateTime.UtcNow ? true : false;
            }
        }
        public DateTime? ApplyJobEntitledEndDateTime { get; set; }

    }
}
