using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Attentions
{
    public class OrganizationAttentionDTO
    {
        public bool HasHiredJobsAttention { get; set; }
        public bool HasNewJobApplicationAttention { get; set; }
        public bool HasOfferFreelanceJobsAttention​ { 
            get
            {
                return HasHiredJobsAttention || HasNewJobApplicationAttention;
            }
        }

        public bool HasConsultationInvitesAttention​ { get; set; }
        public bool HasConsultationApplicantsAttention​ { get; set; }
        public bool HasConsultationOrdersAttention​ { get; set; }

        public bool HasSeekConsultations​Attention​
        {
            get
            {
                return HasConsultationInvitesAttention
                    || HasConsultationApplicantsAttention
                    || HasConsultationOrdersAttention;
            }
        }
    }
}
