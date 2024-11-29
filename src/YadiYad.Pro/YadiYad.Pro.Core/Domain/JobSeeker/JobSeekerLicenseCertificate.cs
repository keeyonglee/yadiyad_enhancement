using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.JobSeeker
{
    public class JobSeekerLicenseCertificate : BaseEntityExtension
    {
        public string ProfessionalAssociationName { get; set; }
        public string LicenseCertificateName { get; set; }
        public int? DownloadId { get; set; }

        public int JobSeekerProfileId { get; set; }

        [ForeignKey("JobSeekerProfileId")]
        public JobSeekerProfile JobSeekerProfile { get; set; }
    }
}
