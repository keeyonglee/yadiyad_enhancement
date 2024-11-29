using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Individual
{
    public class IndividualProfile : BaseEntityExtension
    {
        public int CustomerId { get; set; }
        public int Title { get; set; }
        public string FullName { get; set; }
        public string NickName { get; set; }
        public int Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public int NationalityId { get; set; }
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipPostalCode { get; set; }
        public int CityId { get; set; }
        public int StateProvinceId { get; set; }
        public int CountryId { get; set; }
        public bool IsOnline { get; set; }
        public int? PictureId { get; set; }
        public int ProfileImageViewModeId { get; set; }
        public string? SSTRegNo { get; set; }
        public int NumberOfCancellation { get; set; }
        public bool IsTourCompleted { get; set; }
        public ProfileImageViewMode ProfileImageViewMode { get => (ProfileImageViewMode)ProfileImageViewModeId; set => ProfileImageViewModeId = (int)value; }

    }
}