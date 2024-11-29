using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Consultation
{
    public class TimeSlotDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Selected { get; set; }
    }
}
