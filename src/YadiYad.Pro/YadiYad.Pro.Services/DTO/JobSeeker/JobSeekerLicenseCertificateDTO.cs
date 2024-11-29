using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Common;

namespace YadiYad.Pro.Services.DTO.JobSeeker
{
    public class JobSeekerLicenseCertificateDTO
    {
        public int JobSeekerProfileId { get; set; }
        public int Id { get; set; }
        public string ProfessionalAssociationName { get; set; }
        public string LicenseCertificateName { get; set; }


        public int? NewDownloadId { get; set; }
        public int? DownloadId { get; set; }
        public Guid? DownloadGuid { get; set; }
        public string DownloadName { get; set; }


    }
}
