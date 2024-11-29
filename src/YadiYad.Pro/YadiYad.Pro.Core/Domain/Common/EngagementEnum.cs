using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Common
{
    public enum EngagementType
    {
        Job = 1,
        Service = 2,
        Consultation = 3,
    }

    public enum EngagementParty
    {
        Buyer = 1,
        Seller = 2,
        Moderator = 3
    }

    public enum CustomerProfileType
    {
        Individual = 1,
        Organization = 2,
        Admin = 3
    }

    public enum JobEngagementStatus
    {
        [Description("Matching")]
        New = 1,
        [Description("KIV")]
        KeepForFutureReference = 5,
        [Description("Matched")]
        Matched = 6,
        [Description("Invitation Rejected")]
        InvitationRejected = 9,
        [Description("Cancelled By Organization")]
        CancelledByOrganization = 12,
        [Description("Cancelled By Individual")]
        CancelledByIndividual = 13,
        [Description("Pending Payment Verification")]
        PendingPaymentVerification = 14,
        [Description("Revise Payment Required")]
        RevisePaymentRequired = 15,
        [Description("Completed")]
        Completed = 16,
        [Description("Refund Initialized")]
        RefundInitialized = 17,
        [Description("Refunded")]
        Refunded = 18,
        [Description("Rematched")]
        Rematched = 19
    }

    public enum ConsultationEngagementStatus
    {
        [Description("Matching")]
        Matching = 1,
        [Description("Declined")]
        Declined = 3,
        [Description("Paid")]
        Paid = 4,
        [Description("Completed")]
        Completed = 5,
        [Description("Cancelled By Organization")]
        CancelledByOrganization = 6,
        [Description("Cancelled By Consultant")]
        CancelledByConsultant = 7,
        [Description("Refund Initialized")]
        RefundInitialized = 8,
        [Description("Refunded")]
        Refunded = 9,
        [Description("Rematched")]
        Rematched = 10
    }
}
