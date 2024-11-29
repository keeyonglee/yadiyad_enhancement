using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Consultation
{
    public class ConsultationJobSettings : ISettings
    {
        public int InvitationAutoRejectAfterWorkingDays { get; set; }
    }
}
