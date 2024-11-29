using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Service
{
    public class ServiceLicenseCertificate : BaseEntityExtension
    {
        public int ServiceProfileId { get; set; }
        public string ProfessionalAssociationName { get; set; }
        public string LicenseCertificateName { get; set; }
        public int? DownloadId { get; set; }


        [ForeignKey("ServiceProfileId")]
        public ServiceProfile ServiceProfile { get; set; }
    }
}
