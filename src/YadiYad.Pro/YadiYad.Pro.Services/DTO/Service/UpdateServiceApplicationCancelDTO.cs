using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Service
{
    public class UpdateServiceApplicationCancelDTO
    {
        public int ReasonId { get; set; }
        public string Remarks { get; set; }
        [UIHint("Document")]
        public int? AttachmentId { get; set; }
    }
}
