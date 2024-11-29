using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YadiYad.Pro.Services.DTO.Organization
{
    public class OrganizationProfileDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "Please enter name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter registration number")]
        public string RegistrationNo { get; set; }
        [Required(ErrorMessage = "Please select business segment")]
        public int SegmentId { get; set; }
        public bool IsListedCompany { get; set; }
        [Required(ErrorMessage = "Please enter date established")]
        public DateTime DateEstablished { get; set; }
        public string Website { get; set; }
        public string LogoImage { get; set; }
        [Required(ErrorMessage = "Please select company size name")]
        public int CompanySize { get; set; }
        [Required(ErrorMessage = "Please enter address")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Please select state")]
        public int StateProvinceId { get; set; }
        [Required(ErrorMessage = "Please select country")]
        public int CountryId { get; set; }
        [Required(ErrorMessage = "Please select title")]
        public int ContactPersonTitle { get; set; }
        [Required(ErrorMessage = "Please enter name")]
        public string ContactPersonName { get; set; }
        [Required(ErrorMessage = "Please enter position")]
        public string ContactPersonPosition { get; set; }
        [Required(ErrorMessage = "Please enter email")]
        public string ContactPersonEmail { get; set; }
        [Required(ErrorMessage = "Please enter contact")]
        public string ContactPersonContact { get; set; }


        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }


        public string SegmentName { get; set; }
        public string CompanySizeName { get; set; }
        public string StateProvinceName { get; set; }
        public string CountryName { get; set; }
        public string ContactPersonTitleName { get; set; }

        [UIHint("Picture")]
        public int? PictureId { get; set; }
    }
}
