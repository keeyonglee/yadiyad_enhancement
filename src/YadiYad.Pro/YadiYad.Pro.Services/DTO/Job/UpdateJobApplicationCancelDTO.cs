using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Job
{
    public class UpdateJobApplicationCancelDTO
    {
        public int ReasonId { get; set; }
        public string Remarks { get; set; }
        [UIHint("Document")]
        public int? AttachmentId { get; set; }
    }
}
