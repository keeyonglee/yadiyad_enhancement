using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Core.Domain.Organization
{
    public class OrganizationProfile : BaseEntityExtension
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string RegistrationNo { get; set; }
        public int SegmentId { get; set; }
        public bool IsListedCompany { get; set; }
        public DateTime DateEstablished { get; set; }
        public string Website { get; set; }
        public int CompanySize { get; set; }
        public string Address { get; set; }
        public int StateProvinceId { get; set; }
        public int CountryId { get; set; }
        public int ContactPersonTitle { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonPosition { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ContactPersonContact { get; set; }
        public int? PictureId { get; set; }
    }
}
