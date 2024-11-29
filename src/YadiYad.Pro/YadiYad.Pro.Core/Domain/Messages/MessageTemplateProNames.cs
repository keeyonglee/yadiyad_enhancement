using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Messages
{
    public static class MessageTemplateProNames
    {
        public const string ContactUsEnquiry = "Home.ContactUs.Enquiry";
        public const string ContactUsIssues = "Home.ContactUs.Issues";
        public const string ContactUsFeedbacks = "Home.ContactUs.Feedbacks";

        public const string JobInvitationIndividualInvite = "JobInvitation.Individual.Invite";
        public const string JobInvitationOrganisationAccepted = "JobInvitation.Organisation.Accepted";
        public const string JobInvitationOrganisationRejected = "JobInvitation.Organisation.Rejected";

        public const string JobApplicationIndividualUnderShortlist = "JobApplication.Individual.Shortlist";
        public const string JobApplicationIndividualKeepForFuture = "JobApplication.Individual.FutureReference";
        public const string JobApplicationIndividualHire = "JobApplication.Individual.Hire";

        public const string ServiceSellerRequest = "Service.Seller.Request";

        public const string ServiceBuyerAccepted = "Service.Buyer.Accepted";
        public const string ServiceBuyerDeclined = "Service.Buyer.Declined";
        public const string ServiceBuyerReproposed = "Service.Buyer.Reproposed";

        public const string ServiceBuyerConfirm = "Service.Buyer.Confirm";
        public const string ServiceSellerConfirm = "Service.Seller.Confirm";

        public const string ConsultationCandidateInvited = "Consultation.Candidate.Invited";
        public const string ConsultationOrganizationAccepted = "Consultation.Organization.Accepted";
        public const string ConsultationOrganizationDeclined = "Consultation.Organization.Declined";
        public const string ConsultationOrganizationAutoDeclined = "Consultation.Organization.AutoDeclined";
        public const string ConsultationConsultantAutoDeclined = "Consultation.Consultant.AutoDeclined";
        public const string ConsultationCandidatePaid = "Consultation.Candidate.Paid";

        public const string ConsultationBuyerCompleted = "Consultation.Buyer.Completed";
        public const string ConsultationBuyerReschedule = "Consultation.Buyer.Reschedule";
        public const string ConsultationBuyerCancellation = "Consultation.Buyer.Cancellation";

        public const string ConsultationSellerCompleted = "Consultation.Seller.Completed";
        public const string ConsultationSellerReschedule = "Consultation.Seller.Reschedule";
        public const string ConsultationSellerCancellation = "Consultation.Seller.Cancellation";
        public const string ConsultationSellerDeclinedByOrganization = "Consultation.Seller.DeclinedByOrganization";

        public const string BlockSeller = "Customer.BlockSeller";

        public const string DepositRequestNotification = "DepositRequest.Notification";
        public const string DepositRequestReminder = "DepositRequest.Reminder";
        public const string DepositRequestLastReminder = "DepositRequest.LastReminder";
        public const string DepositRequestTerminatingApplicationBuyer = "DepositRequest.TerminatingApplication.Buyer";
        public const string DepositRequestTerminatingApplicationSeller = "DepositRequest.TerminatingApplication.Seller";

        public const string DepositRequestConfirmed = "Project.Deposit.Confirmed";
        public const string DepositRequestNotConfirmed = "Project.Deposit.NotConfirmed";
        public const string DepositRequestBuyerPaid = "Project.Deposit.Buyer.Paid";

        public const string SubmittedNonProjectPayoutRequestMessage = "PayoutRequest.NonProject.New";
        public const string ApprovedNonProjectPayoutRequestMessage = "PayoutRequest.NonProject.Approved";
        public const string AutoApprovedNonProjectPayoutRequestMessage = "PayoutRequest.NonProject.AutoApproved";
        public const string RequiredMoreInfoNonProjectPayoutRequestMessage = "PayoutRequest.NonProject.RequiredMoreInfo";
        public const string ErrorNonProjectPayoutRequestMessage = "PayoutRequest.NonProject.Error";
        public const string PaidNonProjectPayoutRequestMessage = "PayoutRequest.NonProject.Paid";

        public const string SubmittedProjectPayoutRequestMessage = "PayoutRequest.Project.New";
        public const string ApprovedProjectPayoutRequestMessage = "PayoutRequest.Project.Approved";
        public const string AutoApprovedProjectPayoutRequestMessage = "PayoutRequest.Project.AutoApproved";
        public const string RequiredMoreInfoProjectPayoutRequestMessage = "PayoutRequest.Project.RequiredMoreInfo";
        public const string ErrorProjectPayoutRequestMessage = "PayoutRequest.Project.Error";
        public const string PaidProjectPayoutRequestMessage = "PayoutRequest.Project.Paid";

        public const string DepositRequestPaymentVerificationNotification = "DepositRequest.PaymentVerificationNotification";

        public const string PayoutFailInvalidBankAccount = "PayoutRequest.FailInvalidBankAccount";

    }
}
