using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Consultation
{
    public enum ConsultationInvitationStatus
    {
        [Description("New")]
        New = 1,
        [Description("Accepted")]
        Accepted = 2,
        [Description("Declined")]
        DeclinedByIndividual = 3,
        [Description("Paid")]
        Paid = 4,
        [Description("Completed")]
        Completed = 5,
        [Description("Cancelled By Organization")]
        CancelledByOrganization = 6,
        [Description("Cancelled By Consultant")]
        CancelledByIndividual = 7,
        [Description("Declined By Organization")]
        DeclinedByOrganization = 8,
    }

    public enum ModeratorFacilitateConsultationFeeType
    {
        [Description("Complete Consultation")]
        CompleteConsultation = 0,

        [Description("Cancelled By Buyer Less Than 24 Hours")]
        CancelledByBuyerLessThan24Hours = 1,

        [Description("Cancelled By Buyer More Than 24 Hours Less Than 72 Hours")]
        CancelledByBuyerMoreThan24HoursLessThan72Hours = 2,

        [Description("Cancelled By Buyer More Than 72 Hours")]
        CancelledByBuyerMoreThan72Hours = 3,

        [Description("Cancelled By Seller")]
        CancelledBySeller = 4
    }
}
