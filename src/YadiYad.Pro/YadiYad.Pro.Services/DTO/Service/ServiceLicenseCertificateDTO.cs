using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Common;

namespace YadiYad.Pro.Services.DTO.Service
{
    public class ServiceLicenseCertificateDTO
    {
        public int Id { get; set; }
        public int ServiceProfileId { get; set; }
        public string ProfessionalAssociationName { get; set; }
        public string LicenseCertificateName { get; set; }
        public string DownloadName { get; set; }
        public Guid? DownloadGuid { get; set; }
        public int? DownloadId { get; set; }
        public int? NewDownloadId { get; set; }
        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }

    }
}
