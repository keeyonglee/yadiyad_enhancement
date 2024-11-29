using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Services.DTO.Common;

namespace YadiYad.Pro.Services.DTO.Individual
{
    public class IndividualProfileDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string ProfileImage { get; set; }
        [Required(ErrorMessage = "Please enter title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please enter full name")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Please enter nick name")]
        public string NickName { get; set; }
        [Required(ErrorMessage = "Please select gender")]
        public int Gender { get; set; }
        [Required(ErrorMessage = "Please enter date of birth")]
        public DateTime DateOfBirth { get; set; }
        public string DateOfBirthName
        {
            get
            {
                return DateOfBirth.ToShortDateString();
            }
        }
        [Required(ErrorMessage = "Please enter email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter contact no")]
        public string ContactNo { get; set; }
        [Required(ErrorMessage = "Please select nationality")]
        public int NationalityId { get; set; }
        [Required(ErrorMessage = "Please enter address")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Please enter address 1")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required(ErrorMessage = "Please enter postal code")]
        public string ZipPostalCode { get; set; }
        [Required(ErrorMessage = "Please select city")]
        public int CityId { get; set; }
        [Required(ErrorMessage = "Please select state")]
        public int StateProvinceId { get; set; }
        [Required(ErrorMessage = "Please select country")]
        public int CountryId { get; set; }
        [Required(ErrorMessage = "Please select interest hobby")]
        public List<InterestHobbyDTO> InterestHobbies { get; set; } = new List<InterestHobbyDTO>();
        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }


        public string TitleName { get; set; }
        public string GenderText { get; set; }
        public string CityName { get; set; }
        public string NationalityName { get; set; }
        public string StateProvinceName { get; set; }
        public string CountryName { get; set; }
        public bool IsOnline { get; set; }
        [UIHint("Picture")]
        public int? PictureId { get; set; }
        public IFormFile File { get; set; }

        public int ProfileImageViewModeId { get; set; }

        public bool ProfileImagePublicViewable
        {
            get {
                return ProfileImageViewModeId == (int)ProfileImageViewMode.Public;
            }
            set
            {
                if (value)
                {
                    ProfileImageViewModeId = (int)ProfileImageViewMode.Public;
                }
                else
                {
                    ProfileImageViewModeId = (int)ProfileImageViewMode.Organization;
                }
            }
        }
        public string? SSTRegNo { get; set; }
        public BillingAddressDTO? BillingAddress { get; set; }
        public int NumberOfCancellation { get; set; }
        public bool IsTourCompleted { get; set; }
        public int SetDelay { get; set; }
    }
}
